// State.cs
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
  public struct State<S, A> {
    public readonly Func<S, (A a, S s)> Run;
    public A Eval(S state) => Run(state).a;
    public S Exec(S state) => Run(state).s;
    public State(Func<S, (A, S)> run) => (Run) = (run);
    public State<S, A> With(Func<S, S> modification) => new State<S, A>(modification.Compose(Run));
    public static State<S, A> Of(Func<S, (A, S)> run) => new State<S, A>(run);
    public override string ToString() => $"{this.GetType().Simplify()}[{Run.GetType().Simplify()}]";
  }

  public static partial class StateExtensions {
    #region Functor
    public static State<S, B> Map<S, A, B>(this State<S, A> @this, Func<A, B> fn) {
      return new State<S, B>(s => {
        var (result, state) = @this.Run(s);
        return (fn(result), state);
      });
    }
    public static State<S, B> Map<S, A, B>(this Func<A, B> fn, State<S, A> @this) => @this.Map(fn);
    #endregion

    #region Monad
    public static State<S, B> FlatMap<S, A, B>(this State<S, A> @this, Func<A, State<S, B>> fn) {
      return new State<S, B>(s => {
        var (result, state) = @this.Run(s);
        return fn(result).Run(state);
      });
    }
    public static State<S, B> FlatMap<S, A, B>(this Func<A, State<S, B>> fn, State<S, A> @this) => @this.FlatMap(fn);
    public static State<S, B> Bind<S, A, B>(this State<S, A> @this, Func<A, State<S, B>> fn) => @this.FlatMap(fn);
    #endregion

    #region Monad - Lift a function & actions
    public static State<S, R> LiftM<S, A, R>(this Func<A, R> @this, State<S, A> a) => a.FlatMap(xa => State<S, R>.Of(s => (@this(xa), s)));
    public static State<S, R> LiftM<S, A, B, R>(this Func<A, B, R> @this, State<S, A> a, State<S, B> b) => a.FlatMap(xa => b.FlatMap(xb => State<S, R>.Of(s => (@this(xa, xb), s))));
    public static State<S, R> LiftM<S, A, B, C, R>(this Func<A, B, C, R> @this, State<S, A> a, State<S, B> b, State<S, C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => State<S, R>.Of(s => (@this(xa, xb, xc), s)))));
    public static State<S, R> LiftM<S, A, B, C, D, R>(this Func<A, B, C, D, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => State<S, R>.Of(s => (@this(xa, xb, xc, xd), s))))));
    public static State<S, R> LiftM<S, A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d, State<S, E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => State<S, R>.Of(s => (@this(xa, xb, xc, xd, xe), s)))))));
    public static State<S, R> LiftM<S, A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d, State<S, E> e, State<S, F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => State<S, R>.Of(s => (@this(xa, xb, xc, xd, xe, xf), s))))))));
    public static State<S, R> LiftM<S, A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d, State<S, E> e, State<S, F> f, State<S, G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => State<S, R>.Of(s => (@this(xa, xb, xc, xd, xe, xf, xg), s)))))))));
    public static State<S, R> LiftM<S, A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d, State<S, E> e, State<S, F> f, State<S, G> g, State<S, H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => State<S, R>.Of(s => (@this(xa, xb, xc, xd, xe, xf, xg, xh), s))))))))));
    public static State<S, R> LiftM<S, A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d, State<S, E> e, State<S, F> f, State<S, G> g, State<S, H> h, State<S, I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => State<S, R>.Of(s => (@this(xa, xb, xc, xd, xe, xf, xg, xh, xi), s)))))))))));
    public static State<S, R> LiftM<S, A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d, State<S, E> e, State<S, F> f, State<S, G> g, State<S, H> h, State<S, I> i, State<S, J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => State<S, R>.Of(s => (@this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj), s))))))))))));
    #endregion

    #region Applicative Functor
    public static State<S, R> Apply<S, A, R>(this State<S, A> @this, State<S, Func<A, R>> fn) => fn.FlatMap(g => @this.Map(x => g(x)));
    public static State<S, R> Apply<S, A, R>(this State<S, Func<A, R>> fn, State<S, A> @this) => @this.Apply(fn);
    #endregion

    #region Applicative Functor - Lift a function & actions
    public static State<S, R> LiftA<S, A, R>(this Func<A, R> @this, State<S, A> a) => @this.Map(a);
    public static State<S, R> LiftA<S, A, B, R>(this Func<A, B, R> @this, State<S, A> a, State<S, B> b) => @this.Curry().Map(a).Apply(b);
    public static State<S, R> LiftA<S, A, B, C, R>(this Func<A, B, C, R> @this, State<S, A> a, State<S, B> b, State<S, C> c) => @this.Curry().Map(a).Apply(b).Apply(c);
    public static State<S, R> LiftA<S, A, B, C, D, R>(this Func<A, B, C, D, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d);
    public static State<S, R> LiftA<S, A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d, State<S, E> e) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static State<S, R> LiftA<S, A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d, State<S, E> e, State<S, F> f) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static State<S, R> LiftA<S, A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d, State<S, E> e, State<S, F> f, State<S, G> g) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static State<S, R> LiftA<S, A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d, State<S, E> e, State<S, F> f, State<S, G> g, State<S, H> h) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static State<S, R> LiftA<S, A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d, State<S, E> e, State<S, F> f, State<S, G> g, State<S, H> h, State<S, I> i) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static State<S, R> LiftA<S, A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, State<S, A> a, State<S, B> b, State<S, C> c, State<S, D> d, State<S, E> e, State<S, F> f, State<S, G> g, State<S, H> h, State<S, I> i, State<S, J> j) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    #region DebugPrint
    public static void DebugPrint<S, A>(this State<S, A> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Linq Conformance
    public static State<S, R> Select<S, A, R>(this State<S, A> @this, Func<A, R> fn) => @this.Map(fn);
    //public static State<S, R> SelectMany<S, A, B, R>(this State<S, A> @this, Func<A, State<S, B>> fn, Func<A, B, R> select) => State<S, R>.Of(s => @this.FlatMap(a => fn(a).FlatMap<S, B, R>(b => State<S, R>.Of(select(a, b)))));
    #endregion

    #region ToState
    public static State<S, A> ToState<S, A>(this Func<S, (A, S)> fn) => State<S, A>.Of(fn);
    #endregion
  }

  public static partial class Prelude {
    #region Syntactic Sugar

    #endregion
  }
}
