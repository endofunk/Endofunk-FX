// Validation.cs
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
using System.Runtime.Serialization;

namespace Endofunk.FX {

  #region Validation Datatype
  [DataContract]
  public sealed class Validation<L, R> : IEquatable<Validation<L, R>> {
    [DataMember] internal readonly List<L> LValue;
    [DataMember] internal readonly R RValue;
    [DataMember] public readonly bool IsSuccess;
    public bool IsFailure => !IsSuccess;
    private Validation(List<L> failure, R success, bool isSuccess) => (IsSuccess, LValue, RValue) = (isSuccess, !isSuccess ? failure : default, isSuccess ? success : default);
    public static implicit operator Validation<L, R>(R value) => value == null ? Failure<L, R>(new List<L> { }) : Success<L, R>(value);
    public static Validation<L, R> Success(R success) => new Validation<L, R>(default, success, true );
    public static Validation<L, R> Failure(List<L> failure) => new Validation<L, R>(failure, default, false);

    public bool Equals(Validation<L, R> other) {
      if (IsSuccess && other.IsSuccess && RValue.Equals(other.RValue)) return true;
      if (IsFailure && other.IsFailure && LValue.SequenceEqual(other.LValue)) return true;
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
    public override int GetHashCode() => (LValue, RValue, IsSuccess).GetHashCode();

    public IEnumerable<R> AsEnumerable() {
      if (IsSuccess) yield return RValue;
    }
    public override string ToString() => $"{this.GetType().Simplify()}[{IsSuccess}: {this.Fold(s => s.ToString(), r => r.ToString())}]";
  }
  #endregion

  public static class ValidationExtensions {
    #region ForEach
    public static void ForEach<L, R>(this Validation<L, R> @this, Action<R> f) => @this.AsEnumerable().ForEach(f);
    #endregion

    #region Fold
    public static R2 Fold<L, R1, R2>(this Validation<L, R1> @this, Func<List<L>, R2> left, Func<R1, R2> right) => @this.IsSuccess ? right(@this.RValue) : left(@this.LValue);
    #endregion

    #region Functor - MapL, MapR
    public static Validation<L, R2> MapR<L, R, R2>(this Func<R, R2> fn, Validation<L, R> @this) => @this.MapR(fn);
    public static Validation<L, R2> MapR<L, R, R2>(this Validation<L, R> @this, Func<R, R2> fn) => @this.IsSuccess ? Success<L, R2>(fn(@this.RValue)) : Failure<L, R2>(@this.LValue);
    public static Validation<L2, R> MapL<L, L2, R>(this Func<List<L>, List<L2>> fn, Validation<L, R> @this) => @this.MapL(fn);
    public static Validation<L2, R> MapL<L, L2, R>(this Validation<L, R> @this, Func<List<L>, List<L2>> fn) => @this.IsSuccess ? Success<L2, R>(@this.RValue) : Failure<L2, R>(fn(@this.LValue));
    #endregion

    #region Functor - Map (Right Affinity)
    public static Validation<L, R2> Map<L, R, R2>(this Validation<L, R> @this, Func<R, R2> fn) => @this.MapR(fn);
    public static Validation<L, R2> Map<L, R, R2>(this Func<R, R2> fn, Validation<L, R> @this) => @this.MapR(fn);
    public static Func<Validation<L, R>, Validation<L, R2>> Map<L, R, R2>(this Func<R, R2> fn) => @this => @this.MapR(fn);
    #endregion

    #region Functor - Bimap, First, Second
    public static Validation<L2, R2> BiMap<L, L2, R, R2>(this Validation<L, R> @this, Func<List<L>, List<L2>> left, Func<R, R2> right) => @this.IsSuccess ? Success<L2, R2>(right(@this.RValue)) : Failure<L2, R2>(left(@this.LValue));
    public static Validation<L2, R> First<L, R, L2>(Validation<L, R> @this, Func<List<L>, List<L2>> fn) => @this.BiMap(fn, Id);
    public static Validation<L, R2> Second<L, R, R2>(Validation<L, R> @this, Func<R, R2> fn) => @this.BiMap(Id, fn);
    #endregion

    #region Monad - FlatMapL, FlatMapR
    public static Validation<L, R2> FlatMapR<L, R, R2>(this Validation<L, R> @this, Func<R, Validation<L, R2>> f) => @this.IsSuccess ? f(@this.RValue) : Failure<L, R2>(@this.LValue);
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
          return Failure<L, R2>(f.LValue);
        case var t when t.First(false):
          return Failure<L, R2>(f.LValue);
        default:
          return Failure<L, R2>(@this.LValue);
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

