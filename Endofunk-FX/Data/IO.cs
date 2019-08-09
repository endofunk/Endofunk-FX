// IO.cs
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
using System.Linq;
using static Endofunk.FX.Prelude;

namespace Endofunk.FX {

  #region IO Datatype
  public sealed class IO<A> {
    public readonly Func<A> Compute;
    public IO(Func<A> compute) => (Compute) = (compute);
    public static IO<A> Of(Func<A> compute) => new IO<A>(compute);
  }
  #endregion

  public static partial class IOExtensions {
    #region Fold
    public static R Fold<A, R>(this IO<A> @this, Func<A, R> fn) => fn(@this.Compute());
    #endregion

    #region Functor
    public static IO<B> Map<A, B>(this IO<A> @this, Func<A, B> fn) => IO<B>.Of(() => fn(@this.Compute()));
    public static IO<B> Map<A, B>(this Func<A, B> fn, IO<A> @this) => @this.Map(fn);
    public static Func<IO<A>, IO<B>> Map<A, B>(this Func<A, B> fn) => @this => @this.Map(fn);
    #endregion

    #region Monad
    public static IO<B> FlatMap<A, B>(this IO<A> @this, Func<A, IO<B>> fn) => IO<B>.Of(() => fn(@this.Compute()).Compute());
    public static IO<B> FlatMap<A, B>(this Func<A, IO<B>> fn, IO<A> @this) => @this.FlatMap(fn);
    public static Func<IO<A>, IO<B>> FlatMap<A, B>(this Func<A, IO<B>> fn) => a => a.FlatMap(fn);
    public static IO<B> Bind<A, B>(this IO<A> @this, Func<A, IO<B>> fn) => @this.FlatMap(fn);
    #endregion

    #region Kleisli Composition
    public static Func<A, IO<C>> Compose<A, B, C>(this Func<A, IO<B>> f, Func<B, IO<C>> g) => f.Compose(g.FlatMap());
    #endregion

