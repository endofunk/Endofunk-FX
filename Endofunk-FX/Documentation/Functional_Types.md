# Functional Programming Types
In FP its common to lift an element type and/or computation into a data type like `List` or `Maybe` because that enables us to use a common functional API to transform our element type and/or bind computations.

## Common API

Monoid Algebra

Functor Algebra

Monad Algebra

Applicative Functor Algebra

Traversable Functor Algebra

# Monadic Types



## Identity

The `Identity` monad is a monad that does not embody any computational strategy. It simply applies the bound function to its input without any modification. Computationally, there is no reason to use the `Identity` monad instead of the much simpler act of simply applying functions to their arguments. The purpose of the `Identity` monad is its fundamental role in the theory of monad transformers. Any monad transformer applied to the `Identity` monad yields a non-transformer version of that monad.

## List



## Maybe

 The Maybe type encapsulates an optional value. A value of type Maybe either contains a value (represented as Just), or it is empty (represented as Nothing). Using Maybe is a good way to deal with errors or exceptional cases without resorting to drastic measures such as error.





## Result



## Either

The `Either` type represents values with two possibilities: a value of type `Either a b` is either `Left a` or `Right b`.

The `Either` type is sometimes used to represent a value which is either correct or an error; by convention, the `Left` constructor is used to hold an error value and the `Right` constructor is used to hold a correct value (mnemonic: "right" also means "correct").



## Validation

Validation: A data-type like Either but with an accumulating Applicative Functor.

The `Validation` data type is isomorphic to `Either`, but has an instance of `Applicative` that accumulates on the error side. That is to say, if two (or more) errors are encountered, they are appended using a `Semigroup` operation.



## Reader

The `Reader` monad (also called the Environment monad). Represents a computation, which can read values from a shared environment, pass values from function to function, and execute sub-computations in a modified environment. Using `Reader` monad for such computations is often clearer and easier than using the `Control.Monad.State.State` monad.



## Writer

The Writer monad is a programming design pattern which makes it possible to compose functions which return their result values paired with a log string. The final result of a composed function yields both a value, and a concatenation of the logs from each component function application.



## IO



## Tagged



## Union



## Store, Reducer, Subscriber, Action(Tagged)



