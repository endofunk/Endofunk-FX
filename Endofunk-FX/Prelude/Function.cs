﻿// Function.cs
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

namespace Endofunk.FX {

  public static partial class Prelude {

    /// <summary>
    /// Takes its (first) two arguments in the reverse order.
    /// flip :: ((a, b) -> c) -> ((b, a) -> c)
    /// </summary>
    public static Func<B, A, C> Flip<A, B, C>(Func<A, B, C> f) => (b, a) => f(a, b);

    /// <summary>
    /// Takes its (first) two arguments in the reverse order.
    /// flip :: ((a, b) -> c) -> ((b, a) -> c)
    /// </summary>
    public static Func<Func<A, B, C>, Func<B, A, C>> Flip<A, B, C>() => f => (b, a) => f(a, b);

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

    /// <summary>
    /// Left-to-right composition of unary `f` on unary `g`.
    /// </summary>
    public static Func<T, T> Compose<T>(Func<T, T> f, Func<T, T> g) => t => g(f(t));

    #region Fix
    public static Func<T, U> Fix<T, U>(Func<Func<T, U>, Func<T, U>> f) => t => f(Fix(f))(t);
    #endregion

    #region Until
    /// <summary>
    /// Recursively applies a endomorphic function to an argument until a given predicate returns true
    /// until:: (a -> Bool) -> (a -> a) -> a -> a
    /// </summary>
    public static Func<Func<A, A>, Func<A, A>> Until<A>(Func<A, bool> predicate) => f => a => predicate(a) ? a : Until(predicate)(f)(f(a));
    #endregion

  }

  public static class FunctionExtensions {
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

    #region UnCurrying
    public static Func<A, B, C> UnCurry<A, B, C>(this Func<A, Func<B, C>> fn) => (a, b) => fn(a)(b);
    public static Func<A, B, C, D> UnCurry<A, B, C, D>(this Func<A, Func<B, Func<C, D>>> fn) => (a, b, c) => fn(a)(b)(c);
    public static Func<A, B, C, D, E> UnCurry<A, B, C, D, E>(this Func<A, Func<B, Func<C, Func<D, E>>>> fn) => (a, b, c, d) => fn(a)(b)(c)(d);
    public static Func<A, B, C, D, E, F> UnCurry<A, B, C, D, E, F>(this Func<A, Func<B, Func<C, Func<D, Func<E, F>>>>> fn) => (a, b, c, d, e) => fn(a)(b)(c)(d)(e);
    public static Func<A, B, C, D, E, F, G> UnCurry<A, B, C, D, E, F, G>(this Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> fn) => (a, b, c, d, e, f) => fn(a)(b)(c)(d)(e)(f);
    public static Func<A, B, C, D, E, F, G, H> UnCurry<A, B, C, D, E, F, G, H>(this Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> fn) => (a, b, c, d, e, f, g) => fn(a)(b)(c)(d)(e)(f)(g);
    public static Func<A, B, C, D, E, F, G, H, I> UnCurry<A, B, C, D, E, F, G, H, I>(this Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> fn) => (a, b, c, d, e, f, g, h) => fn(a)(b)(c)(d)(e)(f)(g)(h);
    public static Func<A, B, C, D, E, F, G, H, I, J> UnCurry<A, B, C, D, E, F, G, H, I, J>(this Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> fn) => (a, b, c, d, e, f, g, h, i) => fn(a)(b)(c)(d)(e)(f)(g)(h)(i);
    public static Func<A, B, C, D, E, F, G, H, I, J, K> UnCurry<A, B, C, D, E, F, G, H, I, J, K>(this Func<A, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> fn) => (a, b, c, d, e, f, g, h, i, j) => fn(a)(b)(c)(d)(e)(f)(g)(h)(i)(j);
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
    /// <summary>
    /// Left-to-right composition of unary `lhs` on unary `rhs`.
    /// This is the function such that `lhs.Compose(rhs)(a)` = `rhs(lhs(a))`.
    /// </summary>
    public static Func<A, C> Compose<A, B, C>(this Func<A, B> lhs, Func<B, C> rhs) => a => rhs(lhs(a));

    /// <summary>
    /// Right-to-left composition of unary `rhs` on unary `lhs`.
    /// This is the function such that `rhs.Compose(lhs)(a)` = `lhs(rhs(a))`.
    /// </summary>
    public static Func<A, C> Compose<A, B, C>(this Func<B, C> lhs, Func<A, B> rhs) => a => lhs(rhs(a));

    /// <summary>
    /// Right-to-left composition of binary `lhs` on unary `rhs`.
    /// This is the function such that `(lhs.Compose(rhs))(a, b)` = `rhs(lhs(a, b))`.
    /// </summary>
    public static Func<A, B, D> Compose<A, B, C, D>(this Func<A, B, C> lhs, Func<C, D> rhs) => (a, b) => rhs(lhs(a, b));

    /// <summary>
    /// Left-to-right composition of binary `rhs` on unary `lhs`.
    /// This is the function such that `(rhs.Compose(lhs))(a, b)` = `lhs(rhs(a, b))`.
    /// </summary>
    public static Func<A, B, D> Compose<A, B, C, D>(this Func<C, D> lhs, Func<A, B, C> rhs) => (a, b) => lhs(rhs(a, b));
    #endregion

