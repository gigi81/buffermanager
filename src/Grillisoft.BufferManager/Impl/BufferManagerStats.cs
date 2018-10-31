using System.Threading;

namespace Grillisoft.BufferManager
{
    public class BufferManagerStats : IBufferManagerStats, IAllocEvents, ICacheEvents
    {
        private long _allocated;
        private long _cached;

        public long Allocated => Interlocked.Read(ref _allocated);

        public long Cached => Interlocked.Read(ref _cached);

        public long Total => this.Allocated + this.Cached;

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
