/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) sandbox contributors https://github.com/3F/sandbox/graphs/contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System.Numerics;
using BenchmarkDotNet.Attributes;
using net.r_eg.sandbox.algorithms;

namespace net.r_eg.sandbox.BigNum.Tests
{
    /// <summary>
    /// Part of https://twitter.com/github3F/status/1403748080760111106
    /// <br/>
    /// Designed for 128x16 (theoretically up to 128x32).
    /// </summary>
    /// <remarks>For complete 128x128 see <see cref="Int128x128_LodgeX4CorrNoHigh"/>.</remarks>
    public class Int128x16_MLnoCS
    {
        /*
        |              Method |        Mean |     Error |    StdDev |
        |-------------------- |------------:|----------:|----------:|
        | BigIntegerByteArray | 258.8748 ns | 2.9527 ns | 2.7619 ns |
        |         MLnoCSBytes |  16.0659 ns | 0.0523 ns | 0.0437 ns |
        |         MLnoCSVia32 |   4.6830 ns | 0.0331 ns | 0.0309 ns |
        |         MLnoCSVia64 |   7.7752 ns | 0.0294 ns | 0.0260 ns |
        |       MLnoCSEmbdVar |   0.3040 ns | 0.0099 ns | 0.0088 ns |
       */

        private const ushort PRIME_16 = 0x4D2F;
        // * 128-bit number 0x6c62272e07bb014262b821756295c58d

        private uint ra, rb, rc, rd;

        [Benchmark]
        public void BigIntegerByteArray()
        {
            byte[] input = { 0x8d, 0xc5, 0x95, 0x62, 0x75, 0x21, 0xb8, 0x62, 0x42, 0x01, 0xbb, 0x07, 0x2e, 0x27, 0x62, 0x6c };
            BigInteger bi = new BigInteger(input);

            bi *= PRIME_16;
            byte[] _ = bi.ToByteArray();
        }

        [Benchmark]
        public void MLnoCSBytes()
        {
            uint a = 0x6c62272e, b = 0x07bb0142, c = 0x62b82175, d = 0x6295c58d;

            byte[] _ = MulLowNoCorrShifts16.Multiply
            (
                a, b, c, d,
                PRIME_16
            );
        }

        [Benchmark]
        public void MLnoCSVia32()
        {
            uint a = 0x6c62272e, b = 0x07bb0142, c = 0x62b82175, d = 0x6295c58d;

            MulLowNoCorrShifts16.Multiply
            (
                a, b, c, d,
                PRIME_16,
                out uint _, out uint _, out uint _, out uint _
            );
        }

        [Benchmark]
        public void MLnoCSVia64()
        {
            ulong a = 0x6c62272e07bb0142, b = 0x62b821756295c58d;

            ulong _ = MulLowNoCorrShifts16.Multiply
            (
                a, b,
                PRIME_16,
                out ulong _
            );
        }

        [Benchmark]
        public void MLnoCSEmbdVar()
        {
            uint a = 0x6c62272e, b = 0x07bb0142, c = 0x62b82175, d = 0x6295c58d;
            ushort prime = PRIME_16;
            //-
            unchecked{/* MLnoCS (c) Denis Kuzmin <x-3F@outlook.com> github/3F */ulong e=a,f=b,g=c,h=d,l,o,k;e*=prime;f*=prime;g*=prime;h*=prime;l=(g&0xFFFF_FFFF)+(h>>32);o=(f&0xFFFF_FFFF)+(g>>32);k=(e&0xFFFF_FFFF)+(f>>32);ra=(uint)k;rb=(uint)o;rc=(uint)l;rd=(uint)h;}

            _ = ra;
            _ = rb;
            _ = rc;
            _ = rd;
        }
    }
}
