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
using System.Collections.Generic;

namespace Endofunk.FX {
  public class Cont<R, A> {
    public Func<Func<A, R>, R> Run;
    public Cont(Func<Func<A, R>, R> run) => Run = run;
    public static Cont<R, A> Of(A a) => new Cont<R, A>(f => f(a));
    public static Cont<R, A> Create(Func<Func<A, R>, R> run) => new Cont<R, A>(run);
  }

  public static class ContinuationExtensions {
    #region Fold
    public static R Fold<R, A>(this Cont<R, A> @this, Func<A, R> fn) => @this.Run(fn);
    #endregion

    #region Functor
    public static Cont<R, B> Map<R, A, B>(this Cont<R, A> @this, Func<A, B> f) => Cont<R, B>.Create(k => @this.Run(a => k(f(a))));
    public static Cont<R, B> Map<R, A, B>(this Func<A, B> f, Cont<R, A> @this) => @this.Map(f);
    public static Func<Cont<R, A>, Cont<R, B>> Map<R, A, B>(this Func<A, B> f) => @this => @this.Map(f);
    public static Cont<R, B> Select<R, A, B>(this Cont<R, A> @this, Func<A, B> f) => Cont<R, B>.Create(k => @this.Run(a => k(f(a))));
    #endregion

    #region Monad
    public static Cont<R, B> FlatMap<R, A, B>(this Cont<R, A> @this, Func<A, Cont<R, B>> f) => Cont<R, B>.Create(k => @this.Run(a => f(a).Run(k)));
    public static Cont<R, B> FlatMap<R, A, B>(this Func<A, Cont<R, B>> f, Cont<R, A> @this) => @this.FlatMap(f);
    public static Func<Cont<R, A>, Cont<R, B>> FlatMap<R, A, B>(this Func<A, Cont<R, B>> f) => @this => @this.FlatMap(f);
    public static Cont<R, B> Bind<R, A, B>(this Cont<R, A> @this, Func<A, Cont<R, B>> f) => @this.FlatMap(f);
    public static Cont<R, B> SelectMany<R, A, B>(this Cont<R, A> @this, Func<A, Cont<R, B>> f) => @this.FlatMap(f);
    public static Cont<R, R2> SelectMany<R, A, B, R2>(this Cont<R, A> @this, Func<A, Cont<R, B>> fn, Func<A, B, R2> select) => @this.FlatMap(a => fn(a).FlatMap(b => select(a, b).ToCont<R, R2>()));
    #endregion

