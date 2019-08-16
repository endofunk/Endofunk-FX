using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Endofunk.FX;
using static Endofunk.FX.Prelude;

namespace Endofunk.Tests {
  public class TestValidation {
   [SetUp] public void Setup() { }

    [Test] public void SyntacticSugar() {
      Assert.AreEqual(Success<string, int>(2), Validation<string, int>.Success(2));
      Assert.AreEqual(Failure<string, int>("Error Text"), Validation<string, int>.Failure(List("Error Text")));
    }

    private static readonly Func<int, int> Increment = x => x + 1;
    private static readonly Func<int, int> Square = x => x * x;

    [Test] public void FunctorIdentityLaw() => Assert.AreEqual(Success<string, int>(2).Map(Id), Success<string, int>(2));
    [Test] public void FunctorCompositionLaw() => Assert.AreEqual(Success<string, int>(2).Map(Increment).Map(Square), Success<string, int>(2).Map(Increment.Compose(Square)));

    private static readonly Func<int, Validation<string, int>> FailureId = x => Success<string, int>(x);

    [Test] public void MonadFailureIdentityLaw() => Assert.AreEqual(Success<string, int>(2).FlatMap(FailureId), FailureId(2));
    [Test] public void MonadSuccessIdentityLaw() => Assert.AreEqual(Success<string, int>(2).FlatMap(Success<string, int>), Success<string, int>(2));

    private static readonly Func<int, Validation<string, int>> IncrementM = x => Success<string, int>(x + 1);
    private static readonly Func<int, Validation<string, int>> SquareM = x => Success<string, int>(x * x);

    [Test] public void MonadAssociativityLaw() => Assert.AreEqual(Success<string, int>(2).FlatMap(IncrementM).FlatMap(SquareM), Success<string, int>(2).FlatMap(x => IncrementM(x).FlatMap(SquareM)));

    private static readonly Func<int, int> Id = x => x;

    [Test] public void ApplicativeIdentityLaw() => Assert.AreEqual(Success<string, int>(2), Success<string, Func<int, int>>(Id).Apply(2));
    [Test] public void ApplicativeHomomorphismLaw() => Assert.AreEqual(Success<string, Func<int, int>>(Increment).Apply(Success<string, int>(2)), Success<string, int>(Increment(2)));

    private static Func<Func<int, int>, int> HOF = f => f(2);
    [Test] public void ApplicativeInterchangeLaw() => Assert.AreEqual(Success<string, Func<int, int>>(Increment).Apply(Success<string, int>(2)), Success<string, Func<Func<int, int>, int>>(HOF).Apply(Increment));

    [Test]
    public void Applicative1stCompositionLaw() {
      var r = Success<string, Func<int, int>>(Square)
        .Apply(Success<string, Func<int, int>>(Increment)
        .Apply(Success<string, int>(2)));
      var l = Compose<int, int, int>()
        .Map(Success<string, Func<int, int>>(Increment))
        .Apply(Success<string, Func<int, int>>(Square))
        .Apply(Success<string, int>(2));
      Assert.AreEqual(l, r);
    }

    [Test]
    public void Applicative2ndCompositionLaw() {
      var r = Success<string, Func<int, int>>(Square)
        .Apply(Success<string, Func<int, int>>(Increment)
        .Apply(Success<string, int>(2)));
      var l = Success<string, Func<Func<int, int>, Func<Func<int, int>, Func<int, int>>>>(Compose<int, int, int>())
        .Apply(Success<string, Func<int, int>>(Increment))
        .Apply(Success<string, Func<int, int>>(Square))
        .Apply(Success<string, int>(2));
      Assert.AreEqual(l, r);
    }
  }
}
