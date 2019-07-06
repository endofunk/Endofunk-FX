using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Console;

namespace Endofunk.FX {
  public enum Unit { } // functional void

  public static partial class Prelude {

    #region Stream
    public static string ReadToEndOfStream(this Stream stream) => new StreamReader(stream).ReadToEnd();
    #endregion

    public static bool IsMultipleOf(this int n, int m) => n % m == 0;
    public static Predicate<int> IsMultipleOf(this int n) => m => n % m == 0;

    #region Boolean Combinators
    public static bool Validate<A>(this A @this, Predicate<A> predicate) => predicate(@this);
    public static A Bool<A>(this A x, A y, bool cond) => cond ? y : x;
    public static Func<A, Func<A, Func<bool, A>>> Bool<A>() => x => y => cond => cond ? y : x;
    public static bool Not(this bool a) => !a;
    public static bool And(this bool a, bool b) => a && b;
    public static bool Or(this bool a, bool b) => a || b;
    public static Predicate<A> ToPredicate<A>(this Func<A, bool> f) => new Predicate<A>(f);
    public static bool ForEither<A>(this Predicate<A> p, params A[] sm) where A : IComparable<A> {
      foreach (var e in sm) {
        if (p(e)) return true;
      }
      return false;
    }
    public static bool ForEither<A>(this Func<A, bool> p, params A[] sm) where A : IComparable<A> => p.ToPredicate().ForEither(sm);
    public static bool ForAll<A>(this Predicate<A> p, params A[] sm) where A : IComparable<A> => Array.TrueForAll(sm, p);
    public static bool ForAll<A>(this Func<A, bool> p, params A[] sm) where A : IComparable<A> => Array.TrueForAll(sm, p.ToPredicate());
    public static Func<A, bool> Equals<A>(this A value) => a => a.Equals(value);
    #endregion

    #region Lift to Func Form
    public static Func<A> Fun<A>(Func<A> fn) => fn;
    public static Func<A, B> Fun<A, B>(Func<A, B> fn) => fn;
    public static Func<A, B, C> Fun<A, B, C>(Func<A, B, C> fn) => fn;
    public static Func<A, B, C, D> Fun<A, B, C, D>(Func<A, B, C, D> fn) => fn;
    public static Func<A, B, C, D, E> Fun<A, B, C, D, E>(Func<A, B, C, D, E> fn) => fn;
    public static Func<A, B, C, D, E, F> Fun<A, B, C, D, E, F>(Func<A, B, C, D, E, F> fn) => fn;
    public static Func<A, B, C, D, E, F, G> Fun<A, B, C, D, E, F, G>(Func<A, B, C, D, E, F, G> fn) => fn;
    public static Func<A, B, C, D, E, F, G, H> Fun<A, B, C, D, E, F, G, H>(Func<A, B, C, D, E, F, G, H> fn) => fn;
    #endregion

    #region Lift Action to Func Form
    public static Func<Unit> Fun(Action fn) => () => { fn(); return default; };
    public static Func<A, Unit> Fun<A>(Action<A> fn) => a => { fn(a); return default; };
    public static Func<A, B, Unit> Fun<A, B>(Action<A, B> fn) => (a, b) => { fn(a, b); return default; };
    public static Func<A, B, C, Unit> Fun<A, B, C>(Action<A, B, C> fn) => (a, b, c) => { fn(a, b, c); return default; };
    public static Func<A, B, C, D, Unit> Fun<A, B, C, D>(Action<A, B, C, D> fn) => (a, b, c, d) => { fn(a, b, c, d); return default; };
    public static Func<A, B, C, D, E, Unit> Fun<A, B, C, D, E>(Action<A, B, C, D, E> fn) => (a, b, c, d, e) => { fn(a, b, c, d, e); return default; };
    public static Func<A, B, C, D, E, F, Unit> Fun<A, B, C, D, E, F>(Action<A, B, C, D, E, F> fn) => (a, b, c, d, e, f) => { fn(a, b, c, d, e, f); return default; };
    public static Func<A, B, C, D, E, F, G, Unit> Fun<A, B, C, D, E, F, G>(Action<A, B, C, D, E, F, G> fn) => (a, b, c, d, e, f, g) => { fn(a, b, c, d, e, f, g); return default; };
    public static Func<A, B, C, D, E, F, G, H, Unit> Fun<A, B, C, D, E, F, G, H>(Action<A, B, C, D, E, F, G, H> fn) => (a, b, c, d, e, f, g, h) => { fn(a, b, c, d, e, f, g, h); return default; };
    #endregion

