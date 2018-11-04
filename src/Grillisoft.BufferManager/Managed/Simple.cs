using System;
using System.Linq;

namespace Grillisoft.BufferManager.Managed
{
    public class Simple<T> : IBufferManager<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        private readonly IAllocEvents _events;

        public Simple(IAllocEvents allocEvents = null)
        {
            _events = allocEvents;
        }

        public T[][] Allocate(int size)
        {
            var ret = new T[][] { new T[size] };
            _events?.Allocate(size);
            return ret;
        }

        public void Free(T[] data)
        {
            _events?.Free(data.Length);
        }

        public void Free(T[][] data)
        {
            _events?.Free(data.Sum(d => d.Length));
        }

        public void Init(int buffers)
        {
        }
    }
}
