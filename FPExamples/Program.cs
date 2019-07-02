using System;
using System.Collections.Generic;
using System.Linq;
using Endofunk.FX;

using static Endofunk.FX.Prelude;
using static System.Console;

namespace FPExamples {

  using IpOctet = System.ValueTuple<int, int, int, int>;

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
    North, South, East, West, Degree
  }


  public enum IpAddress {
    V4, V6
  }

  class Program {

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

    public static void Process(Tagged<Direction, int> d) => d.Match(
      unmatched: () => WriteLine("Unmatched"),
      (Direction.North.Equals(), _ => WriteLine("North")),
      (Direction.South.Equals(), _ => WriteLine("South")),
      (Direction.East.Equals(), _ => WriteLine("East")),
      (Direction.West.Equals(), _ => WriteLine("West")),
      (Direction.Degree.Equals(), t => WriteLine($"{t.Value}"))
    );

    static void Main(string[] args) {

      //var tupleLens = Lens<(string, int), int>.Of(s => s.Item2, (a, s) => (s.Item1, a));
      //Console.WriteLine(tupleLens.Set(3, ("abc", 2)));

      ////var prismSubject = Prism<Person, Maybe<Subject>>.Of(s => { return s.IsStudent == true ? Some(s.Subject) : None<Subject>(); }, (a, s) => s.IsStudent == true ? new Person());

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

      List(1, 2, 3)
        .Map(x => x + 1)
        .DebugPrint();

      //Some(1)
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
      numbers.DebugPrint();

      var right = Right<string, string>("success");
      right.DebugPrint();

      var identity = Of("id1");
      identity.DebugPrint();

      Person num = null;

      var maybe = Some(num);
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

      Economics.Match(
        () => WriteLine("Default: No Match"),
        (Subject.Mathematics.Equals(), (a) => WriteLine($"Mathematics: {a}")),
        (Subject.Economics.Equals(), (a) => WriteLine($"Economics: {a}")),
        (Subject.Science.Equals(), (a) => WriteLine($"Science: {a}"))
      );

      var res21 = Mathematics.Match(
        () => { WriteLine("Default: No Match"); return default; },
        (Subject.Mathematics.Equals(), (a) => { WriteLine($"Mathematics: {a}"); return a; }
      ),
        (Subject.Economics.Equals(), (a) => { WriteLine($"Economics: {a}"); return a; }
      ),
        (Subject.Science.Equals(), (a) => { WriteLine($"Science: {a}"); return a; }
      )
      );

      var res24 = Medicine.Match(
        () => { WriteLine("Default: No Match"); return default; },
        (Subject.Mathematics.Equals(), (a) => { WriteLine($"Mathematics: {a}"); return a; }
      ),
        (Subject.Economics.Equals(), (a) => { WriteLine($"Economics: {a}"); return a; }
      ),
        (Subject.Science.Equals(), (a) => { WriteLine($"Science: {a}"); return a; }
      )
      );

      //Mathematics
      //  .Case(Subject.Mathematics, a => WriteLine($"Mathematics: {a}"))
      //  .Case(Subject.Economics, a => WriteLine($"Economics: {a}"))
      //  .Default((u, a) => WriteLine($"Unknown : {u} -> {a}"))
      //  .Run();

      //var testd = When
      //  .Its<EnumUnion<Subject, (int, int)>>(x => WriteLine(x))
      //  .And((EnumUnion<Subject, (int, int)> x) => x.Tag.Equals(Subject.Science));


      var dirdegree = Tagged(Direction.Degree, 135);

      dirdegree.Match(
        unmatched: () => WriteLine("Unmatched"),
        (Direction.Degree.Equals(), t => WriteLine($"{t.Value}"))
      );

      List(
        Tagged<Direction, int>(Direction.North),
        Tagged<Direction, int>(Direction.South),
        Tagged<Direction, int>(Direction.East),
        Tagged<Direction, int>(Direction.West),
        Tagged(Direction.Degree, 135)
      ).ForEach(Process);

      var ip = Tagged<IpAddress, (byte, byte, byte, byte), string>(IpAddress.V4, (127, 0, 0, 1));

      ip.Match(
        unmatched: () => WriteLine("Unmatched"),
        (IpAddress.V4.Equals(), t => WriteLine($"IP V4 {t.Value1.Item1}, {t.Value1.Item2}, {t.Value1.Item3}, {t.Value1.Item4}")),
        (IpAddress.V6.Equals(), t => WriteLine($"IP V6 {t.Value2}"))
      );

     

    }
  }
}
 