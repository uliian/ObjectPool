using System.Collections.Generic;

namespace CodeProject.ObjectPool.Core
{
    public interface IPooledObjectBuffer<T>
        where T : PooledObject
    {
        /// <summary>
        ///   The maximum capacity of this buffer.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        ///   The number of objects stored in this buffer.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<T> GetEnumerator();

        /// <summary>
        ///   Tries to dequeue an object from the buffer.
        /// </summary>
        /// <param name="pooledObject">Output pooled object.</param>
        /// <returns>True if <paramref name="pooledObject"/> has a value, false otherwise.</returns>
        bool TryDequeue(out T pooledObject);

        /// <summary>
        ///   Tries to enqueue given object into the buffer.
        /// </summary>
        /// <param name="pooledObject">Input pooled object.</param>
        /// <returns>True if there was enough space to enqueue given object, false otherwise.</returns>
        bool TryEnqueue(T pooledObject);

        /// <summary>
        ///   Resizes the buffer so that it fits to given capacity. If new capacity is smaller than
        ///   current capacity, then exceeding items are returned.
        /// </summary>
        /// <param name="newCapacity">The new capacity of this buffer.</param>
        /// <returns>All exceeding items.</returns>
        IList<T> Resize(int newCapacity);

        /// <summary>
        ///   Tries to remove given object from the buffer.
        /// </summary>
        /// <param name="pooledObject">Pooled object to be removed.</param>
        /// <returns>True if <paramref name="pooledObject"/> has been removed, false otherwise.</returns>
        bool TryRemove(T pooledObject);
    }
}