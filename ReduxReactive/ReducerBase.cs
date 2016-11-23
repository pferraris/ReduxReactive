using System;
using System.Collections.Generic;

namespace ReduxReactive
{
    public abstract class ReducerBase<TState> where TState : class, new()
    {
        private Dictionary<Type, Func<TState, IAction, TState>> reducers;

        public TState InitialState { get; private set; }
        protected abstract TState GetInitialState();

        public ReducerBase()
        {
            reducers = new Dictionary<Type, Func<TState, IAction, TState>>();
            InitialState = GetInitialState();
        }

        protected void Register<TAction>(Func<TState, IAction, TState> reducer) where TAction : IAction
        {
            reducers[typeof(TAction)] = reducer;
        }

        public TState Execute(TState state, IAction action)
        {
            if (reducers.ContainsKey(action.GetType()))
                return reducers[action.GetType()](state, action);
            return state;
        }
    }
}
