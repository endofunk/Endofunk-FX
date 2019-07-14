[![Platform](https://img.shields.io/badge/Platforms-Windows%20%7C%20Android%20%7CmacOS%20%7C%20iOS%20%7C%20watchOS%20%7C%20tvOS%20%7C%20Linux-4E4E4E.svg?colorA=28a745)](#Platform-Support)

[![Twitter](https://img.shields.io/badge/Twitter-@codefunctor-blue.svg?style=flat)](http://twitter.com/codefunctor)

![](https://raw.githubusercontent.com/endofunk/Endofunk-FX/master/Images/endofunk.png)

This library provides a functional-programming core library: `Endofunk.FX`, that adds many of the data types needed for exploiting a functional programming style in your codebase; more data types will be added in due course.

# Functional Data Types
The following is an incompete list of the functional data types included in `Endofunc.FX`. 

| Type  | Overview |
|-------|----------|
| Identity | The Identity type is a trivial type to access functor, monad, applicative functor, etc. algebras. |
| Maybe | The Maybe type encapsulates an optional value. A value of type Maybe a either contains a value of type a (represented as Just a), or it is empty (represented as Nothing)|
| Either| The Either type encapsulates a logical disjunction of two possibilities: either Left or Right. |
| Result | The Result type is similar to the Either type except that the left disjunction is fixed to capture of a C# Exception. |
| Validation | The Validation data type is isomorphic to Either, but has an instance of Applicative that accumulates on the error side. |
| Reader | The Reader type (also called the Environment monad). Represents a computation, which can read values from a shared environment, pass values from function to function, and execute sub-computations in a modified environment. |
| State |  The State monad wraps computations in the context of reading and modifying a global state object. |
| Coyoneda | The Coyoneda is a contravariant Functor suitable for Yoneda reduction. |
| Yoneda | The yoneda is a covariant Functor suitable for Yoneda reduction. |
| Tagged | The Tagged union type is comparable to Swift enums with generically associated values. |
| Store | The Store is modelled on the Redux concept. |
| Reducer | The Reducer is modelled on the Redux concept. |
| Subscriber | The Suscriber is modelled on the Redux concept. |
| *Action* | The Tagged type fulfills the role of the Action in the Redux concept. |

All types support `Functor`, `Applicative Functor` and `Monad`; with `monadic lifters`, `applicative lifters`, `Kleisli monadic composition`, `Linq`, `Traverse` and `Sequence`.

Further enhancements and functional syntactic sugar has been incorporated in a `Endofunc.FX.Prelude` static class to simplify general use of the functional data types.


## Nu-get

Nu-get package | Description
---------------|-------------
[Endofunk.FX](https://www.nuget.org/packages/Endofunk.FX/) | All of the core data types and functional 'prelude'. 
[Endofunk.Data](https://www.nuget.org/packages/Endofunk.FX.Data/) | Functional wrapper for SQLClient and SQLite (WIP)
[Endofunk.Net](https://www.nuget.org/packages/Endofunk.FX.Net/) | Functional wrapper for System.Net (WIP)

## License
[License.md](https://github.com/endofunk/Endofunk-FX/blob/master/License.md)

# Future Plans

- The `IO`, `Writer` and `Union` type monads are a work in progress. 
- In addition to this `STAB` and `SA` Optics type covering `Lens` and `Prims` are also a work in progress and will be added shortly.  In future this may transition to `profunctor` optics.
- I intend to explore adding functional extension methods to the C# 8.0 `Nullable` types to turn Nullable into a 1st class functional type similar to `Maybe`, and hopefully to also enabe smooth transforms between `value` and `class` element types.
- Functional concurrency with `Task`, and/or `Promise / Future` types.
- ...

This documentation is a work in progress and will be significantly enhanced as part of the firming up of the API, the data types and the usage examples for each data type. 

Secondly we have a goal to provide existing functional API wrappers frequently used .NET Core functonality;  `Endofunc.Net` and `EndoFunc.Data` are the first two functional API wrapperss for Web, SQLite and SqlClient connections respectively. 

Examples will be added in due course to demonstrate how these APIs can be used to simplify and crash proof your code without sacrificing features.
