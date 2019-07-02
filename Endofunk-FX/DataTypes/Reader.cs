using System;
using static Endofunk.FX.Prelude;

namespace Endofunk.FX {

  #region Reader Datatype
  public struct Reader<R, A> : IEquatable<Reader<R, A>> {
    internal readonly Func<R, A> Value;
    public Func<R, A> RunReader => Value;
    private Reader(Func<R, A> value) => (Value) = (value);
    public static Reader<R, A> Of(Func<R, A> f) => new Reader<R, A>(f);
    public A Run(R environment) => Value(environment);
    public static Reader<R, A> Pure(A a) => Of(Const<A, R>(a));
    public static implicit operator Reader<R, A>(A value) => Pure(value);
    public override string ToString() => $"Reader<{typeof(R).Simplify()}, {typeof(A).Simplify()}>[{Value.GetType().Simplify()}]";

    public bool Equals(Reader<R, A> other) {
      return Value.Equals(other.Value);
    }

    public override bool Equals(object obj) {
      if (obj == null) return false;
      var objType = obj.GetType();
      var isStruct = objType.IsValueType && !objType.IsEnum;
      if (obj is Reader<R, A> && isStruct) {
        return Equals((Reader<R, A>)obj);
      }
      return false;
    }

    public static bool operator ==(Reader<R, A> @this, Reader<R, A> other) => @this.Equals(other);
    public static bool operator !=(Reader<R, A> @this, Reader<R, A> other) => !(@this == other);
    public override int GetHashCode() => Value.GetHashCode();
  }
  #endregion

  public static partial class Prelude {

    #region Functor
    public static Reader<R, B> Map<R, A, B>(this Reader<R, A> @this, Func<A, B> f) => Reader<R, B>.Of(@this.RunReader.Compose(f));
    public static Reader<R, B> Map<R, A, B>(this Func<A, B> f, Reader<R, A> @this) => Reader<R, B>.Of(@this.RunReader.Compose(f));
    #endregion

    #region Monad
    public static Reader<R, B> FlatMap<R, A, B>(this Reader<R, A> @this, Func<A, Reader<R, B>> f) => Reader<R, B>.Of(e => f(@this.RunReader(e)).RunReader(e));
    public static Reader<R, B> FlatMap<R, A, B>(this Func<A, Reader<R, B>> f, Reader<R, A> @this) => @this.FlatMap(f);
    public static Func<Reader<R, A>, Reader<R, B>> FlatMap<R, A, B>(this Func<A, Reader<R, B>> f) => a => a.FlatMap(f);
    #endregion

    #region Kleisli Composition
    public static Func<A, Reader<R, C>> Compose<R, A, B, C>(this Func<A, Reader<R, B>> f, Func<B, Reader<R, C>> g) => f.Compose(g.FlatMap());
    #endregion

