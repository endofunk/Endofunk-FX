using System;

namespace Endofunk.FX {

  #region Tagged 1
  public sealed class Tagged<E, T1> where E : Enum {
    public readonly E Tag;
    public readonly bool HasValue;
    private readonly T1 UnionValue;
    private Tagged() { }
    internal Tagged(E tag, bool hasvalue, T1 unionvalue) => (Tag, HasValue, UnionValue) = (tag, hasvalue, unionvalue);
    public T1 Value => !HasValue ? throw new InvalidOperationException($"Can't return {typeof(T1).Simplify()}, Nothing embedded.") : UnionValue;
    public override string ToString() => $"Tagged<{typeof(E).Simplify()}, {typeof(T1).Simplify()}>[{Value}]";
  }

  public static partial class Prelude {
    #region Syntactic Sugar
    public static Tagged<E, T1> Tagged<E, T1>(E tag) where E : Enum => new Tagged<E, T1>(tag, false, default);
    public static Tagged<E, T1> Tagged<E, T1>(E tag, T1 value) where E : Enum => new Tagged<E, T1>(tag, true, value);
    #endregion

    #region Functor
    public static Tagged<E, R> Map<E, T1, R>(this Tagged<E, T1> @this, Func<T1, R> f) where E : Enum => Tagged(@this.Tag, f(@this.Value));
    public static Tagged<E, R> Map<E, T1, R>(this Func<T1, R> f, Tagged<E, T1> @this) where E : Enum => Tagged(@this.Tag, f(@this.Value));
    #endregion

    #region Monad
    public static Tagged<E, R> FlatMap<E, T1, R>(this Tagged<E, T1> @this, Func<(E, T1), Tagged<E, R>> f) where E : Enum => f((@this.Tag, @this.Value));
    public static Tagged<E, R> FlatMap<E, T1, R>(this Func<(E, T1), Tagged<E, R>> f, Tagged<E, T1> @this) where E : Enum => f((@this.Tag, @this.Value));
    public static Tagged<E, R> Bind<E, T1, R>(this Tagged<E, T1> @this, Func<(E, T1), Tagged<E, R>> f) where E : Enum => f((@this.Tag, @this.Value));
    #endregion

