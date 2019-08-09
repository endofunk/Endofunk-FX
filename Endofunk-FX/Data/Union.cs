//// Prelude.cs
////
//// MIT License
//// Copyright (c) 2019 endofunk
////
//// Permission is hereby granted, free of charge, to any person obtaining a copy
//// of this software and associated documentation files (the "Software"), to deal
//// in the Software without restriction, including without limitation the rights
//// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//// copies of the Software, and to permit persons to whom the Software is
//// furnished to do so, subject to the following conditions:
////
//// The above copyright notice and this permission notice shall be included in
//// all copies or substantial portions of the Software.
////
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//// THE SOFTWARE.

//using System;

//namespace Endofunk.FX {

//  #region Union 1
//  public sealed class Union<T1> {
//    public readonly bool HasValue;
//    private readonly T1 UnionValue;
//    private Union() { }
//    internal Union(bool hasvalue, T1 unionvalue) => (HasValue, UnionValue) = (hasvalue, unionvalue);
//    public T1 Value => !HasValue ? throw new InvalidOperationException($"Can't return {typeof(T1).Simplify()}, Nothing embedded.") : UnionValue;
//    public override string ToString() => $"{this.GetType().Simplify()}[{UnionValue.ToString()}]";
//    public static implicit operator Union<T1>(T1 t1) => new Union<T1>(true, t1);
//  }

//  public static partial class UnionExtensions {
//    #region Syntactic Sugar
//    public static Union<T1> Union<T1>() => new Union<T1>(false, default);
//    public static Union<T1> Union<T1>(T1 value) => new Union<T1>(true, value);
//    #endregion

//    #region Functor
//    public static Union<R> Map<T1, R>(this Union<T1> @this, Func<T1, R> f) => Union(f(@this.Value));
//    public static Union<R> Map<T1, R>(this Func<T1, R> f, Union<T1> @this) => Union(f(@this.Value));
//    #endregion

//    #region Monad
//    public static Union<R> FlatMap<T1, R>(this Union<T1> @this, Func<T1, Union<R>> f) => f(@this.Value);
//    public static Union<R> FlatMap<T1, R>(this Func<T1, Union<R>> f, Union<T1> @this) => f(@this.Value);
//    public static Union<R> Bind<T1, R>(this Union<T1> @this, Func<T1, Union<R>> f) => f(@this.Value);
//    #endregion

//    #region Applicative Functor
//    public static Union<R> Apply<T1, R>(this Union<T1> @this, Union<Func<T1, R>> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<R> Apply<T1, R>(this Union<Func<T1, R>> fn, Union<T1> @this) => fn.FlatMap(g => @this.Map(x => g(x)));
//    #endregion

//    #region Applicative Functor - Lift a function & actions
//    public static Union<R> LiftA<T1, R>(this Func<T1, R> @this, Union<T1> t1) => @this.Map(t1);
//    public static Union<R> LiftA<T1, T2, R>(this Func<T1, T2, R> @this, Union<T1> t1, Union<T2> t2) => @this.Curry().Map(t1).Apply(t2);
//    public static Union<R> LiftA<T1, T2, T3, R>(this Func<T1, T2, T3, R> @this, Union<T1> t1, Union<T2> t2, Union<T3> t3) => @this.Curry().Map(t1).Apply(t2).Apply(t3);
//    public static Union<R> LiftA<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> @this, Union<T1> t1, Union<T2> t2, Union<T3> t3, Union<T4> t4) => @this.Curry().Map(t1).Apply(t2).Apply(t3).Apply(t4);
//    public static Union<R> LiftA<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> @this, Union<T1> t1, Union<T2> t2, Union<T3> t3, Union<T4> t4, Union<T5> t5) => @this.Curry().Map(t1).Apply(t2).Apply(t3).Apply(t4).Apply(t5);
//    public static Union<R> LiftA<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> @this, Union<T1> t1, Union<T2> t2, Union<T3> t3, Union<T4> t4, Union<T5> t5, Union<T6> t6) => @this.Curry().Map(t1).Apply(t2).Apply(t3).Apply(t4).Apply(t5).Apply(t6);
//    public static Union<R> LiftA<T1, T2, T3, T4, T5, T6, T7, R>(this Func<T1, T2, T3, T4, T5, T6, T7, R> @this, Union<T1> t1, Union<T2> t2, Union<T3> t3, Union<T4> t4, Union<T5> t5, Union<T6> t6, Union<T7> t7) => @this.Curry().Map(t1).Apply(t2).Apply(t3).Apply(t4).Apply(t5).Apply(t6).Apply(t7);
//    public static Union<R> LiftA<T1, T2, T3, T4, T5, T6, T7, T8, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, R> @this, Union<T1> t1, Union<T2> t2, Union<T3> t3, Union<T4> t4, Union<T5> t5, Union<T6> t6, Union<T7> t7, Union<T8> t8) => @this.Curry().Map(t1).Apply(t2).Apply(t3).Apply(t4).Apply(t5).Apply(t6).Apply(t7).Apply(t8);
//    public static Union<R> LiftA<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> @this, Union<T1> t1, Union<T2> t2, Union<T3> t3, Union<T4> t4, Union<T5> t5, Union<T6> t6, Union<T7> t7, Union<T8> t8, Union<T9> t9) => @this.Curry().Map(t1).Apply(t2).Apply(t3).Apply(t4).Apply(t5).Apply(t6).Apply(t7).Apply(t8).Apply(t9);
//    #endregion

//    #region Match
//    public static void Match<T1>(this Union<T1> @this, (Func<Union<T1>, bool> predicate, Action<Union<T1>> f) eval) {
//      if (eval.predicate(@this)) eval.f(@this);
//    }

