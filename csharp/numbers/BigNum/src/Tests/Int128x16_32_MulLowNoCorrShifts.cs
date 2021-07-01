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

using System;
using System.Globalization;
using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace net.r_eg.sandbox.BigNum.Tests
{
    /// <summary>
    /// Part of https://twitter.com/github3F/status/1403748080760111106
    /// <br/>
    /// Designed for 128x16 (expandable up to 128x32).
    /// </summary>
    /// <remarks>For complete 128x128 see <see cref="Int128x128_LodgeX4CorrNoHigh60Mid6x2"/>.</remarks>
    public class Int128x16_32_MulLowNoCorrShifts
    {
        private const uint PRIME_16 = 0x4D2F;
        // * 128-bit number 0x6c62272e07bb014262b821756295c58d

        /*
        |                         Method |         Mean |      Error |     StdDev |
        |------------------------------- |-------------:|-----------:|-----------:|
        |            BigIntegerByteArray |   260.613 ns |  2.2352 ns |  2.0908 ns |
        |             BigIntegerParseHex |   944.771 ns |  9.7613 ns |  9.1307 ns |
        |             BigIntegerParseDec | 6,208.890 ns | 45.8376 ns | 40.6338 ns |
        |                  ManuallyBasic |    74.069 ns |  0.6500 ns |  0.6080 ns |
        |               ManuallyNoCopyTo |    31.808 ns |  0.2676 ns |  0.2503 ns |
        |              Manually4GetBytes |    43.697 ns |  0.2525 ns |  0.2361 ns |
        |        ManuallyIndividualBytes |    18.661 ns |  0.1670 ns |  0.1562 ns |
        | ManuallyIndividualBytesNoArray |     7.098 ns |  0.0738 ns |  0.0690 ns |
       */

        [Benchmark]
        public void BigIntegerByteArray()
        {
            byte[] input = { 0x8d, 0xc5, 0x95, 0x62, 0x75, 0x21, 0xb8, 0x62, 0x42, 0x01, 0xbb, 0x07, 0x2e, 0x27, 0x62, 0x6c };
            BigInteger bi = new BigInteger(input);

            bi *= PRIME_16;
            byte[] _ = bi.ToByteArray();
        }

        [Benchmark]
        public void BigIntegerParseHex()
        {
            var bi = BigInteger.Parse("6c62272e07bb014262b821756295c58d", NumberStyles.AllowHexSpecifier);

            bi *= PRIME_16;
            byte[] _ = bi.ToByteArray();
        }
        
        [Benchmark]
        public void BigIntegerParseDec()
        {
            var bi = BigInteger.Parse("144066263297769815596495629667062367629", NumberStyles.AllowLeadingSign);

            bi *= PRIME_16;
            byte[] _ = bi.ToByteArray();
        }

        [Benchmark]
        public void ManuallyBasic()
        {
            ulong[] def128 = { 0x6c62272e, 0x07bb0142, 0x62b82175, 0x6295c58d };
            unchecked
            {
                var a = def128[0] * PRIME_16;
                var b = def128[1] * PRIME_16;
                var c = def128[2] * PRIME_16;
                var d = def128[3] * PRIME_16;

                var _br1 = BitConverter.GetBytes((((c & 0xFFFFFFFF) + (d >> 32)) << 32) + (d & 0xFFFFFFFF));
                var _br2 = BitConverter.GetBytes((((a & 0xFFFFFFFF) + (b >> 32)) << 32) + (b & 0xFFFFFFFF) + (c >> 32));

                byte[] final = new byte[16];
                _br1.CopyTo(final, 0);
                _br2.CopyTo(final, 8);
            }
        }

        [Benchmark]
        public void ManuallyNoCopyTo()
        {
            unchecked
            {
                ulong[] def128 = { 0x6c62272e, 0x07bb0142, 0x62b82175, 0x6295c58d };

                var a = def128[0] * PRIME_16;
                var b = def128[1] * PRIME_16;
                var c = def128[2] * PRIME_16;
                var d = def128[3] * PRIME_16;

                var h = BitConverter.GetBytes((((c & 0xFFFFFFFF) + (d >> 32)) << 32) + (d & 0xFFFFFFFF));
                var l = BitConverter.GetBytes((((a & 0xFFFFFFFF) + (b >> 32)) << 32) + (b & 0xFFFFFFFF) + (c >> 32));

                byte[] _ =
                {
                    h[0], h[1], h[2], h[3], h[4], h[5], h[6], h[7],
                    l[0], l[1], l[2], l[3], l[4], l[5], l[6], l[7],
                };
            }
        }

        [Benchmark]
        public void Manually4GetBytes()
        {
            unchecked
            {
                ulong[] def128 = { 0x6c62272e, 0x07bb0142, 0x62b82175, 0x6295c58d };

                var a = def128[0] * PRIME_16;
                var b = def128[1] * PRIME_16;
                var c = def128[2] * PRIME_16;
                var d = def128[3] * PRIME_16;

                var _br1 = BitConverter.GetBytes(d & 0xFFFFFFFF);
                var _br2 = BitConverter.GetBytes((c & 0xFFFFFFFF) + (d >> 32));
                var _br3 = BitConverter.GetBytes((b & 0xFFFFFFFF) + (c >> 32));
                var _br4 = BitConverter.GetBytes((a & 0xFFFFFFFF) + (b >> 32));

                byte[] _ =
                {
                    _br1[0], _br1[1], _br1[2], _br1[3],
                    _br2[0], _br2[1], _br2[2], _br2[3],
                    _br3[0], _br3[1], _br3[2], _br3[3],
                    _br4[0], _br4[1], _br4[2], _br4[3],
                };
            }
        }

        [Benchmark]
        public void ManuallyIndividualBytes()
        {
            unchecked
            {
                ulong[] def128 = { 0x6c62272e, 0x07bb0142, 0x62b82175, 0x6295c58d };

                var a = def128[0] * PRIME_16;
                var b = def128[1] * PRIME_16;
                var c = def128[2] * PRIME_16;
                var d = def128[3] * PRIME_16;

                var r2 = (c & 0xFFFFFFFF) + (d >> 32);
                var r3 = (b & 0xFFFFFFFF) + (c >> 32);
                var r4 = (a & 0xFFFFFFFF) + (b >> 32);

                byte[] _ =
                {
                    (byte)(d & 0xFF), (byte)(d >> 8 & 0xFF), (byte)(d >> 16 & 0xFF), (byte)(d >> 24 & 0xFF),
                    (byte)(r2 & 0xFF), (byte)(r2 >> 8 & 0xFF), (byte)(r2 >> 16 & 0xFF), (byte)(r2 >> 24 & 0xFF),
                    (byte)(r3 & 0xFF), (byte)(r3 >> 8 & 0xFF), (byte)(r3 >> 16 & 0xFF), (byte)(r3 >> 24 & 0xFF),
                    (byte)(r4 & 0xFF), (byte)(r4 >> 8 & 0xFF), (byte)(r4 >> 16 & 0xFF), (byte)(r4 >> 24 & 0xFF),
                };
            }
        }

        [Benchmark]
        public void ManuallyIndividualBytesNoArray()
        {
            ulong a = 0x6c62272e, b = 0x07bb0142, c = 0x62b82175, d = 0x6295c58d, r2, r3, r4;
            unchecked
            {
                a *= PRIME_16;
                b *= PRIME_16;
                c *= PRIME_16;
                d *= PRIME_16;
            }

            r2 = (c & 0xFFFFFFFF) + (d >> 32);
            r3 = (b & 0xFFFFFFFF) + (c >> 32);
            r4 = (a & 0xFFFFFFFF) + (b >> 32);

            byte[] _ =
            {
                (byte)(d & 0xFF), (byte)(d >> 8 & 0xFF), (byte)(d >> 16 & 0xFF), (byte)(d >> 24 & 0xFF),
                (byte)(r2 & 0xFF), (byte)(r2 >> 8 & 0xFF), (byte)(r2 >> 16 & 0xFF), (byte)(r2 >> 24 & 0xFF),
                (byte)(r3 & 0xFF), (byte)(r3 >> 8 & 0xFF), (byte)(r3 >> 16 & 0xFF), (byte)(r3 >> 24 & 0xFF),
                (byte)(r4 & 0xFF), (byte)(r4 >> 8 & 0xFF), (byte)(r4 >> 16 & 0xFF), (byte)(r4 >> 24 & 0xFF),
            };
        }
    }
}
