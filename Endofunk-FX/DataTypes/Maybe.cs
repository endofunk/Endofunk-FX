using System;
using System.Collections.Generic;

using static Endofunk.FX.Prelude;

namespace Endofunk.FX {

  #region Maybe Datatype
  /// <summary>
  /// The Maybe type with associated operations.
  /// 
  /// The Maybe type encapsulates an optional value. A value of type Maybe a either contains
  /// a value of type a (represented as Just), or it is empty (represented as Nothing). 
  /// Using Maybe is a good way to deal with errors or exceptional cases without resorting to 
  /// drastic measures such as error.
  /// 
  /// The Maybe type is also a monad. It is a simple kind of error monad, where all errors are 
  /// represented by Nothing. A richer error monad can be built using the Either, Result or Validation types.
  /// </summary>
  public struct Maybe<A> : IEquatable<Maybe<A>> {
    internal readonly A Value;
    public readonly bool IsJust;
    public bool IsNothing => !IsJust;
    private Maybe(A value, bool isJust = true) => (IsJust, Value) = (isJust == false || value == null) ? (false, default) : (true, value);
    public static implicit operator Maybe<A>(A value) => value == null ? Nothing<A>() : Just(value);
    public static Maybe<A> Just(A value) => new Maybe<A>(value);
    public static Maybe<A> Nothing() => new Maybe<A>(default, false);
    public override string ToString() => $"Maybe<{typeof(A).Simplify()}>[{(IsJust ? "Just" : "Nothing")}{(IsNothing ? "" : ": " + this.Fold(s => s.ToString(), () => ""))}]";

    /// <summary>
    /// Determines whether the specified <see cref="Endofunk.FX.Maybe`1"/> is equal to the current <see cref="T:Endofunk.FX.Maybe`1"/>.
    /// </summary>
    /// <param name="other">The <see cref="Endofunk.FX.Maybe`1"/> to compare with the current <see cref="T:Endofunk.FX.Maybe`1"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="Endofunk.FX.Maybe`1"/> is equal to the current
    /// <see cref="T:Endofunk.FX.Maybe`1"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(Maybe<A> other) {
      switch ((IsJust, other.IsJust)) {
        case var t when t.And(true, true):
          return Value.Equals(other.Value);
        case var t when t.And(false, false):
          return true;
        default:
          return false;
      }
    }

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:Endofunk.FX.Maybe`1"/>.
    /// </summary>
    /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:Endofunk.FX.Maybe`1"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="T:Endofunk.FX.Maybe`1"/>;
    /// otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj) {
      if (obj == null) return false;
      var objType = obj.GetType();
      var isStruct = objType.IsValueType && !objType.IsEnum;
      if (obj is Maybe<A> && isStruct) {
        return Equals((Maybe<A>)obj);
      }
      return false;
    }

    /// <summary>
    /// Determines whether a specified instance of <see cref="Endofunk.FX.Maybe`1"/> is equal to another specified <see cref="Endofunk.FX.Maybe<A>"/>.
    /// </summary>
    /// <param name="this">The first <see cref="Endofunk.FX.Maybe`1"/> to compare.</param>
    /// <param name="other">The second <see cref="Endofunk.FX.Maybe`1"/> to compare.</param>
    /// <returns><c>true</c> if <c>this</c> and <c>other</c> are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Maybe<A> @this, Maybe<A> other) => @this.Equals(other);

    /// <summary>
    /// Determines whether a specified instance of <see cref="Endofunk.FX.Maybe`1"/> is not equal to another specified <see cref="Endofunk.FX.Maybe<A>"/>.
    /// </summary>
    /// <param name="this">The first <see cref="Endofunk.FX.Maybe`1"/> to compare.</param>
    /// <param name="other">The second <see cref="Endofunk.FX.Maybe`1"/> to compare.</param>
    /// <returns><c>true</c> if <c>this</c> and <c>other</c> are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Maybe<A> @this, Maybe<A> other) => !(@this == other);

    /// <summary>
    /// Serves as a hash function for a <see cref="T:Endofunk.FX.Maybe`1"/> object.
    /// </summary>
    /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
    public override int GetHashCode() => (Value, IsJust).GetHashCode();

