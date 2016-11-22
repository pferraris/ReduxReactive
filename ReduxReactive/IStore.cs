using System;

namespace ReduxReactive
{
    public delegate IAction Dispatcher(IAction action);

    public interface IStore<TState> : IObservable<TState>
    {
        TState State { get; }
        Dispatcher Dispatch { get; }
    }
}