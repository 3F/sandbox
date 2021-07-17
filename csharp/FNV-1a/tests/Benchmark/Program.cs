using BenchmarkDotNet.Running;

namespace net.r_eg.sandbox.Hash.Tests
{
    class Program
    {
        static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
