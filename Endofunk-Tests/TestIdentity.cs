using System;
using NUnit.Framework;
using Endofunk.FX;
using static Endofunk.FX.Prelude;

namespace Endofunk.Tests {
  public class TestIdentity {
    [SetUp] public void Setup() {}

    [Test] public void SyntacticSugar() {
      Assert.AreEqual(Of(2), Identity<int>.Of(2));
    }

    private static readonly Func<int, int> Increment = x => x + 1;
    private static readonly Func<int, int> Square = x => x * x;

    [Test] public void FunctorIdentityLaw() => Assert.AreEqual(Of(2).Map(Id), Of(2));
    [Test] public void FunctorCompositionLaw() => Assert.AreEqual(Of(2).Map(Increment).Map(Square), Of(2).Map(Increment.Compose(Square)));

    private static readonly Func<int, Identity<int>> LeftId = x => Of(x);

    [Test] public void MonadLeftIdentityLaw() => Assert.AreEqual(Of(2).FlatMap(LeftId), LeftId(2));
    [Test] public void MonadRightIdentityLaw() => Assert.AreEqual(Of(2).FlatMap(Of), Of(2));

    private static readonly Func<int, Identity<int>> IncrementM = x => Of(x + 1);
    private static readonly Func<int, Identity<int>> SquareM = x => Of(x * x);

    [Test] public void MonadAssociativityLaw() => Assert.AreEqual(Of(2).FlatMap(IncrementM).FlatMap(SquareM), Of(2).FlatMap(x => IncrementM(x).FlatMap(SquareM)));

    private static readonly Func<int, int> Id = x => x;

    [Test] public void ApplicativeIdentityLaw() => Assert.AreEqual(Of(2), Of(Id).Apply(2));
    [Test] public void ApplicativeHomomorphismLaw() => Assert.AreEqual(Of(Increment).Apply(Of(2)), Of(Increment(2)));

    private static Func<Func<int, int>, int> HOF = f => f(2);
    [Test] public void ApplicativeInterchangeLaw() => Assert.AreEqual(Of(Increment).Apply(Of(2)), Of(HOF).Apply(Increment));
  
    [Test] public void Applicative1stCompositionLaw() {
      var r = Of(Increment).Apply(Of(Square).Apply(Of(2)));
      var l = Compose<int, int, int>()
        .Map(Of(Increment))
        .Apply(Of(Square))
        .Apply(Of(2));
      Assert.AreEqual(l, r);
    }

    [Test] public void Applicative2ndCompositionLaw() {
      var r = Of(Increment)
        .Apply(Of(Square)
        .Apply(Of(2)));
      var l = Of(Compose<int, int, int>())
        .Apply(Of(Increment))
        .Apply(Of(Square))
        .Apply(Of(2));
      Assert.AreEqual(l, r);
    }
  }
}