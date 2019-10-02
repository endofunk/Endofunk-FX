// Lazy.cs
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
using static Endofunk.FX.Prelude;

namespace Endofunk.FX {
  public static class LazyExtensions {
    #region Fold
    public static R Fold<A, R>(this Lazy<A> @this, Func<A, R> f) => f(@this.Value);
    #endregion

    #region Functor
    public static Lazy<R> Select<A, R>(this Lazy<A> @this, Func<A, R> f) => new Lazy<R>(() => f(@this.Value));
    public static Lazy<R> Map<A, R>(this Lazy<A> @this, Func<A, R> fn) => @this.Select(fn);
    public static Lazy<R> Map<A, R>(this Func<A, R> fn, Lazy<A> @this) => @this.Select(fn);
    public static Func<Lazy<A>, Lazy<R>> Map<A, R>(this Func<A, R> fn) => @this => @this.Select(fn);
    #endregion

    #region Monad
    public static Lazy<R> SelectMany<A, R>(this Lazy<A> @this, Func<A, Lazy<R>> f) => new Lazy<R>(() => f(@this.Value).Value);
    public static Lazy<R> SelectMany<A, B, R>(this Lazy<A> @this, Func<A, Lazy<B>> f, Func<A, B, R> fab) => new Lazy<R>(() => {
      var a = @this.Value;
      return fab(a, f(a).Value);
    });
    public static Lazy<R> FlatMap<A, R>(this Lazy<A> @this, Func<A, Lazy<R>> f) => @this.SelectMany(f);
    public static Lazy<R> FlatMap<A, R>(this Func<A, Lazy<R>> f, Lazy<A> @this) => @this.SelectMany(f);
    public static Func<Lazy<A>, Lazy<B>> FlatMap<A, B>(this Func<A, Lazy<B>> f) => a => a.SelectMany(f);
    public static Lazy<R> Bind<A, R>(this Lazy<A> @this, Func<A, Lazy<R>> f) => @this.SelectMany(f);
    #endregion

    #region Kleisli Composition
    public static Func<A, Lazy<C>> Compose<A, B, C>(this Func<A, Lazy<B>> f, Func<B, Lazy<C>> g) => f.Compose(g.FlatMap());
    #endregion

