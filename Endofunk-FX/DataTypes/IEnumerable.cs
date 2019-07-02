using System;
using System.Collections.Generic;
using System.Linq;

namespace Endofunk.FX {

  public static partial class Prelude {

    #region Zip
    public static IEnumerable<S4> Zip<S1, S2, S3, S4>(this IEnumerable<S1> s1, IEnumerable<S2> s2, IEnumerable<S3> s3, Func<S1, S2, S3, S4> f) {
      var (a, b, c) = (s1.GetEnumerator(), s2.GetEnumerator(), s3.GetEnumerator());
      while (a.MoveNext() && b.MoveNext() && c.MoveNext()) {
        yield return f(a.Current, b.Current, c.Current);
      }
    }

    public static IEnumerable<S5> Zip<S1, S2, S3, S4, S5>(this IEnumerable<S1> s1, IEnumerable<S2> s2, IEnumerable<S3> s3, IEnumerable<S4> s4, Func<S1, S2, S3, S4, S5> f) {
      var (a, b, c, d) = (s1.GetEnumerator(), s2.GetEnumerator(), s3.GetEnumerator(), s4.GetEnumerator());
      while (a.MoveNext() && b.MoveNext() && c.MoveNext() && d.MoveNext()) {
        yield return f(a.Current, b.Current, c.Current, d.Current);
      }
    }
    #endregion

    #region IEnumerable - Fold
    public static U Fold<T, U>(this IEnumerable<T> ts, U identity, Func<U, T, U> fn) {
      var accumulator = identity;
      foreach (T element in ts) { accumulator = fn(accumulator, element); }
      return accumulator;
    }
    public static Func<IEnumerable<T>, U> Fold<T, U>(U identity, Func<U, T, U> fn) => ts => Prelude.Fold<T, U>(ts, identity, fn);
    #endregion

    #region Functor
    public static IEnumerable<U> Map<T, U>(this Func<T, U> fn, IEnumerable<T> ts) => ts.Map(fn);
    public static IEnumerable<U> Map<T, U>(this IEnumerable<T> ts, Func<T, U> fn) => ts.Fold(new List<U>(), (a, e) => { a.Add(fn(e)); return a; });
    #endregion

    #region Flatten
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> ts) => ts.Fold(new List<T>(), (a, e) => { if (e != null) { a.AddRange(e); }; return a; });
    #endregion

    #region Monad
    public static IEnumerable<U> FlatMap<T, U>(this Func<T, IEnumerable<U>> fn, IEnumerable<T> ts) => ts.FlatMap(fn);
    public static IEnumerable<U> FlatMap<T, U>(this IEnumerable<T> ts, Func<T, IEnumerable<U>> fn) => ts.Map(fn).Flatten();
    public static Func<IEnumerable<A>, IEnumerable<B>> FlatMap<A, B>(this Func<A, IEnumerable<B>> f) => a => a.FlatMap(f);
    public static IEnumerable<U> Bind<T, U>(this IEnumerable<T> ts, Func<T, IEnumerable<U>> fn) => ts.FlatMap(fn);
    #endregion

    #region Kleisli Composition
    public static Func<A, IEnumerable<C>> Compose<A, B, C>(this Func<A, IEnumerable<B>> f, Func<B, IEnumerable<C>> g) => f.Compose(g.FlatMap());
    #endregion

