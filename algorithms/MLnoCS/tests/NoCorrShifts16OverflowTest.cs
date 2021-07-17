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
using System.Linq;
using net.r_eg.sandbox.algorithms;
using Xunit;

namespace Tests
{
    using static _svc.Members;

    public class NoCorrShifts16OverflowTest
    {
        [Fact]
        public void MulManyTest1()
        {
            ushort prime = 0x6D5C;

            uint a = 0x4BD4823E, b = 0xCC5D03EB, c = 0x19E07DB8, d = 0xFFD5DABE;

            byte[] bprime = BitConverter.GetBytes(prime).ToArray();

            for(int i = 0; i < 500; ++i)
            {
                byte[] bi = MultiplyViaBigInteger
                (
                    BitConverter.GetBytes(((ulong)c << 32) + d).Concat(BitConverter.GetBytes(((ulong)a << 32) + b)).ToArray(),
                    bprime
                );

                MulLowNoCorrShifts16.Multiply
                (
                    ref a, ref b, ref c, ref d,
                    prime
                );

                if(bi.Length == 1)
                {
                    Assert.True(0 == d && 0 == c && 0 == b && 0 == a);
                }
                else
                {
                    Assert.Equal(BitConverter.ToUInt32(bi, 0), d);
                    Assert.Equal(BitConverter.ToUInt32(bi, 4), c);
                    Assert.Equal(BitConverter.ToUInt32(bi, 8), b);
                    Assert.Equal(BitConverter.ToUInt32(bi, 12), a);
                }
            }
        }

        [Fact]
        public void MulOrderTest1()
        {
            ulong inputHigh = 0x4BD4823ECC5D03EB;
            ulong inputLow = 0x19E07DB8FFD5DABE;

            ulong high = MulLowNoCorrShifts16.Multiply
            (
                inputHigh, inputLow,
                1,
                out ulong low
            );

            Assert.Equal(inputHigh, high);
            Assert.Equal(inputLow, low);
        }
    }
}
