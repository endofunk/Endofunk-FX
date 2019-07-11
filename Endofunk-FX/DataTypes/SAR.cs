// SAR.cs
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
using System.Linq;
using System.Threading.Tasks;
using static Endofunk.FX.Prelude;

namespace Endofunk.FX {

  public interface IAction {
    object Value { get; }
    Type @Type { get; }
  }

  public class Store<S, A> {
    internal readonly Reducer<S, A> Reducer;
    internal readonly List<Subscriber<S>> Subscribers = List<Subscriber<S>>();
    internal S State;
    private Store(Reducer<S, A> reducer, S state) => (Reducer, State) = (reducer, state);
    public static Store<S, A> Create(Reducer<S, A> reducer, S state) => new Store<S, A>(reducer, state);

    public Subscriber<S> Subscribe(Action<S> action) {
      var subscriber = Subscriber<S>.Create(action);
      Subscribers.Add(subscriber);
      subscriber.Update(State);
      return subscriber;
    }

    private async Task DispatchActions(params A[] actions) => await Task.Run(() => State = actions.Aggregate(State, Reducer.Reduce));
    public void DispatchAsync(params A[] actions) => DispatchActions(actions).ContinueWith(t => UpdateSubscribers());

    internal void UpdateSubscribers() {
      Subscribers.RemoveAll((s) => s.HasCrashed);
      Subscribers.ForEach((s) => s.Update(State));
    }
  }

  public struct Reducer<S, A> {
    public readonly Func<S, A, S> Reduce;
    private Reducer(Func<S, A, S> reduce) => (Reduce) = (reduce);
    public static Reducer<S, A> Create(Func<S, A, S> reduce) => new Reducer<S, A>(reduce);
  }

  public struct Subscriber<S> {
    public readonly Guid Id;
    public readonly Action<S> Function;
    public bool HasCrashed { get; private set; }
    private Subscriber(Action<S> function) => (Id, Function, HasCrashed) = (Guid.NewGuid(), function, false);
    public static Subscriber<S> Create(Action<S> action) => new Subscriber<S>(action);

    public void Update(S state) {
      if (HasCrashed) return;
      try {
        Function(state);
      } catch (Exception e) {
        LogDefault(e);
        HasCrashed = true;
      }
    }
  }

  public static partial class Prelude {
    public static int UnSubscribe<S, A>(this Store<S, A> @this, Subscriber<S> subscriber) => @this.Subscribers.RemoveAll(s => s.Id == subscriber.Id);
    public static void Dispatch<S, A>(this Store<S, A> @this, params A[] actions) {
      @this.State = actions.Aggregate(@this.State, @this.Reducer.Reduce);
      @this.UpdateSubscribers();
    }
  }
}