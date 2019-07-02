using System;
using System.Collections.Generic;

namespace Endofunk.FX {

  public struct Coyoneda<V, A, B> {
    public readonly V Value;
    public readonly Func<A, B> Transform;
    public Coyoneda(V value, Func<A, B> transform) => (Value, Transform) = (value, transform);
  }

  public static partial class Prelude {

    #region Syntactic Sugar
    public static Coyoneda<V, B, B> ToCoyo<V, B>(this V value) => new Coyoneda<V, B, B>(value, Id);
    #endregion

    #region Functor
    public static Coyoneda<V, A, C> Map<V, A, B, C>(this Coyoneda<V, A, B> @this, Func<B, C> g) => new Coyoneda<V, A, C>(@this.Value, a => g(@this.Transform(a)));
    #endregion

    #region Linq Conformance
    public static Coyoneda<V, A, C> Select<V, A, B, C>(this Coyoneda<V, A, B> @this, Func<B, C> f) => @this.Map(f);
    #endregion

    #region Coyoneda Run Conformance per Functor Data Type
    public static R Run<V, R>(this Coyoneda<V, V, R> @this) => @this.Transform(@this.Value);
    public static Identity<R> Run<V, R>(this Coyoneda<Identity<V>, V, R> @this) => @this.Value.Map(v => @this.Transform(v));
    public static Maybe<R> Run<V, R>(this Coyoneda<Maybe<V>, V, R> @this) => @this.Value.Map(v => @this.Transform(v));
    public static IEnumerable<R> Run<V, R>(this Coyoneda<IEnumerable<V>, V, R> @this) => @this.Value.Map(v => @this.Transform(v));
    public static Either<string, R> Run<V, R>(this Coyoneda<Either<string, V>, V, R> @this) => @this.Value.Map(v => @this.Transform(v));
    public static Validation<string, R> Run<V, R>(this Coyoneda<Validation<string, V>, V, R> @this) => @this.Value.Map(v => @this.Transform(v));
    public static Result<R> Run<V, R>(this Coyoneda<Result<V>, V, R> @this) => @this.Value.Map(v => @this.Transform(v));
    #endregion

  }
}
