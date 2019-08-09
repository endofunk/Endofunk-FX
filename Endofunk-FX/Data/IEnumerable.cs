// IEnumerable.cs
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

  public static partial class IEnumerableExtensions {
    #region Zip
    public static IEnumerable<R> Zip<A, B, C, R>(this IEnumerable<A> sa, IEnumerable<B> sb, IEnumerable<C> sc, Func<A, B, C, R> f) {
      var (a, b, c) = (sa.GetEnumerator(), sb.GetEnumerator(), sc.GetEnumerator());
      while (a.MoveNext() && b.MoveNext() && c.MoveNext()) {
        yield return f(a.Current, b.Current, c.Current);
      }
    }

    public static IEnumerable<R> Zip<A, B, C, D, R>(this IEnumerable<A> sa, IEnumerable<B> sb, IEnumerable<C> sc, IEnumerable<D> sd, Func<A, B, C, D, R> f) {
      var (a, b, c, d) = (sa.GetEnumerator(), sb.GetEnumerator(), sc.GetEnumerator(), sd.GetEnumerator());
      while (a.MoveNext() && b.MoveNext() && c.MoveNext() && d.MoveNext()) {
        yield return f(a.Current, b.Current, c.Current, d.Current);
      }
    }

    public static (IEnumerable<A>, IEnumerable<B>) UnZip<A, B>(this IEnumerable<(A, B)> sab) => sab.Fold((Enumerable.Empty<A>(), Enumerable.Empty<B>()), (a, e) => (a.Item1.Append(e.Item1), a.Item2.Append(e.Item2)));
    public static (IEnumerable<A>, IEnumerable<B>, IEnumerable<C>) UnZip<A, B, C>(this IEnumerable<(A, B, C)> sabc) => sabc.Fold((Enumerable.Empty<A>(), Enumerable.Empty<B>(), Enumerable.Empty<C>()), (a, e) => (a.Item1.Append(e.Item1), a.Item2.Append(e.Item2), a.Item3.Append(e.Item3)));
    public static (IEnumerable<A>, IEnumerable<B>, IEnumerable<C>, IEnumerable<D>) UnZip<A, B, C, D>(this IEnumerable<(A, B, C, D)> sabcd) => sabcd.Fold((Enumerable.Empty<A>(), Enumerable.Empty<B>(), Enumerable.Empty<C>(), Enumerable.Empty<D>()), (a, e) => (a.Item1.Append(e.Item1), a.Item2.Append(e.Item2), a.Item3.Append(e.Item3), a.Item4.Append(e.Item4)));
    #endregion

    #region Foreach
    public static void ForEach<A>(this IEnumerable<A> @this, Action<A> f) {
      foreach (A e in @this) { f(e); }
    }
    #endregion

    #region Head / Tail
    public static A Head<A>(this IEnumerable<A> @this) => @this.First();
    public static IEnumerable<A> Tail<A>(this IEnumerable<A> @this) => @this.Skip(1);
    #endregion

    #region IEnumerable - Fold
    public static R Fold<A, R>(this IEnumerable<A> xs, R id, Func<R, A, R> fn) => xs.Aggregate(id, fn);
    public static Func<IEnumerable<A>, R> Fold<A, R>(R id, Func<R, A, R> fn) => ts => ts.Aggregate<A, R>(id, fn);
    public static R FoldBack<A, R>(this IEnumerable<A> xs, R id, Func<R, A, R> fn) => xs.Reverse().Aggregate(id, fn);
    #endregion

    #region Functor
    public static IEnumerable<R> Map<A, R>(this Func<A, R> fn, IEnumerable<A> xs) => xs.Map(fn);
    public static IEnumerable<R> Map<A, R>(this IEnumerable<A> xs, Func<A, R> fn) => xs.Aggregate(Enumerable.Empty<R>(), (a, e) => a.Append(fn(e)));
    public static Func<IEnumerable<A>, IEnumerable<R>> Map<A, R>(this Func<A, R> fn) => xs => xs.Map(fn);
    #endregion

    #region MapReduce {
    public static R MapReduce<A, B, R>(this IEnumerable<A> xs, Func<A, B> f, R id, Func<R, B, R> fn) => xs.Fold(id, (r, a) => fn(r, f(a)));
    public static Func<IEnumerable<A>, R> MapReduce<A, B, R>(this Func<A, B> f, R id, Func<R, B, R> fn) => xs => xs.MapReduce(f, id, fn);
    #endregion

    #region Flatten
    public static IEnumerable<A> Flatten<A>(this IEnumerable<IEnumerable<A>> xs) => xs.Aggregate(Enumerable.Empty<A>(), (a, e) => a.Concat(e));
    #endregion

    #region Monad
    public static IEnumerable<R> FlatMap<A, R>(this Func<A, IEnumerable<R>> fn, IEnumerable<A> ts) => ts.FlatMap(fn);
    public static IEnumerable<R> FlatMap<A, R>(this IEnumerable<A> ts, Func<A, IEnumerable<R>> fn) => ts.Map(fn).Flatten();
    public static Func<IEnumerable<A>, IEnumerable<R>> FlatMap<A, R>(this Func<A, IEnumerable<R>> f) => a => a.FlatMap(f);
    public static IEnumerable<R> Bind<A, R>(this IEnumerable<A> ts, Func<A, IEnumerable<R>> fn) => ts.FlatMap(fn);
    #endregion

    #region Kleisli Composition
    public static Func<A, IEnumerable<C>> Compose<A, B, C>(this Func<A, IEnumerable<B>> f, Func<B, IEnumerable<C>> g) => f.Compose(g.FlatMap());
    #endregion

    #region Monad - Lift a function & actions
    public static IEnumerable<R> LiftM<A, R>(this Func<A, R> @this, IEnumerable<A> a) => a.FlatMap(xa => EnumerableNew(@this(xa)));
    public static IEnumerable<R> LiftM<A, B, R>(this Func<A, B, R> @this, IEnumerable<A> a, IEnumerable<B> b) => a.FlatMap(xa => b.FlatMap(xb => EnumerableNew(@this(xa, xb))));
    public static IEnumerable<R> LiftM<A, B, C, R>(this Func<A, B, C, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => EnumerableNew(@this(xa, xb, xc)))));
    public static IEnumerable<R> LiftM<A, B, C, D, R>(this Func<A, B, C, D, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => EnumerableNew(@this(xa, xb, xc, xd))))));
    public static IEnumerable<R> LiftM<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => EnumerableNew(@this(xa, xb, xc, xd, xe)))))));
    public static IEnumerable<R> LiftM<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => EnumerableNew(@this(xa, xb, xc, xd, xe, xf))))))));
    public static IEnumerable<R> LiftM<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => EnumerableNew(@this(xa, xb, xc, xd, xe, xf, xg)))))))));
    public static IEnumerable<R> LiftM<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g, IEnumerable<H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => EnumerableNew(@this(xa, xb, xc, xd, xe, xf, xg, xh))))))))));
    public static IEnumerable<R> LiftM<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g, IEnumerable<H> h, IEnumerable<I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => EnumerableNew(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi)))))))))));
    public static IEnumerable<R> LiftM<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g, IEnumerable<H> h, IEnumerable<I> i, IEnumerable<J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => EnumerableNew(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj))))))))))));
    #endregion

    #region Filterable
    public static IEnumerable<A> Filter<A>(this Func<A, bool> fn, IEnumerable<A> xs) => xs.Where(fn);
    public static IEnumerable<A> Filter<A>(this IEnumerable<A> xs, Func<A, bool> fn) => xs.Where(fn);
    #endregion

    #region Applicative Functor
    public static IEnumerable<R> Apply<A, R>(this IEnumerable<A> xs, IEnumerable<Func<A, R>> fn) => fn.FlatMap(g => xs.Map(x => g(x)));
    public static IEnumerable<R> Apply<A, R>(this IEnumerable<Func<A, R>> fn, IEnumerable<A> ts) => ts.Apply(fn);
    public static IEnumerable<A> ToIEnumerable<A>(this A a) => EnumerableNew(a);
    #endregion

    #region Applicative Functor - Lift a function & actions
    public static IEnumerable<R> LiftA<A, R>(this Func<A, R> fn, IEnumerable<A> a) => fn.Map(a);
    public static IEnumerable<R> LiftA<A, B, R>(this Func<A, B, R> fn, IEnumerable<A> a, IEnumerable<B> b) => fn.Curry().Map(a).Apply(b);
    public static IEnumerable<R> LiftA<A, B, C, R>(this Func<A, B, C, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c) => fn.Curry().Map(a).Apply(b).Apply(c);
    public static IEnumerable<R> LiftA<A, B, C, D, R>(this Func<A, B, C, D, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d);
    public static IEnumerable<R> LiftA<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static IEnumerable<R> LiftA<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static IEnumerable<R> LiftA<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static IEnumerable<R> LiftA<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g, IEnumerable<H> h) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static IEnumerable<R> LiftA<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g, IEnumerable<H> h, IEnumerable<I> i) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static IEnumerable<R> LiftA<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g, IEnumerable<H> h, IEnumerable<I> i, IEnumerable<J> j) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    #region Traverse
    public static Maybe<IEnumerable<B>> TraverseM<A, B>(this IEnumerable<A> @this, Func<A, Maybe<B>> f) => @this.Fold(Just(Enumerable.Empty<B>()), (a, e) => a.FlatMap(xs => f(e).Map(x => xs.Append(x))));
    public static Identity<IEnumerable<R>> TraverseM<A, R>(this IEnumerable<A> @this, Func<A, Identity<R>> f) => @this.Fold(Of(Enumerable.Empty<R>()), (a, e) => a.FlatMap(xs => f(e).Map(x => xs.Append(x))));
    public static Result<IEnumerable<R>> TraverseM<A, R>(this IEnumerable<A> @this, Func<A, Result<R>> f) => @this.Fold(Value(Enumerable.Empty<R>()), (a, e) => a.FlatMap(xs => f(e).Map(x => xs.Append(x))));
    public static IO<IEnumerable<R>> TraverseM<A, R>(this IEnumerable<A> @this, Func<A, IO<R>> f) => @this.Fold(Enumerable.Empty<R>().ToIO(), (a, e) => a.FlatMap(xs => f(e).Map(x => xs.Append(x))));
    public static Reader<S, IEnumerable<R>> TraverseM<S, A, R>(this IEnumerable<A> @this, Func<A, Reader<S, R>> f) => @this.Fold(Enumerable.Empty<R>().ToReader<S, IEnumerable<R>>(), (a, e) => a.FlatMap(xs => f(e).Map(x => xs.Append(x))));
    public static Either<L, IEnumerable<R>> TraverseM<L, A, R>(this IEnumerable<A> @this, Func<A, Either<L, R>> f) => @this.Fold(Right<L, IEnumerable<R>>(Enumerable.Empty<R>()), (a, e) => a.FlatMap(xs => f(e).Map(x => xs.Append(x))));
    public static Validation<L, IEnumerable<R>> TraverseM<L, A, R>(this IEnumerable<A> @this, Func<A, Validation<L, R>> f) => @this.Fold(Success<L, IEnumerable<R>>(Enumerable.Empty<R>()), (a, e) => a.FlatMap(xs => f(e).Map(x => xs.Append(x))));

    public static Maybe<IEnumerable<B>> TraverseA<A, B>(this IEnumerable<A> @this, Func<A, Maybe<B>> f) => @this.Fold(Just(Enumerable.Empty<B>()), (a, e) => Just(Append<B>().Curry()).Apply(a).Apply(f(e)));
    public static Identity<IEnumerable<R>> TraverseA<A, R>(this IEnumerable<A> @this, Func<A, Identity<R>> f) => @this.Fold(Of(Enumerable.Empty<R>()), (a, e) => Of(Append<R>().Curry()).Apply(a).Apply(f(e)));
    public static Result<IEnumerable<R>> TraverseA<A, R>(this IEnumerable<A> @this, Func<A, Result<R>> f) => @this.Fold(Value(Enumerable.Empty<R>()), (a, e) => Value(Append<R>().Curry()).Apply(a).Apply(f(e)));
    public static IO<IEnumerable<R>> TraverseA<A, R>(this IEnumerable<A> @this, Func<A, IO<R>> f) => @this.Fold(Enumerable.Empty<R>().ToIO(), (a, e) => Append<R>().Curry().ToIO().Apply(a).Apply(f(e)));
    public static Reader<S, IEnumerable<R>> TraverseA<S, A, R>(this IEnumerable<A> @this, Func<A, Reader<S, R>> f) => @this.Fold(Enumerable.Empty<R>().ToReader<S, IEnumerable<R>>(), (a, e) => Append<R>().Curry().ToReader<S, Func<IEnumerable<R>, Func<R, IEnumerable<R>>>>().Apply(a).Apply(f(e)));
    public static Either<L, IEnumerable<R>> TraverseA<L, A, R>(this IEnumerable<A> @this, Func<A, Either<L, R>> f) => @this.Fold(Right<L, IEnumerable<R>>(Enumerable.Empty<R>()), (a, e) => Right<L, Func<IEnumerable<R>, Func<R, IEnumerable<R>>>>(Append<R>().Curry()).Apply(a).Apply(f(e)));
    public static Validation<L, IEnumerable<R>> TraverseA<L, A, R>(this IEnumerable<A> @this, Func<A, Validation<L, R>> f) => @this.Fold(Success<L, IEnumerable<R>>(Enumerable.Empty<R>()), (a, e) => Success<L, Func<IEnumerable<R>, Func<R, IEnumerable<R>>>>(Append<R>().Curry()).Apply(a).Apply(f(e)));

    public static Maybe<IEnumerable<B>> Traverse<A, B>(this IEnumerable<A> @this, Func<A, Maybe<B>> f) => @this.TraverseA(f);
    public static Identity<IEnumerable<B>> Traverse<A, B>(this IEnumerable<A> @this, Func<A, Identity<B>> f) => @this.TraverseA(f);
    public static Result<IEnumerable<R>> Traverse<A, R>(this IEnumerable<A> @this, Func<A, Result<R>> f) => @this.TraverseA(f);
    public static IO<IEnumerable<R>> Traverse<A, R>(this IEnumerable<A> @this, Func<A, IO<R>> f) => @this.TraverseA(f);
    public static Reader<S, IEnumerable<R>> Traverse<S, A, R>(this IEnumerable<A> @this, Func<A, Reader<S, R>> f) => @this.TraverseA(f);
    public static Either<L, IEnumerable<R>> Traverse<L, A, R>(this IEnumerable<A> @this, Func<A, Either<L, R>> f) => @this.TraverseA(f);
    public static Validation<L, IEnumerable<R>> Traverse<L, A, R>(this IEnumerable<A> @this, Func<A, Validation<L, R>> f) => @this.TraverseA(f);
    #endregion

    #region Sequence
    public static Maybe<IEnumerable<A>> SequenceM<A>(this IEnumerable<Maybe<A>> @this) => @this.TraverseM(Id<Maybe<A>>());
    public static Identity<IEnumerable<A>> SequenceM<A>(this IEnumerable<Identity<A>> @this) => @this.TraverseM(Id<Identity<A>>());
    public static Result<IEnumerable<A>> SequenceM<A>(this IEnumerable<Result<A>> @this) => @this.TraverseM(Id<Result<A>>());
    public static IO<IEnumerable<A>> SequenceM<A>(this IEnumerable<IO<A>> @this) => @this.TraverseM(Id<IO<A>>());
    public static Reader<S, IEnumerable<A>> SequenceM<S, A>(this IEnumerable<Reader<S, A>> @this) => @this.TraverseM(Id<Reader<S, A>>());
    public static Either<L, IEnumerable<A>> SequenceM<L, A>(this IEnumerable<Either<L, A>> @this) => @this.TraverseM(Id<Either<L, A>>());
    public static Validation<L, IEnumerable<A>> SequenceM<L, A>(this IEnumerable<Validation<L, A>> @this) => @this.TraverseM(Id<Validation<L, A>>());

    public static Maybe<IEnumerable<A>> SequenceA<A>(this IEnumerable<Maybe<A>> @this) => @this.TraverseA(Id<Maybe<A>>());
    public static Identity<IEnumerable<A>> SequenceA<A>(this IEnumerable<Identity<A>> @this) => @this.TraverseA(Id<Identity<A>>());
    public static Result<IEnumerable<A>> SequenceA<A>(this IEnumerable<Result<A>> @this) => @this.TraverseA(Id<Result<A>>());
    public static IO<IEnumerable<A>> SequenceA<A>(this IEnumerable<IO<A>> @this) => @this.TraverseA(Id<IO<A>>());
    public static Reader<S, IEnumerable<A>> SequenceA<S, A>(this IEnumerable<Reader<S, A>> @this) => @this.TraverseA(Id<Reader<S, A>>());
    public static Either<L, IEnumerable<A>> SequenceA<L, A>(this IEnumerable<Either<L, A>> @this) => @this.TraverseA(Id<Either<L, A>>());
    public static Validation<L, IEnumerable<A>> SequenceA<L, A>(this IEnumerable<Validation<L, A>> @this) => @this.TraverseA(Id<Validation<L, A>>());

    public static Maybe<IEnumerable<A>> Sequence<A>(this IEnumerable<Maybe<A>> @this) => @this.SequenceA();
    public static Identity<IEnumerable<A>> Sequence<A>(this IEnumerable<Identity<A>> @this) => @this.SequenceA();
    public static Result<IEnumerable<A>> Sequence<A>(this IEnumerable<Result<A>> @this) => @this.SequenceA();
    public static IO<IEnumerable<A>> Sequence<A>(this IEnumerable<IO<A>> @this) => @this.SequenceA();
    public static Reader<S, IEnumerable<A>> Sequence<S, A>(this IEnumerable<Reader<S, A>> @this) => @this.SequenceA();
    public static Either<L, IEnumerable<A>> Sequence<L, A>(this IEnumerable<Either<L, A>> @this) => @this.SequenceA();
    public static Validation<L, IEnumerable<A>> Sequence<L, A>(this IEnumerable<Validation<L, A>> @this) => @this.SequenceA();
    #endregion

    #region Join Recreated
    public static IEnumerable<R> JoinAP<A, B, C, R>(this IEnumerable<A> outer, IEnumerable<B> inner, Func<A, C> outerKey, Func<B, C> innerKey, Func<A, B, R> result) {
      Func<A, B, IEnumerable<R>> f = (a, b) => outerKey(a).Equals(innerKey(b)) ? EnumerableNew(result(a, b)) : Enumerable.Empty<R>();
      return outer.Map(f.Curry()).Apply(inner).Flatten();
    }

    public static IEnumerable<R> JoinAP<A, B, R>(this IEnumerable<A> outer, IEnumerable<B> inner, Func<A, B, bool> predicate, Func<A, B, R> result) {
      Func<A, B, IEnumerable<R>> f = (a, b) => predicate(a, b) ? EnumerableNew(result(a, b)) : Enumerable.Empty<R>();
      return outer.Map(f.Curry()).Apply(inner).Flatten();
    }
    #endregion

    #region String
    public static string Join<A>(this IEnumerable<A> @this, string separator) => string.Join(separator, @this);
    #endregion

    #region DebugPrint
    public static void DebugPrint<A>(this IEnumerable<A> xs, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", $"{xs.GetType().Simplify()}{xs.ToJsonString()}");
    #endregion

    #region ToList
    public static Func<A, List<B>> ToList<A, B>(this Func<A, B> f) => a => List<B>(f(a));
    #endregion
  }

  public static partial class Prelude {
    #region Append
    public static Func<IEnumerable<A>, A, IEnumerable<A>> Append<A>() => (xs, x) => xs.Append(x);
    #endregion

    #region Syntactic Sugar - Some / None
    public static List<A> List<A>(params A[] value) => value.ToList<A>();
    #endregion

    #region New
    public static IEnumerable<A> EnumerableNew<A>(params A[] elements) => Enumerable.Empty<A>().Concat(elements);
    #endregion
  }
}