    #region Applicative Functor
    public static Tagged<E, R> Apply<E, T1, R>(this Tagged<E, T1> @this, Tagged<E, Func<T1, R>> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, R> Apply<E, T1, R>(this Tagged<E, Func<T1, R>> fn, Tagged<E, T1> @this) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    #endregion

    #region Applicative Functor - Lift a function & actions
    public static Tagged<E, R> LiftA<E, T1, R>(this Func<T1, R> @this, Tagged<E, T1> t1) where E : Enum => @this.Map(t1);
    public static Tagged<E, R> LiftA<E, T1, T2, R>(this Func<T1, T2, R> @this, Tagged<E, T1> t1, Tagged<E, T2> t2) where E : Enum => @this.Curry().Map(t1).Apply(t2);
    public static Tagged<E, R> LiftA<E, T1, T2, T3, R>(this Func<T1, T2, T3, R> @this, Tagged<E, T1> t1, Tagged<E, T2> t2, Tagged<E, T3> t3) where E : Enum => @this.Curry().Map(t1).Apply(t2).Apply(t3);
    public static Tagged<E, R> LiftA<E, T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> @this, Tagged<E, T1> t1, Tagged<E, T2> t2, Tagged<E, T3> t3, Tagged<E, T4> t4) where E : Enum => @this.Curry().Map(t1).Apply(t2).Apply(t3).Apply(t4);
    public static Tagged<E, R> LiftA<E, T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> @this, Tagged<E, T1> t1, Tagged<E, T2> t2, Tagged<E, T3> t3, Tagged<E, T4> t4, Tagged<E, T5> t5) where E : Enum => @this.Curry().Map(t1).Apply(t2).Apply(t3).Apply(t4).Apply(t5);
    public static Tagged<E, R> LiftA<E, T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> @this, Tagged<E, T1> t1, Tagged<E, T2> t2, Tagged<E, T3> t3, Tagged<E, T4> t4, Tagged<E, T5> t5, Tagged<E, T6> t6) where E : Enum => @this.Curry().Map(t1).Apply(t2).Apply(t3).Apply(t4).Apply(t5).Apply(t6);
    public static Tagged<E, R> LiftA<E, T1, T2, T3, T4, T5, T6, T7, R>(this Func<T1, T2, T3, T4, T5, T6, T7, R> @this, Tagged<E, T1> t1, Tagged<E, T2> t2, Tagged<E, T3> t3, Tagged<E, T4> t4, Tagged<E, T5> t5, Tagged<E, T6> t6, Tagged<E, T7> t7) where E : Enum => @this.Curry().Map(t1).Apply(t2).Apply(t3).Apply(t4).Apply(t5).Apply(t6).Apply(t7);
    public static Tagged<E, R> LiftA<E, T1, T2, T3, T4, T5, T6, T7, T8, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, R> @this, Tagged<E, T1> t1, Tagged<E, T2> t2, Tagged<E, T3> t3, Tagged<E, T4> t4, Tagged<E, T5> t5, Tagged<E, T6> t6, Tagged<E, T7> t7, Tagged<E, T8> t8) where E : Enum => @this.Curry().Map(t1).Apply(t2).Apply(t3).Apply(t4).Apply(t5).Apply(t6).Apply(t7).Apply(t8);
    public static Tagged<E, R> LiftA<E, T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> @this, Tagged<E, T1> t1, Tagged<E, T2> t2, Tagged<E, T3> t3, Tagged<E, T4> t4, Tagged<E, T5> t5, Tagged<E, T6> t6, Tagged<E, T7> t7, Tagged<E, T8> t8, Tagged<E, T9> t9) where E : Enum => @this.Curry().Map(t1).Apply(t2).Apply(t3).Apply(t4).Apply(t5).Apply(t6).Apply(t7).Apply(t8).Apply(t9);
    #endregion

    #region Match
    public static void Match<E, T1>(this Tagged<E, T1> @this, Action unmatched, params (Func<E, bool> predicate, Action<Tagged<E, T1>> f)[] eval) where E : Enum {
      foreach (var (predicate, f) in eval) {
        if (predicate(@this.Tag)) {
          f(@this);
          return;
        }
      }
      unmatched();
    }

    public static R Match<E, T1, R>(this Tagged<E, T1> @this, Func<R> unmatched, params (Func<E, bool> predicate, Func<Tagged<E, T1>, R> f)[] eval) where E : Enum {
      foreach (var (predicate, f) in eval) {
        if (predicate(@this.Tag)) {
          return f(@this);
        }
      }
      return unmatched();
    }
    #endregion

    #region DebugPrint
    public static void DebugPrint<E, T1>(this Tagged<E, T1> @this, string title = "") where E : Enum => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Linq Conformance
    public static Tagged<E, R> Select<E, T1, R>(this Tagged<E, T1> @this, Func<T1, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, R> SelectMany<E, T1, R>(this Tagged<E, T1> @this, Func<(E, T1), Tagged<E, R>> f) where E : Enum => @this.FlatMap(f);
    #endregion
  }
  #endregion

  #region Tagged 2
  public sealed class Tagged<E, T1, T2> where E : Enum {
    public readonly E Tag;
    private readonly int Index;
    public bool HasValue1 => Index == 1;
    public bool HasValue2 => Index == 2;
    private readonly T1 UnionValue1;
    private readonly T2 UnionValue2;
    private Tagged() { }
    internal Tagged(E tag, int index, T1 unionvalue1, T2 unionvalue2) => (Tag, Index, UnionValue1, UnionValue2) = (tag, index, unionvalue1, unionvalue2);
    public T1 Value1 => !HasValue1 ? throw new InvalidOperationException($"Can't return {typeof(T1).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue1;
    public T2 Value2 => !HasValue2 ? throw new InvalidOperationException($"Can't return {typeof(T2).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue2;
    public override string ToString() => $"Tagged<{typeof(E).Simplify()}, {typeof(T1).Simplify()}, {typeof(T2).Simplify()}>[{Value1}, {Value2}]";
  }

  public static partial class Prelude {
    #region Syntactic Sugar
    public static Tagged<E, T1, T2> Tagged<E, T1, T2>(E tag) where E : Enum => new Tagged<E, T1, T2>(tag, 0, default, default);
    public static Tagged<E, T1, T2> Tagged<E, T1, T2>(E tag, T1 value) where E : Enum => new Tagged<E, T1, T2>(tag, 1, value, default);
    public static Tagged<E, T1, T2> Tagged<E, T1, T2>(E tag, T2 value) where E : Enum => new Tagged<E, T1, T2>(tag, 2, default, value);
    #endregion

    #region Functor
    public static Tagged<E, R, T2> Map<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Func<T1, R> f) where E : Enum => Tagged<E, R, T2>(@this.Tag, f(@this.Value1));
    public static Tagged<E, T1, R> Map<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Func<T2, R> f) where E : Enum => Tagged<E, T1, R>(@this.Tag, f(@this.Value2));
    #endregion

    #region Monad
    public static Tagged<E, R, T2> FlatMap<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Func<(E, T1), Tagged<E, R, T2>> f) where E : Enum => f((@this.Tag, @this.Value1));
    public static Tagged<E, T1, R> FlatMap<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Func<(E, T2), Tagged<E, T1, R>> f) where E : Enum => f((@this.Tag, @this.Value2));
    public static Tagged<E, R, T2> Bind<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Func<(E, T1), Tagged<E, R, T2>> f) where E : Enum => f((@this.Tag, @this.Value1));
    public static Tagged<E, T1, R> Bind<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Func<(E, T2), Tagged<E, T1, R>> f) where E : Enum => f((@this.Tag, @this.Value2));
    #endregion

    #region Applicative Functor
    public static Tagged<E, R, T2> Apply<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Tagged<E, Func<T1, R>, T2> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, R> Apply<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Tagged<E, T1, Func<T2, R>> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, R, T2> Apply<E, T1, T2, R>(this Tagged<E, Func<T1, R>, T2> fn, Tagged<E, T1, T2> @this) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, R> Apply<E, T1, T2, R>(this Tagged<E, T1, Func<T2, R>> fn, Tagged<E, T1, T2> @this) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    #endregion

    #region Match
    public static void Match<E, T1, T2>(this Tagged<E, T1, T2> @this, Action unmatched, params (Func<E, bool> predicate, Action<Tagged<E, T1, T2>> f)[] eval) where E : Enum {
      foreach (var (predicate, f) in eval) {
        if (predicate(@this.Tag)) {
          f(@this);
          return;
        }
      }
      unmatched();
    }

    public static R Match<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Func<R> unmatched, params (Func<E, bool> predicate, Func<Tagged<E, T1, T2>, R> f)[] eval) where E : Enum {
      foreach (var (predicate, f) in eval) {
        if (predicate(@this.Tag)) {
          return f(@this);
        }
      }
      return unmatched();
    }
    #endregion

    #region DebugPrint
    public static void DebugPrint<E, T1, T2>(this Tagged<E, T1, T2> @this, string title = "") where E : Enum => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Linq Conformance
    public static Tagged<E, R, T2> Select<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Func<T1, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, R> Select<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Func<T2, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, R, T2> SelectMany<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Func<(E, T1), Tagged<E, R, T2>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, R> SelectMany<E, T1, T2, R>(this Tagged<E, T1, T2> @this, Func<(E, T2), Tagged<E, T1, R>> f) where E : Enum => @this.FlatMap(f);
    #endregion
  }
  #endregion

  #region Tagged 3
  public sealed class Tagged<E, T1, T2, T3> where E : Enum {
    public readonly E Tag;
    private readonly int Index;
    public bool HasValue1 => Index == 1;
    public bool HasValue2 => Index == 2;
    public bool HasValue3 => Index == 3;
    private readonly T1 UnionValue1;
    private readonly T2 UnionValue2;
    private readonly T3 UnionValue3;
    private Tagged() { }
    internal Tagged(E tag, int index, T1 unionvalue1, T2 unionvalue2, T3 unionvalue3) => (Tag, Index, UnionValue1, UnionValue2, UnionValue3) = (tag, index, unionvalue1, unionvalue2, unionvalue3);
    public T1 Value1 => !HasValue1 ? throw new InvalidOperationException($"Can't return {typeof(T1).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue1;
    public T2 Value2 => !HasValue2 ? throw new InvalidOperationException($"Can't return {typeof(T2).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue2;
    public T3 Value3 => !HasValue3 ? throw new InvalidOperationException($"Can't return {typeof(T3).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue3;
    public override string ToString() => $"Tagged<{typeof(E).Simplify()}, {typeof(T1).Simplify()}, {typeof(T2).Simplify()}, {typeof(T3).Simplify()}>[{Value1}, {Value2}, {Value3}]";
  }

  public static partial class Prelude {
    #region Syntactic Sugar
    public static Tagged<E, T1, T2, T3> Tagged<E, T1, T2, T3>(E tag) where E : Enum => new Tagged<E, T1, T2, T3>(tag, 0, default, default, default);
    public static Tagged<E, T1, T2, T3> Tagged<E, T1, T2, T3>(E tag, T1 value) where E : Enum => new Tagged<E, T1, T2, T3>(tag, 1, value, default, default);
    public static Tagged<E, T1, T2, T3> Tagged<E, T1, T2, T3>(E tag, T2 value) where E : Enum => new Tagged<E, T1, T2, T3>(tag, 2, default, value, default);
    public static Tagged<E, T1, T2, T3> Tagged<E, T1, T2, T3>(E tag, T3 value) where E : Enum => new Tagged<E, T1, T2, T3>(tag, 3, default, default, value);
    #endregion

    #region Functor
    public static Tagged<E, R, T2, T3> Map<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<T1, R> f) where E : Enum => Tagged<E, R, T2, T3>(@this.Tag, f(@this.Value1));
    public static Tagged<E, T1, R, T3> Map<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<T2, R> f) where E : Enum => Tagged<E, T1, R, T3>(@this.Tag, f(@this.Value2));
    public static Tagged<E, T1, T2, R> Map<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<T3, R> f) where E : Enum => Tagged<E, T1, T2, R>(@this.Tag, f(@this.Value3));
    #endregion

    #region Monad
    public static Tagged<E, R, T2, T3> FlatMap<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<(E, T1), Tagged<E, R, T2, T3>> f) where E : Enum => f((@this.Tag, @this.Value1));
    public static Tagged<E, T1, R, T3> FlatMap<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<(E, T2), Tagged<E, T1, R, T3>> f) where E : Enum => f((@this.Tag, @this.Value2));
    public static Tagged<E, T1, T2, R> FlatMap<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<(E, T3), Tagged<E, T1, T2, R>> f) where E : Enum => f((@this.Tag, @this.Value3));
    public static Tagged<E, R, T2, T3> Bind<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<(E, T1), Tagged<E, R, T2, T3>> f) where E : Enum => f((@this.Tag, @this.Value1));
    public static Tagged<E, T1, R, T3> Bind<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<(E, T2), Tagged<E, T1, R, T3>> f) where E : Enum => f((@this.Tag, @this.Value2));
    public static Tagged<E, T1, T2, R> Bind<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<(E, T3), Tagged<E, T1, T2, R>> f) where E : Enum => f((@this.Tag, @this.Value3));
    #endregion

    #region Applicative Functor
    public static Tagged<E, R, T2, T3> Apply<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Tagged<E, Func<T1, R>, T2, T3> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, R, T3> Apply<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Tagged<E, T1, Func<T2, R>, T3> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, T2, R> Apply<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Tagged<E, T1, T2, Func<T3, R>> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    #endregion

    #region Match
    public static void Match<E, T1, T2, T3>(this Tagged<E, T1, T2, T3> @this, Action unmatched, params (Func<E, bool> predicate, Action<Tagged<E, T1, T2, T3>> f)[] eval) where E : Enum {
      foreach (var (predicate, f) in eval) {
        if (predicate(@this.Tag)) {
          f(@this);
          return;
        }
      }
      unmatched();
    }

    public static R Match<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<R> unmatched, params (Func<E, bool> predicate, Func<Tagged<E, T1, T2, T3>, R> f)[] eval) where E : Enum {
      foreach (var (predicate, f) in eval) {
        if (predicate(@this.Tag)) {
          return f(@this);
        }
      }
      return unmatched();
    }
    #endregion

    #region DebugPrint
    public static void DebugPrint<E, T1, T2, T3>(this Tagged<E, T1, T2, T3> @this, string title = "") where E : Enum => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Linq Conformance
    public static Tagged<E, R, T2, T3> Select<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<T1, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, R, T3> Select<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<T2, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, T2, R> Select<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<T3, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, R, T2, T3> SelectMany<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<(E, T1), Tagged<E, R, T2, T3>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, R, T3> SelectMany<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<(E, T2), Tagged<E, T1, R, T3>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, T2, R> SelectMany<E, T1, T2, T3, R>(this Tagged<E, T1, T2, T3> @this, Func<(E, T3), Tagged<E, T1, T2, R>> f) where E : Enum => @this.FlatMap(f);
    #endregion
  }
  #endregion

  #region Tagged 4
  public sealed class Tagged<E, T1, T2, T3, T4> where E : Enum {
    public readonly E Tag;
    private readonly int Index;
    public bool HasValue1 => Index == 1;
    public bool HasValue2 => Index == 2;
    public bool HasValue3 => Index == 3;
    public bool HasValue4 => Index == 4;
    private readonly T1 UnionValue1;
    private readonly T2 UnionValue2;
    private readonly T3 UnionValue3;
    private readonly T4 UnionValue4;
    private Tagged() { }
    internal Tagged(E tag, int index, T1 unionvalue1, T2 unionvalue2, T3 unionvalue3, T4 unionvalue4) => (Tag, Index, UnionValue1, UnionValue2, UnionValue3, UnionValue4) = (tag, index, unionvalue1, unionvalue2, unionvalue3, unionvalue4);
    public T1 Value1 => !HasValue1 ? throw new InvalidOperationException($"Can't return {typeof(T1).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue1;
    public T2 Value2 => !HasValue2 ? throw new InvalidOperationException($"Can't return {typeof(T2).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue2;
    public T3 Value3 => !HasValue3 ? throw new InvalidOperationException($"Can't return {typeof(T3).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue3;
    public T4 Value4 => !HasValue4 ? throw new InvalidOperationException($"Can't return {typeof(T4).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue4;
    public override string ToString() => $"Tagged<{typeof(E).Simplify()}, {typeof(T1).Simplify()}, {typeof(T2).Simplify()}, {typeof(T3).Simplify()}, {typeof(T4).Simplify()}>[{Value1}, {Value2}, {Value3}, {Value4}]";
  }

  public static partial class Prelude {
    #region Syntactic Sugar
    public static Tagged<E, T1, T2, T3, T4> Tagged<E, T1, T2, T3, T4>(E tag) where E : Enum => new Tagged<E, T1, T2, T3, T4>(tag, 0, default, default, default, default);
    public static Tagged<E, T1, T2, T3, T4> Tagged<E, T1, T2, T3, T4>(E tag, T1 value) where E : Enum => new Tagged<E, T1, T2, T3, T4>(tag, 1, value, default, default, default);
    public static Tagged<E, T1, T2, T3, T4> Tagged<E, T1, T2, T3, T4>(E tag, T2 value) where E : Enum => new Tagged<E, T1, T2, T3, T4>(tag, 2, default, value, default, default);
    public static Tagged<E, T1, T2, T3, T4> Tagged<E, T1, T2, T3, T4>(E tag, T3 value) where E : Enum => new Tagged<E, T1, T2, T3, T4>(tag, 3, default, default, value, default);
    public static Tagged<E, T1, T2, T3, T4> Tagged<E, T1, T2, T3, T4>(E tag, T4 value) where E : Enum => new Tagged<E, T1, T2, T3, T4>(tag, 4, default, default, default, value);
    #endregion

    #region Functor
    public static Tagged<E, R, T2, T3, T4> Map<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<T1, R> f) where E : Enum => Tagged<E, R, T2, T3, T4>(@this.Tag, f(@this.Value1));
    public static Tagged<E, T1, R, T3, T4> Map<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<T2, R> f) where E : Enum => Tagged<E, T1, R, T3, T4>(@this.Tag, f(@this.Value2));
    public static Tagged<E, T1, T2, R, T4> Map<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<T3, R> f) where E : Enum => Tagged<E, T1, T2, R, T4>(@this.Tag, f(@this.Value3));
    public static Tagged<E, T1, T2, T3, R> Map<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<T4, R> f) where E : Enum => Tagged<E, T1, T2, T3, R>(@this.Tag, f(@this.Value4));
    #endregion

    #region Monad
    public static Tagged<E, R, T2, T3, T4> FlatMap<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<(E, T1), Tagged<E, R, T2, T3, T4>> f) where E : Enum => f((@this.Tag, @this.Value1));
    public static Tagged<E, T1, R, T3, T4> FlatMap<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<(E, T2), Tagged<E, T1, R, T3, T4>> f) where E : Enum => f((@this.Tag, @this.Value2));
    public static Tagged<E, T1, T2, R, T4> FlatMap<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<(E, T3), Tagged<E, T1, T2, R, T4>> f) where E : Enum => f((@this.Tag, @this.Value3));
    public static Tagged<E, T1, T2, T3, R> FlatMap<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<(E, T4), Tagged<E, T1, T2, T3, R>> f) where E : Enum => f((@this.Tag, @this.Value4));
    public static Tagged<E, R, T2, T3, T4> Bind<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<(E, T1), Tagged<E, R, T2, T3, T4>> f) where E : Enum => f((@this.Tag, @this.Value1));
    public static Tagged<E, T1, R, T3, T4> Bind<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<(E, T2), Tagged<E, T1, R, T3, T4>> f) where E : Enum => f((@this.Tag, @this.Value2));
    public static Tagged<E, T1, T2, R, T4> Bind<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<(E, T3), Tagged<E, T1, T2, R, T4>> f) where E : Enum => f((@this.Tag, @this.Value3));
    public static Tagged<E, T1, T2, T3, R> Bind<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<(E, T4), Tagged<E, T1, T2, T3, R>> f) where E : Enum => f((@this.Tag, @this.Value4));
    #endregion

    #region Applicative Functor
    public static Tagged<E, R, T2, T3, T4> Apply<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Tagged<E, Func<T1, R>, T2, T3, T4> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, R, T3, T4> Apply<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Tagged<E, T1, Func<T2, R>, T3, T4> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, T2, R, T4> Apply<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Tagged<E, T1, T2, Func<T3, R>, T4> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, T2, T3, R> Apply<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Tagged<E, T1, T2, T3, Func<T4, R>> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    #endregion

    #region Match
    public static void Match<E, T1, T2, T3, T4>(this Tagged<E, T1, T2, T3, T4> @this, Action unmatched, params (Func<E, bool> predicate, Action<Tagged<E, T1, T2, T3, T4>> f)[] eval) where E : Enum {
      foreach (var (predicate, f) in eval) {
        if (predicate(@this.Tag)) {
          f(@this);
          return;
        }
      }
      unmatched();
    }

    public static R Match<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<R> unmatched, params (Func<E, bool> predicate, Func<Tagged<E, T1, T2, T3, T4>, R> f)[] eval) where E : Enum {
      foreach (var (predicate, f) in eval) {
        if (predicate(@this.Tag)) {
          return f(@this);
        }
      }
      return unmatched();
    }
    #endregion

    #region DebugPrint
    public static void DebugPrint<E, T1, T2, T3, T4>(this Tagged<E, T1, T2, T3, T4> @this, string title = "") where E : Enum => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Linq Conformance
    public static Tagged<E, R, T2, T3, T4> Select<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<T1, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, R, T3, T4> Select<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<T2, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, T2, R, T4> Select<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<T3, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, T2, T3, R> Select<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<T4, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, R, T2, T3, T4> SelectMany<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<(E, T1), Tagged<E, R, T2, T3, T4>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, R, T3, T4> SelectMany<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<(E, T2), Tagged<E, T1, R, T3, T4>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, T2, R, T4> SelectMany<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<(E, T3), Tagged<E, T1, T2, R, T4>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, T2, T3, R> SelectMany<E, T1, T2, T3, T4, R>(this Tagged<E, T1, T2, T3, T4> @this, Func<(E, T4), Tagged<E, T1, T2, T3, R>> f) where E : Enum => @this.FlatMap(f);
    #endregion
  }
  #endregion

  #region Tagged 5
  public sealed class Tagged<E, T1, T2, T3, T4, T5> where E : Enum {
    public readonly E Tag;
    private readonly int Index;
    public bool HasValue1 => Index == 1;
    public bool HasValue2 => Index == 2;
    public bool HasValue3 => Index == 3;
    public bool HasValue4 => Index == 4;
    public bool HasValue5 => Index == 5;
    private readonly T1 UnionValue1;
    private readonly T2 UnionValue2;
    private readonly T3 UnionValue3;
    private readonly T4 UnionValue4;
    private readonly T5 UnionValue5;
    private Tagged() { }
    internal Tagged(E tag, int index, T1 unionvalue1, T2 unionvalue2, T3 unionvalue3, T4 unionvalue4, T5 unionvalue5) => (Tag, Index, UnionValue1, UnionValue2, UnionValue3, UnionValue4, UnionValue5) = (tag, index, unionvalue1, unionvalue2, unionvalue3, unionvalue4, unionvalue5);
    public T1 Value1 => !HasValue1 ? throw new InvalidOperationException($"Can't return {typeof(T1).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue1;
    public T2 Value2 => !HasValue2 ? throw new InvalidOperationException($"Can't return {typeof(T2).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue2;
    public T3 Value3 => !HasValue3 ? throw new InvalidOperationException($"Can't return {typeof(T3).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue3;
    public T4 Value4 => !HasValue4 ? throw new InvalidOperationException($"Can't return {typeof(T4).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue4;
    public T5 Value5 => !HasValue5 ? throw new InvalidOperationException($"Can't return {typeof(T5).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue5;
    public override string ToString() => $"Tagged<{typeof(E).Simplify()}, {typeof(T1).Simplify()}, {typeof(T2).Simplify()}, {typeof(T3).Simplify()}, {typeof(T4).Simplify()}, {typeof(T5).Simplify()}>[{Value1}, {Value2}, {Value3}, {Value4}, {Value5}]";
  }

  public static partial class Prelude {
    #region Syntactic Sugar
    public static Tagged<E, T1, T2, T3, T4, T5> Tagged<E, T1, T2, T3, T4, T5>(E tag) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5>(tag, 0, default, default, default, default, default);
    public static Tagged<E, T1, T2, T3, T4, T5> Tagged<E, T1, T2, T3, T4, T5>(E tag, T1 value) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5>(tag, 1, value, default, default, default, default);
    public static Tagged<E, T1, T2, T3, T4, T5> Tagged<E, T1, T2, T3, T4, T5>(E tag, T2 value) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5>(tag, 2, default, value, default, default, default);
    public static Tagged<E, T1, T2, T3, T4, T5> Tagged<E, T1, T2, T3, T4, T5>(E tag, T3 value) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5>(tag, 3, default, default, value, default, default);
    public static Tagged<E, T1, T2, T3, T4, T5> Tagged<E, T1, T2, T3, T4, T5>(E tag, T4 value) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5>(tag, 4, default, default, default, value, default);
    public static Tagged<E, T1, T2, T3, T4, T5> Tagged<E, T1, T2, T3, T4, T5>(E tag, T5 value) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5>(tag, 5, default, default, default, default, value);
    #endregion

    #region Functor
    public static Tagged<E, R, T2, T3, T4, T5> Map<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<T1, R> f) where E : Enum => Tagged<E, R, T2, T3, T4, T5>(@this.Tag, f(@this.Value1));
    public static Tagged<E, T1, R, T3, T4, T5> Map<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<T2, R> f) where E : Enum => Tagged<E, T1, R, T3, T4, T5>(@this.Tag, f(@this.Value2));
    public static Tagged<E, T1, T2, R, T4, T5> Map<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<T3, R> f) where E : Enum => Tagged<E, T1, T2, R, T4, T5>(@this.Tag, f(@this.Value3));
    public static Tagged<E, T1, T2, T3, R, T5> Map<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<T4, R> f) where E : Enum => Tagged<E, T1, T2, T3, R, T5>(@this.Tag, f(@this.Value4));
    public static Tagged<E, T1, T2, T3, T4, R> Map<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<T5, R> f) where E : Enum => Tagged<E, T1, T2, T3, T4, R>(@this.Tag, f(@this.Value5));
    #endregion

    #region Monad
    public static Tagged<E, R, T2, T3, T4, T5> FlatMap<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T1), Tagged<E, R, T2, T3, T4, T5>> f) where E : Enum => f((@this.Tag, @this.Value1));
    public static Tagged<E, T1, R, T3, T4, T5> FlatMap<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T2), Tagged<E, T1, R, T3, T4, T5>> f) where E : Enum => f((@this.Tag, @this.Value2));
    public static Tagged<E, T1, T2, R, T4, T5> FlatMap<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T3), Tagged<E, T1, T2, R, T4, T5>> f) where E : Enum => f((@this.Tag, @this.Value3));
    public static Tagged<E, T1, T2, T3, R, T5> FlatMap<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T4), Tagged<E, T1, T2, T3, R, T5>> f) where E : Enum => f((@this.Tag, @this.Value4));
    public static Tagged<E, T1, T2, T3, T4, R> FlatMap<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T5), Tagged<E, T1, T2, T3, T4, R>> f) where E : Enum => f((@this.Tag, @this.Value5));
    public static Tagged<E, R, T2, T3, T4, T5> Bind<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T1), Tagged<E, R, T2, T3, T4, T5>> f) where E : Enum => f((@this.Tag, @this.Value1));
    public static Tagged<E, T1, R, T3, T4, T5> Bind<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T2), Tagged<E, T1, R, T3, T4, T5>> f) where E : Enum => f((@this.Tag, @this.Value2));
    public static Tagged<E, T1, T2, R, T4, T5> Bind<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T3), Tagged<E, T1, T2, R, T4, T5>> f) where E : Enum => f((@this.Tag, @this.Value3));
    public static Tagged<E, T1, T2, T3, R, T5> Bind<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T4), Tagged<E, T1, T2, T3, R, T5>> f) where E : Enum => f((@this.Tag, @this.Value4));
    public static Tagged<E, T1, T2, T3, T4, R> Bind<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T5), Tagged<E, T1, T2, T3, T4, R>> f) where E : Enum => f((@this.Tag, @this.Value5));
    #endregion

    #region Applicative Functor
    public static Tagged<E, R, T2, T3, T4, T5> Apply<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Tagged<E, Func<T1, R>, T2, T3, T4, T5> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, R, T3, T4, T5> Apply<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Tagged<E, T1, Func<T2, R>, T3, T4, T5> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, T2, R, T4, T5> Apply<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Tagged<E, T1, T2, Func<T3, R>, T4, T5> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, T2, T3, R, T5> Apply<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Tagged<E, T1, T2, T3, Func<T4, R>, T5> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, T2, T3, T4, R> Apply<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Tagged<E, T1, T2, T3, T4, Func<T5, R>> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    #endregion

    #region Match
    public static void Match<E, T1, T2, T3, T4, T5>(this Tagged<E, T1, T2, T3, T4, T5> @this, Action unmatched, params (Func<E, bool> predicate, Action<Tagged<E, T1, T2, T3, T4, T5>> f)[] eval) where E : Enum {
      foreach (var (predicate, f) in eval) {
        if (predicate(@this.Tag)) {
          f(@this);
          return;
        }
      }
      unmatched();
    }

    public static R Match<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<R> unmatched, params (Func<E, bool> predicate, Func<Tagged<E, T1, T2, T3, T4, T5>, R> f)[] eval) where E : Enum {
      foreach (var (predicate, f) in eval) {
        if (predicate(@this.Tag)) {
          return f(@this);
        }
      }
      return unmatched();
    }
    #endregion

    #region DebugPrint
    public static void DebugPrint<E, T1, T2, T3, T4, T5>(this Tagged<E, T1, T2, T3, T4, T5> @this, string title = "") where E : Enum => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Linq Conformance
    public static Tagged<E, R, T2, T3, T4, T5> Select<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<T1, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, R, T3, T4, T5> Select<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<T2, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, T2, R, T4, T5> Select<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<T3, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, T2, T3, R, T5> Select<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<T4, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, T2, T3, T4, R> Select<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<T5, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, R, T2, T3, T4, T5> SelectMany<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T1), Tagged<E, R, T2, T3, T4, T5>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, R, T3, T4, T5> SelectMany<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T2), Tagged<E, T1, R, T3, T4, T5>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, T2, R, T4, T5> SelectMany<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T3), Tagged<E, T1, T2, R, T4, T5>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, T2, T3, R, T5> SelectMany<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T4), Tagged<E, T1, T2, T3, R, T5>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, T2, T3, T4, R> SelectMany<E, T1, T2, T3, T4, T5, R>(this Tagged<E, T1, T2, T3, T4, T5> @this, Func<(E, T5), Tagged<E, T1, T2, T3, T4, R>> f) where E : Enum => @this.FlatMap(f);
    #endregion
  }
  #endregion

  #region Tagged 6
  public sealed class Tagged<E, T1, T2, T3, T4, T5, T6> where E : Enum {
    public readonly E Tag;
    private readonly int Index;
    public bool HasValue1 => Index == 1;
    public bool HasValue2 => Index == 2;
    public bool HasValue3 => Index == 3;
    public bool HasValue4 => Index == 4;
    public bool HasValue5 => Index == 5;
    public bool HasValue6 => Index == 6;
    private readonly T1 UnionValue1;
    private readonly T2 UnionValue2;
    private readonly T3 UnionValue3;
    private readonly T4 UnionValue4;
    private readonly T5 UnionValue5;
    private readonly T6 UnionValue6;
    private Tagged() { }
    internal Tagged(E tag, int index, T1 unionvalue1, T2 unionvalue2, T3 unionvalue3, T4 unionvalue4, T5 unionvalue5, T6 unionvalue6) => (Tag, Index, UnionValue1, UnionValue2, UnionValue3, UnionValue4, UnionValue5, UnionValue6) = (tag, index, unionvalue1, unionvalue2, unionvalue3, unionvalue4, unionvalue5, unionvalue6);
    public T1 Value1 => !HasValue1 ? throw new InvalidOperationException($"Can't return {typeof(T1).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue1;
    public T2 Value2 => !HasValue2 ? throw new InvalidOperationException($"Can't return {typeof(T2).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue2;
    public T3 Value3 => !HasValue3 ? throw new InvalidOperationException($"Can't return {typeof(T3).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue3;
    public T4 Value4 => !HasValue4 ? throw new InvalidOperationException($"Can't return {typeof(T4).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue4;
    public T5 Value5 => !HasValue5 ? throw new InvalidOperationException($"Can't return {typeof(T5).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue5;
    public T6 Value6 => !HasValue6 ? throw new InvalidOperationException($"Can't return {typeof(T6).Simplify()}, " + (Index == 0 ? "Nothing" : $"T{Index}") + " embedded.") : UnionValue6;
    public override string ToString() => $"Tagged<{typeof(E).Simplify()}, {typeof(T1).Simplify()}, {typeof(T2).Simplify()}, {typeof(T3).Simplify()}, {typeof(T4).Simplify()}, {typeof(T5).Simplify()}, {typeof(T6).Simplify()}>[{Value1}, {Value2}, {Value3}, {Value4}, {Value5}, {Value6}]";
  }

  public static partial class Prelude {
    #region Syntactic Sugar
    public static Tagged<E, T1, T2, T3, T4, T5, T6> Tagged<E, T1, T2, T3, T4, T5, T6>(E tag) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5, T6>(tag, 0, default, default, default, default, default, default);
    public static Tagged<E, T1, T2, T3, T4, T5, T6> Tagged<E, T1, T2, T3, T4, T5, T6>(E tag, T1 value) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5, T6>(tag, 1, value, default, default, default, default, default);
    public static Tagged<E, T1, T2, T3, T4, T5, T6> Tagged<E, T1, T2, T3, T4, T5, T6>(E tag, T2 value) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5, T6>(tag, 2, default, value, default, default, default, default);
    public static Tagged<E, T1, T2, T3, T4, T5, T6> Tagged<E, T1, T2, T3, T4, T5, T6>(E tag, T3 value) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5, T6>(tag, 3, default, default, value, default, default, default);
    public static Tagged<E, T1, T2, T3, T4, T5, T6> Tagged<E, T1, T2, T3, T4, T5, T6>(E tag, T4 value) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5, T6>(tag, 4, default, default, default, value, default, default);
    public static Tagged<E, T1, T2, T3, T4, T5, T6> Tagged<E, T1, T2, T3, T4, T5, T6>(E tag, T5 value) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5, T6>(tag, 5, default, default, default, default, value, default);
    public static Tagged<E, T1, T2, T3, T4, T5, T6> Tagged<E, T1, T2, T3, T4, T5, T6>(E tag, T6 value) where E : Enum => new Tagged<E, T1, T2, T3, T4, T5, T6>(tag, 6, default, default, default, default, default, value);
    #endregion

    #region Functor
    public static Tagged<E, R, T2, T3, T4, T5, T6> Map<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<T1, R> f) where E : Enum => Tagged<E, R, T2, T3, T4, T5, T6>(@this.Tag, f(@this.Value1));
    public static Tagged<E, T1, R, T3, T4, T5, T6> Map<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<T2, R> f) where E : Enum => Tagged<E, T1, R, T3, T4, T5, T6>(@this.Tag, f(@this.Value2));
    public static Tagged<E, T1, T2, R, T4, T5, T6> Map<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<T3, R> f) where E : Enum => Tagged<E, T1, T2, R, T4, T5, T6>(@this.Tag, f(@this.Value3));
    public static Tagged<E, T1, T2, T3, R, T5, T6> Map<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<T4, R> f) where E : Enum => Tagged<E, T1, T2, T3, R, T5, T6>(@this.Tag, f(@this.Value4));
    public static Tagged<E, T1, T2, T3, T4, R, T6> Map<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<T5, R> f) where E : Enum => Tagged<E, T1, T2, T3, T4, R, T6>(@this.Tag, f(@this.Value5));
    public static Tagged<E, T1, T2, T3, T4, T5, R> Map<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<T6, R> f) where E : Enum => Tagged<E, T1, T2, T3, T4, T5, R>(@this.Tag, f(@this.Value6));
    #endregion

    #region Monad
    public static Tagged<E, R, T2, T3, T4, T5, T6> FlatMap<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T1), Tagged<E, R, T2, T3, T4, T5, T6>> f) where E : Enum => f((@this.Tag, @this.Value1));
    public static Tagged<E, T1, R, T3, T4, T5, T6> FlatMap<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T2), Tagged<E, T1, R, T3, T4, T5, T6>> f) where E : Enum => f((@this.Tag, @this.Value2));
    public static Tagged<E, T1, T2, R, T4, T5, T6> FlatMap<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T3), Tagged<E, T1, T2, R, T4, T5, T6>> f) where E : Enum => f((@this.Tag, @this.Value3));
    public static Tagged<E, T1, T2, T3, R, T5, T6> FlatMap<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T4), Tagged<E, T1, T2, T3, R, T5, T6>> f) where E : Enum => f((@this.Tag, @this.Value4));
    public static Tagged<E, T1, T2, T3, T4, R, T6> FlatMap<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T5), Tagged<E, T1, T2, T3, T4, R, T6>> f) where E : Enum => f((@this.Tag, @this.Value5));
    public static Tagged<E, T1, T2, T3, T4, T5, R> FlatMap<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T6), Tagged<E, T1, T2, T3, T4, T5, R>> f) where E : Enum => f((@this.Tag, @this.Value6));
    public static Tagged<E, R, T2, T3, T4, T5, T6> Bind<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T1), Tagged<E, R, T2, T3, T4, T5, T6>> f) where E : Enum => f((@this.Tag, @this.Value1));
    public static Tagged<E, T1, R, T3, T4, T5, T6> Bind<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T2), Tagged<E, T1, R, T3, T4, T5, T6>> f) where E : Enum => f((@this.Tag, @this.Value2));
    public static Tagged<E, T1, T2, R, T4, T5, T6> Bind<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T3), Tagged<E, T1, T2, R, T4, T5, T6>> f) where E : Enum => f((@this.Tag, @this.Value3));
    public static Tagged<E, T1, T2, T3, R, T5, T6> Bind<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T4), Tagged<E, T1, T2, T3, R, T5, T6>> f) where E : Enum => f((@this.Tag, @this.Value4));
    public static Tagged<E, T1, T2, T3, T4, R, T6> Bind<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T5), Tagged<E, T1, T2, T3, T4, R, T6>> f) where E : Enum => f((@this.Tag, @this.Value5));
    public static Tagged<E, T1, T2, T3, T4, T5, R> Bind<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T6), Tagged<E, T1, T2, T3, T4, T5, R>> f) where E : Enum => f((@this.Tag, @this.Value6));
    #endregion

    #region Applicative Functor
    public static Tagged<E, R, T2, T3, T4, T5, T6> Apply<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Tagged<E, Func<T1, R>, T2, T3, T4, T5, T6> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, R, T3, T4, T5, T6> Apply<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Tagged<E, T1, Func<T2, R>, T3, T4, T5, T6> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, T2, R, T4, T5, T6> Apply<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Tagged<E, T1, T2, Func<T3, R>, T4, T5, T6> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, T2, T3, R, T5, T6> Apply<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Tagged<E, T1, T2, T3, Func<T4, R>, T5, T6> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, T2, T3, T4, R, T6> Apply<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Tagged<E, T1, T2, T3, T4, Func<T5, R>, T6> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    public static Tagged<E, T1, T2, T3, T4, T5, R> Apply<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Tagged<E, T1, T2, T3, T4, T5, Func<T6, R>> fn) where E : Enum => fn.FlatMap(g => @this.Map(x => g.Item2(x)));
    #endregion

    #region Match
    public static void Match<E, T1, T2, T3, T4, T5, T6>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Action unmatched, params (Func<E, bool> predicate, Action<Tagged<E, T1, T2, T3, T4, T5, T6>> f)[] eval) where E : Enum {
      foreach (var (predicate, f) in eval) {
        if (predicate(@this.Tag)) {
          f(@this);
          return;
        }
      }
      unmatched();
    }

    public static R Match<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<R> unmatched, params (Func<E, bool> predicate, Func<Tagged<E, T1, T2, T3, T4, T5, T6>, R> f)[] eval) where E : Enum {
      foreach (var (predicate, f) in eval) {
        if (predicate(@this.Tag)) {
          return f(@this);
        }
      }
      return unmatched();
    }
    #endregion

    #region DebugPrint
    public static void DebugPrint<E, T1, T2, T3, T4, T5, T6>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, string title = "") where E : Enum => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Linq Conformance
    public static Tagged<E, R, T2, T3, T4, T5, T6> Select<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<T1, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, R, T3, T4, T5, T6> Select<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<T2, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, T2, R, T4, T5, T6> Select<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<T3, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, T2, T3, R, T5, T6> Select<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<T4, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, T2, T3, T4, R, T6> Select<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<T5, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, T1, T2, T3, T4, T5, R> Select<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<T6, R> f) where E : Enum => @this.Map(f);
    public static Tagged<E, R, T2, T3, T4, T5, T6> SelectMany<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T1), Tagged<E, R, T2, T3, T4, T5, T6>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, R, T3, T4, T5, T6> SelectMany<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T2), Tagged<E, T1, R, T3, T4, T5, T6>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, T2, R, T4, T5, T6> SelectMany<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T3), Tagged<E, T1, T2, R, T4, T5, T6>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, T2, T3, R, T5, T6> SelectMany<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T4), Tagged<E, T1, T2, T3, R, T5, T6>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, T2, T3, T4, R, T6> SelectMany<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T5), Tagged<E, T1, T2, T3, T4, R, T6>> f) where E : Enum => @this.FlatMap(f);
    public static Tagged<E, T1, T2, T3, T4, T5, R> SelectMany<E, T1, T2, T3, T4, T5, T6, R>(this Tagged<E, T1, T2, T3, T4, T5, T6> @this, Func<(E, T6), Tagged<E, T1, T2, T3, T4, T5, R>> f) where E : Enum => @this.FlatMap(f);
    #endregion
  }
  #endregion

}

