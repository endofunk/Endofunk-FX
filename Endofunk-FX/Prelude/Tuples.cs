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
    #region Tuple Constructor 
    public static Func<A, B, (A, B)> Tuple<A, B>() => (a, b) => (a, b);
    public static Func<A, B, C, (A, B, C)> Tuple<A, B, C>() => (a, b, c) => (a, b, c);
    public static Func<A, B, C, D, (A, B, C, D)> Tuple<A, B, C, D>() => (a, b, c, d) => (a, b, c, d);
    public static Func<A, B, C, D, E, (A, B, C, D, E)> Tuple<A, B, C, D, E>() => (a, b, c, d, e) => (a, b, c, d, e);
    public static Func<A, B, C, D, E, F, (A, B, C, D, E, F)> Tuple<A, B, C, D, E, F>() => (a, b, c, d, e, f) => (a, b, c, d, e, f);
    public static Func<A, B, C, D, E, F, G, (A, B, C, D, E, F, G)> Tuple<A, B, C, D, E, F, G>() => (a, b, c, d, e, f, g) => (a, b, c, d, e, f, g);
    #endregion

    #region Tuple First, Second, Third, Fourth
    public static Func<(A a, B b), A> First<A, B>() => t => t.a;
    public static Func<(A a, B b), B> Second<A, B>() => t => t.b;
    public static Func<(A a, B b, C c), A> First<A, B, C>() => t => t.a;
    public static Func<(A a, B b, C c), B> Second<A, B, C>() => t => t.b;
    public static Func<(A a, B b, C c), C> Third<A, B, C>() => t => t.c;
    public static Func<(A a, B b, C c, D d), A> First<A, B, C, D>() => t => t.a;
    public static Func<(A a, B b, C c, D d), B> Second<A, B, C, D>() => t => t.b;
    public static Func<(A a, B b, C c, D d), C> Third<A, B, C, D>() => t => t.c;
    public static Func<(A a, B b, C c, D d), D> Fourth<A, B, C, D>() => t => t.d;
    #endregion
  }

  public static class TupleExtensions {
    #region Tuple Predicates
    public static bool And<A, B>(this (A a, B b) tuple, A a, B b) where A : IEquatable<A> where B : IEquatable<B> => tuple.a.Equals(a) && tuple.b.Equals(b);
    public static bool Or<A, B>(this (A a, B b) tuple, A a, B b) where A : IEquatable<A> where B : IEquatable<B> => tuple.a.Equals(a) || tuple.b.Equals(b);
    public static bool First<A, B>(this (A a, B b) tuple, A a) where A : IEquatable<A> where B : IEquatable<B> => tuple.a.Equals(a);
    public static bool Second<A, B>(this (A a, B b) tuple, B b) where A : IEquatable<A> where B : IEquatable<B> => tuple.b.Equals(b);
    #endregion

    #region Tuple splat
    public static R Apply<A, R>(this ValueTuple<A> t, Func<A, R> f) => f(t.Item1);
    public static R Apply<A, B, R>(this (A a, B b) t, Func<A, B, R> f) => f(t.a, t.b);
    public static R Apply<A, B, C, R>(this (A a, B b, C c) t, Func<A, B, C, R> f) => f(t.a, t.b, t.c);
    public static R Apply<A, B, C, D, R>(this (A a, B b, C c, D d) t, Func<A, B, C, D, R> f) => f(t.a, t.b, t.c, t.d);
    public static R Apply<A, B, C, D, E, R>(this (A a, B b, C c, D d, E e) t, Func<A, B, C, D, E, R> f) => f(t.a, t.b, t.c, t.d, t.e);
    public static R Apply<A, B, C, D, E, F, R>(this (A a, B b, C c, D d, E e, F f) t, Func<A, B, C, D, E, F, R> f) => f(t.a, t.b, t.c, t.d, t.e, t.f);
    public static R Apply<A, B, C, D, E, F, G, R>(this (A a, B b, C c, D d, E e, F f, G g) t, Func<A, B, C, D, E, F, G, R> f) => f(t.a, t.b, t.c, t.d, t.e, t.f, t.g);

    public static Func<(A a, B b), R> ToTuple<A, B, R>(this Func<A, B, R> f) => t => f(t.a, t.b);
    public static Func<(A a, B b, C c), R> ToTuple<A, B, C, R>(this Func<A, B, C, R> f) => t => f(t.a, t.b, t.c);
    public static Func<(A a, B b, C c, D d), R> ToTuple<A, B, C, D, R>(this Func<A, B, C, D, R> f) => t => f(t.a, t.b, t.c, t.d);
    public static Func<(A a, B b, C c, D d, E e), R> ToTuple<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> f) => t => f(t.a, t.b, t.c, t.d, t.e);
    public static Func<(A a, B b, C c, D d, E e, F f), R> ToTuple<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> f) => t => f(t.a, t.b, t.c, t.d, t.e, t.f);
    public static Func<(A a, B b, C c, D d, E e, F f, G g), R> ToTuple<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> f) => t => f(t.a, t.b, t.c, t.d, t.e, t.f, t.g);
    #endregion
  }
}
