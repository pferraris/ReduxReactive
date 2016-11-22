using System;
using System.Collections.Generic;
using System.Linq;

namespace ReduxReactive
{
    public delegate TState Reducer<TState>(TState state, IAction action);
    public delegate Func<Dispatcher, Dispatcher> Middleware<TState>(IStore<TState> store);

    public class Store<TState> : IStore<TState>
    {
        private List<IObserver<TState>> observers;
        private Reducer<TState> reducer;

        public TState State { get; private set; }
        public Dispatcher Dispatch { get; private set; }

        public Store(Reducer<TState> reducer, TState initialState = default(TState), params Middleware<TState>[] middlewares)
        {
            observers = new List<IObserver<TState>>();
            this.reducer = reducer;
            InitDispatcher(middlewares);
            State = initialState;
        }

        private void InitDispatcher(params Middleware<TState>[] middlewares)
        {
            Dispatch = CallReducer;
            foreach (var middleware in middlewares.Reverse())
                Dispatch = middleware(this)(Dispatch);
        }

        private IAction CallReducer(IAction action)
        {
            lock (reducer)
            {
                State = reducer(State, action);
            }
            observers.ForEach(x => x.OnNext(State));
            return action;
        }


        public IDisposable Subscribe(IObserver<TState> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            observer.OnNext(State);
            return new Unsubscriber(observer, observers);
        }

        private class Unsubscriber : IDisposable
        {
            private IObserver<TState> observer;
            private List<IObserver<TState>> observers;

            public Unsubscriber(IObserver<TState> observer, List<IObserver<TState>> observers)
            {
                this.observer = observer;
                this.observers = observers;
            }

            public void Dispose()
            {
                if (observers.Contains(observer))
                    observers.Remove(observer);
            }
        }
    }
}