//    public static R Match<T1, R>(this Union<T1> @this, Func<R> unmatched, (Func<Union<T1>, bool> predicate, Func<Union<T1>, R> f) eval) {
//      if (eval.predicate(@this)) {
//        return eval.f(@this);
//      }
//      return unmatched();
//    }
//    #endregion

//    #region DebugPrint
//    public static void DebugPrint<T1>(this Union<T1> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
//    #endregion

//    #region Linq Conformance
//    public static Union<R> Select<T1, R>(this Union<T1> @this, Func<T1, R> f) => @this.Map(f);
//    public static Union<R> SelectMany<T1, R>(this Union<T1> @this, Func<T1, Union<R>> f) => @this.FlatMap(f);
//    #endregion
//  }
//  #endregion

//  #region Union 2
//  public sealed class Union<T1, T2> {
//    private readonly int Index;
//    public bool HasValue1 => Index == 1;
//    public bool HasValue2 => Index == 2;
//    private readonly T1 UnionValue1;
//    private readonly T2 UnionValue2;
//    private Union() { }
//    internal Union(int index, T1 unionvalue1, T2 unionvalue2) => (Index, UnionValue1, UnionValue2) = (index, unionvalue1, unionvalue2);
//    public T1 Value1 => !HasValue1 ? throw new InvalidOperationException($"Can't return {typeof(T1).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue1;
//    public T2 Value2 => !HasValue2 ? throw new InvalidOperationException($"Can't return {typeof(T2).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue2;
//    public override string ToString() => $"Union<{typeof(T1).Simplify()}, {typeof(T2).Simplify()}>[{Value1}, {Value2}]";
//    public static implicit operator Union<T1, T2>(T1 t1) => new Union<T1, T2>(1, t1, default);
//    public static implicit operator Union<T1, T2>(T2 t2) => new Union<T1, T2>(2, default, t2);
//  }

//  public static partial class UnionExtensions {
//    #region Syntactic Sugar
//    public static Union<T1, T2> Union<T1, T2>() => new Union<T1, T2>(0, default, default);
//    public static Union<T1, T2> Union<T1, T2>(T1 value) => new Union<T1, T2>(1, value, default);
//    public static Union<T1, T2> Union<T1, T2>(T2 value) => new Union<T1, T2>(2, default, value);
//    #endregion

//    #region Functor
//    public static Union<R, T2> Map<T1, T2, R>(this Union<T1, T2> @this, Func<T1, R> f) => Union<R, T2>(f(@this.Value1));
//    public static Union<T1, R> Map<T1, T2, R>(this Union<T1, T2> @this, Func<T2, R> f) => Union<T1, R>(f(@this.Value2));
//    #endregion

//    #region Monad
//    public static Union<R, T2> FlatMap<T1, T2, R>(this Union<T1, T2> @this, Func<T1, Union<R, T2>> f) => f(@this.Value1);
//    public static Union<T1, R> FlatMap<T1, T2, R>(this Union<T1, T2> @this, Func<T2, Union<T1, R>> f) => f(@this.Value2);
//    public static Union<R, T2> Bind<T1, T2, R>(this Union<T1, T2> @this, Func<T1, Union<R, T2>> f) => f(@this.Value1);
//    public static Union<T1, R> Bind<T1, T2, R>(this Union<T1, T2> @this, Func<T2, Union<T1, R>> f) => f(@this.Value2);
//    #endregion

//    #region Applicative Functor
//    public static Union<R, T2> Apply<T1, T2, R>(this Union<T1, T2> @this, Union<Func<T1, R>, T2> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, R> Apply<T1, T2, R>(this Union<T1, T2> @this, Union<T1, Func<T2, R>> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<R, T2> Apply<T1, T2, R>(this Union<Func<T1, R>, T2> fn, Union<T1, T2> @this) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, R> Apply<T1, T2, R>(this Union<T1, Func<T2, R>> fn, Union<T1, T2> @this) => fn.FlatMap(g => @this.Map(x => g(x)));
//    #endregion

//    #region Match
//    public static void Match<T1, T2>(this Union<T1, T2> @this, Action unmatched, params (Predicate<Union<T1, T2>> predicate, Action<Union<T1, T2>> f)[] eval) {
//      foreach (var (predicate, f) in eval) {
//        if (predicate(@this)) {
//          f(@this);
//          return;
//        }
//      }
//      unmatched();
//    }

//    public static R Match<T1, T2, R>(this Union<T1, T2> @this, Func<R> unmatched, params (Predicate<Union<T1, T2>> predicate, Func<Union<T1, T2>, R> f)[] eval) {
//      foreach (var (predicate, f) in eval) {
//        if (predicate(@this)) return f(@this);
//      }
//      return unmatched();
//    }
//    #endregion

//    #region DebugPrint
//    public static void DebugPrint<T1, T2>(this Union<T1, T2> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
//    #endregion

//    #region Linq Conformance
//    public static Union<R, T2> Select<T1, T2, R>(this Union<T1, T2> @this, Func<T1, R> f) => @this.Map(f);
//    public static Union<T1, R> Select<T1, T2, R>(this Union<T1, T2> @this, Func<T2, R> f) => @this.Map(f);
//    public static Union<R, T2> SelectMany<T1, T2, R>(this Union<T1, T2> @this, Func<T1, Union<R, T2>> f) => @this.FlatMap(f);
//    public static Union<T1, R> SelectMany<T1, T2, R>(this Union<T1, T2> @this, Func<T2, Union<T1, R>> f) => @this.FlatMap(f);
//    #endregion
//  }
//  #endregion