    #region Lift to Curried Func Form
    public static Func<A, Func<B>> Fun<A, B>(Func<A, Func<B>> fn) => fn;
    public static Func<A, Func<B, C>> Fun<A, B, C>(Func<A, Func<B, C>> fn) => fn;
    public static Func<A, Func<B, Func<C, D>>> Fun<A, B, C, D>(Func<A, Func<B, Func<C, D>>> fn) => fn;
    public static Func<A, Func<B, Func<C, Func<D, E>>>> Fun<A, B, C, D, E>(Func<A, Func<B, Func<C, Func<D, E>>>> fn) => fn;
    public static Func<A, Func<B, Func<C, Func<D, Func<E, F>>>>> Fun<A, B, C, D, E, F>(Func<A, Func<B, Func<C, Func<D, Func<E, F>>>>> fn) => fn;
    public static Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Fun<A, B, C, D, E, F, G>(Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> fn) => fn;
    public static Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Fun<A, B, C, D, E, F, G, H>(Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> fn) => fn;
    #endregion

    #region Currying
    public static Func<A, Func<B, C>> Curry<A, B, C>(this Func<A, B, C> fn) => a => b => fn(a, b);
    public static Func<A, Func<B, Func<C, D>>> Curry<A, B, C, D>(this Func<A, B, C, D> fn) => a => b => c => fn(a, b, c);
    public static Func<A, Func<B, Func<C, Func<D, E>>>> Curry<A, B, C, D, E>(this Func<A, B, C, D, E> fn) => a => b => c => d => fn(a, b, c, d);
    public static Func<A, Func<B, Func<C, Func<D, Func<E, F>>>>> Curry<A, B, C, D, E, F>(this Func<A, B, C, D, E, F> fn) => a => b => c => d => e => fn(a, b, c, d, e);
    public static Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Curry<A, B, C, D, E, F, G>(this Func<A, B, C, D, E, F, G> fn) => a => b => c => d => e => f => fn(a, b, c, d, e, f);
    public static Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Curry<A, B, C, D, E, F, G, H>(this Func<A, B, C, D, E, F, G, H> fn) => a => b => c => d => e => f => g => fn(a, b, c, d, e, f, g);
    public static Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Curry<A, B, C, D, E, F, G, H, I>(this Func<A, B, C, D, E, F, G, H, I> fn) => a => b => c => d => e => f => g => h => fn(a, b, c, d, e, f, g, h);
    public static Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Curry<A, B, C, D, E, F, G, H, I, J>(this Func<A, B, C, D, E, F, G, H, I, J> fn) => a => b => c => d => e => f => g => h => i => fn(a, b, c, d, e, f, g, h, i);
    public static Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Curry<A, B, C, D, E, F, G, H, I, J, K>(this Func<A, B, C, D, E, F, G, H, I, J, K> fn) => a => b => c => d => e => f => g => h => i => j => fn(a, b, c, d, e, f, g, h, i, j);
    #endregion

