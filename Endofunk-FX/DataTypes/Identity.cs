// Identity.cs
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
using System.Runtime.Serialization;

namespace Endofunk.FX {
  #region Identity Datatype
  [DataContract]
  public class Identity<A> : IEquatable<Identity<A>> {
    [DataMember] internal readonly A Value;
    private Identity(A value) => (Value) = (value);
    public static implicit operator Identity<A>(A value) => Of(value);
    public static Identity<A> Of(A value) => new Identity<A>(value);
    public override string ToString() => $"{this.GetType().Simplify()}[{Value.ToString()}]";
    public bool Equals(Identity<A> other) => Value.Equals(other.Value);

    public override bool Equals(object obj) {
      if (obj == null) return false;
      var objType = obj.GetType();
      var isStruct = objType.IsValueType && !objType.IsEnum;
      if (obj is Identity<A> && isStruct) {
        return Equals((Identity<A>)obj);
      }
      return false;
    }

    public static bool operator ==(Identity<A> @this, Identity<A> other) => @this.Equals(other);
    public static bool operator !=(Identity<A> @this, Identity<A> other) => !(@this == other);
    public override int GetHashCode() => Value.GetHashCode();

    public IEnumerable<A> AsEnumerable() {
      yield return Value;
    }
  }
  #endregion

  public static partial class IdentityExtensions {
    #region Fold
    public static R Fold<A, R>(this Identity<A> @this, Func<A, R> fn) => fn(@this.Value);
    #endregion

    #region Functor
    public static Identity<R> Map<A, R>(this Func<A, R> fn, Identity<A> @this) => @this.Map(fn);
    public static Identity<R> Map<A, R>(this Identity<A> @this, Func<A, R> fn) => fn(@this.Value);
    public static Func<Identity<A>, Identity<R>> Map<A, R>(this Func<A, R> fn) => xas => xas.Map(fn);
    #endregion

    #region Monad
    public static Identity<R> FlatMap<A, R>(this Identity<A> @this, Func<A, Identity<R>> fn) => fn(@this.Value);
    public static Identity<R> FlatMap<A, R>(this Func<A, Identity<R>> fn, Identity<A> @this) => @this.FlatMap(fn);
    public static Func<Identity<A>, Identity<B>> FlatMap<A, B>(this Func<A, Identity<B>> f) => a => a.FlatMap(f);
    public static Identity<R> Bind<A, R>(this Identity<A> @this, Func<A, Identity<R>> fn) => @this.FlatMap(fn);
    #endregion

    #region Kleisli Composition
    public static Func<A, Identity<C>> Compose<A, B, C>(this Func<A, Identity<B>> f, Func<B, Identity<C>> g) => f.Compose(g.FlatMap());
    #endregion

