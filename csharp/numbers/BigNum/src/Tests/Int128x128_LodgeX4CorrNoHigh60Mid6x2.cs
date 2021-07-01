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
    /// Speed tests of <see cref="LodgeX4CorrNoHigh"/> implementation.
    /// </summary>
    /// <remarks>https://twitter.com/github3F/status/1410358979033813000</remarks>
    public class Int128x128_LodgeX4CorrNoHigh60Mid6x2
    {
        /*
        |                               Method |       Mean |     Error |    StdDev |
        |------------------------------------- |-----------:|----------:|----------:|
        |                    BigInteger128x128 | 402.457 ns | 2.3621 ns | 1.9724 ns |
        |       LodgeX4CorrNoHigh128x128_Bytes |  30.842 ns | 0.1940 ns | 0.1720 ns |
        |      LodgeX4CorrNoHigh128x128_UInt64 |  21.077 ns | 0.1118 ns | 0.1046 ns |
        | LodgeX4CorrNoHigh128x128_UInt64via32 |  18.991 ns | 0.0841 ns | 0.0787 ns |
        |   LodgeX4CorrNoHigh128x128_EmbdBytes |  15.342 ns | 0.0596 ns | 0.0529 ns |
        | LodgeX4CorrNoHigh128x128_10x_EmbdVar |   5.604 ns | 0.0276 ns | 0.0244 ns |
         */

        [Benchmark]
        public void BigInteger128x128()
        {
            BigInteger bi = new BigInteger(new byte[] { 0xEF, 0xF2, 0x6E, 0xBC, 0xCC, 0x70, 0xEF, 0x81, 0xED, 0x0F, 0xF3, 0x80, 0x19, 0x27, 0xF4, 0xC1 });
            BigInteger bim = new BigInteger(new byte[] { 0x94, 0xFF, 0x30, 0xEA, 0xF1, 0xBE, 0x49, 0x17, 0xD2, 0xAC, 0xD0, 0x42, 0x01, 0x3F, 0xF0, 0xDE });

            byte[] _ = (bi * bim).ToByteArray();
        }

        [Benchmark]
        public void LodgeX4CorrNoHigh128x128_Bytes()
        {
            byte[] _ = LodgeX4CorrNoHigh.Multiply
            (
                0xC1F42719, 0x80F30FED, 0x81EF70CC, 0xBC6EF2EF,
                0xDEF03F01, 0x42D0ACD2, 0x1749BEF1, 0xEA30FF94
            );
        }

        [Benchmark]
        public void LodgeX4CorrNoHigh128x128_UInt64()
        {
            ulong _ = LodgeX4CorrNoHigh.Multiply
            (
                0xC1F4271980F30FED, 0x81EF70CCBC6EF2EF,
                0xDEF03F0142D0ACD2, 0x1749BEF1EA30FF94,
                out _
            );
        }

        [Benchmark]
        public void LodgeX4CorrNoHigh128x128_UInt64via32()
        {
            ulong _ = LodgeX4CorrNoHigh.Multiply
            (
                0xC1F42719, 0x80F30FED, 0x81EF70CC, 0xBC6EF2EF,
                0xDEF03F01, 0x42D0ACD2, 0x1749BEF1, 0xEA30FF94,
                out _
            );
        }


        [Benchmark]
        public void LodgeX4CorrNoHigh128x128_EmbdBytes()
        {
            uint a = 0xC1F42719, b = 0x80F30FED, c = 0x81EF70CC, d = 0xBC6EF2EF;
            uint ma = 0xDEF03F01, mb = 0x42D0ACD2, mc = 0x1749BEF1, md = 0xEA30FF94;

            ulong high, low;
            unchecked{/*(c) Denis Kuzmin <x-3F@outlook.com> github/3F */ulong A=(ulong)b*mb;ulong B=A&0xFFFF_FFFF;ulong C=((A>>32)+B+(a*ma))&0xFFFF_FFFF;ulong D=(a>b)?a-b:b-a;ulong E=(ma>mb)?ma-mb:mb-ma;if(D!=0&&E!=0){ulong F=D*E;if((!(a>b)&&(ma>mb))||((a>b)&&!(ma>mb))){C+=F&0xFFFF_FFFF;}else{C-=F&0xFFFF_FFFF;}}ulong G=(C<<32)+B;A=(ulong)c*mc;ulong H=(ulong)d*md;B=(H>>32)+(H&0xFFF_FFFF_FFFF_FFFF)+(A&0xFFF_FFFF_FFFF_FFFF)+((A&0xFFF_FFFF)<<32);C=((((A>>28)+(A>>60)+(H>>60))<<28)>>16)+(B>>48);ulong I=B&0xFFFF_FFFF_FFFF;D=(c>d)?c-d:d-c;E=(mc>md)?mc-md:md-mc;if(D!=0&&E!=0){ulong F=D*E;ulong J=(F>>48);ulong K=F&0xFFFF_FFFF_FFFF;B=I;if((!(c>d)&&(mc>md))||((c>d)&&!(mc>md))){I+=K;C+=J;if(B>(I&0xFFFF_FFFF_FFFF))++C;}else{I-=K;C-=J;if(B<(I&0xFFFF_FFFF_FFFF))--C;}}ulong L=((I&0xFFFF_FFFF)<<32)+(H&0xFFFF_FFFF);C=G+L+((C<<16)+((I>>32)&0xFFFF));G=((ulong)a<<32)+b;I=((ulong)c<<32)+d;A=((ulong)ma<<32)+mb;H=((ulong)mc<<32)+md;D=(G>I)?G-I:I-G;E=(A>H)?A-H:H-A;if(D!=0&&E!=0){ulong F=D*E;if((!(G>I)&&(A>H))||((G>I)&&!(A>H))){C+=F;}else{C-=F;}}low=L;high=C;}

            byte[] _ =
            {
                (byte)(low & 0xFF),
                (byte)(low >> 8 & 0xFF),
                (byte)(low >> 16 & 0xFF),
                (byte)(low >> 24 & 0xFF),
                (byte)(low >> 32 & 0xFF),
                (byte)(low >> 40 & 0xFF),
                (byte)(low >> 48 & 0xFF),
                (byte)(low >> 56 & 0xFF),

                (byte)(high & 0xFF),
                (byte)(high >> 8 & 0xFF),
                (byte)(high >> 16 & 0xFF),
                (byte)(high >> 24 & 0xFF),
                (byte)(high >> 32 & 0xFF),
                (byte)(high >> 40 & 0xFF),
                (byte)(high >> 48 & 0xFF),
                (byte)(high >> 56 & 0xFF),
            };
        }

        [Benchmark]
        public void LodgeX4CorrNoHigh128x128_10x_EmbdVar()
        {
            uint a = 0xC1F42719, b = 0x80F30FED, c = 0x81EF70CC, d = 0xBC6EF2EF;
            uint ma = 0xDEF03F01, mb = 0x42D0ACD2, mc = 0x1749BEF1, md = 0xEA30FF94;

            ulong high, low;

            for(int i = 0; i < 10; ++i)
                unchecked{/*(c) Denis Kuzmin <x-3F@outlook.com> github/3F */ulong A=(ulong)b*mb;ulong B=A&0xFFFF_FFFF;ulong C=((A>>32)+B+(a*ma))&0xFFFF_FFFF;ulong D=(a>b)?a-b:b-a;ulong E=(ma>mb)?ma-mb:mb-ma;if(D!=0&&E!=0){ulong F=D*E;if((!(a>b)&&(ma>mb))||((a>b)&&!(ma>mb))){C+=F&0xFFFF_FFFF;}else{C-=F&0xFFFF_FFFF;}}ulong G=(C<<32)+B;A=(ulong)c*mc;ulong H=(ulong)d*md;B=(H>>32)+(H&0xFFF_FFFF_FFFF_FFFF)+(A&0xFFF_FFFF_FFFF_FFFF)+((A&0xFFF_FFFF)<<32);C=((((A>>28)+(A>>60)+(H>>60))<<28)>>16)+(B>>48);ulong I=B&0xFFFF_FFFF_FFFF;D=(c>d)?c-d:d-c;E=(mc>md)?mc-md:md-mc;if(D!=0&&E!=0){ulong F=D*E;ulong J=(F>>48);ulong K=F&0xFFFF_FFFF_FFFF;B=I;if((!(c>d)&&(mc>md))||((c>d)&&!(mc>md))){I+=K;C+=J;if(B>(I&0xFFFF_FFFF_FFFF))++C;}else{I-=K;C-=J;if(B<(I&0xFFFF_FFFF_FFFF))--C;}}ulong L=((I&0xFFFF_FFFF)<<32)+(H&0xFFFF_FFFF);C=G+L+((C<<16)+((I>>32)&0xFFFF));G=((ulong)a<<32)+b;I=((ulong)c<<32)+d;A=((ulong)ma<<32)+mb;H=((ulong)mc<<32)+md;D=(G>I)?G-I:I-G;E=(A>H)?A-H:H-A;if(D!=0&&E!=0){ulong F=D*E;if((!(G>I)&&(A>H))||((G>I)&&!(A>H))){C+=F;}else{C-=F;}}low=L;high=C;}
        }
    }
}
