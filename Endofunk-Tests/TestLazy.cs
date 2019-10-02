using System;
using NUnit.Framework;
using Endofunk.FX;
using static Endofunk.FX.Prelude;

namespace Endofunk.Tests {
  public class TestLazy {
    [SetUp] public void Setup() {}

    [Test] public void SyntacticSugar() {
      Assert.AreEqual((new Lazy<int>(2)).Value, Lazy(2).Value);
    }

    private static readonly Func<int, int> Increment = x => x + 1;
    private static readonly Func<int, int> Square = x => x * x;

    [Test] public void FunctorLazyLaw() => Assert.AreEqual(Lazy(2).Map(Id).Value, Lazy(2).Value);
    [Test] public void FunctorCompositionLaw() => Assert.AreEqual(Lazy(2).Map(Increment).Map(Square).Value, Lazy(2).Map(Increment.Compose(Square)).Value);

    private static readonly Func<int, Lazy<int>> LeftId = x => Lazy(x);

    [Test] public void MonadLeftLazyLaw() => Assert.AreEqual(Lazy(2).FlatMap(LeftId).Value, LeftId(2).Value);
    [Test] public void MonadRightLazyLaw() => Assert.AreEqual(Lazy(2).FlatMap(Lazy).Value, Lazy(2).Value);

    private static readonly Func<int, Lazy<int>> IncrementM = x => Lazy(x + 1);
    private static readonly Func<int, Lazy<int>> SquareM = x => Lazy(x * x);

    [Test] public void MonadAssociativityLaw() => Assert.AreEqual(Lazy(2).FlatMap(IncrementM).FlatMap(SquareM).Value, Lazy(2).FlatMap(x => IncrementM(x).FlatMap(SquareM)).Value);

    private static readonly Func<int, int> Id = x => x;

    [Test] public void ApplicativeLazyLaw() => Assert.AreEqual(Lazy(2).Value, Lazy(Id).Apply(Lazy(2)).Value);
    [Test] public void ApplicativeHomomorphismLaw() => Assert.AreEqual(Lazy(Increment).Apply(Lazy(2)).Value, Lazy(Increment(2)).Value);

    private static Func<Func<int, int>, int> HOF = f => f(2);
    [Test] public void ApplicativeInterchangeLaw() => Assert.AreEqual(Lazy(Increment).Apply(Lazy(2)).Value, Lazy(HOF).Apply(Lazy(Increment)).Value);
  
    [Test] public void Applicative1stCompositionLaw() {
      var r = Lazy(Square).Apply(Lazy(Increment).Apply(Lazy(2)));
      var l = Compose<int, int, int>()
        .Map(Lazy(Increment))
        .Apply(Lazy(Square))
        .Apply(Lazy(2));
      Assert.AreEqual(l.Value, r.Value);
    }

    [Test] public void Applicative2ndCompositionLaw() {
      var r = Lazy(Square).Apply(Lazy(Increment).Apply(Lazy(2)));
      var l = Lazy(Compose<int, int, int>())
        .Apply(Lazy(Increment))
        .Apply(Lazy(Square))
        .Apply(Lazy(2));
      Assert.AreEqual(l.Value, r.Value);
    }
  }
}