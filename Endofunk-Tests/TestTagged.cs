using System;
using NUnit.Framework;
using Endofunk.FX;
using static Endofunk.FX.Prelude;

namespace Endofunk.Tests {
  public enum Test { One, Two }

  public class TestTagged {
    [SetUp] public void Setup() {}

    [Test] public void SyntacticSugar() {
      Assert.AreEqual(Tagged(Test.One, 1), new Tagged<Test, int>(Test.One, true, 1));
      Assert.AreEqual(Tagged(Test.Two, 2), new Tagged<Test, int>(Test.Two, true, 2));
    }

    private static readonly Func<int, int> Increment = x => x + 1;
    private static readonly Func<int, int> Square = x => x * x;

    [Test] public void FunctorIdentityLaw() => Assert.AreEqual(Tagged(Test.One, 2).Map(Id), Tagged(Test.One, 2));
    [Test] public void FunctorCompositionLaw() => Assert.AreEqual(Tagged(Test.One, 2).Map(Increment).Map(Square), Tagged(Test.One, 2).Map(Increment.Compose(Square)));

    private static readonly Func<(Test, int), Tagged<Test, int>> LeftId = x => Tagged(x.Item1, x.Item2);

    [Test] public void MonadLeftIdentityLaw() => Assert.AreEqual(Tagged(Test.One, 2).FlatMap(LeftId), LeftId((Test.One, 2)));
    [Test] public void MonadRightIdentityLaw() => Assert.AreEqual(Tagged(Test.One, 2).FlatMap(t => Tagged(t.Item1, t.Item2)), Tagged(Test.One, 2));

    private static readonly Func<(Test, int), Tagged<Test, int>> IncrementM = t => Tagged(t.Item1, t.Item2 + 1);
    private static readonly Func<(Test, int), Tagged<Test, int>> SquareM = t => Tagged(t.Item1, t.Item2 * t.Item2);

    [Test] public void MonadAssociativityLaw() => Assert.AreEqual(Tagged(Test.One, 2).FlatMap(IncrementM).FlatMap(SquareM), Tagged(Test.One, 2).FlatMap(x => IncrementM(x).FlatMap(SquareM)));

    private static readonly Func<int, int> Id = x => x;

    [Test] public void ApplicativeIdentityLaw() => Assert.AreEqual(Just(2), Just(Id).Apply(2));
    [Test] public void ApplicativeHomomorphismLaw() => Assert.AreEqual(Just(Increment).Apply(Just(2)), Just(Increment(2)));

    private static Func<Func<int, int>, int> HOF = f => f(2);
    [Test] public void ApplicativeInterchangeLaw() => Assert.AreEqual(Just(Increment).Apply(Just(2)), Just(HOF).Apply(Increment));
  
    [Test] public void Applicative1stCompositionLaw() {
      var r = Just(Square).Apply(Just(Increment).Apply(Just(2)));
      var l = Compose<int, int, int>()
        .Map(Just(Increment))
        .Apply(Just(Square))
        .Apply(Just(2));
      Assert.AreEqual(l, r);
    }

    [Test] public void Applicative2ndCompositionLaw() {
      var r = Just(Square)
        .Apply(Just(Increment)
        .Apply(Just(2)));
      var l = Just(Compose<int, int, int>())
        .Apply(Just(Increment))
        .Apply(Just(Square))
        .Apply(Just(2));
      Assert.AreEqual(l, r);
    }
  }
}