    #region Monad - Lift a function & actions
    public static IO<R> LiftM<A, R>(this Func<A, R> @this, IO<A> a) => a.FlatMap(xa => IO<R>.Of(() => @this(xa)));
    public static IO<R> LiftM<A, B, R>(this Func<A, B, R> @this, IO<A> a, IO<B> b) => a.FlatMap(xa => b.FlatMap(xb => IO<R>.Of(() => @this(xa, xb))));
    public static IO<R> LiftM<A, B, C, R>(this Func<A, B, C, R> @this, IO<A> a, IO<B> b, IO<C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => IO<R>.Of(() => @this(xa, xb, xc)))));
    public static IO<R> LiftM<A, B, C, D, R>(this Func<A, B, C, D, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => IO<R>.Of(() => @this(xa, xb, xc, xd))))));
    public static IO<R> LiftM<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d, IO<E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => IO<R>.Of(() => @this(xa, xb, xc, xd, xe)))))));
    public static IO<R> LiftM<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d, IO<E> e, IO<F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => IO<R>.Of(() => @this(xa, xb, xc, xd, xe, xf))))))));
    public static IO<R> LiftM<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d, IO<E> e, IO<F> f, IO<G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => IO<R>.Of(() => @this(xa, xb, xc, xd, xe, xf, xg)))))))));
    public static IO<R> LiftM<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d, IO<E> e, IO<F> f, IO<G> g, IO<H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => IO<R>.Of(() => @this(xa, xb, xc, xd, xe, xf, xg, xh))))))))));
    public static IO<R> LiftM<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d, IO<E> e, IO<F> f, IO<G> g, IO<H> h, IO<I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => IO<R>.Of(() => @this(xa, xb, xc, xd, xe, xf, xg, xh, xi)))))))))));
    public static IO<R> LiftM<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d, IO<E> e, IO<F> f, IO<G> g, IO<H> h, IO<I> i, IO<J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => IO<R>.Of(() => @this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj))))))))))));
    #endregion

    #region Applicative Functor
    public static IO<R> Apply<A, R>(this IO<A> @this, IO<Func<A, R>> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
    public static IO<R> Apply<A, R>(this IO<Func<A, R>> fn, IO<A> @this) => @this.Apply(fn);
    #endregion

    #region Applicative Functor - Lift a function & actions
    public static IO<R> LiftA<A, R>(this Func<A, R> @this, IO<A> a) => @this.Map(a);
    public static IO<R> LiftA<A, B, R>(this Func<A, B, R> @this, IO<A> a, IO<B> b) => @this.Curry().Map(a).Apply(b);
    public static IO<R> LiftA<A, B, C, R>(this Func<A, B, C, R> @this, IO<A> a, IO<B> b, IO<C> c) => @this.Curry().Map(a).Apply(b).Apply(c);
    public static IO<R> LiftA<A, B, C, D, R>(this Func<A, B, C, D, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d);
    public static IO<R> LiftA<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d, IO<E> e) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static IO<R> LiftA<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d, IO<E> e, IO<F> f) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static IO<R> LiftA<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d, IO<E> e, IO<F> f, IO<G> g) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static IO<R> LiftA<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d, IO<E> e, IO<F> f, IO<G> g, IO<H> h) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static IO<R> LiftA<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d, IO<E> e, IO<F> f, IO<G> g, IO<H> h, IO<I> i) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static IO<R> LiftA<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, IO<A> a, IO<B> b, IO<C> c, IO<D> d, IO<E> e, IO<F> f, IO<G> g, IO<H> h, IO<I> i, IO<J> j) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    #region Traverse
    public static IEnumerable<IO<B>> Traverse<A, B>(this IO<A> @this, Func<A, IEnumerable<B>> f) => @this.Fold(a => f(a).Map(ToIO));
    public static Maybe<IO<B>> Traverse<A, B>(this IO<A> @this, Func<A, Maybe<B>> f) => @this.Fold(a => f(a).Map(ToIO));
    public static Result<IO<B>> Traverse<A, B>(this IO<A> @this, Func<A, Result<B>> f) => @this.Fold(a => f(a).Map(ToIO));
    public static Identity<IO<B>> Traverse<A, B>(this IO<A> @this, Func<A, Identity<B>> f) => @this.Fold(a => f(a).Map(ToIO));
    public static Reader<R, IO<B>> Traverse<R, A, B>(this IO<A> @this, Func<A, Reader<R, B>> f) => @this.Fold(a => f(a).Map(ToIO));
    public static Either<L, IO<B>> Traverse<L, A, B>(this IO<A> @this, Func<A, Either<L, B>> f) => @this.Fold(a => f(a).Map(ToIO));
    public static Validation<L, IO<B>> Traverse<L, A, B>(this IO<A> @this, Func<A, Validation<L, B>> f) => @this.Fold(a => f(a).Map(ToIO));
    #endregion

    #region Sequence
    public static IEnumerable<IO<A>> Sequence<A>(IO<IEnumerable<A>> @this) => @this.Traverse(Id<IEnumerable<A>>());
    public static Maybe<IO<A>> Sequence<A>(IO<Maybe<A>> @this) => @this.Traverse(Id<Maybe<A>>());
    public static Result<IO<A>> Sequence<A>(IO<Result<A>> @this) => @this.Traverse(Id<Result<A>>());
    public static Identity<IO<A>> Sequence<A>(IO<Identity<A>> @this) => @this.Traverse(Id<Identity<A>>());
    public static Reader<R, IO<A>> Sequence<R, A>(IO<Reader<R, A>> @this) => @this.Traverse(Id<Reader<R, A>>());
    public static Either<L, IO<A>> Sequence<L, A>(IO<Either<L, A>> @this) => @this.Traverse(Id<Either<L, A>>());
    public static Validation<L, IO<A>> Sequence<L, A>(IO<Validation<L, A>> @this) => @this.Traverse(Id<Validation<L, A>>());
    #endregion 

    #region Linq Conformance
    public static IO<R> Select<A, R>(this IO<A> @this, Func<A, R> fn) => @this.Map(fn);
    public static IO<R> SelectMany<A, R>(this IO<A> @this, Func<A, IO<R>> fn) => @this.FlatMap(fn);
    public static IO<R> SelectMany<A, B, R>(this IO<A> @this, Func<A, IO<B>> fn, Func<A, B, R> select) => @this.FlatMap(a => fn(a).FlatMap(b => select(a, b).ToIO()));
    #endregion

    #region ToIO
    public static IO<A> ToIO<A>(this A a) => IO<A>.Of(() => a);
    #endregion
  }

  public static partial class Prelude {
    #region Syntactic Sugar
    public static Func<A, IO<A>> ToIO<A>() => a => a.ToIO();
    #endregion
  }
}