    #region Monad - Lift a function & actions
    public static Cont<R, R2> LiftM<R, A, R2>(this Func<A, R2> @this, Cont<R, A> a) => a.FlatMap(xa => @this(xa).ToCont<R, R2>());
    public static Cont<R, R2> LiftM<R, A, B, R2>(this Func<A, B, R2> @this, Cont<R, A> a, Cont<R, B> b) => a.FlatMap(xa => b.FlatMap(xb => @this(xa, xb).ToCont<R, R2>()));
    public static Cont<R, R2> LiftM<R, A, B, C, R2>(this Func<A, B, C, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => @this(xa, xb, xc).ToCont<R, R2>())));
    public static Cont<R, R2> LiftM<R, A, B, C, D, R2>(this Func<A, B, C, D, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => @this(xa, xb, xc, xd).ToCont<R, R2>()))));
    public static Cont<R, R2> LiftM<R, A, B, C, D, E, R2>(this Func<A, B, C, D, E, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d, Cont<R, E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => @this(xa, xb, xc, xd, xe).ToCont<R, R2>())))));
    public static Cont<R, R2> LiftM<R, A, B, C, D, E, F, R2>(this Func<A, B, C, D, E, F, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d, Cont<R, E> e, Cont<R, F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => @this(xa, xb, xc, xd, xe, xf).ToCont<R, R2>()))))));
    public static Cont<R, R2> LiftM<R, A, B, C, D, E, F, G, R2>(this Func<A, B, C, D, E, F, G, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d, Cont<R, E> e, Cont<R, F> f, Cont<R, G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => @this(xa, xb, xc, xd, xe, xf, xg).ToCont<R, R2>())))))));
    public static Cont<R, R2> LiftM<R, A, B, C, D, E, F, G, H, R2>(this Func<A, B, C, D, E, F, G, H, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d, Cont<R, E> e, Cont<R, F> f, Cont<R, G> g, Cont<R, H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => @this(xa, xb, xc, xd, xe, xf, xg, xh).ToCont<R, R2>()))))))));
    public static Cont<R, R2> LiftM<R, A, B, C, D, E, F, G, H, I, R2>(this Func<A, B, C, D, E, F, G, H, I, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d, Cont<R, E> e, Cont<R, F> f, Cont<R, G> g, Cont<R, H> h, Cont<R, I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => @this(xa, xb, xc, xd, xe, xf, xg, xh, xi).ToCont<R, R2>())))))))));
    public static Cont<R, R2> LiftM<R, A, B, C, D, E, F, G, H, I, J, R2>(this Func<A, B, C, D, E, F, G, H, I, J, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d, Cont<R, E> e, Cont<R, F> f, Cont<R, G> g, Cont<R, H> h, Cont<R, I> i, Cont<R, J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => @this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj).ToCont<R, R2>()))))))))));
    #endregion

    #region Applicative Functor
    public static Cont<R, B> Apply<R, A, B>(this Cont<R, A> @this, Cont<R, Func<A, B>> f) => f.FlatMap(g => @this.Map(g));
    public static Cont<R, B> Apply<R, A, B>(this Cont<R, Func<A, B>> f, Cont<R, A> @this) => @this.Apply(f);
    public static Func<Cont<R, A>, Cont<R, B>> Apply<R, A, B>(this Cont<R, Func<A, B>> f) => @this => @this.Apply(f);
    #endregion

    #region Applicative Functor - Lift a function & actions
    public static Cont<R, R2> LiftA<R, A, R2>(this Func<A, R2> @this, Cont<R, A> a) => @this.Map(a);
    public static Cont<R, R2> LiftA<R, A, B, R2>(this Func<A, B, R2> @this, Cont<R, A> a, Cont<R, B> b) => @this.Curry().Map(a).Apply(b);
    public static Cont<R, R2> LiftA<R, A, B, C, R2>(this Func<A, B, C, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c) => @this.Curry().Map(a).Apply(b).Apply(c);
    public static Cont<R, R2> LiftA<R, A, B, C, D, R2>(this Func<A, B, C, D, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d);
    public static Cont<R, R2> LiftA<R, A, B, C, D, E, R2>(this Func<A, B, C, D, E, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d, Cont<R, E> e) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static Cont<R, R2> LiftA<R, A, B, C, D, E, F, R2>(this Func<A, B, C, D, E, F, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d, Cont<R, E> e, Cont<R, F> f) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static Cont<R, R2> LiftA<R, A, B, C, D, E, F, G, R2>(this Func<A, B, C, D, E, F, G, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d, Cont<R, E> e, Cont<R, F> f, Cont<R, G> g) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static Cont<R, R2> LiftA<R, A, B, C, D, E, F, G, H, R2>(this Func<A, B, C, D, E, F, G, H, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d, Cont<R, E> e, Cont<R, F> f, Cont<R, G> g, Cont<R, H> h) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static Cont<R, R2> LiftA<R, A, B, C, D, E, F, G, H, I, R2>(this Func<A, B, C, D, E, F, G, H, I, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d, Cont<R, E> e, Cont<R, F> f, Cont<R, G> g, Cont<R, H> h, Cont<R, I> i) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static Cont<R, R2> LiftA<R, A, B, C, D, E, F, G, H, I, J, R2>(this Func<A, B, C, D, E, F, G, H, I, J, R2> @this, Cont<R, A> a, Cont<R, B> b, Cont<R, C> c, Cont<R, D> d, Cont<R, E> e, Cont<R, F> f, Cont<R, G> g, Cont<R, H> h, Cont<R, I> i, Cont<R, J> j) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    //#region Traverse
    //public static IEnumerable<Cont<R, B>> Traverse<R, A, B>(this Cont<R, A> @this, Func<A, IEnumerable<B>> f) => @this.Fold(a => f(a).Map(Of<R, B>()));
    //public static Maybe<Cont<R, B>> Traverse<R, A, B>(this Cont<R, A> @this, Func<A, Maybe<B>> f) => @this.Fold<R, B>(a => f(a).Map(Of<R, B>()));
    //#endregion

    #region Callcc
    /// The callcc function; call with current continuation; is there to allow “escaping” from the current continuation.
    public static Cont<R, A> CallCC<R, A, B>(Func<Func<A, Cont<R, B>>, Cont<R, A>> f) => Cont<R, A>.Create(k => f(a => Cont<R, B>.Create(x => k(a))).Run(k));
    #endregion

    #region Syntactic Sugar
    public static Cont<R, A> ToCont<R, A>(this A a) => Cont<R, A>.Create(k => k(a));
    public static Cont<R, B> ToCont<R, A, B>(this Cont<R, A> @this, Func<A, B> f) => Cont<R, B>.Create(k => @this.Run(f.Compose(k)));
    public static Cont<R, C> ToCont<R, A, B, C>(this Cont<R, A> @this, Cont<R, B> @that, Func<A, B, C> f) => Cont<R, C>.Create(k => @this.Run(a => that.Run(b => k(f(a, b)))));
    public static Cont<R, D> ToCont<R, A, B, C, D>(this Cont<R, A> @this, Cont<R, B> @thatb, Cont<R, C> @thatc, Func<A, B, C, D> f) => Cont<R, D>.Create(k => @this.Run(a => @thatb.Run(b => @thatc.Run(c => k(f(a, b, c))))));
    //public static Func<A, Cont<R, A>> Of<R, A>() => a => Cont<R, A>.Of(a);
    #endregion
  }
}
