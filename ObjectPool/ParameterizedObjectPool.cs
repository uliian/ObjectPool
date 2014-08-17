﻿/* 
 * Generic Object Pool Implementation
 *  
 * Implemented by Ofir Makmal, 28/1/2013
 *
 * My Blog: Blogs.microsoft.co.il/blogs/OfirMakmal
 * Email:   Ofir.Makmal@gmail.com
 * 
 */

using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace CodeProject.ObjectPool
{
    public sealed class ParameterizedObjectPool<TKey, TValue> : ObjectPool where TValue : PooledObject
    {
        private readonly ConcurrentDictionary<TKey, ObjectPool<TValue>> _pools = new ConcurrentDictionary<TKey, ObjectPool<TValue>>();

        private int _minimumPoolSize = DefaultPoolMinimumSize;
        private int _maximumPoolSize = DefaultPoolMaximumSize;

        #region Public Properties

        /// <summary>
        /// Gets or sets the minimum number of objects in the pool.
        /// </summary>
        public int MinimumPoolSize
        {
            get { return _minimumPoolSize; }
            set
            {
                // Validating pool limits, exception is thrown if invalid
                ValidatePoolLimits(value, _maximumPoolSize);
                _minimumPoolSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of objects that could be available at the same time in the pool.
        /// </summary>
        public int MaximumPoolSize
        {
            get { return _maximumPoolSize; }
            set
            {
                // Validating pool limits, exception is thrown if invalid
                ValidatePoolLimits(_minimumPoolSize, value);
                _maximumPoolSize = value;
            }
        }

        /// <summary>
        /// Gets the Factory method that will be used for creating new objects. 
        /// </summary>
        public Func<TKey, TValue> FactoryMethod { get; private set; }

        #endregion
        
        #region C'tor and Initialization code

        /// <summary>
        /// Initializes a new pool with default settings.
        /// </summary>
        public ParameterizedObjectPool()
        {
            InitializePool(DefaultPoolMinimumSize, DefaultPoolMaximumSize, null);
        }

        /// <summary>
        /// Initializes a new pool with specified minimum pool size and maximum pool size
        /// </summary>
        /// <param name="minimumPoolSize">The minimum pool size limit.</param>
        /// <param name="maximumPoolSize">The maximum pool size limit</param>
        public ParameterizedObjectPool(int minimumPoolSize, int maximumPoolSize)
        {
            InitializePool(minimumPoolSize, maximumPoolSize, null);
        }

        /// <summary>
        /// Initializes a new pool with specified factory method.
        /// </summary>
        /// <param name="factoryMethod">The factory method that will be used to create new objects.</param>
        public ParameterizedObjectPool(Func<TKey, TValue> factoryMethod)
        {
            InitializePool(DefaultPoolMinimumSize, DefaultPoolMaximumSize, factoryMethod);
        }

        /// <summary>
        /// Initializes a new pool with specified factory method and minimum and maximum size.
        /// </summary>
        /// <param name="minimumPoolSize">The minimum pool size limit.</param>
        /// <param name="maximumPoolSize">The maximum pool size limit</param>
        /// <param name="factoryMethod">The factory method that will be used to create new objects.</param>
        public ParameterizedObjectPool(int minimumPoolSize, int maximumPoolSize, Func<TKey, TValue> factoryMethod)
        {
            InitializePool(minimumPoolSize, maximumPoolSize, factoryMethod);
        }

        private void InitializePool(int minimumPoolSize, int maximumPoolSize, Func<TKey, TValue> factoryMethod)
        {
            // Validating pool limits, exception is thrown if invalid
            ValidatePoolLimits(minimumPoolSize, maximumPoolSize);

            // Assigning properties
            FactoryMethod = factoryMethod;
            _maximumPoolSize = maximumPoolSize;
            _minimumPoolSize = minimumPoolSize;
        }

        #endregion

        public TValue GetObject(TKey key)
        {
            ObjectPool<TValue> pool;
            if (!_pools.TryGetValue(key, out pool)) {
                // Initialize the new pool
                var factory = FactoryMethod;
                pool = new ObjectPool<TValue>(MinimumPoolSize, MaximumPoolSize, PrepareFactoryMethod(key));
                if (!_pools.TryAdd(key, pool)) {
                    // Someone added the pool in the meantime!
                    _pools.TryGetValue(key, out pool);
                }
            }
            Debug.Assert(pool != null);
            return pool.GetObject();
        }

        private Func<TValue> PrepareFactoryMethod(TKey key)
        {
            var factory = FactoryMethod;
            if (factory == null) {
                // Use the default parameterless constructor.
                return null;
            }
            return () => factory(key);
        } 
    }
}