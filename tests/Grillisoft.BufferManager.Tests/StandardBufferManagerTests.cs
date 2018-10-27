using System;
using Xunit;
using Grillisoft.BufferManager;
using System.Linq;

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

            Assert.True(size <= alloc.Sum(i => i.Length));
        }
    }
}
