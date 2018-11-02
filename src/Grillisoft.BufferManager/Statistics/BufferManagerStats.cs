namespace Grillisoft.BufferManager.Statistics
{
    public class BufferManagerStats
    {
        public readonly IAllocEvents Allocated = new AllocEvents();

        public readonly IAllocEvents Cached = new AllocEvents();

        public long Total => this.Allocated.Allocated + this.Cached.Allocated;
    }
}
