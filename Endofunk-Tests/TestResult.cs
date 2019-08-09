using System;
using NUnit.Framework;
using Endofunk.FX;
using static Endofunk.FX.Prelude;
using System.Runtime.ExceptionServices;

namespace Endofunk.Tests {
  public class TestResult {
    [SetUp] public void Setup() { }

    [Test] public void SyntacticSugar() {
      Assert.AreEqual(Value(2), Result<int>.Value(2));
      Assert.AreEqual(Try<int>(() => throw new ArgumentException("arg error")), Result<int>.Try(() => throw new ArgumentException("arg error")));
      Assert.AreEqual(Error<int>(ExceptionDispatchInfo.Capture(new ArgumentException("arg error"))), Result<int>.Error(ExceptionDispatchInfo.Capture(new ArgumentException("arg error"))));
    }

    private static readonly Func<int, int> Increment = x => x + 1;
    private static readonly Func<int, int> Square = x => x * x;

    [Test] public void FunctorIdentityLaw() => Assert.AreEqual(Value(2).Map(Id), Value(2));
    [Test] public void FunctorCompositionLaw() => Assert.AreEqual(Value(2).Map(Increment).Map(Square), Value(2).Map(Increment.Compose(Square)));

    private static readonly Func<int, Result<int>> LeftId = x => Value(x);

    [Test] public void MonadLeftIdentityLaw() => Assert.AreEqual(Value(2).FlatMap(LeftId), LeftId(2));
    [Test] public void MonadRightIdentityLaw() => Assert.AreEqual(Value(2).FlatMap(Value), Value(2));

    private static readonly Func<int, Result<int>> IncrementM = x => Value(x + 1);
    private static readonly Func<int, Result<int>> SquareM = x => Value(x * x);

    [Test] public void MonadAssociativityLaw() => Assert.AreEqual(Value(2).FlatMap(IncrementM).FlatMap(SquareM), Value(2).FlatMap(x => IncrementM(x).FlatMap(SquareM)));

    private static readonly Func<int, int> Id = x => x;

    [Test] public void ApplicativeIdentityLaw() => Assert.AreEqual(Value(2), Value(Id).Apply(2));
    [Test] public void ApplicativeHomomorphismLaw() => Assert.AreEqual(Value(Increment).Apply(Value(2)), Value(Increment(2)));

    private static Func<Func<int, int>, int> HOF = f => f(2);
    [Test] public void ApplicativeInterchangeLaw() => Assert.AreEqual(Value(Increment).Apply(Value(2)), Value(HOF).Apply(Increment));

    [Test]
    public void Applicative1stCompositionLaw() {
      var r = Value(Increment).Apply(Value(Square).Apply(Value(2)));
      var l = Compose<int, int, int>()
        .Map(Value(Increment))
        .Apply(Value(Square))
        .Apply(Value(2));
      Assert.AreEqual(l, r);
    }

    [Test]
    public void Applicative2ndCompositionLaw() {
      var r = Value(Increment)
        .Apply(Value(Square)
        .Apply(Value(2)));
      var l = Value(Compose<int, int, int>())
        .Apply(Value(Increment))
        .Apply(Value(Square))
        .Apply(Value(2));
      Assert.AreEqual(l, r);
    }
  }
}
