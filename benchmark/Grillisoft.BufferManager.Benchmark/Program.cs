using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using System.Linq;
using Grillisoft.BufferManager.Managed;

namespace Grillisoft.BufferManager.Benchmark
{
    public class Program
    {
        [ClrJob(baseline: true), CoreJob, MonoJob, CoreRtJob]
        [RPlotExporter, RankColumn]
        public class BufferManagerBenchmark
        {
            private Simple<byte> _simple;
            private Standard<byte> _standard;
            private Standard<byte> _standardNoClear;

            [Params(1024, 1024 * 16)]
            public int N;

            [Params(16, 1024)]
            public int M;

            [GlobalSetup]
            public void Setup()
            {
                _simple = new Simple<byte>();
                _standard = new Standard<byte>(true, N);
                _standardNoClear = new Standard<byte>(false, N);
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
                var alloc = Enumerable.Range(1, M).Select(i => manager.Allocate(N)).ToArray();
                foreach (var a in alloc)
                    manager.Free(a);
            }
        }

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BufferManagerBenchmark>();
        }
    }
}
