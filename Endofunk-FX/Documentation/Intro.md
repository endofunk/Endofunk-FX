# Introduction

Welcome to the `Endofunk.FX` functional programming language extension for `C#`. 



# Functional Programming

Functional Programming (FP) has for decades been a darling only within computer science circles; prized for its strong ties to mathematical theory and its abstract nature. Yet much of that behind the scenes effort has ultimately led to a greater adoption of FP enabling features in mainstream languages; meaning that for the most part; a FP style of programming can be fairly easily exploited in most mainstream languages. 

> Functional programming is a programming paradigm — a style of building the structure and elements of computer programs — that treats computation as the evaluation of mathematical functions and avoids changing-state and mutable data



## History

The term Functional Programming was first mentioned in P.J Landin's  "The Next 700 Programming Languages" published in 1965.

> The word "denotative" seems more appropriate than nonproeedural, declarative or functional. The antithesis of denotative is "imperative." Effectively "denotative" means "can be mapped into ISWM without using jumping or assignment," given appropriate primitives. 
>
> It follows that `functional programming` has little to do with functional notation. It is a trivial and pointless task to rearrange some piece of symbolism into prefixed operators and heavy bracketing. It is an intellectually demanding activity to characterize some physical or logical system as a set of entities and functional relations among them. 

A more detailed explanation of Functional Programming is covered by John Backus in his 1977 Turing Award lecture:

> "Can Programming Be Liberated From the von Neumann Style? A Functional Style and its Algebra of Programs". 

He defined functional programs as being built up in a hierarchical way by means of "combining forms" that allow an "algebra of programs"; in modern language, this means that functional programs follow the principle of compositionality. Backus's paper ultimately popularized research into functional programming, although it emphasized function-level programming rather than the lambda calculus style which has come to be associated with functional programming.

Obviously FP with its strong tie to mathematics is ultimately a collaboration of the work done by mathematicians like:

- Moses Schönfinkel; known for the invention of [combinatory logic](https://en.wikipedia.org/wiki/Combinatory_logic).
- Haskell Curry; work on [combinatory logic](https://en.wikipedia.org/wiki/Combinatory_logic).
- Alonzo Church; known for the invention of [lambda calculus](https://en.wikipedia.org/wiki/Lambda_calculus).
- [Samuel Eilenberg](https://en.wikipedia.org/wiki/Samuel_Eilenberg) and [Saunders Mac Lane](https://en.wikipedia.org/wiki/Saunders_Mac_Lane); known for the invention of [category theory](https://en.wikipedia.org/wiki/Category_theory).



## Functional Programming Principles

FP is based on some simple principles with wide ranging implications:

- Pure function
- Composition
- Immutability
- Referential transparency
- Functions as first-class entities
- Higher-order functions
- Declarative programming



## Pure Function

In computer programming, a pure function is a function that has the following properties:

- Its return value is the same for the same arguments (no variation with local static variables, non-local variables, mutable reference arguments or input streams from I/O devices).
- Its evaluation has no side effects (no mutation of local static variables, non-local variables, mutable reference arguments or I/O streams).



For example:

```c#
int Increment(int x) => x + 1;
int Add(int x, int y) => x + y;
```



## Composition

The act of putting two functions together to form a third function where the output of one function is the input of the other.

```c#
Func<A, C> Compose<A, B, C>(this Func<A, B> lhs, Func<B, C> rhs) => a => rhs(lhs(a));
Func<int, int> square = x => x * x;
Func<int, int> increment = x => x + 1;
var incrementSquare = increment.Compose(square);
Console.WriteLine(incrementSquare(2)); // 9
```



## Immutability

An immutable object's state cannot change after construction. In other words, the constructor is the only way you can mutate the object's state. If you want to change an immutable object, you don't — you create a new object.

> *Object-oriented programming makes code understandable by encapsulating moving parts. Functional programming makes code understandable by* minimizing *moving parts.*
>
> — Michael Feathers, author of *Working with Legacy Code*

Immutable classes make a host of typically worrisome things go away. The more mutation you have, the more testing that's required to ensure your code behaves as expected. If you minimize the number of places where mutation occurs the smaller space there are for errors to occur and consequently reduce the places to test. 



## Referential Transparency

An expression that can be replaced with its value without changing the behavior of the program is said to be referentially transparent. 

Say we have function greet:

```C#
Action<string> greet = () => "Hello World!";
```

Any invocation of `greet()` can be replaced with `Hello World!` without affecting the behaviour of the program; hence `greet` is a referentially transparent expression.



## Functions as first-class entities

Functions are considered first-class functions when they can be used in the same way as  values and objects :

- Stored in a variable 
- Stored in a property inside an object 
- Stored in a collection type 
- Can be passed as an argument  to a function (see higher-order function)
- Can be returned by a function (see higher-order function)

```C#
// Stored in a variable
Func<int, int, int> add = (a, b) => a + b;
Func<int, int, int> minus = (a, b) => a - b;

// Stored in a property inside an object
class Reducer<S, A> {
  Func<S, A, S> Reduce;
  Reducer(Func<S, A, S> reduce) => Reduce = reduce;
}

// Stored in a collection type
var binaryOps = new List<Func<int, int, int>> { add, minus };
```



## Higher-order function

A function which takes a function as an argument and/or returns a function.

```c#
// Higher order function; binary operation as an argument
C BinaryOp<A, B, C>(A a, B b, Func<A, B, C> f) => f(a, b);

// Higher order functions; return a function
Func<int, int, int> add = (a, b) => BinaryOp(a, b, (x, y) => x + y);
Func<int, int, int> minus = (a, b) => BinaryOp(a, b, (x, y) => x - y);
Func<int, int, int> multiply = (a, b) => BinaryOp(a, b, (x, y) => x * y);
Func<int, int, int> divide = (a, b) => BinaryOp(a, b, (x, y) => x / y);
```



## Declarative programming

Declarative programming is a style of programming in which the programmer defines what needs to be accomplished by a program without detailing its control flow. 

```C#
// Imperatively increment values in a list
List<int> Increment(List<int> nums) {
  var r = List<int> {};
  foreach (var num in nums) {
    result.Append(num + 1);
  }
  return r;
}

var nums = List<int> { 1, 2, 3, 4, 5 };
var result = Increment(nums);

// Declaratively increment values in a list
var nums = List<int> { 1, 2, 3, 4, 5 };
var result = nums.Select(x => x + 1);
```





Series on Functional Programming

Introduction:

Functions

Types and Values