    #region Partial Application
    public static Func<B, C> Partial<A, B, C>(this Func<A, B, C> fn, A a) => b => fn(a, b);
    public static Func<B, C, D> Partial<A, B, C, D>(this Func<A, B, C, D> fn, A a) => (b, c) => fn(a, b, c);
    public static Func<C, D> Partial<A, B, C, D>(this Func<A, B, C, D> fn, A a, B b) => c => fn(a, b, c);
    public static Func<B, C, D, E> Partial<A, B, C, D, E>(this Func<A, B, C, D, E> fn, A a) => (b, c, d) => fn(a, b, c, d);
    public static Func<C, D, E> Partial<A, B, C, D, E>(this Func<A, B, C, D, E> fn, A a, B b) => (c, d) => fn(a, b, c, d);
    public static Func<D, E> Partial<A, B, C, D, E>(this Func<A, B, C, D, E> fn, A a, B b, C c) => d => fn(a, b, c, d);
    public static Func<B, C, D, E, F> Partial<A, B, C, D, E, F>(this Func<A, B, C, D, E, F> fn, A a) => (b, c, d, e) => fn(a, b, c, d, e);
    public static Func<C, D, E, F> Partial<A, B, C, D, E, F>(this Func<A, B, C, D, E, F> fn, A a, B b) => (c, d, e) => fn(a, b, c, d, e);
    public static Func<D, E, F> Partial<A, B, C, D, E, F>(this Func<A, B, C, D, E, F> fn, A a, B b, C c) => (d, e) => fn(a, b, c, d, e);
    public static Func<E, F> Partial<A, B, C, D, E, F>(this Func<A, B, C, D, E, F> fn, A a, B b, C c, D d) => e => fn(a, b, c, d, e);
    public static Func<B, C, D, E, F, G> Partial<A, B, C, D, E, F, G>(this Func<A, B, C, D, E, F, G> fn, A a) => (b, c, d, e, f) => fn(a, b, c, d, e, f);
    public static Func<C, D, E, F, G> Partial<A, B, C, D, E, F, G>(this Func<A, B, C, D, E, F, G> fn, A a, B b) => (c, d, e, f) => fn(a, b, c, d, e, f);
    public static Func<D, E, F, G> Partial<A, B, C, D, E, F, G>(this Func<A, B, C, D, E, F, G> fn, A a, B b, C c) => (d, e, f) => fn(a, b, c, d, e, f);
    public static Func<E, F, G> Partial<A, B, C, D, E, F, G>(this Func<A, B, C, D, E, F, G> fn, A a, B b, C c, D d) => (e, f) => fn(a, b, c, d, e, f);
    public static Func<F, G> Partial<A, B, C, D, E, F, G>(this Func<A, B, C, D, E, F, G> fn, A a, B b, C c, D d, E e) => f => fn(a, b, c, d, e, f);
    #endregion

    #region Composition
    public static Func<A, C> Compose<A, B, C>(this Func<A, B> lhs, Func<B, C> rhs) => a => rhs(lhs(a));
    #endregion

    #region Basic Combinators
    public static T Id<T>(T t) => t;
    public static Func<U, T> Const<T, U>(T t) => u => t;
    public static Func<T, T> Id<T>() => t => t;
    #endregion

    #region Tuple Predicates
    public static bool And<A, B>(this (A, B) tuple, A a, B b) where A : IEquatable<A> where B : IEquatable<B> => tuple.Item1.Equals(a) && tuple.Item2.Equals(b);
    public static bool Or<A, B>(this (A, B) tuple, A a, B b) where A : IEquatable<A> where B : IEquatable<B> => tuple.Item1.Equals(a) || tuple.Item2.Equals(b);
    public static bool First<A, B>(this (A, B) tuple, A a) where A : IEquatable<A> where B : IEquatable<B> => tuple.Item1.Equals(a);
    public static bool Second<A, B>(this (A, B) tuple, B b) where A : IEquatable<A> where B : IEquatable<B> => tuple.Item2.Equals(b);
    #endregion

