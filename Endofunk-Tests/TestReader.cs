using System;
using NUnit.Framework;
using Endofunk.FX;
using static Endofunk.FX.Prelude;

namespace Endofunk.Tests {
  public class TestReader {
   [SetUp] public void Setup() { }

    //[Test] public void SyntacticSugar() {
    //  Assert.AreEqual(Right<string, int>(2), Either<string, int>.Right(2));
    //  Assert.AreEqual(Left<string, int>("Error Text"), Either<string, int>.Left("Error Text"));
    //}

    private static readonly Func<int, int> Increment = x => x + 1;
    private static readonly Func<int, int> Square = x => x * x;

    [Test] public void FunctorIdentityLaw() => Assert.AreEqual(Right<string, int>(2).Map(Id), Right<string, int>(2));
    [Test] public void FunctorCompositionLaw() => Assert.AreEqual(Right<string, int>(2).Map(Increment).Map(Square), Right<string, int>(2).Map(Increment.Compose(Square)));

    private static readonly Func<int, Either<string, int>> LeftId = x => Right<string, int>(x);

    [Test] public void MonadLeftIdentityLaw() => Assert.AreEqual(Right<string, int>(2).FlatMap(LeftId), LeftId(2));
    [Test] public void MonadRightIdentityLaw() => Assert.AreEqual(Right<string, int>(2).FlatMap(Right<string, int>), Right<string, int>(2));

    private static readonly Func<int, Either<string, int>> IncrementM = x => Right<string, int>(x + 1);
    private static readonly Func<int, Either<string, int>> SquareM = x => Right<string, int>(x * x);

    [Test] public void MonadAssociativityLaw() => Assert.AreEqual(Right<string, int>(2).FlatMap(IncrementM).FlatMap(SquareM), Right<string, int>(2).FlatMap(x => IncrementM(x).FlatMap(SquareM)));

    private static readonly Func<int, int> Id = x => x;

    [Test] public void ApplicativeIdentityLaw() => Assert.AreEqual(Right<string, int>(2), Right<string, Func<int, int>>(Id).Apply(2));
    [Test] public void ApplicativeHomomorphismLaw() => Assert.AreEqual(Right<string, Func<int, int>>(Increment).Apply(Right<string, int>(2)), Right<string, int>(Increment(2)));

    private static Func<Func<int, int>, int> HOF = f => f(2);
    [Test] public void ApplicativeInterchangeLaw() => Assert.AreEqual(Right<string, Func<int, int>>(Increment).Apply(Right<string, int>(2)), Right<string, Func<Func<int, int>, int>>(HOF).Apply(Increment));

    [Test]
    public void Applicative1stCompositionLaw() {
      var r = Right<string, Func<int, int>>(Increment)
        .Apply(Right<string, Func<int, int>>(Square)
        .Apply(Right<string, int>(2)));
      var l = Compose<int, int, int>()
        .Map(Right<string, Func<int, int>>(Increment))
        .Apply(Right<string, Func<int, int>>(Square))
        .Apply(Right<string, int>(2));
      Assert.AreEqual(l, r);
    }

    [Test]
    public void Applicative2ndCompositionLaw() {
      var r = Right<string, Func<int, int>>(Increment)
        .Apply(Right<string, Func<int, int>>(Square)
        .Apply(Right<string, int>(2)));
      var l = Right<string, Func<Func<int, int>, Func<Func<int, int>, Func<int, int>>>>(Compose<int, int, int>())
        .Apply(Right<string, Func<int, int>>(Increment))
        .Apply(Right<string, Func<int, int>>(Square))
        .Apply(Right<string, int>(2));
      Assert.AreEqual(l, r);
    }
  }
}