//  #region Union 3
//  public sealed class Union<T1, T2, T3> {
//    private readonly int Index;
//    public bool HasValue1 => Index == 1;
//    public bool HasValue2 => Index == 2;
//    public bool HasValue3 => Index == 3;
//    private readonly T1 UnionValue1;
//    private readonly T2 UnionValue2;
//    private readonly T3 UnionValue3;
//    private Union() { }
//    internal Union(int index, T1 unionvalue1, T2 unionvalue2, T3 unionvalue3) => (Index, UnionValue1, UnionValue2, UnionValue3) = (index, unionvalue1, unionvalue2, unionvalue3);
//    public T1 Value1 => !HasValue1 ? throw new InvalidOperationException($"Can't return {typeof(T1).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue1;
//    public T2 Value2 => !HasValue2 ? throw new InvalidOperationException($"Can't return {typeof(T2).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue2;
//    public T3 Value3 => !HasValue3 ? throw new InvalidOperationException($"Can't return {typeof(T3).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue3;
//    public override string ToString() => $"Union<{typeof(T1).Simplify()}, {typeof(T2).Simplify()}, {typeof(T3).Simplify()}>[{Value1}, {Value2}, {Value3}]";
//    public static implicit operator Union<T1, T2, T3>(T1 t1) => new Union<T1, T2, T3>(1, t1, default, default);
//    public static implicit operator Union<T1, T2, T3>(T2 t2) => new Union<T1, T2, T3>(2, default, t2, default);
//    public static implicit operator Union<T1, T2, T3>(T3 t3) => new Union<T1, T2, T3>(3, default, default, t3);
//  }

//  public static partial class UnionExtensions {
//    #region Syntactic Sugar
//    public static Union<T1, T2, T3> Union<T1, T2, T3>() => new Union<T1, T2, T3>(0, default, default, default);
//    public static Union<T1, T2, T3> Union<T1, T2, T3>(T1 value) => new Union<T1, T2, T3>(1, value, default, default);
//    public static Union<T1, T2, T3> Union<T1, T2, T3>(T2 value) => new Union<T1, T2, T3>(2, default, value, default);
//    public static Union<T1, T2, T3> Union<T1, T2, T3>(T3 value) => new Union<T1, T2, T3>(3, default, default, value);
//    #endregion

//    #region Functor
//    public static Union<R, T2, T3> Map<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T1, R> f) => Union<R, T2, T3>(f(@this.Value1));
//    public static Union<T1, R, T3> Map<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T2, R> f) => Union<T1, R, T3>(f(@this.Value2));
//    public static Union<T1, T2, R> Map<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T3, R> f) => Union<T1, T2, R>(f(@this.Value3));
//    #endregion

//    #region Monad
//    public static Union<R, T2, T3> FlatMap<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T1, Union<R, T2, T3>> f) => f(@this.Value1);
//    public static Union<T1, R, T3> FlatMap<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T2, Union<T1, R, T3>> f) => f(@this.Value2);
//    public static Union<T1, T2, R> FlatMap<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T3, Union<T1, T2, R>> f) => f(@this.Value3);
//    public static Union<R, T2, T3> Bind<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T1, Union<R, T2, T3>> f) => f(@this.Value1);
//    public static Union<T1, R, T3> Bind<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T2, Union<T1, R, T3>> f) => f(@this.Value2);
//    public static Union<T1, T2, R> Bind<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T3, Union<T1, T2, R>> f) => f(@this.Value3);
//    #endregion

//    #region Applicative Functor
//    public static Union<R, T2, T3> Apply<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Union<Func<T1, R>, T2, T3> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, R, T3> Apply<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Union<T1, Func<T2, R>, T3> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, T2, R> Apply<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Union<T1, T2, Func<T3, R>> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    #endregion

//    #region Match
//    public static void Match<T1, T2, T3>(this Union<T1, T2, T3> @this, Action unmatched, params (Predicate<Union<T1, T2, T3>> predicate, Action<Union<T1, T2, T3>> f)[] eval) {
//      foreach (var (predicate, f) in eval) {
//        if (predicate(@this)) {
//          f(@this);
//          return;
//        }
//      }
//      unmatched();
//    }

//    public static R Match<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<R> unmatched, params (Predicate<Union<T1, T2, T3>> predicate, Func<Union<T1, T2, T3>, R> f)[] eval) {
//      foreach (var (predicate, f) in eval) {
//        if (predicate(@this)) {
//          return f(@this);
//        }
//      }
//      return unmatched();
//    }
//    #endregion

//    #region DebugPrint
//    public static void DebugPrint<T1, T2, T3>(this Union<T1, T2, T3> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
//    #endregion

//    #region Linq Conformance
//    public static Union<R, T2, T3> Select<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T1, R> f) => @this.Map(f);
//    public static Union<T1, R, T3> Select<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T2, R> f) => @this.Map(f);
//    public static Union<T1, T2, R> Select<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T3, R> f) => @this.Map(f);
//    public static Union<R, T2, T3> SelectMany<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T1, Union<R, T2, T3>> f) => @this.FlatMap(f);
//    public static Union<T1, R, T3> SelectMany<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T2, Union<T1, R, T3>> f) => @this.FlatMap(f);
//    public static Union<T1, T2, R> SelectMany<T1, T2, T3, R>(this Union<T1, T2, T3> @this, Func<T3, Union<T1, T2, R>> f) => @this.FlatMap(f);
//    #endregion
//  }
//  #endregion

