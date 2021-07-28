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

namespace net.r_eg.sandbox.BigNum.Tests
{
    /// <summary>
    /// Speed tests of LX4Cnh implementation.
    /// </summary>
    /// <remarks>https://twitter.com/github3F/status/1410358979033813000</remarks>
    public class Int128x128_LodgeX4CorrNoHigh
    {
        /*
        |                    Method |       Mean |     Error |    StdDev |
        |-------------------------- |-----------:|----------:|----------:|
        |         BigInteger128x128 | 408.568 ns | 2.0051 ns | 1.8756 ns |
        |       LX4Cnh128x128_Bytes |  30.088 ns | 0.1178 ns | 0.0983 ns |
        |      LX4Cnh128x128_UInt64 |  20.337 ns | 0.0700 ns | 0.0655 ns |
        | LX4Cnh128x128_UInt64via32 |  17.895 ns | 0.0952 ns | 0.0795 ns |
        |   LX4Cnh128x128_EmbdBytes |  14.541 ns | 0.0618 ns | 0.0516 ns |
        |     LX4Cnh128x128_EmbdVar |   4.233 ns | 0.0135 ns | 0.0106 ns |
       */

        /* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        #! Moved to https://github.com/3F/LX4Cnh
        */
    }
}
