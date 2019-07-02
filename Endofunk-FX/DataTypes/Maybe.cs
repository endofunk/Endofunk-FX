using System;
using System.Collections.Generic;

using static Endofunk.FX.Prelude;

namespace Endofunk.FX {

  #region Maybe Datatype
  public struct Maybe<A> : IEquatable<Maybe<A>> {
    internal readonly A Value;
    public readonly bool IsSome;
    public bool IsNone => !IsSome;
    private Maybe(A value, bool isSome = true) => (IsSome, Value) = (isSome == false || value == null) ? (false, default) : (true, value);
    public static implicit operator Maybe<A>(A value) => value == null ? None<A>() : Some(value);
    public static Maybe<A> Some(A value) => new Maybe<A>(value);
    public static Maybe<A> None() => new Maybe<A>(default, false);
    public override string ToString() => $"Maybe<{typeof(A).Simplify()}>[{(IsSome ? "Some" : "None")}{(IsNone ? "" : ": " + this.Fold(s => s.ToString(), () => ""))}]";

    public bool Equals(Maybe<A> other) {
      switch ((IsSome, other.IsSome)) {
        case var t when t.And(true, true):
          return Value.Equals(other.Value);
        case var t when t.And(false, false):
          return true;
        default:
          return false;
      }
    }

    public override bool Equals(object obj) {
      if (obj == null) return false;
      var objType = obj.GetType();
      var isStruct = objType.IsValueType && !objType.IsEnum;
      if (obj is Maybe<A> && isStruct) {
        return Equals((Maybe<A>)obj);
      }
      return false;
    }

    public static bool operator ==(Maybe<A> @this, Maybe<A> other) => @this.Equals(other);
    public static bool operator !=(Maybe<A> @this, Maybe<A> other) => !(@this == other);
    public override int GetHashCode() => (Value, IsSome).GetHashCode();

    public IEnumerable<A> AsEnumerable() {
      if (IsSome) yield return Value;
    }
  }
  #endregion

  public static partial class Prelude {

    #region Fold
    public static R Fold<A, R>(this Maybe<A> @this, Func<A, R> some, Func<R> none) => @this.IsSome ? some(@this.Value) : none();
    #endregion

    #region Functor
    public static Maybe<R> Map<A, R>(this Func<A, R> fn, Maybe<A> @this) => @this.Map(fn);
    public static Maybe<R> Map<A, R>(this Maybe<A> @this, Func<A, R> fn) => @this.IsSome ? Some(fn(@this.Value)) : None<R>();
    #endregion

    #region Monad
    public static Maybe<R> FlatMap<A, R>(this Maybe<A> @this, Func<A, Maybe<R>> fn) => @this.IsSome ? fn(@this.Value) : None<R>();
    public static Maybe<R> FlatMap<A, R>(this Func<A, Maybe<R>> fn, Maybe<A> @this) => @this.FlatMap(fn);
    public static Func<Maybe<A>, Maybe<B>> FlatMap<A, B>(this Func<A, Maybe<B>> f) => a => a.FlatMap(f);
    public static Maybe<R> Bind<A, R>(this Maybe<A> @this, Func<A, Maybe<R>> fn) => @this.FlatMap(fn);
    #endregion

    #region Kleisli Composition
    public static Func<A, Maybe<C>> Compose<A, B, C>(this Func<A, Maybe<B>> f, Func<B, Maybe<C>> g) => f.Compose(g.FlatMap());
    #endregion

