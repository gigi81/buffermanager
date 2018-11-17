using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System;
using Grillisoft.BufferManager.Unmanaged;
using Grillisoft.BufferManager.Statistics;

namespace Grillisoft.BufferManager.Tests
{
    public class StandardUnmanagedBufferManagerTests
    {
        private static int BufferSize => Environment.SystemPageSize;

        protected IUnmanagedBufferManager CreateManager(BufferManagerStats stats)
        {
            return new ConcurrentProxy(new Standard(false, BufferSize, stats.Allocated, stats.Cached));
        }

        protected int CalculateAllocated(int size, int count = 1)
        {
            return ((((size - 1) / BufferSize) + 1) * BufferSize) * count;
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void Allocate(int size)
        {
            var stats = new BufferManagerStats();

            using (var manager = CreateManager(stats))
            {
                var alloc = manager.Allocate(size);
                Assert.Equal(CalculateAllocated(size), alloc.Sum(a => a.Size));
                Assert.Equal(CalculateAllocated(size), stats.Allocated.Allocated);
            }

            Assert.Equal(0, stats.Allocated.Allocated);
            Assert.Equal(0, stats.Cached.Allocated);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void AllocateAndFree(int size)
        {
            var stats = new BufferManagerStats();
            using (var manager = CreateManager(stats))
            {
                var alloc = manager.Allocate(size);

                Assert.Equal(CalculateAllocated(size), alloc.Sum(a => a.Size));
                Assert.Equal(CalculateAllocated(size), stats.Allocated.Allocated);

                manager.Free(alloc);
                Assert.Equal(0, stats.Allocated.Allocated);
                Assert.Equal(CalculateAllocated(size), stats.Cached.Allocated);
            }

            Assert.Equal(0, stats.Allocated.Allocated);
            Assert.Equal(0, stats.Cached.Allocated);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void AllocateSingleAndFree(int suggestedSize)
        {
            var stats = new BufferManagerStats();
            using (var manager = CreateManager(stats))
            {
                var alloc = manager.AllocateSingle(suggestedSize);

                Assert.Equal(BufferSize, alloc.Size);
                Assert.Equal(BufferSize, stats.Allocated.Allocated);

                manager.Free(alloc);

                Assert.Equal(0, stats.Allocated.Allocated);
                Assert.Equal(BufferSize, stats.Cached.Allocated);
            }

            Assert.Equal(0, stats.Allocated.Allocated);
            Assert.Equal(0, stats.Cached.Allocated);
        }

        [Theory]
        [InlineData(100, 100)]
        [InlineData(1000, 100)]
        [InlineData(10000, 100)]
        [InlineData(100000, 100)]
        public void MultipleAllocAndFree(int size, int count)
        {
            var stats = new BufferManagerStats();
            using (var manager = CreateManager(stats))
            {
                for (int i = 0; i < count; i++)
                {
                    var alloc = manager.Allocate(size);
                    manager.Free(alloc);
                }

                //nothing allocated, 1 buffer cached
                Assert.Equal(0, stats.Allocated.Allocated);
                Assert.Equal(CalculateAllocated(size), stats.Cached.Allocated);

                var allocs = Enumerable.Range(1, count).Select(i => manager.Allocate(size)).ToList();

                //all count allocated, nothing cached
                Assert.Equal(CalculateAllocated(size, count), allocs.Sum(b => b.Sum(b1 => b1.Size)));
                Assert.Equal(CalculateAllocated(size, count), stats.Allocated.Allocated);
                Assert.Equal(0, stats.Cached.Allocated);

                var first = allocs.Take(count / 2).ToArray();
                foreach (var b in first)
                {
                    manager.Free(b);
                    allocs.Remove(b);
                }

                //half and half allocated and cached
                Assert.Equal(CalculateAllocated(size, count) / 2, stats.Allocated.Allocated);
                Assert.Equal(CalculateAllocated(size, count) / 2, stats.Cached.Allocated);

                foreach (var b in allocs)
                    manager.Free(b);

                //nothing allocated, all count cached
                Assert.Equal(0, stats.Allocated.Allocated);
                Assert.Equal(CalculateAllocated(size, count), stats.Cached.Allocated);
            }

            Assert.Equal(0, stats.Allocated.Allocated);
            Assert.Equal(0, stats.Cached.Allocated);
        }

        [Theory]
        [InlineData(100, 10000)]
        [InlineData(1000, 10000)]
        [InlineData(10000, 10000)]
        [InlineData(100000, 10000)]
        [InlineData(4096, 10000)]
        public void ParallelAllocAndFree(int size, int count)
        {
            var randomize = new Random((int)DateTime.Now.Ticks);
            var stats = new BufferManagerStats();

            using (var manager = CreateManager(stats))
            {
                Parallel.ForEach(Enumerable.Range(1, count), i =>
                {
                    var alloc = manager.Allocate(size);
                    manager.Free(alloc);
                });

                Assert.Equal(0, stats.Allocated.Allocated);
                Assert.True(stats.Cached.Allocated >= CalculateAllocated(size));
                Assert.True(stats.Cached.Allocated <= CalculateAllocated(size) * count);
            }

            Assert.Equal(0, stats.Allocated.Allocated);
            Assert.Equal(0, stats.Cached.Allocated);
        }
    }
}