//  #region Union 4
//  public sealed class Union<T1, T2, T3, T4> {
//    private readonly int Index;
//    public bool HasValue1 => Index == 1;
//    public bool HasValue2 => Index == 2;
//    public bool HasValue3 => Index == 3;
//    public bool HasValue4 => Index == 4;
//    private readonly T1 UnionValue1;
//    private readonly T2 UnionValue2;
//    private readonly T3 UnionValue3;
//    private readonly T4 UnionValue4;
//    private Union() { }
//    internal Union(int index, T1 unionvalue1, T2 unionvalue2, T3 unionvalue3, T4 unionvalue4) => (Index, UnionValue1, UnionValue2, UnionValue3, UnionValue4) = (index, unionvalue1, unionvalue2, unionvalue3, unionvalue4);
//    public T1 Value1 => !HasValue1 ? throw new InvalidOperationException($"Can't return {typeof(T1).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue1;
//    public T2 Value2 => !HasValue2 ? throw new InvalidOperationException($"Can't return {typeof(T2).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue2;
//    public T3 Value3 => !HasValue3 ? throw new InvalidOperationException($"Can't return {typeof(T3).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue3;
//    public T4 Value4 => !HasValue4 ? throw new InvalidOperationException($"Can't return {typeof(T4).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue4;
//    public override string ToString() => $"Union<{typeof(T1).Simplify()}, {typeof(T2).Simplify()}, {typeof(T3).Simplify()}, {typeof(T4).Simplify()}>[{Value1}, {Value2}, {Value3}, {Value4}]";
//    public static implicit operator Union<T1, T2, T3, T4>(T1 t1) => new Union<T1, T2, T3, T4>(1, t1, default, default, default);
//    public static implicit operator Union<T1, T2, T3, T4>(T2 t2) => new Union<T1, T2, T3, T4>(2, default, t2, default, default);
//    public static implicit operator Union<T1, T2, T3, T4>(T3 t3) => new Union<T1, T2, T3, T4>(3, default, default, t3, default);
//    public static implicit operator Union<T1, T2, T3, T4>(T4 t4) => new Union<T1, T2, T3, T4>(4, default, default, default, t4);
//  }

//  public static partial class UnionExtensions {
//    #region Syntactic Sugar
//    public static Union<T1, T2, T3, T4> Union<T1, T2, T3, T4>() => new Union<T1, T2, T3, T4>(0, default, default, default, default);
//    public static Union<T1, T2, T3, T4> Union<T1, T2, T3, T4>(T1 value) => new Union<T1, T2, T3, T4>(1, value, default, default, default);
//    public static Union<T1, T2, T3, T4> Union<T1, T2, T3, T4>(T2 value) => new Union<T1, T2, T3, T4>(2, default, value, default, default);
//    public static Union<T1, T2, T3, T4> Union<T1, T2, T3, T4>(T3 value) => new Union<T1, T2, T3, T4>(3, default, default, value, default);
//    public static Union<T1, T2, T3, T4> Union<T1, T2, T3, T4>(T4 value) => new Union<T1, T2, T3, T4>(4, default, default, default, value);
//    #endregion

//    #region Functor
//    public static Union<R, T2, T3, T4> Map<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T1, R> f) => Union<R, T2, T3, T4>(f(@this.Value1));
//    public static Union<T1, R, T3, T4> Map<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T2, R> f) => Union<T1, R, T3, T4>(f(@this.Value2));
//    public static Union<T1, T2, R, T4> Map<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T3, R> f) => Union<T1, T2, R, T4>(f(@this.Value3));
//    public static Union<T1, T2, T3, R> Map<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T4, R> f) => Union<T1, T2, T3, R>(f(@this.Value4));
//    #endregion

//    #region Monad
//    public static Union<R, T2, T3, T4> FlatMap<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T1, Union<R, T2, T3, T4>> f) => f(@this.Value1);
//    public static Union<T1, R, T3, T4> FlatMap<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T2, Union<T1, R, T3, T4>> f) => f(@this.Value2);
//    public static Union<T1, T2, R, T4> FlatMap<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T3, Union<T1, T2, R, T4>> f) => f(@this.Value3);
//    public static Union<T1, T2, T3, R> FlatMap<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T4, Union<T1, T2, T3, R>> f) => f(@this.Value4);
//    public static Union<R, T2, T3, T4> Bind<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T1, Union<R, T2, T3, T4>> f) => f(@this.Value1);
//    public static Union<T1, R, T3, T4> Bind<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T2, Union<T1, R, T3, T4>> f) => f(@this.Value2);
//    public static Union<T1, T2, R, T4> Bind<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T3, Union<T1, T2, R, T4>> f) => f(@this.Value3);
//    public static Union<T1, T2, T3, R> Bind<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T4, Union<T1, T2, T3, R>> f) => f(@this.Value4);
//    #endregion

//    #region Applicative Functor
//    public static Union<R, T2, T3, T4> Apply<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Union<Func<T1, R>, T2, T3, T4> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, R, T3, T4> Apply<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Union<T1, Func<T2, R>, T3, T4> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, T2, R, T4> Apply<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Union<T1, T2, Func<T3, R>, T4> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, T2, T3, R> Apply<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Union<T1, T2, T3, Func<T4, R>> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    #endregion

//    #region Match
//    public static void Match<T1, T2, T3, T4>(this Union<T1, T2, T3, T4> @this, Action unmatched, params (Predicate<Union<T1, T2, T3, T4>> predicate, Action<Union<T1, T2, T3, T4>> f)[] eval) {
//      foreach (var (predicate, f) in eval) {
//        if (predicate(@this)) {
//          f(@this);
//          return;
//        }
//      }
//      unmatched();
//    }

