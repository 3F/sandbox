using Xunit;

namespace net.r_eg.sandbox.Hash.Tests
{
    public class HashValues
    {
        [Fact]
        public void Test1()
        {
            const string _MSG = "*LodgeX4CorrNoHigh* (LX4Cnh) algorithm of the high-speed multiplications of **128-bit** numbers (full range, 128 × 128).";

            Assert.Equal
            (
                FNV1a.GetHash128Call(_MSG, out ulong low1),
                FNV1a.GetHash128LX4Cnh(_MSG, out ulong low2)
            );
            Assert.Equal(low1, low2);
        }

        [Fact]
        public void Test2()
        {
            for(int i = 0; i < 1000; ++i)
            {
                string msg = $"LodgeX4CorrNoHigh (LX4Cnh) for {System.Guid.NewGuid()}";

                Assert.Equal
                (
                    FNV1a.GetHash128Call(msg, out ulong low1),
                    FNV1a.GetHash128LX4Cnh(msg, out ulong low2)
                );
                Assert.Equal(low1, low2);
            }
        }
    }
}
