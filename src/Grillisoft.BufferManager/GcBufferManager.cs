using System;

namespace Grillisoft.BufferManager
{
    public class GcBufferManager<T> : IBufferManager<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        public T[][] Allocate(int size)
        {
            return new T[][] { new T[size] };
        }

        public void Free(T[] data)
        {
        }

        public void Free(T[][] data)
        {
        }
    }
}