//    public static R Match<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<R> unmatched, params (Predicate<Union<T1, T2, T3, T4>> predicate, Func<Union<T1, T2, T3, T4>, R> f)[] eval) {
//      foreach (var (predicate, f) in eval) {
//        if (predicate(@this)) {
//          return f(@this);
//        }
//      }
//      return unmatched();
//    }
//    #endregion

//    #region DebugPrint
//    public static void DebugPrint<T1, T2, T3, T4>(this Union<T1, T2, T3, T4> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
//    #endregion

//    #region Linq Conformance
//    public static Union<R, T2, T3, T4> Select<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T1, R> f) => @this.Map(f);
//    public static Union<T1, R, T3, T4> Select<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T2, R> f) => @this.Map(f);
//    public static Union<T1, T2, R, T4> Select<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T3, R> f) => @this.Map(f);
//    public static Union<T1, T2, T3, R> Select<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T4, R> f) => @this.Map(f);
//    public static Union<R, T2, T3, T4> SelectMany<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T1, Union<R, T2, T3, T4>> f) => @this.FlatMap(f);
//    public static Union<T1, R, T3, T4> SelectMany<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T2, Union<T1, R, T3, T4>> f) => @this.FlatMap(f);
//    public static Union<T1, T2, R, T4> SelectMany<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T3, Union<T1, T2, R, T4>> f) => @this.FlatMap(f);
//    public static Union<T1, T2, T3, R> SelectMany<T1, T2, T3, T4, R>(this Union<T1, T2, T3, T4> @this, Func<T4, Union<T1, T2, T3, R>> f) => @this.FlatMap(f);
//    #endregion
//  }
//  #endregion

//  #region Union 5
//  public sealed class Union<T1, T2, T3, T4, T5> {
//    private readonly int Index;
//    public bool HasValue1 => Index == 1;
//    public bool HasValue2 => Index == 2;
//    public bool HasValue3 => Index == 3;
//    public bool HasValue4 => Index == 4;
//    public bool HasValue5 => Index == 5;
//    private readonly T1 UnionValue1;
//    private readonly T2 UnionValue2;
//    private readonly T3 UnionValue3;
//    private readonly T4 UnionValue4;
//    private readonly T5 UnionValue5;
//    private Union() { }
//    internal Union(int index, T1 unionvalue1, T2 unionvalue2, T3 unionvalue3, T4 unionvalue4, T5 unionvalue5) => (Index, UnionValue1, UnionValue2, UnionValue3, UnionValue4, UnionValue5) = (index, unionvalue1, unionvalue2, unionvalue3, unionvalue4, unionvalue5);
//    public T1 Value1 => !HasValue1 ? throw new InvalidOperationException($"Can't return {typeof(T1).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue1;
//    public T2 Value2 => !HasValue2 ? throw new InvalidOperationException($"Can't return {typeof(T2).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue2;
//    public T3 Value3 => !HasValue3 ? throw new InvalidOperationException($"Can't return {typeof(T3).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue3;
//    public T4 Value4 => !HasValue4 ? throw new InvalidOperationException($"Can't return {typeof(T4).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue4;
//    public T5 Value5 => !HasValue5 ? throw new InvalidOperationException($"Can't return {typeof(T5).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue5;
//    public override string ToString() => $"Union<{typeof(T1).Simplify()}, {typeof(T2).Simplify()}, {typeof(T3).Simplify()}, {typeof(T4).Simplify()}, {typeof(T5).Simplify()}>[{Value1}, {Value2}, {Value3}, {Value4}, {Value5}]";
//    public static implicit operator Union<T1, T2, T3, T4, T5>(T1 t1) => new Union<T1, T2, T3, T4, T5>(1, t1, default, default, default, default);
//    public static implicit operator Union<T1, T2, T3, T4, T5>(T2 t2) => new Union<T1, T2, T3, T4, T5>(2, default, t2, default, default, default);
//    public static implicit operator Union<T1, T2, T3, T4, T5>(T3 t3) => new Union<T1, T2, T3, T4, T5>(3, default, default, t3, default, default);
//    public static implicit operator Union<T1, T2, T3, T4, T5>(T4 t4) => new Union<T1, T2, T3, T4, T5>(4, default, default, default, t4, default);
//    public static implicit operator Union<T1, T2, T3, T4, T5>(T5 t5) => new Union<T1, T2, T3, T4, T5>(5, default, default, default, default, t5);
//  }

//  public static partial class UnionExtensions {
//    #region Syntactic Sugar
//    public static Union<T1, T2, T3, T4, T5> Union<T1, T2, T3, T4, T5>() => new Union<T1, T2, T3, T4, T5>(0, default, default, default, default, default);
//    public static Union<T1, T2, T3, T4, T5> Union<T1, T2, T3, T4, T5>(T1 value) => new Union<T1, T2, T3, T4, T5>(1, value, default, default, default, default);
//    public static Union<T1, T2, T3, T4, T5> Union<T1, T2, T3, T4, T5>(T2 value) => new Union<T1, T2, T3, T4, T5>(2, default, value, default, default, default);
//    public static Union<T1, T2, T3, T4, T5> Union<T1, T2, T3, T4, T5>(T3 value) => new Union<T1, T2, T3, T4, T5>(3, default, default, value, default, default);
//    public static Union<T1, T2, T3, T4, T5> Union<T1, T2, T3, T4, T5>(T4 value) => new Union<T1, T2, T3, T4, T5>(4, default, default, default, value, default);
//    public static Union<T1, T2, T3, T4, T5> Union<T1, T2, T3, T4, T5>(T5 value) => new Union<T1, T2, T3, T4, T5>(5, default, default, default, default, value);
//    #endregion