    /// <summary>
    /// Returns the input typed as IEnumerable&lt;<typeparamref name="A"/>&gt;.
    /// </summary>
    public IEnumerable<A> AsEnumerable() {
      if (IsJust) yield return Value;
    }
  }
  #endregion

  public static partial class Prelude {

    #region Fold
    public static U Fold<T, U>(this Maybe<T> ts, U identity, Func<U, T, U> fn) {
      var accumulator = identity;
      foreach (T element in ts.AsEnumerable()) { accumulator = fn(accumulator, element); }
      return accumulator;
    }

    public static R Fold<A, R>(this Maybe<A> @this, Func<A, R> just, Func<R> nothing) => @this.IsJust ? just(@this.Value) : nothing();
    #endregion

    #region Functor
    public static Maybe<U> Map<T, U>(this Maybe<T> @this, Func<T, U> fn) => @this.Fold(Maybe<U>.Nothing(), (a, e) => @this.IsJust ? Maybe<U>.Just(fn(e)) : a);
    public static Maybe<R> Map<A, R>(this Func<A, R> fn, Maybe<A> @this) => @this.Map(fn);
    #endregion

    #region Monad
    public static Maybe<R> FlatMap<A, R>(this Maybe<A> @this, Func<A, Maybe<R>> fn) => @this.IsJust ? fn(@this.Value) : Nothing<R>();
    public static Maybe<R> FlatMap<A, R>(this Func<A, Maybe<R>> fn, Maybe<A> @this) => @this.FlatMap(fn);
    public static Func<Maybe<A>, Maybe<B>> FlatMap<A, B>(this Func<A, Maybe<B>> f) => a => a.FlatMap(f);
    public static Maybe<R> Bind<A, R>(this Maybe<A> @this, Func<A, Maybe<R>> fn) => @this.FlatMap(fn);
    #endregion

    #region Kleisli Composition
    public static Func<A, Maybe<C>> Compose<A, B, C>(this Func<A, Maybe<B>> f, Func<B, Maybe<C>> g) => f.Compose(g.FlatMap());
    #endregion

    #region Monad - Lift a function & actions
    public static Maybe<R> LiftM<A, R>(this Func<A, R> @this, Maybe<A> a) => a.FlatMap(xa => Just(@this(xa)));
    public static Maybe<R> LiftM<A, B, R>(this Func<A, B, R> @this, Maybe<A> a, Maybe<B> b) => a.FlatMap(xa => b.FlatMap(xb => Just(@this(xa, xb))));
    public static Maybe<R> LiftM<A, B, C, R>(this Func<A, B, C, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => Just(@this(xa, xb, xc)))));
    public static Maybe<R> LiftM<A, B, C, D, R>(this Func<A, B, C, D, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => Just(@this(xa, xb, xc, xd))))));
    public static Maybe<R> LiftM<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => Just(@this(xa, xb, xc, xd, xe)))))));
    public static Maybe<R> LiftM<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => Just(@this(xa, xb, xc, xd, xe, xf))))))));
    public static Maybe<R> LiftM<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f, Maybe<G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => Just(@this(xa, xb, xc, xd, xe, xf, xg)))))))));
    public static Maybe<R> LiftM<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f, Maybe<G> g, Maybe<H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => Just(@this(xa, xb, xc, xd, xe, xf, xg, xh))))))))));
    public static Maybe<R> LiftM<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f, Maybe<G> g, Maybe<H> h, Maybe<I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => Just(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi)))))))))));
    public static Maybe<R> LiftM<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Maybe<A> a, Maybe<B> b, Maybe<C> c, Maybe<D> d, Maybe<E> e, Maybe<F> f, Maybe<G> g, Maybe<H> h, Maybe<I> i, Maybe<J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => Just(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj))))))))))));
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

    #region Zip
    public static Maybe<(A, B)> Zip<A, B>(this Maybe<A> @this, Maybe<B> other) => Tuple<A, B>().ZipWith(@this, other);
    public static Maybe<C> ZipWith<A, B, C>(this Func<A, B, C> f, Maybe<A> ma, Maybe<B> mb) => f.LiftM(ma, mb);
    public static (Maybe<A>, Maybe<B>) UnZip<A, B>(this Maybe<(A, B)> @this) => (First<A, B>().LiftM(@this), Second<A, B>().LiftM(@this));
    #endregion

    #region Match
    public static void Match<A>(this Maybe<A> @this, Action nothing, Action<A> just) {
      switch (@this.IsJust) {
        case true:
          just(@this.Value);
          return;
        default:
          nothing();
          return;
      }
    }
    public static R Match<A, R>(this Maybe<A> @this, Func<R> nothing, Func<A, R> just) => @this.IsJust ? just(@this.Value) : nothing();
    #endregion

    #region DebugPrint
    public static void DebugPrint<A>(this Maybe<A> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Syntactic Sugar - Just / Nothing
    public static Maybe<A> Just<A>(A value) => Maybe<A>.Just(value);
    public static Maybe<A> Nothing<A>() => Maybe<A>.Nothing();
    public static Maybe<A> ToMaybe<A>(this A a) => Just(a);
    public static Func<A, Maybe<B>> ToMaybe<A, B>(this Func<A, B> f) => a => Maybe<B>.Just(f(a));
    #endregion

    #region Traverse
    public static IEnumerable<Maybe<R>> Traverse<A, R>(this Maybe<A> @this, Func<A, IEnumerable<R>> fn) => @this.Match(() => List(Nothing<R>()), t => fn(t).Map(Just));
    #endregion

    #region Linq Conformance
    public static Maybe<R> Select<A, R>(this Maybe<A> @this, Func<A, R> fn) => @this.Map(fn);
    public static Maybe<R> SelectMany<A, R>(this Maybe<A> @this, Func<A, Maybe<R>> fn) => @this.FlatMap(fn);
    public static Maybe<R> SelectMany<A, B, R>(this Maybe<A> @this, Func<A, Maybe<B>> fn, Func<A, B, R> select) => @this.FlatMap(a => fn(a).FlatMap(b => select(a, b).ToMaybe()));
    #endregion

    #region GetOrElse
    public static A GetOrElse<A>(this Maybe<A> @this, A other) => @this.IsJust ? @this.Value : other;
    #endregion
 
  }

}
