using System;

namespace Endofunk.FX {
  public sealed class Prism<S, T, A, B> {
    public readonly Func<B, T> Inject;
    public readonly Func<S, Maybe<A>> TryGet;
    private Prism() { }
    private Prism(Func<S, Maybe<A>> tryGet, Func<B, T> inject) => (Inject, TryGet) = (inject, tryGet);
    public static Prism<S, T, A, B> Of(Func<S, Maybe<A>> tryGet, Func<B, T> inject) => new Prism<S, T, A, B>(tryGet, inject);
  }

  public static partial class Prelude {
    #region Over
    public static Func<S, Maybe<T>> Over<S, T, A, B>(this Prism<S, T, A, B> @this, Func<A, B> f) => s => {
      var a = @this.TryGet(s);
      return a.IsJust ? Just(@this.Inject(f(a.Value))) : Nothing<T>();
    };

    public static Func<S, Maybe<T>> Over<S, T, A, B>(this Func<A, B> f, Prism<S, T, A, B> @this) => s => @this.Over(f)(s);
    #endregion

    #region Compose 
    public static Prism<S, T, C, D> Compose<S, T, A, B, C, D>(this Prism<S, T, A, B> @this, Prism<A, B, C, D> other) => Prism<S, T, C, D>.Of(s => @this.TryGet(s).FlatMap(other.TryGet), s => @this.Inject(other.Inject(s)));
    #endregion

    #region Then
    public static Prism<S, T, C, D> Then<S, T, A, B, C, D>(this Prism<S, T, A, B> @this, Prism<A, B, C, D> other) => @this.Compose(other);
    #endregion
  }

  public sealed class Prism<S, A> {
    private Prism<S, S, A, A> prism;
    public Func<A, S> Inject => prism.Inject;
    public Func<S, Maybe<A>> TryGet => prism.TryGet;
    private Prism() { }
    private Prism(Func<S, Maybe<A>> tryGet, Func<A, S> inject) => prism = Prism<S, S, A, A>.Of(tryGet, inject);
    public static Prism<S, A> Of(Func<S, Maybe<A>> tryGet, Func<A, S> inject) => new Prism<S, A>(tryGet, inject);
  }

  public static partial class Prelude {
    #region Over
    public static Func<S, Maybe<S>> Over<S, A>(this Prism<S, A> @this, Func<A, A> f) => s => {
      var a = @this.TryGet(s);
      return a.IsJust ? Just(@this.Inject(f(a.Value))) : Nothing<S>();
    };

    public static Func<S, Maybe<S>> Over<S, A>(this Func<A, A> f, Prism<S, A> @this) => s => @this.Over(f)(s);
    #endregion

    #region Compose 
    public static Prism<S, B> Compose<S, A, B>(this Prism<S, A> @this, Prism<A, B> other) => Prism<S, B>.Of(s => @this.TryGet(s).FlatMap(other.TryGet), s => @this.Inject(other.Inject(s)));
    #endregion

    #region Then
    public static Prism<S, B> Then<S, A, B>(this Prism<S, A> @this, Prism<A, B> other) => @this.Compose(other);
    #endregion

  }
}