//    #region Functor
//    public static Union<R, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T1, R> f) => Union<R, T2, T3, T4, T5>(f(@this.Value1));
//    public static Union<T1, R, T3, T4, T5> Map<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T2, R> f) => Union<T1, R, T3, T4, T5>(f(@this.Value2));
//    public static Union<T1, T2, R, T4, T5> Map<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T3, R> f) => Union<T1, T2, R, T4, T5>(f(@this.Value3));
//    public static Union<T1, T2, T3, R, T5> Map<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T4, R> f) => Union<T1, T2, T3, R, T5>(f(@this.Value4));
//    public static Union<T1, T2, T3, T4, R> Map<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T5, R> f) => Union<T1, T2, T3, T4, R>(f(@this.Value5));
//    #endregion

//    #region Monad
//    public static Union<R, T2, T3, T4, T5> FlatMap<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T1, Union<R, T2, T3, T4, T5>> f) => f(@this.Value1);
//    public static Union<T1, R, T3, T4, T5> FlatMap<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T2, Union<T1, R, T3, T4, T5>> f) => f(@this.Value2);
//    public static Union<T1, T2, R, T4, T5> FlatMap<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T3, Union<T1, T2, R, T4, T5>> f) => f(@this.Value3);
//    public static Union<T1, T2, T3, R, T5> FlatMap<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T4, Union<T1, T2, T3, R, T5>> f) => f(@this.Value4);
//    public static Union<T1, T2, T3, T4, R> FlatMap<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T5, Union<T1, T2, T3, T4, R>> f) => f(@this.Value5);
//    public static Union<R, T2, T3, T4, T5> Bind<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T1, Union<R, T2, T3, T4, T5>> f) => f(@this.Value1);
//    public static Union<T1, R, T3, T4, T5> Bind<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T2, Union<T1, R, T3, T4, T5>> f) => f(@this.Value2);
//    public static Union<T1, T2, R, T4, T5> Bind<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T3, Union<T1, T2, R, T4, T5>> f) => f(@this.Value3);
//    public static Union<T1, T2, T3, R, T5> Bind<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T4, Union<T1, T2, T3, R, T5>> f) => f(@this.Value4);
//    public static Union<T1, T2, T3, T4, R> Bind<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T5, Union<T1, T2, T3, T4, R>> f) => f(@this.Value5);
//    #endregion

//    #region Applicative Functor
//    public static Union<R, T2, T3, T4, T5> Apply<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Union<Func<T1, R>, T2, T3, T4, T5> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, R, T3, T4, T5> Apply<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Union<T1, Func<T2, R>, T3, T4, T5> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, T2, R, T4, T5> Apply<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Union<T1, T2, Func<T3, R>, T4, T5> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, T2, T3, R, T5> Apply<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Union<T1, T2, T3, Func<T4, R>, T5> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, T2, T3, T4, R> Apply<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Union<T1, T2, T3, T4, Func<T5, R>> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    #endregion

//    #region Match
//    public static void Match<T1, T2, T3, T4, T5>(this Union<T1, T2, T3, T4, T5> @this, Action unmatched, params (Predicate<Union<T1, T2, T3, T4, T5>> predicate, Action<Union<T1, T2, T3, T4, T5>> f)[] eval) {
//      foreach (var (predicate, f) in eval) {
//        if (predicate(@this)) {
//          f(@this);
//          return;
//        }
//      }
//      unmatched();
//    }

//    public static R Match<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<R> unmatched, params (Predicate<Union<T1, T2, T3, T4, T5>> predicate, Func<Union<T1, T2, T3, T4, T5>, R> f)[] eval) {
//      foreach (var (predicate, f) in eval) {
//        if (predicate(@this)) {
//          return f(@this);
//        }
//      }
//      return unmatched();
//    }
//    #endregion

//    #region DebugPrint
//    public static void DebugPrint<T1, T2, T3, T4, T5>(this Union<T1, T2, T3, T4, T5> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
//    #endregion

//    #region Linq Conformance
//    public static Union<R, T2, T3, T4, T5> Select<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T1, R> f) => @this.Map(f);
//    public static Union<T1, R, T3, T4, T5> Select<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T2, R> f) => @this.Map(f);
//    public static Union<T1, T2, R, T4, T5> Select<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T3, R> f) => @this.Map(f);
//    public static Union<T1, T2, T3, R, T5> Select<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T4, R> f) => @this.Map(f);
//    public static Union<T1, T2, T3, T4, R> Select<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T5, R> f) => @this.Map(f);
//    public static Union<R, T2, T3, T4, T5> SelectMany<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T1, Union<R, T2, T3, T4, T5>> f) => @this.FlatMap(f);
//    public static Union<T1, R, T3, T4, T5> SelectMany<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T2, Union<T1, R, T3, T4, T5>> f) => @this.FlatMap(f);
//    public static Union<T1, T2, R, T4, T5> SelectMany<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T3, Union<T1, T2, R, T4, T5>> f) => @this.FlatMap(f);
//    public static Union<T1, T2, T3, R, T5> SelectMany<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T4, Union<T1, T2, T3, R, T5>> f) => @this.FlatMap(f);
//    public static Union<T1, T2, T3, T4, R> SelectMany<T1, T2, T3, T4, T5, R>(this Union<T1, T2, T3, T4, T5> @this, Func<T5, Union<T1, T2, T3, T4, R>> f) => @this.FlatMap(f);
//    #endregion
//  }
//  #endregion

