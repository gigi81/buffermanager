using Grillisoft.BufferManager.Managed;
using Grillisoft.BufferManager.Statistics;

namespace Grillisoft.BufferManager.Tests
{
    public class StandardManagedBufferManagerTests : BaseBufferManagerTests
    {
        private const int BufferSize = 4096;

        protected override IBufferManager<byte> CreateManager(BufferManagerStats stats)
        {
            return new ConcurrentProxy<byte>(new Standard<byte>(true, BufferSize, stats.Allocated, stats.Cached));
        }

        protected override int CalculateAllocated(int size)
        {
            return (((size - 1) / BufferSize) + 1) * BufferSize;
        }
    }
}
