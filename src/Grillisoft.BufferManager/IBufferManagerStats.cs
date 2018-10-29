namespace Grillisoft.BufferManager
{
    public interface IBufferManagerStats
    {
        long Allocated { get; }

        long Cached { get; }

        long Total { get; }
    }
}