//  #region Union 6
//  public sealed class Union<T1, T2, T3, T4, T5, T6> {
//    private readonly int Index;
//    public bool HasValue1 => Index == 1;
//    public bool HasValue2 => Index == 2;
//    public bool HasValue3 => Index == 3;
//    public bool HasValue4 => Index == 4;
//    public bool HasValue5 => Index == 5;
//    public bool HasValue6 => Index == 6;
//    private readonly T1 UnionValue1;
//    private readonly T2 UnionValue2;
//    private readonly T3 UnionValue3;
//    private readonly T4 UnionValue4;
//    private readonly T5 UnionValue5;
//    private readonly T6 UnionValue6;
//    private Union() { }
//    internal Union(int index, T1 unionvalue1, T2 unionvalue2, T3 unionvalue3, T4 unionvalue4, T5 unionvalue5, T6 unionvalue6) => (Index, UnionValue1, UnionValue2, UnionValue3, UnionValue4, UnionValue5, UnionValue6) = (index, unionvalue1, unionvalue2, unionvalue3, unionvalue4, unionvalue5, unionvalue6);
//    public T1 Value1 => !HasValue1 ? throw new InvalidOperationException($"Can't return {typeof(T1).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue1;
//    public T2 Value2 => !HasValue2 ? throw new InvalidOperationException($"Can't return {typeof(T2).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue2;
//    public T3 Value3 => !HasValue3 ? throw new InvalidOperationException($"Can't return {typeof(T3).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue3;
//    public T4 Value4 => !HasValue4 ? throw new InvalidOperationException($"Can't return {typeof(T4).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue4;
//    public T5 Value5 => !HasValue5 ? throw new InvalidOperationException($"Can't return {typeof(T5).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue5;
//    public T6 Value6 => !HasValue6 ? throw new InvalidOperationException($"Can't return {typeof(T6).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue6;
//    public override string ToString() => $"Union<{typeof(T1).Simplify()}, {typeof(T2).Simplify()}, {typeof(T3).Simplify()}, {typeof(T4).Simplify()}, {typeof(T5).Simplify()}, {typeof(T6).Simplify()}>[{Value1}, {Value2}, {Value3}, {Value4}, {Value5}, {Value6}]";
//    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T1 t1) => new Union<T1, T2, T3, T4, T5, T6>(1, t1, default, default, default, default, default);
//    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T2 t2) => new Union<T1, T2, T3, T4, T5, T6>(2, default, t2, default, default, default, default);
//    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T3 t3) => new Union<T1, T2, T3, T4, T5, T6>(3, default, default, t3, default, default, default);
//    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T4 t4) => new Union<T1, T2, T3, T4, T5, T6>(4, default, default, default, t4, default, default);
//    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T5 t5) => new Union<T1, T2, T3, T4, T5, T6>(5, default, default, default, default, t5, default);
//    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T6 t6) => new Union<T1, T2, T3, T4, T5, T6>(6, default, default, default, default, default, t6);
//  }

//  public static partial class UnionExtensions {
//    #region Syntactic Sugar
//    public static Union<T1, T2, T3, T4, T5, T6> Union<T1, T2, T3, T4, T5, T6>() => new Union<T1, T2, T3, T4, T5, T6>(0, default, default, default, default, default, default);
//    public static Union<T1, T2, T3, T4, T5, T6> Union<T1, T2, T3, T4, T5, T6>(T1 value) => new Union<T1, T2, T3, T4, T5, T6>(1, value, default, default, default, default, default);
//    public static Union<T1, T2, T3, T4, T5, T6> Union<T1, T2, T3, T4, T5, T6>(T2 value) => new Union<T1, T2, T3, T4, T5, T6>(2, default, value, default, default, default, default);
//    public static Union<T1, T2, T3, T4, T5, T6> Union<T1, T2, T3, T4, T5, T6>(T3 value) => new Union<T1, T2, T3, T4, T5, T6>(3, default, default, value, default, default, default);
//    public static Union<T1, T2, T3, T4, T5, T6> Union<T1, T2, T3, T4, T5, T6>(T4 value) => new Union<T1, T2, T3, T4, T5, T6>(4, default, default, default, value, default, default);
//    public static Union<T1, T2, T3, T4, T5, T6> Union<T1, T2, T3, T4, T5, T6>(T5 value) => new Union<T1, T2, T3, T4, T5, T6>(5, default, default, default, default, value, default);
//    public static Union<T1, T2, T3, T4, T5, T6> Union<T1, T2, T3, T4, T5, T6>(T6 value) => new Union<T1, T2, T3, T4, T5, T6>(6, default, default, default, default, default, value);
//    #endregion

//    #region Functor
//    public static Union<R, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T1, R> f) => Union<R, T2, T3, T4, T5, T6>(f(@this.Value1));
//    public static Union<T1, R, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T2, R> f) => Union<T1, R, T3, T4, T5, T6>(f(@this.Value2));
//    public static Union<T1, T2, R, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T3, R> f) => Union<T1, T2, R, T4, T5, T6>(f(@this.Value3));
//    public static Union<T1, T2, T3, R, T5, T6> Map<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T4, R> f) => Union<T1, T2, T3, R, T5, T6>(f(@this.Value4));
//    public static Union<T1, T2, T3, T4, R, T6> Map<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T5, R> f) => Union<T1, T2, T3, T4, R, T6>(f(@this.Value5));
//    public static Union<T1, T2, T3, T4, T5, R> Map<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T6, R> f) => Union<T1, T2, T3, T4, T5, R>(f(@this.Value6));
//    #endregion

