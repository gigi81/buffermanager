using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System;
using Grillisoft.BufferManager.Managed;
using Grillisoft.BufferManager.Statistics;

namespace Grillisoft.BufferManager.Tests
{
    public abstract class StandardBaseBufferManagerTests
    {
        protected abstract IBufferManager<byte> CreateManager(BufferManagerStats stats);

        protected abstract int CalculateAllocated(int size);

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void Allocate(int size)
        {
            var stats = new BufferManagerStats();
            var manager = CreateManager(stats);
            var alloc = manager.Allocate(size);

            Assert.True(size <= alloc.Sum(a => a.Length));
            Assert.Equal(CalculateAllocated(size), stats.Allocated.Allocated);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void AllocateAndFree(int size)
        {
            var stats = new BufferManagerStats();
            var manager = CreateManager(stats);
            var alloc = manager.Allocate(size);

            Assert.True(size <= alloc.Sum(a => a.Length));

            manager.Free(alloc);
            Assert.Equal(0, stats.Allocated.Allocated);
            Assert.Equal(CalculateAllocated(size), stats.Cached.Allocated);
        }

        [Theory]
        [InlineData(100, 100)]
        [InlineData(1000, 100)]
        [InlineData(10000, 100)]
        [InlineData(100000, 100)]
        public void MultipleAllocAndFree(int size, int count)
        {
            var stats = new BufferManagerStats();
            var manager = CreateManager(stats);

            for (int i = 0; i < count; i++)
            {
                var alloc = manager.Allocate(size);

                Assert.True(size <= alloc.Sum(a => a.Length));

                manager.Free(alloc);
            }

            //nothing allocated, 1 buffer cached
            Assert.Equal(0, stats.Allocated.Allocated);
            Assert.Equal(CalculateAllocated(size), stats.Cached.Allocated);

            var allocs = Enumerable.Range(1, count).Select(i => manager.Allocate(size)).ToList();

            //all count allocated, nothing cached
            Assert.Equal(CalculateAllocated(size) * count, stats.Allocated.Allocated);
            Assert.Equal(0, stats.Cached.Allocated);

            var first = allocs.Take(count / 2).ToArray();
            foreach (var b in first)
            {
                manager.Free(b);
                allocs.Remove(b);
            }

            //half and half allocated and cached
            Assert.Equal(CalculateAllocated(size) * count / 2, stats.Allocated.Allocated);
            Assert.Equal(CalculateAllocated(size) * count / 2, stats.Cached.Allocated);

            foreach (var b in allocs)
                manager.Free(b);

            //nothing allocated, all count cached
            Assert.Equal(0, stats.Allocated.Allocated);
            Assert.Equal(CalculateAllocated(size) * count, stats.Cached.Allocated);
        }

        [Theory]
        [InlineData(100, 10000)]
        [InlineData(1000, 10000)]
        [InlineData(10000, 10000)]
        [InlineData(100000, 10000)]
        [InlineData(4096, 10000)]
        public void ParallelAllocAndFree(int size, int count)
        {
            var stats = new BufferManagerStats();
            var manager = CreateManager(stats);
            var randomize = new Random((int) DateTime.Now.Ticks);

            Parallel.ForEach(Enumerable.Range(1, count), i =>
            {
                var alloc = manager.Allocate(size);

                Assert.True(size <= alloc.Sum(a => a.Length));

                manager.Free(alloc);
            });

            Assert.Equal(0, stats.Allocated.Allocated);
            Assert.True(stats.Cached.Allocated >= CalculateAllocated(size));
            Assert.True(stats.Cached.Allocated <= CalculateAllocated(size) * count);
        }
    }
}
