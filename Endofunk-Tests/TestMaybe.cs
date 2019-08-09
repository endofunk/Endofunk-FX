using System;
using NUnit.Framework;
using Endofunk.FX;
using static Endofunk.FX.Prelude;

namespace Endofunk.Tests {
  public class TestMaybe {
    [SetUp] public void Setup() {}

    [Test] public void SyntacticSugar() {
      Assert.AreEqual(Just(2), Maybe<int>.Just(2));
      Assert.AreEqual(Nothing<int>(), Maybe<int>.Nothing());
    }

    private static readonly Func<int, int> Increment = x => x + 1;
    private static readonly Func<int, int> Square = x => x * x;

    [Test] public void FunctorIdentityLaw() => Assert.AreEqual(Just(2).Map(Id), Just(2));
    [Test] public void FunctorCompositionLaw() => Assert.AreEqual(Just(2).Map(Increment).Map(Square), Just(2).Map(Increment.Compose(Square)));

    private static readonly Func<int, Maybe<int>> LeftId = x => Just(x);

    [Test] public void MonadLeftIdentityLaw() => Assert.AreEqual(Just(2).FlatMap(LeftId), LeftId(2));
    [Test] public void MonadRightIdentityLaw() => Assert.AreEqual(Just(2).FlatMap(Just), Just(2));

    private static readonly Func<int, Maybe<int>> IncrementM = x => Just(x + 1);
    private static readonly Func<int, Maybe<int>> SquareM = x => Just(x * x);

    [Test] public void MonadAssociativityLaw() => Assert.AreEqual(Just(2).FlatMap(IncrementM).FlatMap(SquareM), Just(2).FlatMap(x => IncrementM(x).FlatMap(SquareM)));

    private static readonly Func<int, int> Id = x => x;

    [Test] public void ApplicativeIdentityLaw() => Assert.AreEqual(Just(2), Just(Id).Apply(2));
    [Test] public void ApplicativeHomomorphismLaw() => Assert.AreEqual(Just(Increment).Apply(Just(2)), Just(Increment(2)));

    private static Func<Func<int, int>, int> HOF = f => f(2);
    [Test] public void ApplicativeInterchangeLaw() => Assert.AreEqual(Just(Increment).Apply(Just(2)), Just(HOF).Apply(Increment));
  
    [Test] public void Applicative1stCompositionLaw() {
      var r = Just(Increment).Apply(Just(Square).Apply(Just(2)));
      var l = Compose<int, int, int>()
        .Map(Just(Increment))
        .Apply(Just(Square))
        .Apply(Just(2));
      Assert.AreEqual(l, r);
    }

    [Test] public void Applicative2ndCompositionLaw() {
      var r = Just(Increment)
        .Apply(Just(Square)
        .Apply(Just(2)));
      var l = Just(Compose<int, int, int>())
        .Apply(Just(Increment))
        .Apply(Just(Square))
        .Apply(Just(2));
      Assert.AreEqual(l, r);
    }
  }
}