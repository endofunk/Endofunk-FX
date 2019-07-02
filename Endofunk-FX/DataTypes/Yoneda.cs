using System;
using System.Collections.Generic;

namespace Endofunk.FX {

  public struct Yoneda<A, B> {
    internal readonly object Functor;
    internal readonly Func<A, B> Transform;
    public Yoneda(object functor, Func<A, B> transform) => (Functor, Transform) = (functor, transform);
  }

  public static partial class Prelude {

    #region Syntactic Sugar
    public static Yoneda<A, A> ToYoneda<A>(this object functor) => new Yoneda<A, A>(functor, Id);
    #endregion

    #region Functor
    public static Yoneda<A, C> Map<A, B, C>(this Yoneda<A, B> @this, Func<B, C> g) => new Yoneda<A, C>(@this.Functor, a => g(@this.Transform(a)));
    #endregion

    #region Linq Conformance
    public static Yoneda<A, C> Select<A, B, C>(this Yoneda<A, B> @this, Func<B, C> f) => @this.Map(f);
    #endregion

    #region Yoneda Run Conformance
    public static Identity<R> RunIdentity<V, R>(this Yoneda<V, R> @this) => ((Identity<V>)@this.Functor).Map(@this.Transform);
    public static Maybe<R> RunMaybe<V, R>(this Yoneda<V, R> @this) => ((Maybe<V>)@this.Functor).Map(@this.Transform);
    public static IEnumerable<R> RunIEnumerable<V, R>(this Yoneda<V, R> @this) => ((IEnumerable<V>)@this.Functor).Map((V v) => @this.Transform(v));
    public static Either<string, R> RunEither<V, R>(this Yoneda<V, R> @this) => ((Either<string, V>)@this.Functor).Map(@this.Transform);
    public static Result<R> RunResult<V, R>(this Yoneda<V, R> @this) => ((Result<V>)@this.Functor).Map(@this.Transform);
    public static Validation<string, R> RunValidation<V, R>(this Yoneda<V, R> @this) => ((Validation<string, V>)@this.Functor).Map(@this.Transform);
    #endregion

  }
}
