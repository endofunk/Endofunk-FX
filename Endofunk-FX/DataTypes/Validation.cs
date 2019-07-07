using System;
using System.Collections.Generic;

namespace Endofunk.FX {

  #region Validation Datatype
  public struct Validation<L, R> : IEquatable<Validation<L, R>> {
    internal readonly List<L> LValue;
    internal readonly R RValue;
    private enum Status {
      Fail, Success
    }
    private readonly Status State;
    public bool IsSuccess => State == Status.Success;
    public bool IsFail => State == Status.Fail;
    private Validation(List<L> fail, R success, Status state) => (State, LValue, RValue) = (state, state == Status.Fail ? fail : default, RValue = state == Status.Success ? success : default);
    public static implicit operator Validation<L, R>(R value) => value == null ? Prelude.Fail<L, R>(new List<L> { }) : Prelude.Success<L, R>(value);
    public static Validation<L, R> Success(R success) => new Validation<L, R>(default, success, Status.Success);
    public static Validation<L, R> Fail(List<L> fail) => new Validation<L, R>(fail, default, Status.Fail);

    public bool Equals(Validation<L, R> other) {
      if (IsSuccess && other.IsSuccess && RValue.Equals(other.RValue)) return true;
      if (IsFail && other.IsFail && LValue.Equals(other.LValue)) return true;
      return false;
    }

    public override bool Equals(object obj) {
      if (obj == null) return false;
      var objType = obj.GetType();
      var isStruct = objType.IsValueType && !objType.IsEnum;
      if (obj is Validation<L, R> && isStruct) {
        return Equals((Validation<L, R>)obj);
      }
      return false;
    }

    public static bool operator ==(Validation<L, R> @this, Validation<L, R> other) => @this.Equals(other);
    public static bool operator !=(Validation<L, R> @this, Validation<L, R> other) => !(@this == other);
    public override int GetHashCode() => (LValue, RValue, State).GetHashCode();

    public IEnumerable<R> AsEnumerable() {
      if (IsSuccess) yield return RValue;
    }
    public override string ToString() => $"Validation<{typeof(L).Simplify()}, {typeof(R).Simplify()}>[{State}: {this.Fold(s => s.ToString(), r => r.ToString())}]";
  }
  #endregion

  public static partial class Prelude {

    #region Fold
    public static R2 Fold<L, R1, R2>(this Validation<L, R1> @this, Func<List<L>, R2> l, Func<R1, R2> r) => @this.IsSuccess ? r(@this.RValue) : l(@this.LValue);
    #endregion

    #region Functor - MapL, MapR
    public static Validation<L, R2> MapR<L, R, R2>(this Func<R, R2> fn, Validation<L, R> @this) => @this.MapR(fn);
    public static Validation<L, R2> MapR<L, R, R2>(this Validation<L, R> @this, Func<R, R2> fn) => @this.IsSuccess ? Success<L, R2>(fn(@this.RValue)) : Fail<L, R2>(@this.LValue);
    public static Validation<L2, R> MapL<L, L2, R>(this Func<List<L>, List<L2>> fn, Validation<L, R> @this) => @this.MapL(fn);
    public static Validation<L2, R> MapL<L, L2, R>(this Validation<L, R> @this, Func<List<L>, List<L2>> fn) => @this.IsSuccess ? Success<L2, R>(@this.RValue) : Fail<L2, R>(fn(@this.LValue));
    #endregion

    #region Functor - Map (Right Affinity)
    public static Validation<L, R2> Map<L, R, R2>(this Validation<L, R> @this, Func<R, R2> fn) => @this.MapR(fn);
    public static Validation<L, R2> Map<L, R, R2>(this Func<R, R2> fn, Validation<L, R> @this) => @this.MapR(fn);
    #endregion

    #region Functor - Bimap, First, Second
    public static Validation<L2, R2> BiMap<L, L2, R, R2>(this Validation<L, R> @this, Func<List<L>, List<L2>> left, Func<R, R2> right) => @this.IsSuccess ? Success<L2, R2>(right(@this.RValue)) : Fail<L2, R2>(left(@this.LValue));
    public static Validation<L2, R> First<L, R, L2>(Validation<L, R> @this, Func<List<L>, List<L2>> fn) => @this.BiMap(fn, Id);
    public static Validation<L, R2> Second<L, R, R2>(Validation<L, R> @this, Func<R, R2> fn) => @this.BiMap(Id, fn);
    #endregion

    #region Monad - FlatMapL, FlatMapR
    public static Validation<L, R2> FlatMapR<L, R, R2>(this Validation<L, R> @this, Func<R, Validation<L, R2>> f) => @this.IsSuccess ? f(@this.RValue) : Fail<L, R2>(@this.LValue);
    public static Validation<L, R2> FlatMapR<L, R, R2>(this Func<R, Validation<L, R2>> f, Validation<L, R> @this) => @this.FlatMap(f);
    #endregion

