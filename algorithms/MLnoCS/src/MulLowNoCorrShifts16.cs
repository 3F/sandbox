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

namespace net.r_eg.sandbox.algorithms
{
    /// <summary>
    /// MulLowNoCorrShifts (MLnoCS).
    /// Part of https://twitter.com/github3F/status/1403748080760111106
    /// <br/>
    /// Designed for 128x16 due to UTF-16 (theoretically up to 128x32).
    /// </summary>
    /// <remarks>For 128x128 see <see cref="LodgeX4CorrNoHigh"/>.</remarks>
    public static class MulLowNoCorrShifts16
    {
        /* --- Embeddable superfast version --- */

        // ----- immutable

        //uint a = 0x6c62272e, b = 0x07bb0142, c = 0x62b82175, d = 0x6295c58d;
        //ushort prime = 0x4D2F;
        ////-
        //uint ra, rb, rc, rd;
        //unchecked{/* MLnoCS (c) Denis Kuzmin <x-3F@outlook.com> github/3F */ulong e=a,f=b,g=c,h=d,l,o,k;e*=prime;f*=prime;g*=prime;h*=prime;l=(g&0xFFFF_FFFF)+(h>>32);o=(f&0xFFFF_FFFF)+(g>>32);k=(e&0xFFFF_FFFF)+(f>>32);ra=(uint)k;rb=(uint)o;rc=(uint)l;rd=(uint)h;}

        // ----- mutable

        //uint a = 0x6c62272e, b = 0x07bb0142, c = 0x62b82175, d = 0x6295c58d;
        //ushort prime = 0x4D2F;
        ////-
        //unchecked{/*[mutable] MLnoCS (c) Denis Kuzmin <x-3F@outlook.com> github/3F */ulong e=a,f=b,g=c,h=d,l,o,k;e*=prime;f*=prime;g*=prime;h*=prime;l=(g&0xFFFF_FFFF)+(h>>32);o=(f&0xFFFF_FFFF)+(g>>32);k=(e&0xFFFF_FFFF)+(f>>32);a=(uint)k;b=(uint)o;c=(uint)l;d=(uint)h;}

        /* -- */

        /// <inheritdoc cref="Multiply(uint, uint, uint, uint, ushort, out uint, out uint, out uint, out uint)"/>
        /// <param name="low">Low 64 bits from a 128-bit result.</param>
        /// <returns>High 64 bits from a 128-bit result.</returns>
        public static ulong Multiply
        (
            ulong a, ulong b,
            ushort prime, 
            out ulong low
        )
        {
            Multiply
            (
                (uint)(a >> 32), (uint)a, (uint)(b >> 32), (uint)b,
                prime,
                out uint ha, out uint hb, out uint la, out uint lb
            );

            low = ((ulong)la << 32) + lb;
            return ((ulong)ha << 32) + hb;
        }

        /// <summary>
        /// High-speed multiplication of a 128-bit x 16-bit numbers.
        /// </summary>
        /// <param name="a">The first high 32 bits of the input value.</param>
        /// <param name="b">Second high 32 bits of the input value.</param>
        /// <param name="c">The first low 32 bits of the input value.</param>
        /// <param name="d">Second low 32 bits of the input value.</param>
        /// <param name="prime">16-bit multiplier.</param>
        /// <param name="ra">The first high 32 bits from a 128-bit result.</param>
        /// <param name="rb">Second high 32 bits from a 128-bit result.</param>
        /// <param name="rc">The first low 32 bits from a 128-bit result.</param>
        /// <param name="rd">Second low 32 bits from a 128-bit result.</param>
        public static void Multiply
        (
            uint a, uint b, uint c, uint d,
            ushort prime, 
            out uint ra, out uint rb, out uint rc, out uint rd
        )
        {
            Multiply(ref a, ref b, ref c, ref d, prime);
            ra = a;
            rb = b;
            rc = c;
            rd = d;
        }

        /// <inheritdoc cref="Multiply(uint, uint, uint, uint, ushort, out uint, out uint, out uint, out uint)"/>
        public static void Multiply
        (
            ref uint a, ref uint b, ref uint c, ref uint d,
            ushort prime
        )
        {
            unchecked
            {
                ulong _a = a, _b = b, _c = c, _d = d, r2, r3, r4;

                _a *= prime;
                _b *= prime;
                _c *= prime;
                _d *= prime;

                r2 = (_c & 0xFFFF_FFFF) + (_d >> 32);
                r3 = (_b & 0xFFFF_FFFF) + (_c >> 32);
                r4 = (_a & 0xFFFF_FFFF) + (_b >> 32);

                /*r*/a = (uint)r4;
                /*r*/b = (uint)r3;
                /*r*/c = (uint)r2;
                /*r*/d = (uint)_d;
            }
        } /**/

        /// <inheritdoc cref="Multiply(uint, uint, uint, uint, ushort, out uint, out uint, out uint, out uint)"/>
        /// <returns>Significant bytes of a 128-bit result.</returns>
        public static byte[] Multiply(uint a, uint b, uint c, uint d, ushort prime)
        {
            Multiply
            (
                a, b, c, d,
                prime,
                out uint ha, out uint hb, out uint la, out uint lb
            );

            return new byte[]
            {
                (byte)(lb & 0xFF), (byte)(lb >> 8 & 0xFF), (byte)(lb >> 16 & 0xFF), (byte)(lb >> 24 & 0xFF),
                (byte)(la & 0xFF), (byte)(la >> 8 & 0xFF), (byte)(la >> 16 & 0xFF), (byte)(la >> 24 & 0xFF),

                (byte)(hb & 0xFF), (byte)(hb >> 8 & 0xFF), (byte)(hb >> 16 & 0xFF), (byte)(hb >> 24 & 0xFF),
                (byte)(ha & 0xFF), (byte)(ha >> 8 & 0xFF), (byte)(ha >> 16 & 0xFF), (byte)(ha >> 24 & 0xFF),
            };
        }
    }
}
