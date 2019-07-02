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
    internal readonly List<Subscriber<S>> Subscribers = new List<Subscriber<S>>();
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