namespace Grillisoft.BufferManager
{
    /// <summary>
    /// A memory allocator used by buffer managers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAllocator<T>
    {
        /// <summary>
        /// Allocate the specified amount of memory <paramref name="size"/> in bytes
        /// </summary>
        /// <param name="size">Size of the buffer to allocate in bytes</param>
        /// <returns></returns>
        T Allocate(int size);

        /// <summary>
        /// De-allocate the specified buffer
        /// </summary>
        /// <param name="buffer">Buffer to deallocate</param>
        void Free(T buffer);

        /// <summary>
        /// Clear the specified buffer by setting all its elements to their default value
        /// </summary>
        /// <param name="buffer">Buffer to clear</param>
        /// <param name="size">Size of the buffer or portion of the buffer to clear</param>
        void Clear(T buffer, int size);
    }
}