    #region Traverse
    public static IEnumerable<Validation<L, B>> Traverse<L, A, B>(this Validation<L, A> @this, Func<A, IEnumerable<B>> f) => @this.Fold(left: l => Enumerable.Empty<Validation<L, B>>().Append(Failure<L, B>(l)), right: a => f(a).Map(Success<L, B>()));
    public static Identity<Validation<L, B>> Traverse<L, A, B>(this Validation<L, A> @this, Func<A, Identity<B>> f) => @this.Fold(left: l => Identity<Validation<L, B>>.Of(Failure<L, B>(l)), right: a => f(a).Map(Success<L, B>()));
    public static Result<Validation<L, B>> Traverse<L, A, B>(this Validation<L, A> @this, Func<A, Result<B>> f) => @this.Fold(left: l => Value(Failure<L, B>(l)), right: a => f(a).Map(Success<L, B>()));
    public static IO<Validation<L, B>> Traverse<L, A, B>(this Validation<L, A> @this, Func<A, IO<B>> f) => @this.Fold(left: l => Failure<L, B>(l).ToIO(), right: a => f(a).Map(Success<L, B>()));
    public static Reader<R, Validation<L, B>> Traverse<L, R, A, B>(this Validation<L, A> @this, Func<A, Reader<R, B>> f) => @this.Fold(left: l => Reader<R, Validation<L, B>>.Pure(Failure<L, B>(l)), right: a => f(a).Map(Success<L, B>()));
    public static Maybe<Validation<L, B>> Traverse<L, A, B>(this Validation<L, A> @this, Func<A, Maybe<B>> f) => @this.Fold(left: l => Just(Failure<L, B>(l)), right: a => f(a).Map(Success<L, B>()));
    public static Either<L2, Validation<L, B>> Traverse<L2, L, A, B>(this Validation<L, A> @this, Func<A, Either<L2, B>> f) => @this.Fold(left: l => Right<L2, Validation<L, B>>(Failure<L, B>(l)), right: a => f(a).Map(Success<L, B>()));
    #endregion

    #region Sequence
    public static IEnumerable<Validation<L, A>> Sequence<L, A>(Validation<L, IEnumerable<A>> @this) => @this.Traverse(Id<IEnumerable<A>>());
    public static Identity<Validation<L, A>> Sequence<L, A>(Validation<L, Identity<A>> @this) => @this.Traverse(Id<Identity<A>>());
    public static Result<Validation<L, A>> Sequence<L, A>(Validation<L, Result<A>> @this) => @this.Traverse(Id<Result<A>>());
    public static IO<Validation<L, A>> Sequence<L, A>(Validation<L, IO<A>> @this) => @this.Traverse(Id<IO<A>>());
    public static Reader<R, Validation<L, A>> Sequence<L, R, A>(Validation<L, Reader<R, A>> @this) => @this.Traverse(Id<Reader<R, A>>());
    public static Maybe<Validation<L, A>> Sequence<L, A>(Validation<L, Maybe<A>> @this) => @this.Traverse(Id<Maybe<A>>());
    public static Either<L2, Validation<L, A>> Sequence<L2, L, A>(Validation<L, Either<L2, A>> @this) => @this.Traverse(Id<Either<L2, A>>());
    #endregion 

    #region Match
    public static void Match<L, R>(this Validation<L, R> @this, Action<List<L>> left, Action<R> right) {
      if (@this.IsSuccess) right(@this.RValue);
      else left(@this.LValue);
    }

    public static R2 Match<L, R, R2>(this Validation<L, R> @this, Func<List<L>, R2> left, Func<R, R2> right) => @this.IsSuccess ? right(@this.RValue) : left(@this.LValue);
    #endregion

    #region DebugPrint
    public static void DebugPrint<L, R>(this Validation<L, R> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Linq Conformance
    public static Validation<L, R2> Select<L, R, R2>(this Validation<L, R> @this, Func<R, R2> fn) => @this.Map(fn);
    public static Validation<L, R2> SelectMany<L, R, R2>(this Validation<L, R> @this, Func<R, Validation<L, R2>> fn) => @this.FlatMap(fn);
    public static Validation<L, R2> SelectMany<L, R, R1, R2>(this Validation<L, R> @this, Func<R, Validation<L, R1>> fn, Func<R, R1, R2> select) => @this.FlatMap(a => fn(a).FlatMap(b => select(a, b).ToValidation<L, R2>()));
    #endregion

    #region ToValidation
    public static Validation<L, R> ToValidation<L, R>(this R r) => Success<L, R>(r);
    public static Validation<string, A> Validate<A>(this Predicate<A> p, A value, string cause) => p(value) ? Success<string, A>(value) : Failure<string, A>(cause);
    public static Func<A, Validation<L, B>> ToValidation<L, A, B>(this Func<A, B> f) => a => Success<L, B>(f(a));
    #endregion
  }

  public static partial class Prelude {
    #region Syntactic Sugar - Success / Failure
    public static Validation<L, R> Success<L, R>(R value) => Validation<L, R>.Success(value);
    public static Validation<L, R> Failure<L, R>(L value) => Validation<L, R>.Failure(List(value));
    public static Validation<L, R> Failure<L, R>(List<L> value) => Validation<L, R>.Failure(value);
    public static Func<R, Validation<L, R>> Success<L, R>() => value => Validation<L, R>.Success(value);
    public static Func<L, Validation<L, R>> Failure<L, R>() => value => Validation<L, R>.Failure(List(value));
    #endregion
  }
}
