// Writer.cs
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
using Unit = System.ValueTuple;
using static Endofunk.FX.Prelude;

namespace Endofunk.FX {
  public struct Writer<W, A> {
    internal readonly W _W;
    internal readonly A _A;
    public (W, A) Run => (_W, _A);
    public W Exec => _W;
    private Writer(W w, A a) => (_W, _A) = (w, a);
    public static Writer<W, A> Of(W w, A a) => new Writer<W, A>(w, a);
    //public static Writer<W, Unit> Tell<W>(W w) => Writer<W, Unit>.Of((Unit(), w));

    public override string ToString() => $"Writer<{typeof(W).Simplify()}, {typeof(A).Simplify()}>[{_W.GetType().Simplify()}]";
  }

  public static partial class WriterExtensions {
    #region Functor
    public static Writer<W, B> Map<W, A, B>(this Writer<W, A> @this, Func<A, B> f) => Writer<W, B>.Of(@this._W, f(@this._A));
    public static Writer<W, B> Map<W, A, B>(this Func<A, B> f, Writer<W, A> @this) => Writer<W, B>.Of(@this._W, f(@this._A));
    #endregion

    #region Monad
    public static Writer<R, B> FlatMap<R, A, B>(this Writer<R, A> @this, Func<A, Writer<R, B>> f) => Writer<R, B>.Of(@this._W, f(@this._A)._A);
    public static Writer<R, B> FlatMap<R, A, B>(this Func<A, Writer<R, B>> f, Writer<R, A> @this) => @this.FlatMap(f);
    #endregion


    //#region Monad - Lift a function & actions
    //public static Writer<S, R> LiftM<S, A, B, R>(this Func<A, B, R> @this, Writer<S, A> a, Writer<S, B> b) => a.FlatMap(m1 => b.FlatMap(m2 => Writer<S, R>.Of(@this._W, @this(m1, m2))));
    //public static Writer<S, R> LiftM<S, A, B, C, R>(this Func<A, B, C, R> @this, Writer<S, A> a, Writer<S, B> b, Writer<S, C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => Writer<S, R>.Of(@this._W, @this(xa, xb, xc)))));
    //public static Writer<S, R> LiftM<S, A, B, C, D, R>(this Func<A, B, C, D, R> @this, Writer<S, A> a, Writer<S, B> b, Writer<S, C> c, Writer<S, D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => Writer<S, R>.Of(@this._W, @this(xa, xb, xc, xd))))));
    //public static Writer<S, R> LiftM<S, A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Writer<S, A> a, Writer<S, B> b, Writer<S, C> c, Writer<S, D> d, Writer<S, E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => Writer<S, R>.Of(@this._W, @this(xa, xb, xc, xd, xe).))))));
    //public static Writer<S, R> LiftM<S, A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Writer<S, A> a, Writer<S, B> b, Writer<S, C> c, Writer<S, D> d, Writer<S, E> e, Writer<S, F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => Writer<S, R>.Of(@this._W, @this(xa, xb, xc, xd, xe, xf))))))));
    //public static Writer<S, R> LiftM<S, A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Writer<S, A> a, Writer<S, B> b, Writer<S, C> c, Writer<S, D> d, Writer<S, E> e, Writer<S, F> f, Writer<S, G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => Writer<S, R>.Of(@this._W, @this(xa, xb, xc, xd, xe, xf, xg)))))))));
    //public static Writer<S, R> LiftM<S, A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Writer<S, A> a, Writer<S, B> b, Writer<S, C> c, Writer<S, D> d, Writer<S, E> e, Writer<S, F> f, Writer<S, G> g, Writer<S, H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => Writer<S, R>.Of(@this._W, @this(xa, xb, xc, xd, xe, xf, xg, xh))))))))));
    //public static Writer<S, R> LiftM<S, A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Writer<S, A> a, Writer<S, B> b, Writer<S, C> c, Writer<S, D> d, Writer<S, E> e, Writer<S, F> f, Writer<S, G> g, Writer<S, H> h, Writer<S, I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => Writer<S, R>.Of(@this._W, @this(xa, xb, xc, xd, xe, xf, xg, xh, xi)))))))))));
    //public static Writer<S, R> LiftM<S, A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Writer<S, A> a, Writer<S, B> b, Writer<S, C> c, Writer<S, D> d, Writer<S, E> e, Writer<S, F> f, Writer<S, G> g, Writer<S, H> h, Writer<S, I> i, Writer<S, J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => Writer<S, R>.Of(@this._W, @this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj))))))))))));
    //#endregion

    #region DebugPrint
    public static void DebugPrint<W, A>(this Writer<W, A> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion
  }

  public static partial class Prelude {

  }
}
