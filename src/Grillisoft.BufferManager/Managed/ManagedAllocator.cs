using System;

namespace Grillisoft.BufferManager.Managed
{
    public class ManagedAllocator<T> : IAllocator<T[]> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        public T[] Allocate(int size)
        {
            return new T[size];
        }

        public T[] CAllocate(int size)
        {
            return new T[size];
        }

        public void Clear(T[] buffer, int size)
        {
            Array.Clear(buffer, 0, size);
        }

        public void Free(T[] buffer)
        {
            //let the GC handle it
        }
    }
}
