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

/*
 History:
    * 2021.06.15: First idea/redesign after 'MulLowNoCorrShifts' algo (128-bit x 16 up to 32) https://twitter.com/github3F/status/1404187562592309256
    * 2021.06.21: Logic formalization. Variations, calculations, and scaling.
    * 2021.06.27: First draft implementation using .NET/C#.
    * 2021.06.30: Final testing and optimizations for dotnet impl https://twitter.com/github3F/status/1410358979033813000
    * 2021.07.01: First public open source version. https://github.com/3F
 */

namespace net.r_eg.sandbox.algorithms
{
    /// <summary>
    /// The high-speed multiplications of 128-bit numbers. <br/>
    /// Use embeddable version to reach super speed ~0.02 ns per multiplication
    /// </summary>
    /// <remarks>https://twitter.com/github3F/status/1410358979033813000</remarks>
    public static class LodgeX4CorrNoHigh
    {
        /* --- Embeddable superfast version --- */

        // One 128x128 multiplication requires less than ~ 0.02 ns == 0.00000000002 sec (1 ns == 0.000000001 sec)

        //uint a = 0xC1F42719, b = 0x80F30FED, c = 0x81EF70CC, d = 0xBC6EF2EF;
        //uint ma = 0xDEF03F01, mb = 0x42D0ACD2, mc = 0x1749BEF1, md = 0xEA30FF94;
        ////-
        //ulong high, low;
        //unchecked{/*(c) Denis Kuzmin <x-3F@outlook.com> github/3F */ulong A=(ulong)b*mb;ulong B=A&0xFFFF_FFFF;ulong C=((A>>32)+B+(a*ma))&0xFFFF_FFFF;ulong D=(a>b)?a-b:b-a;ulong E=(ma>mb)?ma-mb:mb-ma;if(D!=0&&E!=0){ulong F=D*E;if((!(a>b)&&(ma>mb))||((a>b)&&!(ma>mb))){C+=F&0xFFFF_FFFF;}else{C-=F&0xFFFF_FFFF;}}ulong G=(C<<32)+B;A=(ulong)c*mc;ulong H=(ulong)d*md;B=(H>>32)+(H&0xFFF_FFFF_FFFF_FFFF)+(A&0xFFF_FFFF_FFFF_FFFF)+((A&0xFFF_FFFF)<<32);C=((((A>>28)+(A>>60)+(H>>60))<<28)>>16)+(B>>48);ulong I=B&0xFFFF_FFFF_FFFF;D=(c>d)?c-d:d-c;E=(mc>md)?mc-md:md-mc;if(D!=0&&E!=0){ulong F=D*E;ulong J=(F>>48);ulong K=F&0xFFFF_FFFF_FFFF;B=I;if((!(c>d)&&(mc>md))||((c>d)&&!(mc>md))){I+=K;C+=J;if(B>(I&0xFFFF_FFFF_FFFF))++C;}else{I-=K;C-=J;if(B<(I&0xFFFF_FFFF_FFFF))--C;}}ulong L=((I&0xFFFF_FFFF)<<32)+(H&0xFFFF_FFFF);C=G+L+((C<<16)+((I>>32)&0xFFFF));G=((ulong)a<<32)+b;I=((ulong)c<<32)+d;A=((ulong)ma<<32)+mb;H=((ulong)mc<<32)+md;D=(G>I)?G-I:I-G;E=(A>H)?A-H:H-A;if(D!=0&&E!=0){ulong F=D*E;if((!(G>I)&&(A>H))||((G>I)&&!(A>H))){C+=F;}else{C-=F;}}low=L;high=C;}

        /* -- */