    #region Monad - Lift a function & actions
    public static Identity<R> LiftM<A, R>(this Func<A, R> @this, Identity<A> a) => a.FlatMap(xa => @this(xa).ToIdentity());
    public static Identity<R> LiftM<A, B, R>(this Func<A, B, R> @this, Identity<A> a, Identity<B> b) => a.FlatMap(xa => b.FlatMap(xb => @this(xa, xb).ToIdentity()));
    public static Identity<R> LiftM<A, B, C, R>(this Func<A, B, C, R> @this, Identity<A> a, Identity<B> b, Identity<C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => @this(xa, xb, xc).ToIdentity())));
    public static Identity<R> LiftM<A, B, C, D, R>(this Func<A, B, C, D, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => @this(xa, xb, xc, xd).ToIdentity()))));
    public static Identity<R> LiftM<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d, Identity<E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => @this(xa, xb, xc, xd, xe).ToIdentity())))));
    public static Identity<R> LiftM<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d, Identity<E> e, Identity<F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => @this(xa, xb, xc, xd, xe, xf).ToIdentity()))))));
    public static Identity<R> LiftM<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d, Identity<E> e, Identity<F> f, Identity<G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => @this(xa, xb, xc, xd, xe, xf, xg).ToIdentity())))))));
    public static Identity<R> LiftM<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d, Identity<E> e, Identity<F> f, Identity<G> g, Identity<H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => @this(xa, xb, xc, xd, xe, xf, xg, xh).ToIdentity()))))))));
    public static Identity<R> LiftM<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d, Identity<E> e, Identity<F> f, Identity<G> g, Identity<H> h, Identity<I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => @this(xa, xb, xc, xd, xe, xf, xg, xh, xi).ToIdentity())))))))));
    public static Identity<R> LiftM<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d, Identity<E> e, Identity<F> f, Identity<G> g, Identity<H> h, Identity<I> i, Identity<J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => @this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj).ToIdentity()))))))))));
    #endregion

    #region Applicative Functor
    public static Identity<R> Apply<A, R>(this Identity<A> @this, Identity<Func<A, R>> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
    public static Identity<R> Apply<A, R>(this Identity<Func<A, R>> fn, Identity<A> @this) => @this.Apply(fn);
    public static Func<Identity<A>, Identity<R>> Apply<A, R>(this Identity<Func<A, R>> fn) => a => a.Apply(fn);

    /// <summary>
    /// Sequence actions, discarding the value of the first argument.
    /// (*>) :: f a -> f b -> f b
    /// </summary>
    public static Identity<B> DropFirst<A, B>(this Identity<A> @this, Identity<B> other) => Const<B, A>().Flip().LiftA(@this, other);

    /// <summary>
    /// Sequence actions, discarding the value of the second argument.
    /// (<*) :: f a -> f b -> f a
    /// </summary>
    public static Identity<A> DropSecond<A, B>(this Identity<A> @this, Identity<B> other) => Const<A, B>().LiftA(@this, other);
    #endregion

    #region Applicative Functor - Lift a function & actions
    public static Identity<R> LiftA<A, R>(this Func<A, R> @this, Identity<A> a) => @this.Map(a);
    public static Identity<R> LiftA<A, B, R>(this Func<A, B, R> @this, Identity<A> a, Identity<B> b) => @this.Curry().Map(a).Apply(b);
    public static Identity<R> LiftA<A, B, C, R>(this Func<A, B, C, R> @this, Identity<A> a, Identity<B> b, Identity<C> c) => @this.Curry().Map(a).Apply(b).Apply(c);
    public static Identity<R> LiftA<A, B, C, D, R>(this Func<A, B, C, D, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d);
    public static Identity<R> LiftA<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d, Identity<E> e) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static Identity<R> LiftA<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d, Identity<E> e, Identity<F> f) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static Identity<R> LiftA<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d, Identity<E> e, Identity<F> f, Identity<G> g) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static Identity<R> LiftA<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d, Identity<E> e, Identity<F> f, Identity<G> g, Identity<H> h) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static Identity<R> LiftA<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d, Identity<E> e, Identity<F> f, Identity<G> g, Identity<H> h, Identity<I> i) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static Identity<R> LiftA<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Identity<A> a, Identity<B> b, Identity<C> c, Identity<D> d, Identity<E> e, Identity<F> f, Identity<G> g, Identity<H> h, Identity<I> i, Identity<J> j) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    #region Traverse
    public static IEnumerable<Identity<B>> Traverse<A, B>(this Identity<A> @this, Func<A, IEnumerable<B>> f) => @this.Fold(a => f(a).Map(Of));
    public static Maybe<Identity<B>> Traverse<A, B>(this Identity<A> @this, Func<A, Maybe<B>> f) => @this.Fold(a => f(a).Map(Of));
    public static Result<Identity<B>> Traverse<A, B>(this Identity<A> @this, Func<A, Result<B>> f) => @this.Fold(a => f(a).Map(Of));
    public static IO<Identity<B>> Traverse<A, B>(this Identity<A> @this, Func<A, IO<B>> f) => @this.Fold(a => f(a).Map(Of));
    public static Reader<R, Identity<B>> Traverse<R, A, B>(this Identity<A> @this, Func<A, Reader<R, B>> f) => @this.Fold(a => f(a).Map(Of));
    public static Either<L, Identity<B>> Traverse<L, A, B>(this Identity<A> @this, Func<A, Either<L, B>> f) => @this.Fold(a => f(a).Map(Of));
    public static Validation<L, Identity<B>> Traverse<L, A, B>(this Identity<A> @this, Func<A, Validation<L, B>> f) => @this.Fold(a => f(a).Map(Of));
    #endregion

    #region Sequence
    public static IEnumerable<Identity<A>> Sequence<A>(Identity<IEnumerable<A>> @this) => @this.Traverse(Id<IEnumerable<A>>());
    public static Maybe<Identity<A>> Sequence<A>(Identity<Maybe<A>> @this) => @this.Traverse(Id<Maybe<A>>());
    public static Result<Identity<A>> Sequence<A>(Identity<Result<A>> @this) => @this.Traverse(Id<Result<A>>());
    public static IO<Identity<A>> Sequence<A>(Identity<IO<A>> @this) => @this.Traverse(Id<IO<A>>());
    public static Reader<R, Identity<A>> Sequence<R, A>(Identity<Reader<R, A>> @this) => @this.Traverse(Id<Reader<R, A>>());
    public static Either<L, Identity<A>> Sequence<L, A>(Identity<Either<L, A>> @this) => @this.Traverse(Id<Either<L, A>>());
    public static Validation<L, Identity<A>> Sequence<L, A>(Identity<Validation<L, A>> @this) => @this.Traverse(Id<Validation<L, A>>());
    #endregion 

    #region Selective Applicative Functor
    public static Identity<B> Selective<A, B>(this Identity<Either<A, B>> @this, Identity<Func<A, B>> f) => @this.Value.IsRight ? Of(@this.Value.RValue) : Of(f.Value(@this.Value.LValue));
    #endregion

    #region Match
    public static void Match<A>(this Identity<A> @this, Action<A> f) => f(@this.Value);
    public static R Match<A, R>(this Identity<A> @this, Func<A, R> f) => f(@this.Value);
    #endregion

    #region DebugPrint
    public static void DebugPrint<A>(this Identity<A> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Linq Conformance
    public static Identity<R> Select<A, R>(this Identity<A> @this, Func<A, R> fn) => @this.Map(fn);
    public static Identity<R> SelectMany<A, R>(this Identity<A> @this, Func<A, Identity<R>> fn) => @this.FlatMap(fn);
    public static Identity<R> SelectMany<A, B, R>(this Identity<A> @this, Func<A, Identity<B>> fn, Func<A, B, R> select) => @this.FlatMap(a => fn(a).FlatMap(b => select(a, b).ToIdentity()));
    #endregion

    #region Get
    public static A Get<A>(this Identity<A> @this) => @this.Value;
    #endregion

    #region ToIdentity
    public static Func<A, Identity<B>> ToIdentity<A, B>(this Func<A, B> f) => a => Of<B>(f(a));
    #endregion
  }

  public static partial class Prelude {
    #region Syntactic Sugar
    public static Identity<A> ToIdentity<A>(this A a) => Identity<A>.Of(a);
    public static Identity<A> Of<A>(A a) => Identity<A>.Of(a);
    public static Func<A, Identity<A>> Of<A>() => a => Of<A>(a);
    #endregion
  }
}
