using System;

namespace Grillisoft.BufferManager
{
    public interface IBufferManager<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        /// <summary>
        /// Allocates and return one or more arrays for a total of <paramref name="size"/> <see cref="T"/> elements
        /// </summary>
        /// <param name="size">The total size of the arrays to return</param>
        /// <returns>A two dimensional array of <see cref="T"/> with a total size of at least <paramref name="size"/> (total size can be bigger)</returns>
        T[][] Allocate(int size);

        void Free(T[] data);

        void Free(T[][] data);
    }

    public interface IContiguousBufferManager<T> : IBufferManager<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        T[] AllocateContiguous(int length);
    }
}
