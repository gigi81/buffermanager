using System.Linq;
using BenchmarkDotNet.Attributes;
using Grillisoft.BufferManager.Managed;

namespace Grillisoft.BufferManager.Benchmark.Benchmarks
{
    [ClrJob(baseline: true), CoreJob]
    [RPlotExporter, RankColumn]
    public class SingleThreadedBufferManagerBenchmark
    {
        private IBufferManager<byte> _simple;
        private IBufferManager<byte> _standard;
        private IBufferManager<byte> _standardNoClear;

        [Params(1024, 1024 * 16)]
        public int BufferSize;

        [Params(512, 2048)]
        public int Allocations;

        [GlobalSetup]
        public void Setup()
        {
            _simple = new Simple<byte>();
            _standard = new Standard<byte>(true, BufferSize);
            _standardNoClear = new Standard<byte>(false, BufferSize);
        }

        [Benchmark]
        public void Simple()
        {
            AllocAndFree(_simple);
        }

        [Benchmark]
        public void Standard()
        {
            AllocAndFree(_standard);
        }

        [Benchmark]
        public void StandardNoClear()
        {
            AllocAndFree(_standardNoClear);
        }

        private void AllocAndFree(IBufferManager<byte> manager)
        {
            var alloc = Enumerable.Range(1, Allocations).Select(i => manager.Allocate(BufferSize)).ToArray();
            foreach (var a in alloc)
                manager.Free(a);
        }
    }
}
