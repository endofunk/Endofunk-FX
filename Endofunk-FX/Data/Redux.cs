﻿// Redux.cs
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
  public sealed class Store<S, A> {
    private readonly Reducer<S, A> Reducer;
    private readonly List<Subscriber<S>> Subscribers = List<Subscriber<S>>();
    private S State;
    private Action<A> Dispatcher;

    internal Store(Reducer<S, A> reducer, S state) {
      Reducer = reducer;
      State = state;
      Dispatcher = Act<A>(action => State = Reducer.Reduce(State, action));
    }

    internal Store(Reducer<S, A> reducer, S state, params Func<Func<S>, Action<A>, Func<Action<A>, Action<A>>>[] middleware) {
      Reducer = reducer;
      State = state;
      Dispatcher = action => State = Reducer.Reduce(State, action);
      Dispatcher = middleware.Combine()(() => State, Dispatcher)(Dispatcher);
    }
    public static Store<S, A> Create(Reducer<S, A> reducer, S state) => new Store<S, A>(reducer, state);

    public Subscriber<S> Subscribe(Action<S> action) {
      var subscriber = Subscriber<S>(action);
      Subscribers.Add(subscriber);
      subscriber.Update(State);
      return subscriber;
    }
    private async Task DispatchActions(params A[] actions) => await Task.Run(() => actions.ForEach(Dispatcher));
    public void DispatchAsync(params A[] actions) => DispatchActions(actions).ContinueWith(t => UpdateSubscribers());

    private void UpdateSubscribers() {
      Subscribers.RemoveAll((s) => s.HasCrashed);
      Subscribers.ForEach((s) => s.Update(State));
    }

    public void Dispatch(params A[] actions) {
      actions.ForEach(Dispatcher);
      UpdateSubscribers();
    }

    public int UnSubscribe(Subscriber<S> subscriber) => Subscribers.RemoveAll(s => s.Id == subscriber.Id);
    public void Fold(Action<S> f) => f(State);
    public R Fold<R>(Func<S, R> f) => f(State);
  }

  public sealed class Reducer<S, A> {
    public readonly Func<S, A, S> Reduce;
    internal Reducer(Func<S, A, S> reduce) => (Reduce) = (reduce);
    public static Reducer<S, A> Create(Func<S, A, S> reduce) => new Reducer<S, A>(reduce);
  }

  public sealed class Subscriber<S> {
    public readonly Guid Id;
    public readonly Action<S> Compute;
    internal bool HasCrashed { get; private set; }
    internal Subscriber(Action<S> compute) => (Id, Compute, HasCrashed) = (Guid.NewGuid(), compute, false);
    public static Subscriber<S> Create(Action<S> compute) => new Subscriber<S>(compute);

    public void Update(S state) {
      if (HasCrashed) return;
      try {
        Compute(state);
      } catch (Exception e) {
        LogDefault(e);
        HasCrashed = true;
      }
    }
  }

  public static class ReducerExtensions {
    public static Reducer<S, A> Compose<S, A>(this Reducer<S, A> f, Reducer<S, A> g) => new Reducer<S, A>((s, a) => g.Reduce(f.Reduce(s, a), a));
  }

  public static class MiddlewareExtensions {
    public static Func<Func<S>, Action<A>, Func<Action<A>, Action<A>>> Combine<S, A>(this Func<Func<S>, Action<A>, Func<Action<A>, Action<A>>>[] middleware) => (getState, dispatch) => {
      return d => middleware
        .Select(m => m(getState, dispatch))
        .Reverse()
        .Aggregate(d, (d2, next) => next(d2));
    };
  }

  public static partial class Prelude {
    public static Store<S, A> Store<S, A>(Reducer<S, A> reducer, S state) => new Store<S, A>(reducer, state);
    public static Store<S, A> Store<S, A>(Reducer<S, A> reducer, S state, params Func<Func<S>, Action<A>, Func<Action<A>, Action<A>>>[] middleware) => new Store<S, A>(reducer, state, middleware);
    public static Reducer<S, A> Reducer<S, A>(Func<S, A, S> reduce) => new Reducer<S, A>(reduce);
    public static Subscriber<S> Subscriber<S>(Action<S> compute) => new Subscriber<S>(compute);
  }
}


