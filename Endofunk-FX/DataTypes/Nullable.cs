using System;

namespace Endofunk.FX {
  public static partial class Prelude {

    #region Syntactic Sugar
    public static Maybe<A> ToMaybe<A>(this A? @this) where A : struct => !@this.HasValue ? Nothing<A>() : Just(@this.Value);
    public static A? ToNullable<A>(this Maybe<A> @this) where A : struct => !@this.IsJust ? (A?)null : @this.Value;
    #endregion

  }
}
