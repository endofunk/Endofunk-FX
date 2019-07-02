using System;
using System.Collections.Generic;

namespace Endofunk.FX {

  #region Either Datatype
  public struct Either<L, R> : IEquatable<Either<L, R>> {
    internal readonly L LValue;
    internal readonly R RValue;
    private enum Status {
      Left, Right
    }
    private readonly Status State;
    public bool IsRight => State == Status.Right;
    public bool IsLeft => State == Status.Left;
    private Either(L left, R right, Status state) => (State, LValue, RValue) = (state, left, right);
    public static implicit operator Either<L, R>(R value) => value == null ? Prelude.Left<L, R>(default) : Prelude.Right<L, R>(value);
    internal static Either<L, R> Right(R right) => new Either<L, R>(default, right, Status.Right);
    internal static Either<L, R> Left(L left) => new Either<L, R>(left, default, Status.Left);

    public A Eval<A>(Func<L, A> f, Func<R, A> g) {
      switch (State) {
        case Status.Left: return f(LValue);
        default: return g(RValue);
      }
    }

    public bool Equals(Either<L, R> other) {
      if (IsRight && other.IsRight && RValue.Equals(other.RValue)) return true;
      if (IsLeft && other.IsLeft && LValue.Equals(other.LValue)) return true;
      return false;
    }

    public override bool Equals(object obj) {
      if (obj == null) return false;
      var objType = obj.GetType();
      var isStruct = objType.IsValueType && !objType.IsEnum;
      if (obj is Either<L, R> && isStruct) {
        return Equals((Either<L, R>)obj);
      }
      return false;
    }

    public static bool operator ==(Either<L, R> @this, Either<L, R> other) => @this.Equals(other);
    public static bool operator !=(Either<L, R> @this, Either<L, R> other) => !(@this == other);
    public override int GetHashCode() => (LValue, RValue, State).GetHashCode();

    public IEnumerable<R> AsEnumerable() {
      if (IsRight) yield return RValue;
    }

    public override string ToString() => $"Either<{typeof(L).Simplify()}, {typeof(R).Simplify()}>[{State}: {this.Fold(s => s.ToString(), r => r.ToString())}]";
  }
  #endregion

  public static partial class Prelude {

    #region Either - Fold
    public static R2 Fold<L, R, R2>(this Either<L, R> e, Func<L, R2> left, Func<R, R2> right) => e.IsRight ? right(e.RValue) : left(e.LValue);
    #endregion

    #region Functor - MapL, MapR
    public static Either<L, R2> MapR<L, R, R2>(this Func<R, R2> fn, Either<L, R> e) => e.MapR(fn);
    public static Either<L, R2> MapR<L, R, R2>(this Either<L, R> e, Func<R, R2> fn) => e.IsRight ? Right<L, R2>(fn(e.RValue)) : Left<L, R2>(e.LValue);
    public static Either<L2, R> MapL<L, L2, R>(this Func<L, L2> fn, Either<L, R> e) => e.MapL(fn);
    public static Either<L2, R> MapL<L, L2, R>(this Either<L, R> e, Func<L, L2> fn) => e.IsRight ? Right<L2, R>(e.RValue) : Left<L2, R>(fn(e.LValue));
    #endregion

    #region Functor - Map (Right Affinity)
    public static Either<L, R2> Map<L, R, R2>(this Either<L, R> e, Func<R, R2> fn) => e.MapR(fn);
    public static Either<L, R2> Map<L, R, R2>(this Func<R, R2> fn, Either<L, R> e) => e.MapR(fn);
    #endregion

    #region Functor - Bimap, First, Second
    public static Either<L2, R2> BiMap<L, L2, R, R2>(this Either<L, R> @this, Func<L, L2> left, Func<R, R2> right) => @this.IsRight ? Right<L2, R2>(right(@this.RValue)) : Left<L2, R2>(left(@this.LValue));
    public static Either<L2, R> First<L, R, L2>(Either<L, R> @this, Func<L, L2> f) => @this.BiMap(f, Id);
    public static Either<L, R2> Second<L, R, R2>(Either<L, R> @this, Func<R, R2> f) => @this.BiMap(Id, f);
    #endregion

