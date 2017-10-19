#if !NETSTANDARD1_0


using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CodeProject.ObjectPool.Core
{
    public class BlockPooledObjectBuffer<T> : IPooledObjectBuffer<T>,IEnumerable<T>
        where T : PooledObject
    {
        private BlockingCollection<T> _blockingCollection;
        public BlockPooledObjectBuffer(int boundedCapacity)
        {
            this._blockingCollection = new BlockingCollection<T>(boundedCapacity);
            this.Capacity = boundedCapacity;
        }

        /// <summary>
        ///   The maximum capacity of this buffer.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        ///   The number of objects stored in this buffer.
        /// </summary>
        public int Count { get; }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._blockingCollection.GetConsumingEnumerable().GetEnumerator();
        }

        /// <summary>
        ///   Tries to dequeue an object from the buffer.
        /// </summary>
        /// <param name="pooledObject">Output pooled object.</param>
        /// <returns>True if <paramref name="pooledObject"/> has a value, false otherwise.</returns>
        public bool TryDequeue(out T pooledObject)
        {
            return this._blockingCollection.TryTake(out pooledObject);
        }

        /// <summary>
        ///   Tries to enqueue given object into the buffer.
        /// </summary>
        /// <param name="pooledObject">Input pooled object.</param>
        /// <returns>True if there was enough space to enqueue given object, false otherwise.</returns>
        public bool TryEnqueue(T pooledObject)
        {
            return this._blockingCollection.TryAdd(pooledObject);
        }

        /// <summary>
        ///   Resizes the buffer so that it fits to given capacity. If new capacity is smaller than
        ///   current capacity, then exceeding items are returned.
        /// </summary>
        /// <param name="newCapacity">The new capacity of this buffer.</param>
        /// <returns>All exceeding items.</returns>
        public IList<T> Resize(int newCapacity)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Tries to remove given object from the buffer.
        /// </summary>
        /// <param name="pooledObject">Pooled object to be removed.</param>
        /// <returns>True if <paramref name="pooledObject"/> has been removed, false otherwise.</returns>
        public bool TryRemove(T pooledObject)
        {
            throw new NotSupportedException();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
#endif