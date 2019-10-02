using System;
using System.Collections.Generic;
using System.Linq;

namespace Endofunk.FX {
  public sealed class Lens<S, T, A, B> {
    public readonly Func<B, S, T> Set;
    public readonly Func<S, A> Get;
    private Lens() { }
    private Lens(Func<S, A> get, Func<B, S, T> set) => (Get, Set) = (get, set);
    public static Lens<S, T, A, B> Of(Func<S, A> get, Func<B, S, T> set) => new Lens<S, T, A, B>(get, set);
    public Lens<S, A> ToSimpleLens() => Lens<S, A>.Of((Lens<S, S, A, A>)this);
    public static explicit operator Lens<S, S, A, A>(Lens<S, T, A, B> v) => (Lens<S, S, A, A>)v;
  }

  public static partial class Prelude {
    #region Over
    public static Func<S, T> Over<S, T, A, B>(this Lens<S, T, A, B> @this, Func<A, B> f) => s => @this.Set(f(@this.Get(s)), s);
    public static Func<S, T> Over<S, T, A, B>(this Func<A, B> f, Lens<S, T, A, B> @this) => s => @this.Over(f)(s);
    #endregion

    #region Compose 
    public static Lens<S, T, C, D> Compose<S, T, A, B, C, D>(this Lens<S, T, A, B> @this, Lens<A, B, C, D> other) => Lens<S, T, C, D>.Of(@this.Get.Compose(other.Get), (d, s) => @this.Set(other.Set(d, @this.Get(s)), s));
    #endregion

    #region At
    public static Lens<S, T, List<A>, List<A>> At<S, T, A>(this Lens<S, T, List<A>, List<A>> @this, int index) => Lens<S, T, List<A>, List<A>>.Of(s => List(@this.Get(s)[index]), (e, s) => @this.Set(List(@this.Get(s)[index] = e[0]), s));
    #endregion

    #region Then
    public static Lens<S, T, C, D> Then<S, T, A, B, C, D>(this Lens<S, T, A, B> @this, Lens<A, B, C, D> other) => @this.Compose(other);
    #endregion
  }

  public sealed class Lens<S, A> {
    private readonly Lens<S, S, A, A> lens;
    public Func<A, S, S> Set => lens.Set;
    public Func<S, A> Get => lens.Get;
    private Lens() { }
    private Lens(Func<S, A> get, Func<A, S, S> set) => lens = Lens<S, S, A, A>.Of(get, set);
    private Lens(Lens<S, S, A, A> lens) => this.lens = lens;
    public static Lens<S, A> Of(Func<S, A> get, Func<A, S, S> set) => new Lens<S, A>(get, set);
    public static Lens<S, A> Of(Lens<S, S, A, A> lens) => new Lens<S, A>(lens);
  }

  public static partial class Prelude {
    #region Over
    public static Func<S, S> Over<S, A>(this Lens<S, A> @this, Func<A, A> f) => s => @this.Set(f(@this.Get(s)), s);
    public static Func<S, S> Over<S, A>(this Func<A, A> f, Lens<S, A> @this) => s => @this.Set(f(@this.Get(s)), s);
    #endregion

    #region Compose 
    public static Lens<S, B> Compose<S, A, B>(this Lens<S, A> @this, Lens<A, B> other) => Lens<S, B>.Of(@this.Get.Compose(other.Get), (d, s) => @this.Set(other.Set(d, @this.Get(s)), s));
    #endregion

    #region At
    public static Lens<S, List<A>> At<S, A>(this Lens<S, List<A>> @this, int index) => Lens<S, List<A>>.Of(s => List(@this.Get(s)[index]), (e, s) => @this.Set(List(@this.Get(s)[index] = e[0]), s));
    #endregion

    #region Then
    public static Lens<S, B> Then<S, A, B>(this Lens<S, A> @this, Lens<A, B> other) => @this.Compose(other);
    #endregion
  }
}
