// Match.cs
//
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
//
using System;
namespace Endofunk.FX {
  public class MatchVoid<T, U> {
    public readonly T Value;
    public readonly Func<T, U> Predicate;
    private MatchVoid(T value, Func<T, U> predicate) => (Value, Predicate) = (value, predicate);
    public static MatchVoid<T, U> Of(T value, Func<T, U> predicate) => new MatchVoid<T, U>(value, predicate);
  }

  public static class MatchVoidExtensions {
    public static MatchVoid<T, U> Switch<T, U>(this T @this, Func<T, U> predicate) => MatchVoid<T, U>.Of(@this, predicate);

    public static MatchVoid<T, U> Case<T, U>(this MatchVoid<T, U> @this, U @that, Action<T> action) {
      if (@this == null) return @this;
      if (@this.Predicate(@this.Value).Equals(@that)) {
        action(@this.Value);
        return null;
      }
      return @this;
    }

    public static MatchVoid<T, U> Case<T, U>(this MatchVoid<T, U> @this, Predicate<T> condition, Action<T> action) {
      if (@this == null) return @this;
      if (condition(@this.Value)) {
        action(@this.Value);
        return null;
      }
      return @this;
    }

    public static void Else<T, U>(this MatchVoid<T, U> @this, Action<T> action) {
      if (@this != null) action(@this.Value);
    }
  }

  public class MatchValue<T, U, R> {
    public readonly T Value;
    public readonly Func<T, U> Predicate;
    public readonly bool HasResult;
    public readonly R Result;
    private MatchValue(T value, Func<T, U> predicate) => (Value, Predicate, HasResult, Result) = (value, predicate, false, default);
    private MatchValue(T value, Func<T, U> predicate, R result) => (Value, Predicate, HasResult, Result) = (value, predicate, true, result);
    public static MatchValue<T, U, R> Of(T value, Func<T, U> predicate) => new MatchValue<T, U, R>(value, predicate);
    public static MatchValue<T, U, R> Of(T value, Func<T, U> predicate, R result) => new MatchValue<T, U, R>(value, predicate, result);
  }

  public static class MatchValueExtensions {
    public static MatchValue<T, U, R> Switch<T, U, R>(this T @this, Func<T, U> predicate) => MatchValue<T, U, R>.Of(@this, predicate);

    public static MatchValue<T, U, R> Case<T, U, R>(this MatchValue<T, U, R> @this, U @that, Func<T, R> action) {
      if (@this.HasResult) return @this;
      if (@this.Predicate(@this.Value).Equals(@that)) {
        return MatchValue<T, U, R>.Of(@this.Value, @this.Predicate, action(@this.Value));
      }
      return @this;
    }

    public static MatchValue<T, U, R> Case<T, U, R>(this MatchValue<T, U, R> @this, Predicate<T> condition, Func<T, R> action) {
      if (@this.HasResult) return @this;
      if (condition(@this.Value)) {
        return MatchValue<T, U, R>.Of(@this.Value, @this.Predicate, action(@this.Value));
      }
      return @this;
    }

    public static R Else<T, U, R>(this MatchValue<T, U, R> @this, Func<T, R> elseResult) {
      if (@this.HasResult) return @this.Result;
      return elseResult(@this.Value);
    }
  }
}