    #region Monad - Lift a function & actions
    public static Reader<S, R> LiftM<S, A, B, R>(this Func<A, B, R> @this, Reader<S, A> a, Reader<S, B> b) => a.FlatMap(m1 => b.FlatMap(m2 => Reader<S, R>.Pure(@this(m1, m2))));
    public static Reader<S, R> LiftM<S, A, B, C, R>(this Func<A, B, C, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => Reader<S, R>.Pure(@this(xa, xb, xc)))));
    public static Reader<S, R> LiftM<S, A, B, C, D, R>(this Func<A, B, C, D, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => Reader<S, R>.Pure(@this(xa, xb, xc, xd))))));
    public static Reader<S, R> LiftM<S, A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d, Reader<S, E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => Reader<S, R>.Pure(@this(xa, xb, xc, xd, xe)))))));
    public static Reader<S, R> LiftM<S, A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d, Reader<S, E> e, Reader<S, F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => Reader<S, R>.Pure(@this(xa, xb, xc, xd, xe, xf))))))));
    public static Reader<S, R> LiftM<S, A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d, Reader<S, E> e, Reader<S, F> f, Reader<S, G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => Reader<S, R>.Pure(@this(xa, xb, xc, xd, xe, xf, xg)))))))));
    public static Reader<S, R> LiftM<S, A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d, Reader<S, E> e, Reader<S, F> f, Reader<S, G> g, Reader<S, H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => Reader<S, R>.Pure(@this(xa, xb, xc, xd, xe, xf, xg, xh))))))))));
    public static Reader<S, R> LiftM<S, A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d, Reader<S, E> e, Reader<S, F> f, Reader<S, G> g, Reader<S, H> h, Reader<S, I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => Reader<S, R>.Pure(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi)))))))))));
    public static Reader<S, R> LiftM<S, A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d, Reader<S, E> e, Reader<S, F> f, Reader<S, G> g, Reader<S, H> h, Reader<S, I> i, Reader<S, J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => Reader<S, R>.Pure(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj))))))))))));
    #endregion

    #region Applicative Functor
    public static Reader<R, B> Apply<R, A, B>(this Reader<R, A> @this, Reader<R, Func<A, B>> f) => f.FlatMap(g => @this.Map(g));
    public static Reader<R, B> Apply<R, A, B>(this Reader<R, Func<A, B>> f, Reader<R, A> @this) => f.FlatMap(g => @this.Map(g));
    #endregion

    #region Applicative Functor - Lift a function & actions
    public static Reader<S, R> LiftA<S, A, R>(this Func<A, R> @this, Reader<S, A> a) => @this.Map(a);
    public static Reader<S, R> LiftA<S, A, B, R>(this Func<A, B, R> @this, Reader<S, A> a, Reader<S, B> b) => @this.Curry().Map(a).Apply(b);
    public static Reader<S, R> LiftA<S, A, B, C, R>(this Func<A, B, C, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c) => @this.Curry().Map(a).Apply(b).Apply(c);
    public static Reader<S, R> LiftA<S, A, B, C, D, R>(this Func<A, B, C, D, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d);
    public static Reader<S, R> LiftA<S, A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d, Reader<S, E> e) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static Reader<S, R> LiftA<S, A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d, Reader<S, E> e, Reader<S, F> f) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static Reader<S, R> LiftA<S, A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d, Reader<S, E> e, Reader<S, F> f, Reader<S, G> g) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static Reader<S, R> LiftA<S, A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d, Reader<S, E> e, Reader<S, F> f, Reader<S, G> g, Reader<S, H> h) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static Reader<S, R> LiftA<S, A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d, Reader<S, E> e, Reader<S, F> f, Reader<S, G> g, Reader<S, H> h, Reader<S, I> i) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static Reader<S, R> LiftA<S, A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, Reader<S, A> a, Reader<S, B> b, Reader<S, C> c, Reader<S, D> d, Reader<S, E> e, Reader<S, F> f, Reader<S, G> g, Reader<S, H> h, Reader<S, I> i, Reader<S, J> j) => @this.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    #region DebugPrint
    public static void DebugPrint<R, A>(this Reader<R, A> @this, string title = "") => Console.WriteLine("{0}{1}{2}", title, title.IsEmpty() ? "" : " ---> ", @this);
    #endregion

    #region Syntactic Sugar - Some / None
    public static Reader<S, A> ToReader<S, A>(this A a) => Reader<S, A>.Pure(a);
    #endregion

    #region Linq Conformance
    public static Reader<S, R> Select<S, A, R>(this Reader<S, A> @this, Func<A, R> fn) => @this.Map(fn);
    public static Reader<S, R> SelectMany<S, A, R>(this Reader<S, A> @this, Func<A, Reader<S, R>> fn) => @this.FlatMap(fn);
    public static Reader<S, R> SelectMany<S, A, B, R>(this Reader<S, A> @this, Func<A, Reader<S, B>> fn, Func<A, B, R> select) => @this.FlatMap(a => fn(a).FlatMap(b => select(a, b).ToReader<S, R>()));
    #endregion

  }
}
