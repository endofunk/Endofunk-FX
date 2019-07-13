// Result.cs
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
using System.Runtime.ExceptionServices;
using static Endofunk.FX.Prelude;

namespace Endofunk.FX {

  #region Result Datatype
  public struct Result<A> {
    internal readonly A Value;
    internal readonly ExceptionDispatchInfo Failure;
    public enum Status { Failure, Success }
    private readonly Status State;
    public bool IsSuccess => State == Status.Success;
    public bool IsFailure => State == Status.Failure;
    private Result(A value) => (State, Failure, Value) = (Status.Success, default, value);
    private Result(ExceptionDispatchInfo failure) => (State, Failure, Value) = (Status.Failure, failure, default);
    private Result(Func<A> closure) {
      try {
        (State, Failure, Value) = (Status.Success, default, closure());
      } catch (Exception e) {
        (State, Failure, Value) = (Status.Failure, ExceptionDispatchInfo.Capture(e), default);
      }
    }

    public static implicit operator Result<A>(A value) => value == null ? Failed(default) : Success(value);
    public static implicit operator Result<A>(ExceptionDispatchInfo value) => Failed(value);
    public static Result<A> Success(A value) => new Result<A>(value);
    public static Result<A> Failed(ExceptionDispatchInfo e) => new Result<A>(e);
    public static Result<A> Try(Func<A> f) => new Result<A>(f);
    public override string ToString() => $"Result<{typeof(A).Simplify()}>[{State}: {(IsFailure ? Failure.SourceException.Message : this.Fold(s => s.ToString()))}]";

    public bool Equals(Result<A> other) {
      switch ((IsSuccess, other.IsSuccess)) {
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
      if (obj is Result<A> && isStruct) {
        return Equals((Result<A>)obj);
      }
      return false;
    }

    public static bool operator ==(Result<A> @this, Result<A> other) => @this.Equals(other);
    public static bool operator !=(Result<A> @this, Result<A> other) => !(@this == other);
    public override int GetHashCode() => (Value, Failure).GetHashCode();

    public IEnumerable<A> AsEnumerable() {
      if (IsSuccess) yield return Value;
    }
  }
  #endregion

  public static partial class ResultExtensions {
    #region Fold
    public static R Fold<A, R>(this Result<A> @this, Func<A, R> fn) => @this.IsSuccess ? fn(@this.Value) : default;
    public static R Fold<A, R>(this Result<A> @this, Func<A, R> success, Func<ExceptionDispatchInfo, R> failed) => @this.IsSuccess ? success(@this.Value) : failed(@this.Failure);
    #endregion

    #region Functor
    public static Result<R> Map<A, R>(this Func<A, R> fn, Result<A> @this) => @this.Map(fn);
    public static Result<R> Map<A, R>(this Result<A> @this, Func<A, R> fn) => @this.IsSuccess ? Success(fn(@this.Value)) : Failed<R>(@this.Failure);
    #endregion

    #region Monad
    public static Result<R> FlatMap<A, R>(this Result<A> @this, Func<A, Result<R>> fn) => @this.IsSuccess ? fn(@this.Value) : Failed<R>(@this.Failure);
    public static Result<R> FlatMap<A, R>(this Func<A, Result<R>> fn, Result<A> @this) => @this.FlatMap(fn);
    public static Func<Result<A>, Result<B>> FlatMap<A, B>(this Func<A, Result<B>> f) => a => a.FlatMap(f);
    public static Result<R> Bind<A, R>(this Result<A> @this, Func<A, Result<R>> fn) => @this.FlatMap(fn);
    #endregion

    #region Kleisli Composition
    public static Func<A, Result<C>> Compose<A, B, C>(this Func<A, Result<B>> f, Func<B, Result<C>> g) => f.Compose(g.FlatMap());
    #endregion