//    #region Monad
//    public static Union<R, T2, T3, T4, T5, T6> FlatMap<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T1, Union<R, T2, T3, T4, T5, T6>> f) => f(@this.Value1);
//    public static Union<T1, R, T3, T4, T5, T6> FlatMap<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T2, Union<T1, R, T3, T4, T5, T6>> f) => f(@this.Value2);
//    public static Union<T1, T2, R, T4, T5, T6> FlatMap<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T3, Union<T1, T2, R, T4, T5, T6>> f) => f(@this.Value3);
//    public static Union<T1, T2, T3, R, T5, T6> FlatMap<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T4, Union<T1, T2, T3, R, T5, T6>> f) => f(@this.Value4);
//    public static Union<T1, T2, T3, T4, R, T6> FlatMap<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T5, Union<T1, T2, T3, T4, R, T6>> f) => f(@this.Value5);
//    public static Union<T1, T2, T3, T4, T5, R> FlatMap<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T6, Union<T1, T2, T3, T4, T5, R>> f) => f(@this.Value6);
//    public static Union<R, T2, T3, T4, T5, T6> Bind<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T1, Union<R, T2, T3, T4, T5, T6>> f) => f(@this.Value1);
//    public static Union<T1, R, T3, T4, T5, T6> Bind<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T2, Union<T1, R, T3, T4, T5, T6>> f) => f(@this.Value2);
//    public static Union<T1, T2, R, T4, T5, T6> Bind<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T3, Union<T1, T2, R, T4, T5, T6>> f) => f(@this.Value3);
//    public static Union<T1, T2, T3, R, T5, T6> Bind<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T4, Union<T1, T2, T3, R, T5, T6>> f) => f(@this.Value4);
//    public static Union<T1, T2, T3, T4, R, T6> Bind<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T5, Union<T1, T2, T3, T4, R, T6>> f) => f(@this.Value5);
//    public static Union<T1, T2, T3, T4, T5, R> Bind<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T6, Union<T1, T2, T3, T4, T5, R>> f) => f(@this.Value6);
//    #endregion

//    #region Applicative Functor
//    public static Union<R, T2, T3, T4, T5, T6> Apply<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Union<Func<T1, R>, T2, T3, T4, T5, T6> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, R, T3, T4, T5, T6> Apply<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Union<T1, Func<T2, R>, T3, T4, T5, T6> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, T2, R, T4, T5, T6> Apply<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Union<T1, T2, Func<T3, R>, T4, T5, T6> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, T2, T3, R, T5, T6> Apply<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Union<T1, T2, T3, Func<T4, R>, T5, T6> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, T2, T3, T4, R, T6> Apply<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Union<T1, T2, T3, T4, Func<T5, R>, T6> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    public static Union<T1, T2, T3, T4, T5, R> Apply<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Union<T1, T2, T3, T4, T5, Func<T6, R>> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
//    #endregion

//    #region Match
//    public static void Match<T1, T2, T3, T4, T5, T6>(this Union<T1, T2, T3, T4, T5, T6> @this, Action unmatched, params (Predicate<Union<T1, T2, T3, T4, T5, T6>> predicate, Action<Union<T1, T2, T3, T4, T5, T6>> f)[] eval) {
//      foreach (var (predicate, f) in eval) {
//        if (predicate(@this)) {
//          f(@this);
//          return;
//        }
//      }
//      unmatched();
//    }

//    public static R Match<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<R> unmatched, params (Predicate<Union<T1, T2, T3, T4, T5, T6>> predicate, Func<Union<T1, T2, T3, T4, T5, T6>, R> f)[] eval) {
//      foreach (var (predicate, f) in eval) {
//        if (predicate(@this)) {
//          return f(@this);
//        }
//      }
//      return unmatched();
//    }
//    #endregion

//    #region DebugPrint
//    public static void DebugPrint<T1, T2, T3, T4, T5, T6>(this Union<T1, T2, T3, T4, T5, T6> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
//    #endregion

//    #region Linq Conformance
//    public static Union<R, T2, T3, T4, T5, T6> Select<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T1, R> f) => @this.Map(f);
//    public static Union<T1, R, T3, T4, T5, T6> Select<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T2, R> f) => @this.Map(f);
//    public static Union<T1, T2, R, T4, T5, T6> Select<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T3, R> f) => @this.Map(f);
//    public static Union<T1, T2, T3, R, T5, T6> Select<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T4, R> f) => @this.Map(f);
//    public static Union<T1, T2, T3, T4, R, T6> Select<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T5, R> f) => @this.Map(f);
//    public static Union<T1, T2, T3, T4, T5, R> Select<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T6, R> f) => @this.Map(f);
//    public static Union<R, T2, T3, T4, T5, T6> SelectMany<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T1, Union<R, T2, T3, T4, T5, T6>> f) => @this.FlatMap(f);
//    public static Union<T1, R, T3, T4, T5, T6> SelectMany<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T2, Union<T1, R, T3, T4, T5, T6>> f) => @this.FlatMap(f);
//    public static Union<T1, T2, R, T4, T5, T6> SelectMany<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T3, Union<T1, T2, R, T4, T5, T6>> f) => @this.FlatMap(f);
//    public static Union<T1, T2, T3, R, T5, T6> SelectMany<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T4, Union<T1, T2, T3, R, T5, T6>> f) => @this.FlatMap(f);
//    public static Union<T1, T2, T3, T4, R, T6> SelectMany<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T5, Union<T1, T2, T3, T4, R, T6>> f) => @this.FlatMap(f);
//    public static Union<T1, T2, T3, T4, T5, R> SelectMany<T1, T2, T3, T4, T5, T6, R>(this Union<T1, T2, T3, T4, T5, T6> @this, Func<T6, Union<T1, T2, T3, T4, T5, R>> f) => @this.FlatMap(f);
//    #endregion
//  }
//  #endregion

//}
