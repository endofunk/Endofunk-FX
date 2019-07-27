// Prelude.cs
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using static System.Console;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Endofunk.FX {
  public enum Unit { } // functional void

  public static partial class Prelude {

    /// <summary>
    /// Left-to-right composition of unary `lhs` on unary `rhs`.
    /// This is the function such that `lhs.Compose(rhs)(a)` = `rhs(lhs(a))`.
    /// </summary>
    public static Func<Func<B, C>, Func<A, B>, Func<A, C>> Compose<A, B, C>() => (lhs, rhs) => rhs.Compose(lhs);

    #region Stream
    public static string ReadToEndOfStream(this Stream stream) => new StreamReader(stream).ReadToEnd();
    #endregion

    #region Repeat, Replicate, Cycle, SplitAt, Span, Break
    /// <summary>
    /// Repeat x is an infinite list, with x the value of every element.
    /// </summary>
    public static IEnumerable<A> Repeat<A>(this A a) {
      while (true) yield return a;
    }

    /// <summary>
    /// Replicate n x is a list of length n with x the value of every element. It is an instance
    /// of the more general genericReplicate, in which n may be of any integral type.
    /// </summary>
    public static Func<int, IEnumerable<A>> Replicate<A>(this A a) => n => a.Repeat().Take(n);

    /// <summary>
    /// Cycle ties a finite list into a circular one, or equivalently, the infinite repetition
    /// of the original list. It is the identity on infinite lists.
    /// </summary>
    public static IEnumerable<A> Cycle<A>(params A[] xs) {
      while (true) {
        foreach (var x in xs) yield return x;
      }
    }

    /// <summary>
    /// SplitAt n xs returns a tuple where first element is xs prefix of length n and second
    /// element is the remainder of the list:
    /// </summary>
    public static (IEnumerable<A>, IEnumerable<A>) SplitAt<A>(this IEnumerable<A> xs, int n) => (xs.Take(n), xs.Skip(n));

    /// <summary>
    /// Span, applied to a predicate p and a list xs, returns a tuple where first element is
    /// longest prefix (possibly empty) of xs of elements that satisfy p and second element
    /// is the remainder of the list.
    /// </summary>
    public static (IEnumerable<A>, IEnumerable<A>) Span<A>(this IEnumerable<A> xs, Func<A, bool> p) => (xs.TakeWhile(p), xs.SkipWhile(p));

    /// <summary>
    /// Break, applied to a predicate p and a list xs, returns a tuple where first element is 
    /// longest prefix (possibly empty) of xs of elements that do not satisfy p and second 
    /// element is the remainder of the list.
    /// </summary>
    public static (IEnumerable<A>, IEnumerable<A>) Break<A>(this IEnumerable<A> xs, Func<A, bool> p) => xs.Span(x => Not(p(x)));

    /// <summary>
    /// Lines breaks a string up into a list of strings at newline characters. 
    /// The resulting strings do not contain newlines.
    /// </summary>
    public static IEnumerable<string> Lines(this string @this) => @this.Split("\n");

    /// <summary>
    /// Words breaks a string up into a list of words, which were delimited by white space.
    /// </summary>
    public static IEnumerable<string> Words(this string @this) => @this.Split();

    /// <summary>
    /// Lines breaks a string up into a list of strings at newline characters. 
    /// The resulting strings do not contain newlines.
    /// </summary>
    public static string UnLines(this IEnumerable<string> @this) => @this.Join("\n");

    /// <summary>
    /// Words breaks a string up into a list of words, which were delimited by white space.
    /// </summary>
    public static string UnWords(this IEnumerable<string> @this) => @this.Join(" ");
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

    #region Basic Combinators
    public static A Id<A>(A a) => a;
    public static Func<A, B, A> Const<A, B>() => (a, b) => a;
    public static Func<B, A> Const<A, B>(A a) => b => a;
    public static Func<A, A> Id<A>() => a => a;
    public static Func<A, Maybe<A>> Id2<A>() => a => a;
    public static Func<A, B, B> Seq<A, B>() => (a, b) => b;
    public static Func<B, B> Seq<A, B>(A a) => b => b;
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
    private static string SimplifyTypeSyntax(this string @this) {
      var substitutions = new Dictionary<string, string> { { ",", ", " }, { "[", "<" }, { "]", ">" } };
      var regex = new Regex(substitutions.Keys.Select(k => Regex.Escape(k)).Join("|"));
      return regex.Replace(Regex.Replace(@this, @"`\d", ""), m => substitutions[m.Value]);
    }

    public static string Simplify(this Type type) => type.ToString().SimplifyTypeSyntax();
    public static string Simplify(this MethodInfo methodInfo) => methodInfo.ToString().SimplifyTypeSyntax();
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

    #region Serialise to JSON
    public static string ToJsonString<A>(this A obj) {
      var stream = new MemoryStream();
      var serializer = new DataContractJsonSerializer(typeof(A));
      serializer.WriteObject(stream, obj);
      byte[] json = stream.ToArray();
      stream.Close();
      return Encoding.UTF8.GetString(json, 0, json.Length);
    }
    #endregion
  }
}