    #region Monad - Bind, FlatMap (Right Affinity)
    public static Validation<L, R2> Bind<L, R, R2>(this Validation<L, R> @this, Func<R, Validation<L, R2>> f) => @this.FlatMapR(f);
    public static Validation<L, R2> Bind<L, R, R2>(this Func<R, Validation<L, R2>> f, Validation<L, R> @this) => @this.FlatMapR(f);
    public static Validation<L, R2> FlatMap<L, R, R2>(this Validation<L, R> @this, Func<R, Validation<L, R2>> f) => @this.FlatMapR(f);
    public static Validation<L, R2> FlatMap<L, R, R2>(this Func<R, Validation<L, R2>> f, Validation<L, R> @this) => @this.FlatMapR(f);
    public static Func<Validation<L, R>, Validation<L, R2>> FlatMap<L, R, R2>(this Func<R, Validation<L, R2>> f) => a => a.FlatMap(f);
    #endregion

    #region Kleisli Composition
    public static Func<A, Validation<L, C>> Compose<L, A, B, C>(this Func<A, Validation<L, B>> f, Func<B, Validation<L, C>> g) => f.Compose(g.FlatMap());
    #endregion

    #region Monad - Lift a function & actions
    public static Validation<L, R> LiftM<A, L, R>(this Func<A, R> @this, Validation<L, A> a) => a.FlatMap(m1 => Success<L, R>(@this(m1)));
    public static Validation<L, R> LiftM<A, B, L, R>(this Func<A, B, R> @this, Validation<L, A> a, Validation<L, B> b) => a.FlatMap(m1 => b.FlatMap(m2 => Success<L, R>(@this(m1, m2))));
    public static Validation<L, R> LiftM<A, B, C, L, R>(this Func<A, B, C, R> @this, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => Success<L, R>(@this(xa, xb, xc)))));
    public static Validation<L, R> LiftM<A, B, C, D, L, R>(this Func<A, B, C, D, R> @this, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => Success<L, R>(@this(xa, xb, xc, xd))))));
    public static Validation<L, R> LiftM<A, B, C, D, E, L, R>(this Func<A, B, C, D, E, R> @this, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d, Validation<L, E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => Success<L, R>(@this(xa, xb, xc, xd, xe)))))));
    public static Validation<L, R> LiftM<A, B, C, D, E, F, L, R>(this Func<A, B, C, D, E, F, R> @this, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d, Validation<L, E> e, Validation<L, F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => Success<L, R>(@this(xa, xb, xc, xd, xe, xf))))))));
    public static Validation<L, R> LiftM<A, B, C, D, E, F, G, L, R>(this Func<A, B, C, D, E, F, G, R> @this, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d, Validation<L, E> e, Validation<L, F> f, Validation<L, G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => Success<L, R>(@this(xa, xb, xc, xd, xe, xf, xg)))))))));
    public static Validation<L, R> LiftM<A, B, C, D, E, F, G, H, L, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d, Validation<L, E> e, Validation<L, F> f, Validation<L, G> g, Validation<L, H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => Success<L, R>(@this(xa, xb, xc, xd, xe, xf, xg, xh))))))))));
    public static Validation<L, R> LiftM<A, B, C, D, E, F, G, H, I, L, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d, Validation<L, E> e, Validation<L, F> f, Validation<L, G> g, Validation<L, H> h, Validation<L, I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => Success<L, R>(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi)))))))))));
    public static Validation<L, R> LiftM<A, B, C, D, E, F, G, H, I, J, L, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d, Validation<L, E> e, Validation<L, F> f, Validation<L, G> g, Validation<L, H> h, Validation<L, I> i, Validation<L, J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => Success<L, R>(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj))))))))))));
    #endregion

    #region Applicative Functor - ApplyL, ApplyR
    public static Validation<L, R2> ApplyR<L, R, R2>(this Validation<L, R> @this, Validation<L, Func<R, R2>> f) {
      switch ((a: f.IsSuccess, b: @this.IsSuccess)) {
        case var t when t.And(true, true):
          return f.FlatMap(g => @this.MapR(x => g(x)));
        case var t when t.And(false, false):
          f.LValue.AddRange(@this.LValue);
          return Fail<L, R2>(f.LValue);
        case var t when t.First(false):
          return Fail<L, R2>(f.LValue);
        default:
          return Fail<L, R2>(@this.LValue);
      }
    }

    public static Validation<L, R2> ApplyR<L, R, R2>(this Validation<L, Func<R, R2>> fn, Validation<L, R> @this) => @this.ApplyR(fn);
    #endregion

    #region Applicative Functor - Apply (Right Affinity)
    public static Validation<L, R2> Apply<L, R, R2>(this Validation<L, R> @this, Validation<L, Func<R, R2>> fn) => @this.ApplyR(fn);
    public static Validation<L, R2> Apply<L, R, R2>(this Validation<L, Func<R, R2>> fn, Validation<L, R> @this) => @this.ApplyR(fn);
    public static Func<Validation<L, R>, Validation<L, R2>> Apply<L, R, R2>(this Validation<L, Func<R, R2>> fn) => e => e.Apply(fn);

    /// <summary>
    /// Sequence actions, discarding the value of the first argument.
    /// (*>) :: f a -> f b -> f b
    /// </summary>
    public static Validation<L, RB> DropFirst<L, RA, RB>(this Validation<L, RA> @this, Validation<L, RB> other) => Const<RB, RA>().Flip().LiftA(@this, other);

    /// <summary>
    /// Sequence actions, discarding the value of the second argument.
    /// (<*) :: f a -> f b -> f a
    /// </summary>
    public static Validation<L, RA> DropSecond<L, RA, RB>(this Validation<L, RA> @this, Validation<L, RB> other) => Const<RA, RB>().LiftA(@this, other);
    #endregion

    #region Applicative Functor - Lift a function & actions (Right Affinity)
    public static Validation<L, R> LiftA<A, L, R>(this Func<A, R> fn, Validation<L, A> a) => fn.MapR(a); 
    public static Validation<L, R> LiftA<A, B, L, R>(this Func<A, B, R> fn, Validation<L, A> a, Validation<L, B> b) => fn.Curry().MapR(a).Apply(b);
    public static Validation<L, R> LiftA<A, B, C, L, R>(this Func<A, B, C, R> fn, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c) => fn.Curry().MapR(a).Apply(b).Apply(c);
    public static Validation<L, R> LiftA<A, B, C, D, L, R>(this Func<A, B, C, D, R> fn, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d) => fn.Curry().MapR(a).Apply(b).Apply(c).Apply(d);
    public static Validation<L, R> LiftA<A, B, C, D, E, L, R>(this Func<A, B, C, D, E, R> fn, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d, Validation<L, E> e) => fn.Curry().MapR(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static Validation<L, R> LiftA<A, B, C, D, E, F, L, R>(this Func<A, B, C, D, E, F, R> fn, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d, Validation<L, E> e, Validation<L, F> f) => fn.Curry().MapR(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static Validation<L, R> LiftA<A, B, C, D, E, F, G, L, R>(this Func<A, B, C, D, E, F, G, R> fn, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d, Validation<L, E> e, Validation<L, F> f, Validation<L, G> g) => fn.Curry().MapR(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static Validation<L, R> LiftA<A, B, C, D, E, F, G, H, L, R>(this Func<A, B, C, D, E, F, G, H, R> fn, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d, Validation<L, E> e, Validation<L, F> f, Validation<L, G> g, Validation<L, H> h) => fn.Curry().MapR(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static Validation<L, R> LiftA<A, B, C, D, E, F, G, H, I, L, R>(this Func<A, B, C, D, E, F, G, H, I, R> fn, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d, Validation<L, E> e, Validation<L, F> f, Validation<L, G> g, Validation<L, H> h, Validation<L, I> i) => fn.Curry().MapR(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static Validation<L, R> LiftA<A, B, C, D, E, F, G, H, I, J, L, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> fn, Validation<L, A> a, Validation<L, B> b, Validation<L, C> c, Validation<L, D> d, Validation<L, E> e, Validation<L, F> f, Validation<L, G> g, Validation<L, H> h, Validation<L, I> i, Validation<L, J> j) => fn.Curry().MapR(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    #region Match
    public static void Match<L, R>(this Validation<L, R> @this, Action<List<L>> left, Action<R> right) {
      switch (@this.IsSuccess) {
        case true:
          right(@this.RValue);
          return;
        default:
          left(@this.LValue);
          return;
      }
    }

    public static R2 Match<L, R, R2>(this Validation<L, R> @this, Func<List<L>, R2> left, Func<R, R2> right) => @this.IsSuccess ? right(@this.RValue) : left(@this.LValue);
    #endregion

    #region DebugPrint
    public static void DebugPrint<L, R>(this Validation<L, R> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Syntactic Sugar - Right / Left
    public static Validation<L, R> Success<L, R>(R value) => Validation<L, R>.Success(value);
    public static Validation<L, R> Fail<L, R>(L value) => Validation<L, R>.Fail(List(value));
    public static Validation<L, R> Fail<L, R>(List<L> value) => Validation<L, R>.Fail(value);
    public static Validation<L, R> ToValidation<L, R>(this R r) => Success<L, R>(r);
    public static Validation<string, A> Validate<A>(this Predicate<A> p, A value, string cause) => p(value) ? Success<string, A>(value) : Fail<string, A>(cause);
    public static Func<A, Validation<L, B>> ToValidation<L, A, B>(this Func<A, B> f) => a => Success<L, B>(f(a));
    #endregion

    #region Linq Conformance
    public static Validation<L, R2> Select<L, R, R2>(this Validation<L, R> @this, Func<R, R2> fn) => @this.Map(fn);
    public static Validation<L, R2> SelectMany<L, R, R2>(this Validation<L, R> @this, Func<R, Validation<L, R2>> fn) => @this.FlatMap(fn);
    public static Validation<L, R2> SelectMany<L, R, R1, R2>(this Validation<L, R> @this, Func<R, Validation<L, R1>> fn, Func<R, R1, R2> select) => @this.FlatMap(a => fn(a).FlatMap(b => select(a, b).ToValidation<L, R2>()));
    #endregion

  }
}
