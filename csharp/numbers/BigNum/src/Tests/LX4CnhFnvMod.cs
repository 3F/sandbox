﻿/*
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
    /// Test of the optimized implementation (limited) of the LX4Cnh algorithm specially for Fnv1a128
    /// </summary>
    public class LX4CnhFnvMod
    {
        /*
        |            Method |      Mean |     Error |    StdDev |
        |------------------ |----------:|----------:|----------:|
        |            LX4Cnh | 2.6957 ns | 0.0180 ns | 0.0159 ns |
        | LX4CnhFnv1a128Mod | 0.8568 ns | 0.0060 ns | 0.0056 ns |
       */

        /* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        #! Moved to https://github.com/3F/LX4Cnh
        */
    }
}
