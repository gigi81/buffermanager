using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace Grillisoft.BufferManager.Tests
{
    public class BufferManagerTests
    {
        private const int BufferSize = 4096;

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void Allocate(int size)
        {
            var stats = new BufferManagerStats();
            var manager = new StandardBufferManager<byte>(true, BufferSize, stats);
            var alloc = manager.Allocate(size);

            Assert.True(size <= alloc.Sum(a => a.Length));
            Assert.Equal(CalculateAllocated(size), stats.Allocated);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void AllocateAndFree(int size)
        {
            var stats = new BufferManagerStats();
            var manager = new StandardBufferManager<byte>(true, BufferSize, stats);
            var alloc = manager.Allocate(size);

            Assert.True(size <= alloc.Sum(a => a.Length));

            manager.Free(alloc);
            Assert.Equal(0, stats.Allocated);
            Assert.Equal(CalculateAllocated(size), stats.Cached);
        }

        [Theory]
        [InlineData(100, 100)]
        [InlineData(1000, 100)]
        [InlineData(10000, 100)]
        [InlineData(100000, 100)]
        public void MultipleAllocAndFree(int size, int count)
        {
            var stats = new BufferManagerStats();
            var manager = new StandardBufferManager<byte>(true, BufferSize, stats);

            for (int i=0; i< count; i++)
            {
                var alloc = manager.Allocate(size);

                Assert.True(size <= alloc.Sum(a => a.Length));

                manager.Free(alloc);
            }

            Assert.Equal(0, stats.Allocated);
            Assert.Equal(CalculateAllocated(size) * count, stats.Cached);
        }

        [Theory]
        [InlineData(100, 10000)]
        [InlineData(1000, 10000)]
        [InlineData(10000, 10000)]
        [InlineData(100000, 10000)]
        [InlineData(BufferSize, 10000)]
        public void ParallelAllocAndFree(int size, int count)
        {
            var stats = new BufferManagerStats();
            var manager = new StandardBufferManager<byte>(true, BufferSize, stats);
            var randomize = new Random((int) DateTime.Now.Ticks);

            Parallel.ForEach(Enumerable.Range(1, count), i =>
            {
                var alloc = manager.Allocate(size);

                Assert.True(size <= alloc.Sum(a => a.Length));

                Thread.Sleep(randomize.Next() * 1000);

                manager.Free(alloc);
            });

            Assert.Equal(0, stats.Allocated);
            Assert.Equal(CalculateAllocated(size) * count, stats.Cached);
        }

        private static int CalculateAllocated(int size)
        {
            return (((size - 1) / BufferSize) + 1) * BufferSize;
        }
    }
}
