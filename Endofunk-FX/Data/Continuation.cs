// Continuation.cs
//
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
//
using System;

namespace Endofunk.FX {
  // "Mother of all Monads"
  public class Cont<R, A> {
    public Func<Func<A, R>, R> Run;
    public Cont(Func<Func<A, R>, R> run) => Run = run;
    public static Cont<R, A> Of(A a) => new Cont<R, A>(f => f(a));
    public static Cont<R, A> Create(Func<Func<A, R>, R> run) => new Cont<R, A>(run);
  }

  public static class ContinuationExtensions {
    #region Lift
    public static Cont<R, A> Lift<R, A>(this A a) => Cont<R, A>.Create(k => k(a));
    public static Cont<R, C> Lift<R, A, B, C>(this Cont<R, A> @this, Cont<R, B> @that, Func<A, B, C> f) => Cont<R, C>.Create(k => @this.Run(a => that.Run(b => k(f(a, b)))));
    public static Cont<R, D> Lift<R, A, B, C, D>(this Cont<R, A> @this, Cont<R, B> @thatb, Cont<R, C> @thatc, Func<A, B, C, D> f) => Cont<R, D>.Create(k => @this.Run(a => @thatb.Run(b => @thatc.Run(c => k(f(a, b, c))))));
    #endregion

    #region Functor
    public static Cont<R, B> Map<R, A, B>(this Cont<R, A> @this, Func<A, B> f) => Cont<R, B>.Create(k => @this.Run(a => k(f(a))));
    public static Cont<R, B> Lift<R, A, B>(this Cont<R, A> @this, Func<A, B> f) => Cont<R, B>.Create(k => @this.Run(f.Compose(k)));
    #endregion

    #region Monad
    public static Cont<R, B> FlatMap<R, A, B>(this Cont<R, A> @this, Func<A, Cont<R, B>> f) => Cont<R, B>.Create(k => @this.Run(a => f(a).Run(k)));
    public static Cont<R, B> Bind<R, A, B>(this Cont<R, A> @this, Func<A, Cont<R, B>> f) => @this.FlatMap(f);
    #endregion

    #region Linq
    public static Cont<R, B> Select<R, A, B>(this Cont<R, A> @this, Func<A, B> f) => Cont<R, B>.Create(k => @this.Run(a => k(f(a))));
    public static Cont<R, B> SelectMany<R, A, B>(this Cont<R, A> @this, Func<A, Cont<R, B>> f) => @this.FlatMap(f);
    #endregion

    #region Callcc
    /// The callcc function; call with current continuation; is there to allow “escaping” from the current continuation.
    public static Cont<R, A> CallCC<R, A, B>(Func<Func<A, Cont<R, B>>, Cont<R, A>> f) => Cont<R, A>.Create(k => f(a => Cont<R, B>.Create(x => k(a))).Run(k));
    #endregion
  }
}