    #region Flipped
    /// <summary>
    /// Takes its (first) two arguments in the reverse order.
    /// flip :: ((a, b) -> c) -> ((b, a) -> c)
    /// </summary>
    public static Func<B, A, C> Flip<A, B, C>(this Func<A, B, C> f) => (b, a) => f(a, b);

    /// <summary>
    /// Takes its (first) two arguments in the reverse order.
    /// flip :: (a -> b -> c) -> b -> a -> c
    /// </summary>
    public static Func<B, Func<A, C>> Flip<A, B, C>(this Func<A, Func<B, C>> f) => b => a => f(a)(b);
    #endregion

    #region Pipe
    public static R Pipe<A, R>(this A a, Func<A, R> f) => f(a);
    public static R Pipe<A, R>(this Func<A, R> f, A a) => f(a);
    public static R Pipe<A, B, R>(this A a, Func<A, B> f1, Func<B, R> f2) => f2(f1(a));
    public static R Pipe<A, B, C, R>(this A a, Func<A, B> f1, Func<B, C> f2, Func<C, R> f3) => f3(f2(f1(a)));
    public static R Pipe<A, B, C, D, R>(this A a, Func<A, B> f1, Func<B, C> f2, Func<C, D> f3, Func<D, R> f4) => f4(f3(f2(f1(a))));
    public static R Pipe<A, B, C, D, E, R>(this A a, Func<A, B> f1, Func<B, C> f2, Func<C, D> f3, Func<D, E> f4, Func<E, R> f5) => f5(f4(f3(f2(f1(a)))));
    public static R Pipe<A, B, C, D, E, F, R>(this A a, Func<A, B> f1, Func<B, C> f2, Func<C, D> f3, Func<D, E> f4, Func<E, F> f5, Func<F, R> f6) => f6(f5(f4(f3(f2(f1(a))))));
    public static R Pipe<A, B, C, D, E, F, G, R>(this A a, Func<A, B> f1, Func<B, C> f2, Func<C, D> f3, Func<D, E> f4, Func<E, F> f5, Func<F, G> f6, Func<G, R> f7) => f7(f6(f5(f4(f3(f2(f1(a)))))));
    public static R Pipe<A, B, C, D, E, F, G, H, R>(this A a, Func<A, B> f1, Func<B, C> f2, Func<C, D> f3, Func<D, E> f4, Func<E, F> f5, Func<F, G> f6, Func<G, H> f7, Func<H, R> f8) => f8(f7(f6(f5(f4(f3(f2(f1(a))))))));
    public static R Pipe<A, B, C, D, E, F, G, H, I, R>(this A a, Func<A, B> f1, Func<B, C> f2, Func<C, D> f3, Func<D, E> f4, Func<E, F> f5, Func<F, G> f6, Func<G, H> f7, Func<H, I> f8, Func<I, R> f9) => f9(f8(f7(f6(f5(f4(f3(f2(f1(a)))))))));
    public static R Pipe<A, B, C, D, E, F, G, H, I, J, R>(this A a, Func<A, B> f1, Func<B, C> f2, Func<C, D> f3, Func<D, E> f4, Func<E, F> f5, Func<F, G> f6, Func<G, H> f7, Func<H, I> f8, Func<I, J> f9, Func<J, R> f10) => f10(f9(f8(f7(f6(f5(f4(f3(f2(f1(a))))))))));
    public static R Pipe<A, B, C, D, E, F, G, H, I, J, K, R>(this A a, Func<A, B> f1, Func<B, C> f2, Func<C, D> f3, Func<D, E> f4, Func<E, F> f5, Func<F, G> f6, Func<G, H> f7, Func<H, I> f8, Func<I, J> f9, Func<J, K> f10, Func<K, R> f11) => f11(f10(f9(f8(f7(f6(f5(f4(f3(f2(f1(a)))))))))));
    public static R Pipe<A, B, C, D, E, F, G, H, I, J, K, L, R>(this A a, Func<A, B> f1, Func<B, C> f2, Func<C, D> f3, Func<D, E> f4, Func<E, F> f5, Func<F, G> f6, Func<G, H> f7, Func<H, I> f8, Func<I, J> f9, Func<J, K> f10, Func<K, L> f11, Func<L, R> f12) => f12(f11(f10(f9(f8(f7(f6(f5(f4(f3(f2(f1(a))))))))))));
    public static R Pipe<A, B, C, D, E, F, G, H, I, J, K, L, M, R>(this A a, Func<A, B> f1, Func<B, C> f2, Func<C, D> f3, Func<D, E> f4, Func<E, F> f5, Func<F, G> f6, Func<G, H> f7, Func<H, I> f8, Func<I, J> f9, Func<J, K> f10, Func<K, L> f11, Func<L, M> f12, Func<M, R> f13) => f13(f12(f11(f10(f9(f8(f7(f6(f5(f4(f3(f2(f1(a)))))))))))));
    #endregion
  }


}
