using Grillisoft.BufferManager.Statistics;

namespace Grillisoft.BufferManager.Tests
{
    public class StandardBufferManagerTests : BaseBufferManagerTests
    {
        private const int BufferSize = 4096;

        protected override IBufferManager<byte> CreateManager(BufferManagerStats stats)
        {
            return new StandardBufferManager<byte>(true, BufferSize, stats.Allocated, stats.Cached);
        }

        protected override int CalculateAllocated(int size)
        {
            return (((size - 1) / BufferSize) + 1) * BufferSize;
        }
    }
}
