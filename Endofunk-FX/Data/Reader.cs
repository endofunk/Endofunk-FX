// Reader.cs
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

  #region Reader Datatype
  [DataContract] public sealed class Reader<Env, A> : IEquatable<Reader<Env, A>> {
    [DataMember] internal readonly Func<Env, A> Value;
    public Func<Env, A> RunReader => Value;
    private Reader(Func<Env, A> value) => (Value) = (value);
    public static Reader<Env, A> Of(Func<Env, A> f) => new Reader<Env, A>(f);
    public A Run(Env environment) => Value(environment);
    public static Reader<Env, A> Pure(A a) => Of(Const<A, Env>(a));
    public static implicit operator Reader<Env, A>(A value) => Pure(value);
    public override string ToString() => $"{this.GetType().Simplify()}[{Value.GetType().Simplify()}]";

    public bool Equals(Reader<Env, A> other) {
      return Value.Equals(other.Value);
    }

    public override bool Equals(object obj) {
      if (obj == null) return false;
      var objType = obj.GetType();
      var isStruct = objType.IsValueType && !objType.IsEnum;
      if (obj is Reader<Env, A> && isStruct) {
        return Equals((Reader<Env, A>)obj);
      }
      return false;
    }

    public static bool operator ==(Reader<Env, A> @this, Reader<Env, A> other) => @this.Equals(other);
    public static bool operator !=(Reader<Env, A> @this, Reader<Env, A> other) => !(@this == other);
    public override int GetHashCode() => Value.GetHashCode();
  }
  #endregion

  public static partial class ReaderExtensions {
    #region Functor
    public static Reader<Env, B> Map<Env, A, B>(this Reader<Env, A> @this, Func<A, B> f) => Reader<Env, B>.Of(@this.RunReader.Compose(f));
    public static Reader<Env, B> Map<Env, A, B>(this Func<A, B> f, Reader<Env, A> @this) => Reader<Env, B>.Of(@this.RunReader.Compose(f));
    #endregion

    #region Monad
    public static Reader<Env, B> FlatMap<Env, A, B>(this Reader<Env, A> @this, Func<A, Reader<Env, B>> f) => Reader<Env, B>.Of(e => f(@this.RunReader(e)).RunReader(e));
    public static Reader<Env, B> FlatMap<Env, A, B>(this Func<A, Reader<Env, B>> f, Reader<Env, A> @this) => @this.FlatMap(f);
    public static Func<Reader<Env, A>, Reader<Env, B>> FlatMap<Env, A, B>(this Func<A, Reader<Env, B>> f) => a => a.FlatMap(f);
    #endregion

    #region Kleisli Composition
    public static Func<A, Reader<R, C>> Compose<R, A, B, C>(this Func<A, Reader<R, B>> f, Func<B, Reader<R, C>> g) => f.Compose(g.FlatMap());
    #endregion

    #region Monad - Lift a function & actions
    public static Reader<Env, R> LiftM<Env, A, B, R>(this Func<A, B, R> @this, Reader<Env, A> a, Reader<Env, B> b) => a.FlatMap(m1 => b.FlatMap(m2 => Reader<Env, R>.Pure(@this(m1, m2))));
    public static Reader<Env, R> LiftM<Env, A, B, C, R>(this Func<A, B, C, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => Reader<Env, R>.Pure(@this(xa, xb, xc)))));
    public static Reader<Env, R> LiftM<Env, A, B, C, D, R>(this Func<A, B, C, D, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => Reader<Env, R>.Pure(@this(xa, xb, xc, xd))))));
    public static Reader<Env, R> LiftM<Env, A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d, Reader<Env, E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => Reader<Env, R>.Pure(@this(xa, xb, xc, xd, xe)))))));
    public static Reader<Env, R> LiftM<Env, A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d, Reader<Env, E> e, Reader<Env, F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => Reader<Env, R>.Pure(@this(xa, xb, xc, xd, xe, xf))))))));
    public static Reader<Env, R> LiftM<Env, A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d, Reader<Env, E> e, Reader<Env, F> f, Reader<Env, G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => Reader<Env, R>.Pure(@this(xa, xb, xc, xd, xe, xf, xg)))))))));
    public static Reader<Env, R> LiftM<Env, A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d, Reader<Env, E> e, Reader<Env, F> f, Reader<Env, G> g, Reader<Env, H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => Reader<Env, R>.Pure(@this(xa, xb, xc, xd, xe, xf, xg, xh))))))))));
    public static Reader<Env, R> LiftM<Env, A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d, Reader<Env, E> e, Reader<Env, F> f, Reader<Env, G> g, Reader<Env, H> h, Reader<Env, I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => Reader<Env, R>.Pure(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi)))))))))));
    public static Reader<Env, R> LiftM<Env, A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d, Reader<Env, E> e, Reader<Env, F> f, Reader<Env, G> g, Reader<Env, H> h, Reader<Env, I> i, Reader<Env, J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => Reader<Env, R>.Pure(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj))))))))))));
    #endregion

    #region Applicative Functor
    public static Reader<Env, B> Apply<Env, A, B>(this Reader<Env, A> @this, Reader<Env, Func<A, B>> f) => f.FlatMap(g => @this.Map(g));
    public static Reader<Env, B> Apply<Env, A, B>(this Reader<Env, Func<A, B>> f, Reader<Env, A> @this) => f.FlatMap(g => @this.Map(g));
    #endregion

    #region Applicative Functor - Lift a function & actions
    public static Reader<Env, R> LiftA<Env, A, R>(this Func<A, R> @this, Reader<Env, A> a) => @this.Map(a);
    public static Reader<Env, R> LiftA<Env, A, B, R>(this Func<A, B, R> @this, Reader<Env, A> a, Reader<Env, B> b) => @this.Curry().Map(a).Apply(b);
    public static Reader<Env, R> LiftA<Env, A, B, C, R>(this Func<A, B, C, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c) => @this.Curry().Map(a).Apply(b).Apply(c);
    public static Reader<Env, R> LiftA<Env, A, B, C, D, R>(this Func<A, B, C, D, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d);
    public static Reader<Env, R> LiftA<Env, A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d, Reader<Env, E> e) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static Reader<Env, R> LiftA<Env, A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d, Reader<Env, E> e, Reader<Env, F> f) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static Reader<Env, R> LiftA<Env, A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d, Reader<Env, E> e, Reader<Env, F> f, Reader<Env, G> g) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static Reader<Env, R> LiftA<Env, A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d, Reader<Env, E> e, Reader<Env, F> f, Reader<Env, G> g, Reader<Env, H> h) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static Reader<Env, R> LiftA<Env, A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d, Reader<Env, E> e, Reader<Env, F> f, Reader<Env, G> g, Reader<Env, H> h, Reader<Env, I> i) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static Reader<Env, R> LiftA<Env, A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Reader<Env, A> a, Reader<Env, B> b, Reader<Env, C> c, Reader<Env, D> d, Reader<Env, E> e, Reader<Env, F> f, Reader<Env, G> g, Reader<Env, H> h, Reader<Env, I> i, Reader<Env, J> j) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    #region DebugPrint
    public static void DebugPrint<Env, A>(this Reader<Env, A> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Linq Conformance
    public static Reader<Env, R> Select<Env, A, R>(this Reader<Env, A> @this, Func<A, R> fn) => @this.Map(fn);
    public static Reader<Env, R> SelectMany<Env, A, R>(this Reader<Env, A> @this, Func<A, Reader<Env, R>> fn) => @this.FlatMap(fn);
    public static Reader<Env, R> SelectMany<Env, A, B, R>(this Reader<Env, A> @this, Func<A, Reader<Env, B>> fn, Func<A, B, R> select) => @this.FlatMap(a => fn(a).FlatMap(b => select(a, b).ToReader<Env, R>()));
    #endregion

    #region ToReader
    public static Reader<Env, A> ToReader<Env, A>(this A a) => Reader<Env, A>.Pure(a);
    #endregion
  }

  public static partial class Prelude {
    #region Syntactic Sugar
    public static Func<A, Reader<Env, A>> ToReader<Env, A>() => a => Reader<Env, A>.Pure(a);
    #endregion
  }
}