    #region Monad - FlatMapL, FlatMapR
    public static Either<L, R2> FlatMapR<L, R, R2>(this Either<L, R> e, Func<R, Either<L, R2>> fn) => e.IsRight ? fn(e.RValue) : Left<L, R2>(e.LValue);
    public static Either<L, R2> FlatMapR<L, R, R2>(this Func<R, Either<L, R2>> fn, Either<L, R> e) => e.FlatMapR(fn);
    public static Either<L2, R> FlatMapL<L, L2, R>(this Either<L, R> e, Func<L, Either<L2, R>> fn) => e.IsRight ? Right<L2, R>(e.RValue) : fn(e.LValue);
    public static Either<L2, R> FlatMapL<L, L2, R>(this Func<L, Either<L2, R>> fn, Either<L, R> e) => e.FlatMapL(fn);
    #endregion

    #region Monad - Bind, FlatMap (Right Affinity)
    public static Either<L, R2> Bind<L, R, R2>(this Either<L, R> e, Func<R, Either<L, R2>> fn) => e.FlatMapR(fn);
    public static Either<L, R2> Bind<L, R, R2>(this Func<R, Either<L, R2>> fn, Either<L, R> e) => e.FlatMapR(fn);
    public static Either<L, R2> FlatMap<L, R, R2>(this Either<L, R> e, Func<R, Either<L, R2>> fn) => e.FlatMapR(fn);
    public static Either<L, R2> FlatMap<L, R, R2>(this Func<R, Either<L, R2>> fn, Either<L, R> e) => e.FlatMapR(fn);
    public static Func<Either<L, R>, Either<L, R2>> FlatMap<L, R, R2>(this Func<R, Either<L, R2>> f) => a => a.FlatMap(f);
    #endregion

    #region Kleisli Composition
    public static Func<A, Either<L, C>> Compose<L, A, B, C>(this Func<A, Either<L, B>> f, Func<B, Either<L, C>> g) => f.Compose(g.FlatMap());
    #endregion

