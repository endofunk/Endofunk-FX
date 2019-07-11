// Function.cs
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
namespace Endofunk.FX {
  public static partial class Prelude {

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

  }
}
