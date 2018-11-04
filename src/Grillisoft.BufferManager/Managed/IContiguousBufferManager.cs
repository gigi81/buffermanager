using System;

namespace Grillisoft.BufferManager.Managed
{
    public interface IContiguousBufferManager<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        T[] Allocate(int length);

        void Free(T[] data);
    }
}