    #region Monad - Lift a function & actions
    public static Either<L, R> LiftM<A, B, L, R>(this Func<A, B, R> @this, Either<L, A> a, Either<L, B> b) => a.FlatMap(xa => b.FlatMap(xb => Right<L, R>(@this(xa, xb))));
    public static Either<L, R> LiftM<A, B, C, L, R>(this Func<A, B, C, R> @this, Either<L, A> a, Either<L, B> b, Either<L, C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => Right<L, R>(@this(xa, xb, xc)))));
    public static Either<L, R> LiftM<A, B, C, D, L, R>(this Func<A, B, C, D, R> @this, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => Right<L, R>(@this(xa, xb, xc, xd))))));
    public static Either<L, R> LiftM<A, B, C, D, E, L, R>(this Func<A, B, C, D, E, R> @this, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d, Either<L, E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => Right<L, R>(@this(xa, xb, xc, xd, xe)))))));
    public static Either<L, R> LiftM<A, B, C, D, E, F, L, R>(this Func<A, B, C, D, E, F, R> @this, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d, Either<L, E> e, Either<L, F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => Right<L, R>(@this(xa, xb, xc, xd, xe, xf))))))));
    public static Either<L, R> LiftM<A, B, C, D, E, F, G, L, R>(this Func<A, B, C, D, E, F, G, R> @this, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d, Either<L, E> e, Either<L, F> f, Either<L, G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => Right<L, R>(@this(xa, xb, xc, xd, xe, xf, xg)))))))));
    public static Either<L, R> LiftM<A, B, C, D, E, F, G, H, L, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d, Either<L, E> e, Either<L, F> f, Either<L, G> g, Either<L, H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => Right<L, R>(@this(xa, xb, xc, xd, xe, xf, xg, xh))))))))));
    public static Either<L, R> LiftM<A, B, C, D, E, F, G, H, I, L, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d, Either<L, E> e, Either<L, F> f, Either<L, G> g, Either<L, H> h, Either<L, I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => Right<L, R>(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi)))))))))));
    public static Either<L, R> LiftM<A, B, C, D, E, F, G, H, I, J, L, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d, Either<L, E> e, Either<L, F> f, Either<L, G> g, Either<L, H> h, Either<L, I> i, Either<L, J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => Right<L, R>(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj))))))))))));
    #endregion

    #region Applicative Functor - ApplyL, ApplyR
    public static Either<L, R2> ApplyR<L, R, R2>(this Either<L, R> e, Either<L, Func<R, R2>> fn) => fn.FlatMapR(g => e.MapR(x => g(x)));
    public static Either<L, R2> ApplyR<L, R, R2>(this Either<L, Func<R, R2>> fn, Either<L, R> e) => e.Apply(fn);
    public static Either<L2, R> ApplyL<L, L2, R>(this Either<L, R> e, Either<Func<L, L2>, R> fn) => fn.FlatMapL(g => e.MapL(x => g(x)));
    public static Either<L2, R> ApplyL<L, L2, R>(this Either<Func<L, L2>, R> fn, Either<L, R> e) => e.ApplyL(fn);
    #endregion

    #region Applicative Functor - Apply (Right Affinity)
    public static Either<L, R2> Apply<L, R, R2>(this Either<L, R> e, Either<L, Func<R, R2>> fn) => e.ApplyR(fn);
    public static Either<L, R2> Apply<L, R, R2>(this Either<L, Func<R, R2>> fn, Either<L, R> e) => e.ApplyR(fn);
    #endregion

    #region Applicative Functor - Lift a function & actions (Right Affinity)
    public static Either<L, R> LiftA<A, L, R>(this Func<A, R> fn, Either<L, A> a) => fn.Map(a);
    public static Either<L, R> LiftA<A, B, L, R>(this Func<A, B, R> fn, Either<L, A> a, Either<L, B> b) => fn.Curry().Map(a).Apply(b);
    public static Either<L, R> LiftA<A, B, C, L, R>(this Func<A, B, C, R> fn, Either<L, A> a, Either<L, B> b, Either<L, C> c) => fn.Curry().Map(a).Apply(b).Apply(c);
    public static Either<L, R> LiftA<A, B, C, D, L, R>(this Func<A, B, C, D, R> fn, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d);
    public static Either<L, R> LiftA<A, B, C, D, E, L, R>(this Func<A, B, C, D, E, R> fn, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d, Either<L, E> e) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static Either<L, R> LiftA<A, B, C, D, E, F, L, R>(this Func<A, B, C, D, E, F, R> fn, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d, Either<L, E> e, Either<L, F> f) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static Either<L, R> LiftA<A, B, C, D, E, F, G, L, R>(this Func<A, B, C, D, E, F, G, R> fn, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d, Either<L, E> e, Either<L, F> f, Either<L, G> g) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static Either<L, R> LiftA<A, B, C, D, E, F, G, H, L, R>(this Func<A, B, C, D, E, F, G, H, R> fn, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d, Either<L, E> e, Either<L, F> f, Either<L, G> g, Either<L, H> h) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static Either<L, R> LiftA<A, B, C, D, E, F, G, H, I, L, R>(this Func<A, B, C, D, E, F, G, H, I, R> fn, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d, Either<L, E> e, Either<L, F> f, Either<L, G> g, Either<L, H> h, Either<L, I> i) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static Either<L, R> LiftA<A, B, C, D, E, F, G, H, I, J, L, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> fn, Either<L, A> a, Either<L, B> b, Either<L, C> c, Either<L, D> d, Either<L, E> e, Either<L, F> f, Either<L, G> g, Either<L, H> h, Either<L, I> i, Either<L, J> j) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    #region Match
    public static void Match<L, R>(this Either<L, R> e, Action<L> left, Action<R> right) {
      switch (e.IsRight) {
        case true:
          right(e.RValue);
          return;
        default:
          left(e.LValue);
          return;
      }
    }

    public static R2 Match<L, R, R2>(this Either<L, R> @this, Func<L, R2> left, Func<R, R2> right) => (@this.IsRight) ? right(@this.RValue) : left(@this.LValue);
    #endregion

    #region DebugPrint
    public static void DebugPrint<L, R>(this Either<L, R> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Syntactic Sugar - Right / Left
    public static Either<L, R> Right<L, R>(R value) => Either<L, R>.Right(value);
    public static Either<L, R> Left<L, R>(L value) => Either<L, R>.Left(value);
    public static Either<L, R> ToEither<L, R>(this R r) => Right<L, R>(r);
    public static Func<A, Either<L, B>> ToEither<L, A, B>(this Func<A, B> f) => a => Right<L, B>(f(a));
    #endregion

    #region Linq Conformance
    public static Either<L, R2> Select<L, R, R2>(this Either<L, R> @this, Func<R, R2> fn) => @this.Map(fn);
    public static Either<L, R2> SelectMany<L, R, R2>(this Either<L, R> @this, Func<R, Either<L, R2>> fn) => @this.FlatMap(fn);
    public static Either<L, R2> SelectMany<L, R, R1, R2>(this Either<L, R> @this, Func<R, Either<L, R1>> fn, Func<R, R1, R2> select) => @this.FlatMap(a => fn(a).FlatMap(b => select(a, b).ToEither<L, R2>()));
    #endregion

  }

}