    #region Monad - Lift a function & actions
    public static Maybe<R> LiftM<A, B, R>(this Func<A, B, R> @this, Maybe<A> a, Maybe<B> b) => a.FlatMap(xa => b.FlatMap(xb => Some(@this(xa, xb))));
    public static Maybe<R> LiftM<A, B, C, R>(this Func<A, B, C, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => Some(@this(xa, xb, xc)))));
    public static Maybe<R> LiftM<A, B, C, D, R>(this Func<A, B, C, D, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => Some(@this(xa, xb, xc, xd))))));
    public static Maybe<R> LiftM<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => Some(@this(xa, xb, xc, xd, xe)))))));
    public static Maybe<R> LiftM<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => Some(@this(xa, xb, xc, xd, xe, xf))))))));
    public static Maybe<R> LiftM<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f, Maybe<G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => Some(@this(xa, xb, xc, xd, xe, xf, xg)))))))));
    public static Maybe<R> LiftM<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f, Maybe<G> g, Maybe<H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => Some(@this(xa, xb, xc, xd, xe, xf, xg, xh))))))))));
    public static Maybe<R> LiftM<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f, Maybe<G> g, Maybe<H> h, Maybe<I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => Some(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi)))))))))));
    public static Maybe<R> LiftM<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f, Maybe<G> g, Maybe<H> h, Maybe<I> i, Maybe<J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => Some(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj))))))))))));
    #endregion

    #region Applicative Functor
    public static Maybe<R> Apply<A, R>(this Maybe<A> @this, Maybe<Func<A, R>> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
    public static Maybe<R> Apply<A, R>(this Maybe<Func<A, R>> fn, Maybe<A> @this) => @this.Apply(fn);
    #endregion

    #region Applicative Functor - Lift a function & actions
    public static Maybe<R> LiftA<A, R>(this Func<A, R> @this, Maybe<A> a) => @this.Map(a);
    public static Maybe<R> LiftA<A, B, R>(this Func<A, B, R> @this, Maybe<A> a, Maybe<B> b) => @this.Curry().Map(a).Apply(b);
    public static Maybe<R> LiftA<A, B, C, R>(this Func<A, B, C, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c) => @this.Curry().Map(a).Apply(b).Apply(c);
    public static Maybe<R> LiftA<A, B, C, D, R>(this Func<A, B, C, D, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d);
    public static Maybe<R> LiftA<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static Maybe<R> LiftA<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static Maybe<R> LiftA<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f, Maybe<G> g) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static Maybe<R> LiftA<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f, Maybe<G> g, Maybe<H> h) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static Maybe<R> LiftA<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f, Maybe<G> g, Maybe<H> h, Maybe<I> i) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static Maybe<R> LiftA<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f, Maybe<G> g, Maybe<H> h, Maybe<I> i, Maybe<J> j) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    #region Match
    public static void Match<A>(this Maybe<A> @this, Action none, Action<A> just) {
      switch (@this.IsSome) {
        case true:
          just(@this.Value);
          return;
        default:
          none();
          return;
      }
    }
    public static R Match<A, R>(this Maybe<A> @this, Func<R> none, Func<A, R> just) => @this.IsSome ? just(@this.Value) : none();
    #endregion

    #region DebugPrint
    public static void DebugPrint<A>(this Maybe<A> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Syntactic Sugar - Just / None
    public static Maybe<A> Some<A>(A value) => Maybe<A>.Some(value);
    public static Maybe<A> None<A>() => Maybe<A>.None();
    public static Maybe<A> ToMaybe<A>(this A a) => Some(a);
    public static Func<A, Maybe<B>> ToMaybe<A, B>(this Func<A, B> f) => a => Maybe<B>.Some(f(a));
    #endregion

    #region Traverse
    public static IEnumerable<Maybe<R>> Traverse<A, R>(this Maybe<A> @this, Func<A, IEnumerable<R>> fn) => @this.Match(() => List(None<R>()), t => fn(t).Map(Some));
    #endregion

    #region Linq Conformance
    public static Maybe<R> Select<A, R>(this Maybe<A> @this, Func<A, R> fn) => @this.Map(fn);
    public static Maybe<R> SelectMany<A, R>(this Maybe<A> @this, Func<A, Maybe<R>> fn) => @this.FlatMap(fn);
    public static Maybe<R> SelectMany<A, B, R>(this Maybe<A> @this, Func<A, Maybe<B>> fn, Func<A, B, R> select) => @this.FlatMap(a => fn(a).FlatMap(b => select(a, b).ToMaybe()));
    #endregion

    #region GetOrElse
    public static A GetOrElse<A>(this Maybe<A> @this, A other) => @this.IsSome ? @this.Value : other;
    #endregion
 
  }

}