        /// <summary>
        /// High-speed multiplication of a 128-bit x 128-bit numbers.
        /// </summary>
        /// <param name="a">The first high 32 bits of the input value.</param>
        /// <param name="b">Second high 32 bits of the input value.</param>
        /// <param name="c">The first low 32 bits of the input value.</param>
        /// <param name="d">Second low 32 bits of the input value.</param>
        /// <param name="ma">The first high 32 bits of the multiplier.</param>
        /// <param name="mb">Second high 32 bits of the multiplier.</param>
        /// <param name="mc">The first low 32 bits of the multiplier.</param>
        /// <param name="md">Second low 32 bits of the multiplier.</param>
        /// <param name="low">Low 6 bits from a 128-bit result.</param>
        /// <returns>High 64 bits from a 128-bit result.</returns>
        public static ulong Multiply(uint a, uint b, uint c, uint d, uint ma, uint mb, uint mc, uint md, out ulong low)
        {
            unchecked
            {
                ulong r = (ulong)b * mb; // r12
                ulong v = r & 0xFFFF_FFFF; // v1Low

                // we do not use the high bytes in the first block, therefore first low 4 bytes will be enough
                ulong f = ((r >> 32) + v + (a * ma)) & 0xFFFF_FFFF; // f12

                ulong d1 = AbsMinus(a, b); //d11, 0 - FFFF_FFFF
                ulong d2 = AbsMinus(ma, mb); //d12

                if(d1 != 0 && d2 != 0)
                {
                    ulong dd = d1 * d2; // 0 - FFFF_FFFE_0000_0001

                    if((!(a > b) && (ma > mb)) || ((a > b) && !(ma > mb)))
                    {
                        f += dd & 0xFFFF_FFFF;
                    }
                    else
                    {
                        f -= dd & 0xFFFF_FFFF;
                    }
                }

                ulong fHigh = (f << 32) + v;

                r /*r21*/   = (ulong)c * mc;
                ulong r2    = (ulong)d * md;

                v = (r2 >> 32) + (r2 & 0xFFF_FFFF_FFFF_FFFF) + (r & 0xFFF_FFFF_FFFF_FFFF) + ((r & 0xFFF_FFFF) << 32); // v2Middle

                f /*f21*/   = ((((r >> 28) + (r >> 60) + (r2 >> 60)) << 28) >> 16) + (v >> 48);
                ulong f2    = v & 0xFFFF_FFFF_FFFF; //f22
            
                d1 = AbsMinus(c, d);
                d2 = AbsMinus(mc, md);

                if(d1 != 0 && d2 != 0)
                {
                    ulong dd = d1 * d2; // 0 - FFFF_FFFE_0000_0001

                    ulong dd1 = (dd >> 48) /*& 0xFFFF*/;
                    ulong dd2 = dd & 0xFFFF_FFFF_FFFF;

                    v = f2;

                    if((!(c > d) && (mc > md)) || ((c > d) && !(mc > md)))
                    {
                        f2 += dd2;
                        f += dd1;

                        if(v > (f2 & 0xFFFF_FFFF_FFFF)) ++f; // sync f21 and f22 when overflow
                    }
                    else
                    {
                        f2 -= dd2;
                        f -= dd1;

                        if(v < (f2 & 0xFFFF_FFFF_FFFF)) --f;
                    }
                }

                ulong fLowMiddle = ((f2 & 0xFFFF_FFFF) << 32) + (r2 & 0xFFFF_FFFF);

                // overflow is possible but for current 128-bit it's most high numbers
                f = fHigh + fLowMiddle + ((f << 16) + ((f2 >> 32) & 0xFFFF)); // resHigh

                fHigh   = ((ulong)a << 32) + b; //fa
                f2      = ((ulong)c << 32) + d; //fb
                r       = ((ulong)ma << 32) + mb; //fma 
                r2      = ((ulong)mc << 32) + md; //fmb

                d1 = AbsMinus(fHigh, f2);
                d2 = AbsMinus(r, r2);

                if(d1 != 0 && d2 != 0)
                {
                    ulong dd = d1 * d2;

                    if((!(fHigh > f2) && (r > r2)) || ((fHigh > f2) && !(r > r2)))
                    {
                        f += dd;
                    }
                    else
                    {
                        f -= dd;
                    }
                }

                low = fLowMiddle;
                return f;
            }
        } /**/

        /// <summary>
        /// High-speed multiplication of a 128-bit x 128-bit numbers.
        /// </summary>
        /// <param name="a">High 64 bits of the input value.</param>
        /// <param name="b">Low 64 bits of the input value.</param>
        /// <param name="ma">High 64 bits of the multiplier.</param>
        /// <param name="mb">Low 64 bits of the multiplier.</param>
        /// <param name="low">Low 6 bits from a 128-bit result.</param>
        /// <returns>High 64 bits from a 128-bit result.</returns>
        public static ulong Multiply(ulong a, ulong b, ulong ma, ulong mb, out ulong low) => Multiply
        (
            (uint)(a >> 32), (uint)a, (uint)(b >> 32), (uint)b,
            (uint)(ma >> 32), (uint)ma, (uint)(mb >> 32), (uint)mb,
            out low
        );

        /// <returns>Significant bytes of a 128-bit result.</returns>
        /// <inheritdoc cref="Multiply(ulong, ulong, ulong, ulong, out ulong)"/>
        public static byte[] Multiply(ulong a, ulong b, ulong ma, ulong mb) => Multiply
        (
            (uint)(a >> 32), (uint)a, (uint)(b >> 32), (uint)b,
            (uint)(ma >> 32), (uint)ma, (uint)(mb >> 32), (uint)mb
        );

        /// <returns>Significant bytes of a 128-bit result.</returns>
        /// <inheritdoc cref="Multiply(uint, uint, uint, uint, uint, uint, uint, uint, out ulong)"/>
        public static byte[] Multiply(uint a, uint b, uint c, uint d, uint ma, uint mb, uint mc, uint md)
        {
            ulong high = Multiply(a, b, c, d, ma, mb, mc, md, out ulong low);

            return new byte[]
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

        private static ulong AbsMinus(ulong a, ulong b) => (a > b) ? a - b : b - a;
    }
}