    #region Tuple splat
    public static R Apply<A, R>(this ValueTuple<A> t, Func<A, R> f) => f(t.Item1);
    public static R Apply<A, B, R>(this ValueTuple<A, B> t, Func<A, B, R> f) => f(t.Item1, t.Item2);
    public static R Apply<A, B, C, R>(this ValueTuple<A, B, C> t, Func<A, B, C, R> f) => f(t.Item1, t.Item2, t.Item3);
    public static R Apply<A, B, C, D, R>(this ValueTuple<A, B, C, D> t, Func<A, B, C, D, R> f) => f(t.Item1, t.Item2, t.Item3, t.Item4);
    public static R Apply<A, B, C, D, E, R>(this ValueTuple<A, B, C, D, E> t, Func<A, B, C, D, E, R> f) => f(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5);
    public static R Apply<A, B, C, D, E, F, R>(this ValueTuple<A, B, C, D, E, F> t, Func<A, B, C, D, E, F, R> f) => f(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6);
    public static R Apply<A, B, C, D, E, F, G, R>(this ValueTuple<A, B, C, D, E, F, G> t, Func<A, B, C, D, E, F, G, R> f) => f(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7);

    public static Func<(A, B), R> ToTuple<A, B, R>(this Func<A, B, R> f) => (t) => f(t.Item1, t.Item2);
    public static Func<(A, B, C), R> ToTuple<A, B, C, R>(this Func<A, B, C, R> f) => (t) => f(t.Item1, t.Item2, t.Item3);
    public static Func<(A, B, C, D), R> ToTuple<A, B, C, D, R>(this Func<A, B, C, D, R> f) => (t) => f(t.Item1, t.Item2, t.Item3, t.Item4);
    public static Func<(A, B, C, D, E), R> ToTuple<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> f) => (t) => f(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5);
    public static Func<(A, B, C, D, E, F), R> ToTuple<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> f) => (t) => f(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6);
    public static Func<(A, B, C, D, E, F, G), R> ToTuple<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> f) => (t) => f(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7);
    #endregion

    #region Tuple Constructor 
    public static Func<A, B, (A, B)> Tuple<A, B>() => (a, b) => (a, b);
    public static Func<A, B, C, (A, B, C)> Tuple<A, B, C>() => (a, b, c) => (a, b, c);
    public static Func<A, B, C, D, (A, B, C, D)> Tuple<A, B, C, D>() => (a, b, c, d) => (a, b, c, d);
    public static Func<A, B, C, D, E, (A, B, C, D, E)> Tuple<A, B, C, D, E>() => (a, b, c, d, e) => (a, b, c, d, e);
    public static Func<A, B, C, D, E, F, (A, B, C, D, E, F)> Tuple<A, B, C, D, E, F>() => (a, b, c, d, e, f) => (a, b, c, d, e, f);
    public static Func<A, B, C, D, E, F, G, (A, B, C, D, E, F, G)> Tuple<A, B, C, D, E, F, G>() => (a, b, c, d, e, f, g) => (a, b, c, d, e, f, g);
    #endregion

    #region Tuple First, Second, Third, Fourth
    public static Func<(A, B), A> First<A, B>() => t => t.Item1;
    public static Func<(A, B), B> Second<A, B>() => t => t.Item2;
    public static Func<(A, B, C), A> First<A, B, C>() => t => t.Item1;
    public static Func<(A, B, C), B> Second<A, B, C>() => t => t.Item2;
    public static Func<(A, B, C), C> Third<A, B, C>() => t => t.Item3;
    public static Func<(A, B, C, D), A> First<A, B, C, D>() => t => t.Item1;
    public static Func<(A, B, C, D), B> Second<A, B, C, D>() => t => t.Item2;
    public static Func<(A, B, C, D), C> Third<A, B, C, D>() => t => t.Item3;
    public static Func<(A, B, C, D), D> Fourth<A, B, C, D>() => t => t.Item4;
    #endregion

    #region String Methods
    public static string DropLast(this string input, int quantity) {
      if (input.Count() - quantity < 0) { return input; }
      return input.Substring(0, input.Count() - quantity);
    }

    public static string DropFirst(this string input, int quantity) {
      if (input.Count() - quantity < 0) { return input; }
      return input.Substring(quantity);
    }

    public static bool IsEmpty(this string @this) => @this.Length == 0;
    #endregion

    #region Type Methods
    public static string Simplify(this Type type) {
      var result = type.ToString();
      var substitutions = new Dictionary<string, string> {
          {"System.", ""},
          {",", ", "},
          {"[", "<"},
          {"]", ">"},
          {"Int32", "int"},
          {"String", "string"},
          {"Double", "double"},
          {"Float", "float"}
      };
      var regex = new Regex(substitutions.Keys.Select(k => Regex.Escape(k)).Join("|"));
      result = regex.Replace(result, m => substitutions[m.Value]);
      result = Regex.Replace(result, @"Func`\d", "Func");
      result = Regex.IsMatch(result, @"ValueTuple`\d<") ? Regex.Replace(result, @"ValueTuple`\d<", "(").Replace(">", ")") : result;
      return result;
    }
    #endregion

    #region Measure Execution Time in milliseconds
    public static void Measure(string title, Action f) {
      Stopwatch stopwatch = Stopwatch.StartNew();
      f();
      stopwatch.Stop();
      WriteLine("{0} : {1}ms", title, stopwatch.ElapsedMilliseconds);
    }

    public static void Measure<A>(string title, int repeat, Func<A> f) {
      var result = Enumerable.Range(0, repeat).Fold<int, (A, long)>((default, 0L), (a, e) => {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var r = f();
        stopwatch.Stop();
        return (r, a.Item2 + stopwatch.ElapsedMilliseconds);
      });
      WriteLine($"{title} - {result.Item2 / repeat}ms => result: {result.Item1}");
    }
    #endregion

    #region Exception Handling / Logging
    public static string LogDefault(Exception e) => $"message: {e.Message}\ntrace: {e.StackTrace}\n";
    #endregion

    #region With / Then / Select
    public static A With<A>(this A a, Action<A> f) { f(a); return a; }
    public static B Then<A, B>(this A @this, Func<A, B> f) => @this == null ? default : f(@this);
    public static void Then<A>(this A @this, Action<A> f) { if (@this != null) f(@this); }
    #endregion

    #region File / Directory
    public static Result<bool> DeleteFileIfExists(this string filePath) {
      return Try(() => {
        var result = false;
        if (File.Exists(filePath)) {
          File.Delete(filePath);
          result = true;
        }
        return result;
      });
    }

    public static Result<bool> CreateDirectory(this string filePath) {
      return Try(() => {
        var result = false;
        if (!Directory.Exists(filePath)) {
          Directory.CreateDirectory(filePath);
          result = true;
        }
        return result;
      });
    }
    #endregion

    #region DateTime Extension Methods
    public static int DurationInYears(this DateTime from, DateTime to) {
      var years = to.Year - from.Year;
      return (from > to.AddYears(-years)) ? years-- : years;
    }

    public static long ToUnixTimeSeconds(this DateTime from) {
      var timeOffset = new DateTimeOffset(from);
      return timeOffset.ToUnixTimeSeconds();
    }

    public static DateTime UnixTimeSecondsToDateTime(this long from) {
      return DateTimeOffset.FromUnixTimeSeconds(from).DateTime.ToLocalTime();
    }
    #endregion

    #region DebugPrint
    public static void DebugPrint(this string t) => WriteLine(t);
    public static void DebugPrint<A>(A value) => WriteLine(value);
    public static void DebugPrint(string format, params object[] arg) => WriteLine(format, arg);
    public static B DebugPrint<A, B>(A value, B result) {
      WriteLine(value);
      return result;
    }
    #endregion

  }
}
