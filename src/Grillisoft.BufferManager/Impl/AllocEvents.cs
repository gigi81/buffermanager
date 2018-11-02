using System.Threading;

namespace Grillisoft.BufferManager
{
    public class AllocEvents : IAllocEvents
    {
        private long _allocated;

        public long Allocated => Interlocked.Read(ref _allocated);

        public void Allocate(int size)
        {
            Interlocked.Add(ref _allocated, size);
        }

        public void Free(int size)
        {
            Interlocked.Add(ref _allocated, -size);
        }
    }
}
