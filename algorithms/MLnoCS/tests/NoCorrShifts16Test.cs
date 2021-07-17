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
using System.Collections.Generic;
using net.r_eg.sandbox.algorithms;
using Xunit;

namespace Tests
{
    using static _svc.Members;

    public class NoCorrShifts16Test
    {
        public static IEnumerable<object[]> GetMulNumbers()
        {
            yield return new object[]
            {
                new byte[] { 0xEF, 0xF2, 0x6E, 0xBC, 0xCC, 0x70, 0xEF, 0x81, 0xED, 0x0F, 0xF3, 0x80, 0x19, 0x27, 0xF4, 0xC1 },
                new byte[] { 0x36, 0x10, 0 },
                0xC1F42719, 0x80F30FED, 0x81EF70CC, 0xBC6EF2EF,
                0x1036
            };

            yield return new object[]
            {
                new byte[] { 0x8d, 0xc5, 0x95, 0x62, 0x75, 0x21, 0xb8, 0x62, 0x42, 0x01, 0xbb, 0x07, 0x2e, 0x27, 0x62, 0x6c },
                new byte[] { 0x2F, 0x4D, 0},
                0x6c62272e, 0x07bb0142, 0x62b82175, 0x6295c58d,
                0x4D2F
            };

            yield return new object[]
            {
                new byte[] { 0x11, 0xFF, 0xEE, 0xDD, 0xCC, 0xBB, 0xAA, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 },
                new byte[] { 0x00, 0xEA, 0 },
                0x11223344, 0x55667788, 0x99AABBCC, 0xDDEEFF11,
                0xEA00
            };

            yield return new object[]
            {
                new byte[] { 0x11, 0xFF, 0xEE, 0xDD, 0xCC, 0xBB, 0xAA, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 },
                new byte[] { 0xBC, 0xFA, 0 },
                0x11223344, 0x55667788, 0x99AABBCC, 0xDDEEFF11,
                0xFABC
            };

            yield return new object[]
            {
                new byte[] { 0x11, 0xFF, 0xEE, 0xDD, 0xCC, 0xBB, 0xAA, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 },
                new byte[] { 0x22, 0x11, 0 },
                0x11223344, 0x55667788, 0x99AABBCC, 0xDDEEFF11,
                0x1122
            };
        }

        [Theory]
        [MemberData(nameof(GetMulNumbers))]
        public void MulVia32BytesTest1(byte[] input, byte[] mul, uint a, uint b, uint c, uint d, ushort prime)
        {
            byte[] result = MulLowNoCorrShifts16.Multiply
            (
                a, b, c, d,
                prime
            );

            Assert.Equal(MultiplyViaBigInteger(input, mul), result);
        }

        [Theory]
        [MemberData(nameof(GetMulNumbers))]
        public void MulVia32Test1(byte[] input, byte[] mul, uint a, uint b, uint c, uint d, ushort prime)
        {
            MulLowNoCorrShifts16.Multiply
            (
                a, b, c, d,
                prime,
                out uint ha, out uint hb, out uint la, out uint lb
            );

            byte[] data = MultiplyViaBigInteger(input, mul);

            Assert.Equal(BitConverter.ToUInt32(data, 0), lb);
            Assert.Equal(BitConverter.ToUInt32(data, 4), la);
            Assert.Equal(BitConverter.ToUInt32(data, 8), hb);
            Assert.Equal(BitConverter.ToUInt32(data, 12), ha);
        }

        [Theory]
        [MemberData(nameof(GetMulNumbers))]
        public void MulVia32RefTest1(byte[] input, byte[] mul, uint a, uint b, uint c, uint d, ushort prime)
        {
            MulLowNoCorrShifts16.Multiply
            (
                ref a, ref b, ref c, ref d,
                prime
            );

            byte[] data = MultiplyViaBigInteger(input, mul);

            Assert.Equal(BitConverter.ToUInt32(data, 0), d);
            Assert.Equal(BitConverter.ToUInt32(data, 4), c);
            Assert.Equal(BitConverter.ToUInt32(data, 8), b);
            Assert.Equal(BitConverter.ToUInt32(data, 12), a);
        }

        [Theory]
        [MemberData(nameof(GetMulNumbers))]
        public void MulVia64Test1(byte[] input, byte[] mul, uint a, uint b, uint c, uint d, ushort prime)
        {
            ulong high = MulLowNoCorrShifts16.Multiply
            (
                ((ulong)a << 32) + b, ((ulong)c << 32) + d,
                prime,
                out ulong low
            );

            byte[] data = MultiplyViaBigInteger(input, mul);

            Assert.Equal(BitConverter.ToUInt64(data, 0), low);
            Assert.Equal(BitConverter.ToUInt64(data, 8), high);
        }

        [Theory]
        [MemberData(nameof(GetMulNumbers))]
        public void MulVia32EmbdTest1(byte[] input, byte[] mul, uint a, uint b, uint c, uint d, ushort prime)
        {
            uint ra, rb, rc, rd;
            unchecked{/* MLnoCS (c) Denis Kuzmin <x-3F@outlook.com> github/3F */ulong e=a,f=b,g=c,h=d,l,o,k;e*=prime;f*=prime;g*=prime;h*=prime;l=(g&0xFFFF_FFFF)+(h>>32);o=(f&0xFFFF_FFFF)+(g>>32);k=(e&0xFFFF_FFFF)+(f>>32);ra=(uint)k;rb=(uint)o;rc=(uint)l;rd=(uint)h;}

            byte[] data = MultiplyViaBigInteger(input, mul);

            Assert.Equal(BitConverter.ToUInt32(data, 0), rd);
            Assert.Equal(BitConverter.ToUInt32(data, 4), rc);
            Assert.Equal(BitConverter.ToUInt32(data, 8), rb);
            Assert.Equal(BitConverter.ToUInt32(data, 12), ra);
        }

        [Theory]
        [MemberData(nameof(GetMulNumbers))]
        public void MulVia32MutableEmbdTest1(byte[] input, byte[] mul, uint a, uint b, uint c, uint d, ushort prime)
        {
            unchecked{/*[mutable] MLnoCS (c) Denis Kuzmin <x-3F@outlook.com> github/3F */ulong e=a,f=b,g=c,h=d,l,o,k;e*=prime;f*=prime;g*=prime;h*=prime;l=(g&0xFFFF_FFFF)+(h>>32);o=(f&0xFFFF_FFFF)+(g>>32);k=(e&0xFFFF_FFFF)+(f>>32);a=(uint)k;b=(uint)o;c=(uint)l;d=(uint)h;}

            byte[] data = MultiplyViaBigInteger(input, mul);

            Assert.Equal(BitConverter.ToUInt32(data, 0), d);
            Assert.Equal(BitConverter.ToUInt32(data, 4), c);
            Assert.Equal(BitConverter.ToUInt32(data, 8), b);
            Assert.Equal(BitConverter.ToUInt32(data, 12), a);
        }
    }
}