    #region Monad - Lift a function & actions
    public static IEnumerable<R> LiftM<A, B, R>(this Func<A, B, R> @this, IEnumerable<A> a, IEnumerable<B> b) => a.FlatMap(xa => b.FlatMap(xb => List(@this(xa, xb))));
    public static IEnumerable<R> LiftM<A, B, C, R>(this Func<A, B, C, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => List(@this(xa, xb, xc)))));
    public static IEnumerable<R> LiftM<A, B, C, D, R>(this Func<A, B, C, D, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => List(@this(xa, xb, xc, xd))))));
    public static IEnumerable<R> LiftM<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => List(@this(xa, xb, xc, xd, xe)))))));
    public static IEnumerable<R> LiftM<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => List(@this(xa, xb, xc, xd, xe, xf))))))));
    public static IEnumerable<R> LiftM<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => List(@this(xa, xb, xc, xd, xe, xf, xg)))))))));
    public static IEnumerable<R> LiftM<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g, IEnumerable<H> h) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => List(@this(xa, xb, xc, xd, xe, xf, xg, xh))))))))));
    public static IEnumerable<R> LiftM<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g, IEnumerable<H> h, IEnumerable<I> i) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => List(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi)))))))))));
    public static IEnumerable<R> LiftM<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> @this, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g, IEnumerable<H> h, IEnumerable<I> i, IEnumerable<J> j) => a.FlatMap(xa => b.FlatMap(xb => c.FlatMap(xc => d.FlatMap(xd => e.FlatMap(xe => f.FlatMap(xf => g.FlatMap(xg => h.FlatMap(xh => i.FlatMap(xi => j.FlatMap(xj => List(@this(xa, xb, xc, xd, xe, xf, xg, xh, xi, xj))))))))))));
    #endregion

    #region Filterable
    public static IEnumerable<T> Filter<T>(this Func<T, bool> fn, IEnumerable<T> ts) => ts.Filter(fn);
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> ts, Func<T, bool> fn) => ts.Fold(new List<T>(), (a, e) => { if (fn(e)) { a.Add(e); } return a; });
    #endregion

    #region Applicative Functor
    public static IEnumerable<U> Apply<T, U>(this IEnumerable<T> ts, IEnumerable<Func<T, U>> fn) => fn.FlatMap(g => ts.Map(x => g(x)));
    public static IEnumerable<U> Apply<T, U>(this IEnumerable<Func<T, U>> fn, IEnumerable<T> ts) => ts.Apply(fn);
    public static List<T> ToList<T>(this T t) => new List<T> { t };
    #endregion

    #region Applicative Functor - Lift a function & actions
    public static IEnumerable<R> LiftA<A, R>(this Func<A, R> fn, IEnumerable<A> a) => fn.Map(a);
    public static IEnumerable<R> LiftA<A, B, R>(this Func<A, B, R> fn, IEnumerable<A> a, IEnumerable<B> b) => fn.Curry().Map(a).Apply(b);
    public static IEnumerable<R> LiftA<A, B, C, R>(this Func<A, B, C, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c) => fn.Curry().Map(a).Apply(b).Apply(c);
    public static IEnumerable<R> LiftA<A, B, C, D, R>(this Func<A, B, C, D, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d);
    public static IEnumerable<R> LiftA<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e);
    public static IEnumerable<R> LiftA<A, B, C, D, E, F, R>(this Func<A, B, C, D, E, F, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f);
    public static IEnumerable<R> LiftA<A, B, C, D, E, F, G, R>(this Func<A, B, C, D, E, F, G, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g);
    public static IEnumerable<R> LiftA<A, B, C, D, E, F, G, H, R>(this Func<A, B, C, D, E, F, G, H, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g, IEnumerable<H> h) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h);
    public static IEnumerable<R> LiftA<A, B, C, D, E, F, G, H, I, R>(this Func<A, B, C, D, E, F, G, H, I, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g, IEnumerable<H> h, IEnumerable<I> i) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i);
    public static IEnumerable<R> LiftA<A, B, C, D, E, F, G, H, I, J, R>(this Func<A, B, C, D, E, F, G, H, I, J, R> fn, IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c, IEnumerable<D> d, IEnumerable<E> e, IEnumerable<F> f, IEnumerable<G> g, IEnumerable<H> h, IEnumerable<I> i, IEnumerable<J> j) => fn.Curry().Map(a).Apply(b).Apply(c).Apply(d).Apply(e).Apply(f).Apply(g).Apply(h).Apply(i).Apply(j);
    #endregion

    #region Join Recreated
    public static List<A> AddReturn<A>(this List<A> source, A element) {
      source.Add(element);
      return source;
    }

    public static IEnumerable<R> JoinAP<A, B, C, R>(this IEnumerable<A> outer, IEnumerable<B> inner, Func<A, C> outerKey, Func<B, C> innerKey, Func<A, B, R> result) {
      Func<A, B, List<R>> f = (a, b) => outerKey(a).Equals(innerKey(b)) ? List(result(a, b)) : null;
      return outer.Map(f.Curry()).Apply(inner).Flatten();
    }

    public static IEnumerable<R> JoinAP<A, B, R>(this IEnumerable<A> outer, IEnumerable<B> inner, Func<A, B, bool> predicate, Func<A, B, R> result) {
      Func<A, B, List<R>> f = (a, b) => predicate(a, b) ? List(result(a, b)) : null;
      return outer.Map(f.Curry()).Apply(inner).Flatten();
    }
    #endregion

    #region IEnumerable - Collection Helpers (Equivalent to Take, TakeWhile & Skip, SkipWhile)
    public static IEnumerable<A> Take2<A>(this IEnumerable<A> @this, int quantity) => @this.Fold((new List<A>(), 0), (a, e) => (a.Item2 < quantity) ? (a.Item1.AddReturn(e), ++a.Item2) : (a.Item1, ++a.Item2)).Item1;
    public static IEnumerable<A> Skip2<A>(this IEnumerable<A> @this, int quantity) => @this.Fold((new List<A>(), 0), (a, e) => (a.Item2 >= quantity) ? (a.Item1.AddReturn(e), ++a.Item2) : (a.Item1, ++a.Item2)).Item1;
    #endregion

    public static string Join<A>(this IEnumerable<A> @this, string separator) => string.Join(separator, @this);

    #region DebugPrint
    public static void DebugPrint<T>(this IEnumerable<T> ts, string title = "", string delimeter = "") {
      Console.WriteLine("{0}{1}List<{2}>{3}", title, title.IsEmpty() ? "" : " ---> ", typeof(T).Simplify(), ts.Fold($"[{delimeter}", (a, e) => a + $"{e}, {delimeter}").DropLast(delimeter.Length == 0 ? 2 : 3) + $"{delimeter}]");
    }

    public static void DebugPrint<T>(this IEnumerable<IEnumerable<T>> ts, string title = "", string delimeter = "") {
      Console.WriteLine("{0}{1}List<{2}>{3}", title, title.IsEmpty() ? "" : " ---> ", typeof(T).Simplify(), ts.Fold("[", (a, e) => a + delimeter + "[" + e.Fold("", (ai, ei) => ai + $"{ei}, ").DropLast(2) + "], ").DropLast(2) + delimeter + "]");
    }
    #endregion

    #region Syntactic Sugar - Some / None
    public static List<S> List<S>(params S[] value) => value.ToList<S>();
    public static Func<A, List<B>> ToList<A, B>(this Func<A, B> f) => a => List<B>(f(a));
    #endregion

  }
}
