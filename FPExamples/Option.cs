//
// Option.cs
//
// Author:  endofunk
//
// Copyright (c) 2019 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
namespace FPExamples {
  public class Option<A> {
    internal readonly Lazy<A> _Value;
    public A Value => _Value.Value;
    public readonly bool HasSome;
    private Option(bool hasSome, Lazy<A> value) => (HasSome, _Value) = (hasSome, value);
    public static Option<A> Some(Lazy<A> value) => new Option<A>(true, value);
    public static Option<A> None() => new Option<A>(false, default);
  }

  public static class ExtensionsOption {
    public static Option<B> Map<A, B>(this Option<A> @this, Func<A, B> f) => @this.HasSome ? Option<B>.Some(new Lazy<B>(() => f(@this._Value.Value))) : Option<B>.None();
  }
}
