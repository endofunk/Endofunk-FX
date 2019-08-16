// Program.cs
//
// MIT License
// Copyright (c) 2019 endofunk
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using Endofunk.FX;
using static Endofunk.FX.Prelude;
using static System.Console;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;

using System.Runtime.CompilerServices;

namespace FPExamples {

  using IpOctet = ValueTuple<byte, byte, byte, byte>;

  public enum Option { Some, None }

  public enum Either { Left, Right }

  public enum BinaryTree { Empty, Leaf, Node };

  public struct Bicycle {
    public string Make;
    public void RingBell() => WriteLine("Ring, Ring");
  }

  public struct Car {
    public string Make;
    public void Hoot() => WriteLine("Pppaaarrrppp!");
  }

  public partial class Person {
    public readonly string Name;
    public readonly string Surname;
    public readonly bool IsStudent;
    public readonly Subject Subject;
    public Person(string name, string surname, bool isstudent, Subject subject) => (Name, Surname, IsStudent, Subject) = (name, surname, isstudent, subject);
    override public string ToString() => $"Name: {Name}, Surname: {Surname}, IsStudent: {IsStudent}, Subject: {Subject}";
  }

  public enum Subject {
    Mathematics, Science, Engineering, Economics, Medicine
  }

  public struct Subj {
    public const int Mathematics = 0;
    public const int Science = 1;
  }

  public class Point {
    public int X { get; }
    public int Y { get; }
    public Point(int x, int y) => (X, Y) = (x, y);
    public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
  }

  //public enum Shape {
  //  Circle, EquilateralTriangle, Square, Rectangle
  //}

  public enum Direction {
    North = 1, South, East, West, Degree
  }


  public enum IpAddress {
    V4, V6
  }

  class Program {

    
    public static int Increment2(int x) => x + 1;
    public static int Square2(int x) => x + 1;



    public delegate Func<int, int> BinaryDelegate();

    public static int Increment(int x) => x + 1;

    public static Func<int, int> Inc = Increment;

    //public static 

    //private static double Area(EnumUnion<Shape, double> shape) {
    //  switch (shape.Tag) {
    //    case Shape.Circle:
    //      double radius = shape.Value;
    //      return Math.PI * radius * radius;
    //    case Shape.EquilateralTriangle:
    //      double side = shape.Value;
    //      return Math.Sqrt(3) / 4.0 * side * side;
    //    case Shape.Square:
    //      double w = shape.Value;
    //      return w * w;
    //    case Shape.Rectangle:
    //      var (height, width) = ((double, double))shape.Value;
    //      return height * width;
    //    default:
    //      return default;
    //  }
    //}

    //private void PerformAction(object vehicle) => vehicle switch
    //{
    //  Bicycle b => b.RingBell(),
    //  Car c => c.Hoot(),
    //  { } => WriteLine("unknown vehicle")
    //  null => throw new InvalidOperationException($"{typeof(vehicle)}")
    //};

    public static int RectArea(int a, int b) { WriteLine($"{a} + {b} => {a + b}"); return a + b; }
    public static int SquareArea(int a, int b) { WriteLine($"{a} + {b} => {a * b}"); return a * b; }
    public delegate int BinaryOp(int a, int b);

    public static void Process(Tagged<Direction, int> d) => d.Switch(t => t.Tag)
      .Case(Direction.North, _ => WriteLine("North"))
      .Case(Direction.South, _ => WriteLine("South"))
      .Case(Direction.East, _ => WriteLine("East"))
      .Case(Direction.West, _ => WriteLine("West"))
      .Case(Direction.Degree, t => WriteLine($"{t.Value}"))
      .Else(_ => WriteLine("Unmatched"));

