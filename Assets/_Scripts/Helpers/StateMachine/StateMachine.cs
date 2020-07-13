using System;
using System.Collections.Generic;
using Helpers.StateMachine.States;

namespace Helpers.StateMachine
{
    public class StateMachine
    {
        private IState _currentState;

        private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
        private List<Transition> _currentTransitions = new List<Transition>();
        
        /// <summary>
        /// Transition viable from any state. 
        /// </summary>
        private List<Transition> _anyTransitions = new List<Transition>();

        /// <summary>
        /// Get viable state and calls its Tick method.
        /// </summary>
        public void Tick()
        {
            var transition = GetTransition();

            if (transition != null) 
                SetState(transition.To);

            _currentState?.Tick();
        }
        
        private Transition GetTransition()
        {
            // Check if there is transition you can go from any state.
            foreach (var transition in _anyTransitions)
                if (transition.Condition())
                    return transition;

            // Check if there is transition for current state
            foreach (var transition in _currentTransitions)
                if (transition.Condition())
                    return transition;

            return null;
        }

        public void SetState(IState state)
        {
            if (state == _currentState) 
                return;

            // Leave previous state
            _currentState?.OnExit();
            
            // Switch to new state 
            _currentState = state;

            _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
            
            // To avoid going trough null list.
            if (_currentTransitions == null) 
                _currentTransitions = new List<Transition>(0);

            _currentState.OnEnter();
        }

        public void AddTransition(IState from, IState to, Func<bool> predicate)
        {
            // Check if exist from state.
            if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
            {
                transitions = new List<Transition>();
                _transitions[from.GetType()] = transitions;
            }

            transitions.Add(new Transition(to, predicate));
        }

        public void AddAnyTransition(IState state, Func<bool> predicate)
        {
            _anyTransitions.Add(new Transition(state, predicate));
        }

        private class Transition
        {
            public Func<bool> Condition { get; }
            public IState To { get; }

            public Transition(IState to, Func<bool> condition)
            {
                To = to;
                Condition = condition;
            }
        }
    }
}