    #region Monad - Lift a function & actions
    public static Lazy<R> LiftM<A, R>(this Func<A, R> @this, Lazy<A> a) => a.FlatMap(xa => Lazy(@this(xa)));
    public static Lazy<R> LiftM<A, B, R>(this Func<A, B, R> @this, Lazy<A> a, Lazy<B> b) => a.FlatMap(xa => b.FlatMap(xb => Lazy(@this(xa, xb))));
    public static Lazy<R> LiftM<A, B, C, R>(this Func<A, B, C, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => Lazy(@this(xa, xb, xc)))));
    public static Lazy<R> LiftM<A, B, C, D, R>(this Func<A, B, C, D, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => Lazy(@this(xa, xb, xc, xd))))));
    public static Lazy<R> LiftM<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d, Lazy<E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => Lazy(@this(xa, xb, xc, xd, xe)))))));
    public static Lazy<R> LiftM<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d, Lazy<E> e, Lazy<F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => Lazy(@this(xa, xb, xc, xd, xe, xf))))))));
    public static Lazy<R> LiftM<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d, Lazy<E> e, Lazy<F> f, Lazy<G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => Lazy(@this(xa, xb, xc, xd, xe, xf, xg)))))))));
    public static Lazy<R> LiftM<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d, Lazy<E> e, Lazy<F> f, Lazy<G> g, Lazy<H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => Lazy(@this(xa, xb, xc, xd, xe, xf, xg, xh))))))))));
    public static Lazy<R> LiftM<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d, Lazy<E> e, Lazy<F> f, Lazy<G> g, Lazy<H> h, Lazy<I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => Lazy(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi)))))))))));
    public static Lazy<R> LiftM<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d, Lazy<E> e, Lazy<F> f, Lazy<G> g, Lazy<H> h, Lazy<I> i, Lazy<J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => Lazy(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj))))))))))));
    #endregion

    #region Applicative Functor
    public static Lazy<R> Apply<A, R>(this Lazy<A> @this, Lazy<Func<A, R>> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
    public static Lazy<R> Apply<A, R>(this Lazy<Func<A, R>> fn, Lazy<A> @this) => @this.Apply(fn);
    public static Func<Lazy<A>, Lazy<R>> Apply<A, R>(this Lazy<Func<A, R>> fn) => a => a.Apply(fn);

    /// <summary>
    /// Sequence actions, discarding the value of the first argument.
    /// (*>) :: f a -> f b -> f b
    /// </summary>
    public static Lazy<B> DropFirst<A, B>(this Lazy<A> @this, Lazy<B> other) => Const<B, A>().Flip().LiftA(@this, other);

    /// <summary>
    /// Sequence actions, discarding the value of the second argument.
    /// (<*) :: f a -> f b -> f a
    /// </summary>
    public static Lazy<A> DropSecond<A, B>(this Lazy<A> @this, Lazy<B> other) => Const<A, B>().LiftA(@this, other);
    #endregion

    #region Applicative Functor - Lift a function & actions
    public static Lazy<R> LiftA<A, R>(this Func<A, R> @this, Lazy<A> a) => @this.Map(a);
    public static Lazy<R> LiftA<A, B, R>(this Func<A, B, R> @this, Lazy<A> a, Lazy<B> b) => @this.Curry().Map(a).Apply(b);
    public static Lazy<R> LiftA<A, B, C, R>(this Func<A, B, C, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c) => @this.Curry().Map(a).Apply(b).Apply(c);
    public static Lazy<R> LiftA<A, B, C, D, R>(this Func<A, B, C, D, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d);
    public static Lazy<R> LiftA<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d, Lazy<E> e) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static Lazy<R> LiftA<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d, Lazy<E> e, Lazy<F> f) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static Lazy<R> LiftA<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d, Lazy<E> e, Lazy<F> f, Lazy<G> g) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static Lazy<R> LiftA<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d, Lazy<E> e, Lazy<F> f, Lazy<G> g, Lazy<H> h) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static Lazy<R> LiftA<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d, Lazy<E> e, Lazy<F> f, Lazy<G> g, Lazy<H> h, Lazy<I> i) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static Lazy<R> LiftA<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Lazy<A> a, Lazy<B> b, Lazy<C> c, Lazy<D> d, Lazy<E> e, Lazy<F> f, Lazy<G> g, Lazy<H> h, Lazy<I> i, Lazy<J> j) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    #region Traverse
    public static IEnumerable<Lazy<B>> Traverse<A, B>(this Lazy<A> @this, Func<A, IEnumerable<B>> f) => @this.Fold(a => f(a).Map(Lazy));
    public static Identity<Lazy<B>> Traverse<A, B>(this Lazy<A> @this, Func<A, Identity<B>> f) => @this.Fold(a => f(a).Map(Lazy));
    public static Maybe<Lazy<B>> Traverse<A, B>(this Lazy<A> @this, Func<A, Maybe<B>> f) => @this.Fold(a => f(a).Map(Lazy));
    public static Result<Lazy<B>> Traverse<A, B>(this Lazy<A> @this, Func<A, Result<B>> f) => @this.Fold(a => f(a).Map(Lazy));
    public static IO<Lazy<B>> Traverse<A, B>(this Lazy<A> @this, Func<A, IO<B>> f) => @this.Fold(a => f(a).Map(Lazy));
    public static Reader<R, Lazy<B>> Traverse<R, A, B>(this Lazy<A> @this, Func<A, Reader<R, B>> f) => @this.Fold(a => f(a).Map(Lazy));
    public static Either<L, Lazy<B>> Traverse<L, A, B>(this Lazy<A> @this, Func<A, Either<L, B>> f) => @this.Fold(a => f(a).Map(Lazy));
    public static Validation<L, Lazy<B>> Traverse<L, A, B>(this Lazy<A> @this, Func<A, Validation<L, B>> f) => @this.Fold(a => f(a).Map(Lazy));
    #endregion

    #region Sequence
    public static IEnumerable<Lazy<A>> Sequence<A>(Lazy<IEnumerable<A>> @this) => @this.Traverse(Id<IEnumerable<A>>());
    public static Identity<Lazy<A>> Sequence<A>(Lazy<Identity<A>> @this) => @this.Traverse(Id<Identity<A>>());
    public static Maybe<Lazy<A>> Sequence<A>(Lazy<Maybe<A>> @this) => @this.Traverse(Id<Maybe<A>>());
    public static Result<Lazy<A>> Sequence<A>(Lazy<Result<A>> @this) => @this.Traverse(Id<Result<A>>());
    public static IO<Lazy<A>> Sequence<A>(Lazy<IO<A>> @this) => @this.Traverse(Id<IO<A>>());
    public static Reader<R, Lazy<A>> Sequence<R, A>(Lazy<Reader<R, A>> @this) => @this.Traverse(Id<Reader<R, A>>());
    public static Either<L, Lazy<A>> Sequence<L, A>(Lazy<Either<L, A>> @this) => @this.Traverse(Id<Either<L, A>>());
    public static Validation<L, Lazy<A>> Sequence<L, A>(Lazy<Validation<L, A>> @this) => @this.Traverse(Id<Validation<L, A>>());
    #endregion 

    #region Zip
    public static Lazy<(A, B)> Zip<A, B>(this Lazy<A> @this, Lazy<B> other) => Tuple<A, B>().ZipWith(@this, other);
    public static Lazy<C> ZipWith<A, B, C>(this Func<A, B, C> f, Lazy<A> ma, Lazy<B> mb) => f.LiftM(ma, mb);
    public static (Lazy<A>, Lazy<B>) UnZip<A, B>(this Lazy<(A, B)> @this) => (First<A, B>().LiftM(@this), Second<A, B>().LiftM(@this));
    #endregion

    #region Match
    public static void Match<A>(this Lazy<A> @this, Action<A> f) => f(@this.Value);
    public static R Match<A, R>(this Lazy<A> @this, Func<A, R> f) => f(@this.Value);
    #endregion

    #region DebugPrint
    public static void DebugPrint<A>(this Lazy<A> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region ToMaybe
    public static Lazy<A> ToLazy<A>(this A a) => Lazy(a);
    public static Func<A, Lazy<B>> ToLazy<A, B>(this Func<A, B> f) => a => Lazy(f(a));
    #endregion
  }

  public static partial class Prelude {
    #region Syntactic Sugar
    public static Lazy<A> Lazy<A>(A value) => new Lazy<A>(() => value);
    public static Lazy<A> Lazy<A>(Func<A> f) => new Lazy<A>(f);
    public static Func<A, Lazy<A>> Lazy<A>() => a => Lazy(a);
    #endregion
  }
}
