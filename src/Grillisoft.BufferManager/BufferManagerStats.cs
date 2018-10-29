using System.Threading;

namespace Grillisoft.BufferManager
{
    public class BufferManagerStats : IBufferManagerStats, IBufferManagerEvents
    {
        private long _allocated;
        private long _cached;

        public long Allocated => _allocated;

        public long Cached => _cached;

        public long Total => _allocated + _cached;

        public void Allocate(int size)
        {
            Interlocked.Add(ref _allocated, size);
        }

        public void Free(int size)
        {
            Interlocked.Add(ref _allocated, -size);
        }

        public void Cache(int size)
        {
            Interlocked.Add(ref _cached, size);
        }

        public void FreeCache(int size)
        {
            Interlocked.Add(ref _cached, -size);
        }
    }
}
