// Yoneda.cs
//
// MIT License
// Copyright (c) 2019 endofunk
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
using System.Collections.Generic;
using static Endofunk.FX.Prelude;

namespace Endofunk.FX {

  public sealed class Yoneda<A, B> {
    internal readonly object Functor;
    internal readonly Func<A, B> Transform;
    public Yoneda(object functor, Func<A, B> transform) => (Functor, Transform) = (functor, transform);
  }

  public static partial class YonedaExtensions {
    #region Functor
    public static Yoneda<A, C> Map<A, B, C>(this Yoneda<A, B> @this, Func<B, C> g) => new Yoneda<A, C>(@this.Functor, a => g(@this.Transform(a)));
    #endregion

    #region Linq Conformance
    public static Yoneda<A, C> Select<A, B, C>(this Yoneda<A, B> @this, Func<B, C> f) => @this.Map(f);
    #endregion

    #region Yoneda Run Conformance
    public static Identity<R> RunIdentity<V, R>(this Yoneda<V, R> @this) => ((Identity<V>)@this.Functor).Map(@this.Transform);
    public static Maybe<R> RunMaybe<V, R>(this Yoneda<V, R> @this) => ((Maybe<V>)@this.Functor).Map(@this.Transform);
    public static IEnumerable<R> RunIEnumerable<V, R>(this Yoneda<V, R> @this) => ((IEnumerable<V>)@this.Functor).Map((V v) => @this.Transform(v));
    public static Either<string, R> RunEither<V, R>(this Yoneda<V, R> @this) => ((Either<string, V>)@this.Functor).Map(@this.Transform);
    public static Result<R> RunResult<V, R>(this Yoneda<V, R> @this) => ((Result<V>)@this.Functor).Map(@this.Transform);
    public static Validation<string, R> RunValidation<V, R>(this Yoneda<V, R> @this) => ((Validation<string, V>)@this.Functor).Map(@this.Transform);
    #endregion

    #region ToYoneda
    public static Yoneda<A, A> ToYoneda<A>(this object functor) => new Yoneda<A, A>(functor, Id);
    #endregion
  }

  public static partial class Prelude {
    #region Syntactic Sugar

    #endregion
  }
}