    static void Main(string[] args) {

      //var tupleLens = Lens<(string, int), int>.Of(s => s.Item2, (a, s) => (s.Item1, a));
      //Console.WriteLine(tupleLens.Set(3, ("abc", 2)));

      ////var prismSubject = Prism<Person, Maybe<Subject>>.Of(s => { return s.IsStudent == true ? Just(s.Subject) : Nothing<Subject>(); }, (a, s) => s.IsStudent == true ? new Person());

      BinaryOp rectArea = RectArea;
      BinaryOp squareArea = SquareArea;

      BinaryOp comb = rectArea + squareArea;
      //comb += rectArea;
      //comb += squareArea;
      WriteLine("---------------");
      //foreach (BinaryOp f in comb.GetInvocationList()) {
      //  WriteLine(f(3, 4));
      //}
      WriteLine(comb(4, 7));

      WriteLine("---------------");

      //List(1, 2, 3)
        //.Map(x => x + 1)
        //.DebugPrint();

      //Just(1)
      //  .Map(x => x + 2)
      //  .DebugPrint();

      //Right<string, int>(1)because most IDEs can be setup to auto line wrap code for those that prefer it
      //  .Map(x => x + 1)
      //  .DebugPrint();

      //Success<string, int>(1)
      //  .Map(x => x + 1)
      //  .DebugPrint();

      //Of(1)
      //  .Map(x => x + 1)
      //  .DebugPrint();

      //List(1, 2, 3).ToCoyo<List<int>, List<int>>()
      //  .Map(xs => xs.Map(x => x.ToString() + " World!"))
      //  .Run()
      //  .DebugPrint();

      //List(1).ToYoneda<int>()
      //.Map(x => x + 1)
      //.RunIEnumerable()
      //.DebugPrint();


      //Console.WriteLine("Hello World!");

      //var read = IO<string>.Of(() => Console.ReadLine());
      //var write = Fun<string, IO<int>>(v => IO<int>.Of(() => {
      //  Console.WriteLine(v);
      //  return 0;
      //}));

      //var blah1 = from _1 in write("What's your name?")
      //            from name in read
      //            from _2 in write($"Hi {name}, what's your surname?")
      //            from surname in read
      //            select $"{name} {surname}";

      //var blah2 = Fun(() => {
      //  Console.WriteLine("What's your name?");
      //  var name = Console.ReadLine();
      //  Console.WriteLine($"Hi {name}, what's your surname?");
      //  var surname = Console.ReadLine();
      //  return $"{name} {surname}";
      //});

      //Console.WriteLine(blah2());

      //Console.WriteLine(blah1.Compute());

      //var pgm2 = Fun<int, string, int, string, string>((u, v, w, x) => $"{v} {x}")
      //  .LiftM(write("What is your name?"), read, write("what's your surname?"), read);
      //Console.WriteLine(pgm2.Compute());

      //var firstname = "Jack";
      //var lastname = "Sprat";

      //Func<string, string> title = prefix => {
      //  return $"{prefix} {firstname}, {lastname}";
      //};

      //Console.WriteLine(title("Mr")); // "Mr Jack, Sprat"

      string greeting = "";

      Action<string> greet = name => {
        greeting = $"Hi, {name}";
      };

      greet("Brian");
      Console.WriteLine(greeting); // "Hi, Brian"

      //var abs = Math.Abs(Math.Abs(-3));
      //var input = new List<int> { 2, 1, 3, 4, 6, 5 };

      //Func<int, int> id = x => x;

      //var output = input.Map(id);
      //output.DebugPrint();

      IEnumerable<T> AlternationOf<T>(Func<int, T> fn, int n) => Enumerable.Range(n, n).Select(fn);
      //string VowelPattern(int v) => "aeiou".ToCharArray().Map(x => x.ToString()).ToList()[v % 5];
      string BinaryPattern(int v) => (v % 2).ToString();
      IEnumerable<string> BinaryAlternation(int n) => AlternationOf(BinaryPattern, n);
      string BinaryLine(int n) => BinaryAlternation(n).Join(" ");
      string BinaryTriangle(int n) => Enumerable.Range(1, n).Map(BinaryLine).Join("\n");
      string BinarySquare(int n) => Enumerable.Range(1, n).Map(x => BinaryLine(n)).Join("\n");

      WriteLine(BinaryLine(3));
      WriteLine(BinaryTriangle(4));
      WriteLine(BinarySquare(5));

      Measure("Functional1", 10, () => {
        int sum3or5(int a, int e) => (e % 3 == 0 || e % 5 == 0) ? a + e : a;
        return Enumerable.Range(0, 1000)
                         .Aggregate(sum3or5);
      });

      Measure("Functional2", 10, () => {
        return Enumerable.Range(0, 1000)
                         .Aggregate((a, e) => e.IsMultipleOf().ForEither(3, 5) ? a + e : a);
      });

      Measure("Functional3", 10, () => {
        return Enumerable.Range(0, 1000)
                         .Where(e => e.IsMultipleOf().ForEither(3, 5))
                         .Sum();
      });

      Measure("Imperative1", 10, () => {
        var total = 0;
        for (int i = 1; i < 1000; i++) {
          if (i % 3 == 0 || i % 5 == 0) {
            total += i;
          }
        }
        return total;
      });

      Measure("Imperative2", 10, () => {
        var total = 0;
        foreach (var v in Enumerable.Range(0, 1000)) {
          total += (v % 3 == 0 || v % 5 == 0) ? v : 0;
        }
        return total;
      });

      //var t = Try(() => "test")
      //  .Bind(a => Try<string>(() => throw new Exception()))
      //  .Bind(a => Try(a.ToUpper));

      //var numbers = List(List(1), List(2), List(3), List(4), List(5));
      var numbers = List(1, 2, 3, 4, 5);
      //numbers.DebugPrint();

      var right = Right<string, string>("success");
      right.DebugPrint();

      var identity = Of("id1");
      identity.DebugPrint();

      Person num = null;

      var maybe = Just(num);
      maybe.DebugPrint();

      var reader = 2.ToReader<string, int>();
      reader.DebugPrint();

      var validation = Success<string, int>(2);
      validation.DebugPrint();

      var blah = Fun((string a) => WriteLine(a));
      blah("Hello World");

      var is3or5 = Fun((int a) => a.IsMultipleOf().ForEither(3, 5));

      var p = new Point(2, 3);
      var (x1, y1) = p;

      var shoppingList = new[] {
        new { name = "Orange", units = 2.0, price = 10.99, type = "Fruit" },
        new { name = "Lemon", units = 1.0, price = 15.99, type = "Fruit" },
        new { name = "Apple", units = 4.0, price = 15.99, type = "Fruit" },
        new { name = "Fish", units = 1.5, price = 45.99, type = "Meat" },
        new { name = "Pork", units = 1.0, price = 38.99, type = "Meat" },
        new { name = "Lamb", units = 0.75, price = 56.99, type = "Meat" },
        new { name = "Chicken", units = 1.0, price = 35.99, type = "Meat" }
      };

      var ITotal = 0.0; // identity value / seed value
      for (int i = 0; i < shoppingList.Count(); i++) {
        if (shoppingList[i].type == "Fruit") { // predicate / where
          ITotal += shoppingList[i].units * shoppingList[i].price; // reducer / aggregation
        }
      }
      WriteLine($"total = {ITotal}");

      var FTotal1 = shoppingList.Fold(0.0, (a, e) => e.type == "Fruit" ? a + e.units * e.price : a);
      WriteLine($"fruit total = {FTotal1}");

      var FTotal2 = shoppingList.Where(x => x.type == "Fruit").Sum(x => x.units * x.price);
      WriteLine($"fruit total = {FTotal2}");

      var Mathematics = Tagged(Subject.Mathematics, 70);
      var Science = Tagged(Subject.Science, (60, 70, 80));
      var Economics = Tagged(Subject.Economics, (60, 70));
      var Medicine = Tagged(Subject.Medicine, ("ted", 65, 75.5));

      Mathematics
        .Map(x => x * 1.1)
        .DebugPrint();

      Science
        .Map(x => (x.Item1 * 1.1, x.Item2 * 1.2, x.Item3 * 1.3))
        .DebugPrint();

      Economics
        .DebugPrint();

      Economics.Switch(t => t.Tag)
        .Case(Subject.Mathematics, t => WriteLine($"Mathematics: {t}"))
        .Case(Subject.Economics, t => WriteLine($"Economics: {t}"))
        .Case(Subject.Science, t => WriteLine($"Science: {t}"))
        .Else(_ => WriteLine("Default: No Match"));

      var res21 = Mathematics.Switch<Tagged<Subject, int>, Subject, int>(t => t.Tag)
        .Case(Subject.Mathematics, t => { WriteLine($"Mathematics: {t.Value}"); return t.Value; })
        .Case(Subject.Economics, t => { WriteLine($"Economics: {t.Value}"); return t.Value; })
        .Case(Subject.Science, t => { WriteLine($"Science: {t.Value}"); return t.Value; })
        .Else(_ => { WriteLine("Default: No Match"); return default; });
        
      var dirdegree = Tagged(Direction.Degree, 135);

      dirdegree.Switch(t => t.Tag)
        .Case(Direction.Degree, t => WriteLine($"{t.Value}"))
        .Else(_ => WriteLine("Unmatched"));

      List(
        Tagged<Direction, int>(Direction.North),
        Tagged<Direction, int>(Direction.South),
        Tagged<Direction, int>(Direction.East),
        Tagged<Direction, int>(Direction.West),
        Tagged(Direction.Degree, 135)
      ).ForEach(Process);

      var ip = Tagged<IpAddress, IpOctet, string>(IpAddress.V4, (127, 0, 0, 1));
      var ip2 = Tagged<IpAddress, IpOctet, string>(IpAddress.V6, "3456:4455:3445:3455");

      ip2.Switch(t => t.Tag)
        .Case(IpAddress.V4, t => WriteLine($"IP V4 {t.Value1.Item1}, {t.Value1.Item2}, {t.Value1.Item3}, {t.Value1.Item4}"))
        .Case(IpAddress.V6, t=> WriteLine($"IP V6 {t.Value2}"))
        .Else(_ => WriteLine("Unmatched"));

      var numbers2 = List(1, 2, 3, 4, 5);
      //numbers.Map(x => x + 1).DebugPrint();
      numbers.ForEach(Console.WriteLine);

      var strings = List("one", "two", "three", "four");

      strings.Map(Fun((string s) => s.ToUpper()));

      var hdhddh = numbers.Map(x => Just(x).Traverse(y => y.ToResult()));

      //hdhddh
        //.Sequence()
        //.Map(x => x.Sequence())
        //.Match(
        //  failed: e => Console.WriteLine(),
        //  success: x => x.AsEnumerable().ForEach(y => y.DebugPrint())
        //);






    var numbs = List(
        List(1, 2),
        List(2, 3),
        List(3),
        List(4),
        List(5)
      );

      var numbs2 = List(
        List(
          List(Try<int>(() => throw new ArgumentException("test error")))
        ),
        List(
          List(Value(2))
        )
      );

      numbs.DebugPrint();
     
    
      var dgfhs = 2.Pipe(Increment, Increment, Increment, v => $"answer: {v}");
      Console.WriteLine(dgfhs);


      //let example1 = { $0 * 2 } |> until({$0 > 100}) <| 1
      //print(example1) // 128

      var answer = Fun<int, int>(x => x * 2).Pipe(Until<int>(x => x > 100)).Pipe(1);
      Console.WriteLine(answer);

      //var eitherans = Right<string, string>("dsfg");

      //var some = Just(eitherans);

      //var ident = Of(eitherans);

      //var eith = Right<string, Identity<Either<string, string>>>(ident);
   
      //Console.WriteLine(eitherans.ToString());

      //Console.WriteLine(some.ToString());

      //Console.WriteLine(ident.ToString());

      //Console.WriteLine(eith.ToString());

      var numbs3 = List("a", "b", "c");
      var numbs4 = List(List("a"), List("b"), List("c"));

      numbs.DebugPrint();
      numbs2.DebugPrint();
      numbs3.DebugPrint();
      numbs4.DebugPrint();



      //Tagged<Direction, int>(Direction.North).Switch(
      //  () => { },
      //  Eval<Direction, int>(Direction.Degree.Equals().ToPredicate(), t => WriteLine(t.Value)),
      //  Eval<Direction, int>(Direction.North.Equals().ToPredicate(), t => WriteLine("North"))
      //);

      var dir = Tagged<Direction, int>(Direction.Degree, 240);

      dir.Switch(t => t.Tag)
        .Case(Direction.North, _ => WriteLine("North"))
        .Case(Direction.West, _ => WriteLine("West"))
        .Case(t => t.Tag.HasFlag(Direction.Degree) && t.Value > 241, t => WriteLine($"Degree > 241 => {t.Value}"))
        .Case(Direction.Degree, t => WriteLine($"Degree: {t.Value}"))
        .Else(_ => WriteLine("Else no match"));

      dir.Switch(Id)
        .Case(t => t.Tag.HasFlag(Direction.Degree), t => WriteLine($"Degree ===> {t.Value}"))
        .Else(_ => WriteLine("No match found!"));

      var degree = dir.Switch<Tagged<Direction, int>, Direction, int>(t => t.Tag)
        .Case(Direction.North, _ => -1)
        .Case(Direction.West, _ => -1)
        .Case(t => t.Tag.HasFlag(Direction.Degree) && t.Value > 240, t => t.Value)
        .Case(Direction.Degree, t => t.Value + 20)
        .Else(t => -1);

      WriteLine($"Degree ---> {degree}");

      var pers1 = new Person("Jack", "Sprat", true, Subject.Mathematics);

      pers1.Switch(per => per.Name)
        .Case("Jac", per => WriteLine(per.Subject))
        .Case(per => per.Name.Contains("Jaq"), per => WriteLine($"{per.Name}, {per.Surname}"))
        .Case(per => per.Surname.Contains("Sp"), per => WriteLine($"{per.Name}, {per.Surname}, {per.Subject}"))
        .Else(_ => WriteLine("No Match to Person"));

      //Tagged<Option, A> Some<A>(A value) => Tagged<Option, A>(Option.Some, value);
      //Tagged<Option, A> None<A>() => Tagged<Option, A>(Option.None);

      //var opt1 = Some(1);
      //var opt2 = None<int>();

      //opt1.Map(x => x.ToString())
      //    .FlatMap(x => Some(x + " number"))
      //    .Switch(t => t.Tag)
      //    .Case(Option.Some, t => WriteLine($"Some: {t.Value}"))
      //    .Case(Option.None, t => WriteLine("None: "));


      //Tagged<Either, L, R> Left<L, R>(L value) => Tagged<Either, L, R>(Either.Left, value);
      //Tagged<Either, L, R> Right<L, R>(R value) => Tagged<Either, L, R>(Either.Right, value);

      //var eith1 = Left<string, int>("Error");
      //var eith2 = Right<string, int>(2);

      //eith2.Map(Increment)
      //     .FlatMap(x => Right<string, int>(x.Item2 * x.Item2))
      //     .Switch(t => t.Tag)
      //     .Case(Either.Right, t => WriteLine($"Right: {t.Value2}"))
      //     .Case(Either.Left, t => WriteLine($"Left: {t.Value1}"));

      Union<Subject, string> TestUnion(int v) { 
        if (v > 1) { return Subject.Economics; }
        return "LessThanEq 1";
      }

      var unires = TestUnion(2);

      unires
        .Switch(u => u.Index)
        .Case(1, u => WriteLine($"value1: { u.Value1 }"))
        .Case(2, u => WriteLine($"value2: { u.Value2 }"));

      var opt1 = Option<int>
        .Some(new Lazy<int>(() => 2))
        .Map(x => {
          WriteLine("Increment");
          return x + 1;
        })
        .Map(x => {
          WriteLine("Square");
          return x * x;
        });

      //_ = opt1.Value;


      //var mayb1 = Just(1).Lazy().Map(x => {
      //  Console.WriteLine("LazyMap");
      //  return x + 1;
      //}).Map(x => x * x);

      var mayb2 = Maybe<int>.Just(1).Map(x => x + 1);

      Func<int, int> incr2() => x => x + 2;
      
      Func<int, Maybe<int>> incr2m() => x => Maybe<int>.Just(x + 2);
      
      var mayb2b = incr2().Map(mayb2).FlatMap(x => incr2m()(x));

      Console.WriteLine($"{mayb2b}");



    }
  }
}
