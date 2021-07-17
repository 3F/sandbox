using BenchmarkDotNet.Attributes;

namespace net.r_eg.sandbox.Hash.Tests
{
    public class FNV1aTest
    {
        /*
        |         Method |     Mean |   Error |  StdDev |
        |--------------- |---------:|--------:|--------:|
        |        Fnv1a64 | 146.4 ns | 0.87 ns | 0.82 ns |
        | Fnv1a128LX4Cnh | 787.3 ns | 2.62 ns | 2.32 ns |

        Fnv1a128LX4Cnh: Min = 782.364 ns, Q1 = 786.347 ns, Median = 787.392 ns, Q3 = 788.440 ns, Max = 791.124 ns
       */

        private const string MSG = "*LodgeX4CorrNoHigh* (LX4Cnh) algorithm of the high-speed multiplications of **128-bit** numbers (full range, 128 × 128).";

        [Benchmark]
        public void Fnv1a64()
        {
            _ = FNV1a.GetHash64(MSG);
        }

        [Benchmark]
        public void Fnv1a128LX4Cnh()
        {
            _ = FNV1a.GetHash128LX4Cnh(MSG, out ulong _);
        }
    }
}
