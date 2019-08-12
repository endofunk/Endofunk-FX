//
// Maybe.cs
//
// Author:  endofunk
//
// Copyright (c) 2019 
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
using Endofunk.FX;
using static Endofunk.FX.Prelude;

namespace FPExamples.Examples {
  public static class Maybe_Monad {

    /// The Maybe type encapsulates an optional value. A value of type Maybe either contains
    /// a value (represented as Just), or it is empty (represented as Nothing).
    /// 
    /// Using Maybe is a good way to deal with errors or exceptional cases without resorting to 
    /// drastic measures such as error.
    
    public class Employee {
      public int Id { get; }
      public string Name { get; }
      public string Surname { get; }
      public Employee(int id, string name, string surname) => (Id, Name, Surname) = (id, name, surname);
    }

    public static void Assignment() {

      // Creating a Maybe variable using syntactic sugar
      // State: Just
      var just_value = Just(1);

      // The above is equivalent to:
      // State: Just
      var just_value2 = Maybe<int>.Just(1);

      // Creating a Maybe variable using syntactic sugar
      // State: Nothing
      var nothing_value = Nothing<int>();

      // The above is equivalent to:
      // State: Just
      var nothing_value2 = Maybe<int>.Nothing();

      // Creating a Maybe variable using syntactic sugar
      var just_employee = Just(new Employee(1, "Jack", "Sprat"));

    }

    public static void Transform() {
      var value = Just(1);
      var result = value.Map(x => x + 1);

      value = Nothing<int>();
      result = value.Map(x => x + 1);
    }

  }
}