    #region Monad - Lift a function & actions
    public static Result<R> LiftM<A, R>(this Func<A, R> @this, Result<A> a) => a.FlatMap(xa => Success(@this(xa)));
    public static Result<R> LiftM<A, B, R>(this Func<A, B, R> @this, Result<A> a, Result<B> b) => a.FlatMap(xa => b.FlatMap(xb => Success(@this(xa, xb))));
    public static Result<R> LiftM<A, B, C, R>(this Func<A, B, C, R> @this, Result<A> a, Result<B> b, Result<C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => Success(@this(xa, xb, xc)))));
    public static Result<R> LiftM<A, B, C, D, R>(this Func<A, B, C, D, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => Success(@this(xa, xb, xc, xd))))));
    public static Result<R> LiftM<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d, Result<E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => Success(@this(xa, xb, xc, xd, xe)))))));
    public static Result<R> LiftM<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d, Result<E> e, Result<F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => Success(@this(xa, xb, xc, xd, xe, xf))))))));
    public static Result<R> LiftM<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d, Result<E> e, Result<F> f, Result<G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => Success(@this(xa, xb, xc, xd, xe, xf, xg)))))))));
    public static Result<R> LiftM<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d, Result<E> e, Result<F> f, Result<G> g, Result<H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => Success(@this(xa, xb, xc, xd, xe, xf, xg, xh))))))))));
    public static Result<R> LiftM<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d, Result<E> e, Result<F> f, Result<G> g, Result<H> h, Result<I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => Success(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi)))))))))));
    public static Result<R> LiftM<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d, Result<E> e, Result<F> f, Result<G> g, Result<H> h, Result<I> i, Result<J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => Success(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj))))))))))));
    #endregion

    #region Applicative Functor
    public static Result<R> Apply<A, R>(this Result<A> @this, Result<Func<A, R>> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
    public static Result<R> Apply<A, R>(this Result<Func<A, R>> fn, Result<A> @this) => @this.Apply(fn);
    public static Func<Result<A>, Result<R>> Apply<A, R>(this Result<Func<A, R>> fn) => a => a.Apply(fn);

    /// <summary>
    /// Sequence actions, discarding the value of the first argument.
    /// (*>) :: f a -> f b -> f b
    /// </summary>
    public static Result<B> DropFirst<A, B>(this Result<A> @this, Result<B> other) => Const<B, A>().Flip().LiftA(@this, other);

    /// <summary>
    /// Sequence actions, discarding the value of the second argument.
    /// (<*) :: f a -> f b -> f a
    /// </summary>
    public static Result<A> DropSecond<A, B>(this Result<A> @this, Result<B> other) => Const<A, B>().LiftA(@this, other);
    #endregion

    #region Applicative Functor - Lift a function & actions
    public static Result<R> LiftA<A, R>(this Func<A, R> @this, Result<A> a) => @this.Map(a);
    public static Result<R> LiftA<A, B, R>(this Func<A, B, R> @this, Result<A> a, Result<B> b) => @this.Curry().Map(a).Apply(b);
    public static Result<R> LiftA<A, B, C, R>(this Func<A, B, C, R> @this, Result<A> a, Result<B> b, Result<C> c) => @this.Curry().Map(a).Apply(b).Apply(c);
    public static Result<R> LiftA<A, B, C, D, R>(this Func<A, B, C, D, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d);
    public static Result<R> LiftA<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d, Result<E> e) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static Result<R> LiftA<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d, Result<E> e, Result<F> f) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static Result<R> LiftA<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d, Result<E> e, Result<F> f, Result<G> g) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static Result<R> LiftA<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d, Result<E> e, Result<F> f, Result<G> g, Result<H> h) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static Result<R> LiftA<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d, Result<E> e, Result<F> f, Result<G> g, Result<H> h, Result<I> i) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static Result<R> LiftA<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Result<A> a, Result<B> b, Result<C> c, Result<D> d, Result<E> e, Result<F> f, Result<G> g, Result<H> h, Result<I> i, Result<J> j) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    #region Traverse
    public static IEnumerable<Result<B>> Traverse<A, B>(this Result<A> @this, Func<A, IEnumerable<B>> f) => @this.Fold(failed: e => Enumerable.Empty<Result<B>>().Append(Failed<B>(e)), success: a => f(a).Map(Success));
    public static Identity<Result<B>> Traverse<A, B>(this Result<A> @this, Func<A, Identity<B>> f) => @this.Fold(failed: e => Identity<Result<B>>.Of(Failed<B>(e)), success: a => f(a).Map(Success));
    public static Maybe<Result<B>> Traverse<A, B>(this Result<A> @this, Func<A, Maybe<B>> f) => @this.Fold(failed: e => Just(Failed<B>(e)), success: a => f(a).Map(Success));
    public static IO<Result<B>> Traverse<A, B>(this Result<A> @this, Func<A, IO<B>> f) => @this.Fold(failed: e => Failed<B>(e).ToIO<Result<B>>(), success: a => f(a).Map(Success));
    public static Reader<R, Result<B>> Traverse<R, A, B>(this Result<A> @this, Func<A, Reader<R, B>> f) => @this.Fold(failed: e => Reader<R, Result<B>>.Pure(Failed<B>(e)), success: a => f(a).Map(Success));
    public static Either<L, Result<B>> Traverse<L, A, B>(this Result<A> @this, Func<A, Either<L, B>> f) => @this.Fold(failed: e => Right<L, Result<B>>(Failed<B>(e)), success: a => f(a).Map(Success));
    public static Validation<L, Result<B>> Traverse<L, A, B>(this Result<A> @this, Func<A, Validation<L, B>> f) => @this.Fold(failed: e => Success<L, Result<B>>(Failed<B>(e)), success: a => f(a).Map(Success));
    #endregion

    #region Sequence
    public static IEnumerable<Result<A>> Sequence<A>(Result<IEnumerable<A>> @this) => @this.Traverse(Id<IEnumerable<A>>());
    public static Maybe<Result<A>> Sequence<A>(Result<Maybe<A>> @this) => @this.Traverse(Id<Maybe<A>>());
    public static Identity<Result<A>> Sequence<A>(Result<Identity<A>> @this) => @this.Traverse(Id<Identity<A>>());
    public static IO<Result<A>> Sequence<A>(Result<IO<A>> @this) => @this.Traverse(Id<IO<A>>());
    public static Reader<R, Result<A>> Sequence<R, A>(Result<Reader<R, A>> @this) => @this.Traverse(Id<Reader<R, A>>());
    public static Either<L, Result<A>> Sequence<L, A>(Result<Either<L, A>> @this) => @this.Traverse(Id<Either<L, A>>());
    public static Validation<L, Result<A>> Sequence<L, A>(Result<Validation<L, A>> @this) => @this.Traverse(Id<Validation<L, A>>());
    #endregion 

    #region Match
    public static void Match<A>(this Result<A> @this, Action<ExceptionDispatchInfo> failed, Action<A> success) {
      switch (@this.IsSuccess) {
        case true:
          success(@this.Value);
          return;
        default:
          failed(@this.Failure);
          return;
      }
    }

    public static R Match<A, R>(this Result<A> @this, Func<ExceptionDispatchInfo, R> failed, Func<A, R> success) => @this.IsSuccess ? success(@this.Value) : failed(@this.Failure);
    #endregion

    #region DebugPrint
    public static void DebugPrint<A>(this Result<A> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Linq Conformance
    public static Result<R> Select<A, R>(this Result<A> @this, Func<A, R> fn) => @this.Map(fn);
    public static Result<R> SelectMany<A, R>(this Result<A> @this, Func<A, Result<R>> fn) => @this.FlatMap(fn);
    public static Result<R> SelectMany<A, B, R>(this Result<A> @this, Func<A, Result<B>> fn, Func<A, B, R> select) => @this.FlatMap(a => fn(a).FlatMap(b => select(a, b).ToResult()));
    #endregion

    #region GetOrElse
    public static A GetOrElse<A>(this Result<A> @this, A other) => @this.IsSuccess ? @this.Value : other;
    #endregion

    #region ToResult
    public static Result<A> ToResult<A>(this A a) => Success(a);
    public static Result<A> ToResult<A>(this ExceptionDispatchInfo e) => Failed<A>(e);
    public static Func<A, Result<B>> ToResult<A, B>(this Func<A, B> f) => a => Success<B>(f(a));
    #endregion
  }

  public static partial class Prelude {
    #region Syntactic Sugar - Success / Failure 
    public static Result<A> Success<A>(A value) => Result<A>.Success(value);
    public static Func<A, Result<A>> Success<A>() => a => Success<A>(a);
    public static Result<A> Failed<A>(ExceptionDispatchInfo error) => Result<A>.Failed(error);
    public static Func<ExceptionDispatchInfo, Result<A>> Failed<A>() => e => Failed<A>(e);
    public static Result<A> Try<A>(Func<A> f) => Result<A>.Try(f);
    public static Func<Func<A>, Result<A>> Try<A>() => f => Try<A>(f);
    #endregion
  }
}
