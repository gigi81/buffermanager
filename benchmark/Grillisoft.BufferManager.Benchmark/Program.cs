using BenchmarkDotNet.Running;
using Grillisoft.BufferManager.Benchmark.Benchmarks;

namespace Grillisoft.BufferManager.Benchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var summary1 = BenchmarkRunner.Run<SingleThreadedBufferManagerBenchmark>();
            var summary2 = BenchmarkRunner.Run<MultiThreadedBufferManagerBenchmark>();
        }
    }
}
