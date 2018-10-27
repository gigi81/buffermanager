using Xunit;
using System.Linq;
using System.Threading.Tasks;

namespace Grillisoft.BufferManager.Tests
{
    public class BufferManagerTests
    {
        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void Allocate(int size)
        {
            var manager = new StandardBufferManager<byte>();
            var alloc = manager.Allocate(size);

            Assert.True(size <= alloc.Sum(a => a.Length));
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void AllocateAndFree(int size)
        {
            var manager = new StandardBufferManager<byte>();
            var alloc = manager.Allocate(size);

            Assert.True(size <= alloc.Sum(a => a.Length));

            manager.Free(alloc);
        }

        [Theory]
        [InlineData(100, 100)]
        [InlineData(1000, 100)]
        [InlineData(10000, 100)]
        [InlineData(100000, 100)]
        public void MultipleAllocAndFree(int size, int count)
        {
            var manager = new StandardBufferManager<byte>();

            for(int i=0; i< count; i++)
            {
                var alloc = manager.Allocate(size);

                Assert.True(size <= alloc.Sum(a => a.Length));

                manager.Free(alloc);
            }
        }

        [Theory]
        [InlineData(100, 10000)]
        [InlineData(1000, 10000)]
        [InlineData(10000, 10000)]
        [InlineData(100000, 10000)]
        public void ParallelAllocAndFree(int size, int count)
        {
            var manager = new StandardBufferManager<byte>();

            Parallel.ForEach(Enumerable.Range(1, count), i =>
            {
                var alloc = manager.Allocate(size);

                Assert.True(size <= alloc.Sum(a => a.Length));

                manager.Free(alloc);
            });
        }
    }
}
