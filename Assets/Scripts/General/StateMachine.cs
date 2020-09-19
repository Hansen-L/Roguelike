using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = System.Object;

// Notes
// 1. What a finite state machine is
// 2. Examples where you'd use one
//     AI, Animation, Game State
// 3. Parts of a State Machine
//     States & Transitions
// 4. States - 3 Parts
//     Tick - Why it's not Update()
//     OnEnter / OnExit (setup & cleanup)
// 5. Transitions
//     Separated from states so they can be re-used
//     Easy transitions from any state

public class StateMachine
{
    private IState _currentState;

    // _ means instance variable
    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>(); // Dictionary. Keys are states, values are transitions from that state
    private List<Transition> _currentTransitions = new List<Transition>(); // Transitions for the current state
    private List<Transition> _anyTransitions = new List<Transition>(); // These are transitions that can be accessed from any state

    private static List<Transition> EmptyTransitions = new List<Transition>(0);

    public void Tick() // Calls the tick method of the current state. This is called in the dog update loop
    {
        var transition = GetTransition();
		if (transition != null)
		{
			SetState(transition.To);
			Debug.Log(transition.To);
		}

        _currentState?.Tick();
    }

	public void FixedTick()
	{
		_currentState?.FixedTick();
	}

    // Change the current state
    public void SetState(IState state)
    {
        if (state == _currentState)
            return;

        _currentState?.OnExit(); // Run the OnExit code of the current state
        _currentState = state; // Set current state to the new state

        // Looks into _transitions, tries to get transitions with current State as key. If there are transitions for this state, save it to _currentTransitions
        // _currentTransitions holds possible transitions for current State
        _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);

        // If current state doesn't have any transitions, instantiate an empty transition list
        if (_currentTransitions == null)
            _currentTransitions = EmptyTransitions;

        _currentState.OnEnter(); // Run OnEnter code of new state
    }

    // _transitions holds all possible transitions for each state in a dict
    // This method adds a new transition to _transitions
    public void AddTransition(IState from, IState to, Func<bool> predicate)
    {
        // Get the transition list for the `from` state. If it doesn't exist, instantiate an empty list
        if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;
        }

        // Add the new transition to the list for the current state
        transitions.Add(new Transition(to, predicate));
    }

    // Transition from any state
    public void AddAnyTransition(IState state, Func<bool> predicate)
    {
        _anyTransitions.Add(new Transition(state, predicate));
    }

    private class Transition
    {
        public Func<bool> Condition { get; }
        public IState To { get; }

        // Instantiate transition with state to transition to, and boolean to transition
        public Transition(IState to, Func<bool> condition) // Func<bool> condition just means that condition has to be a parameter-less function that returns a bool
        {
            To = to;
            Condition = condition;
        }
    }

    private Transition GetTransition() // Check if we need to transition to a new state
    {
        // Check all booleans in the _anyTransitions list
        foreach (var transition in _anyTransitions)
            if (transition.Condition())
                return transition;

        // Check boolean conditions for current State's transitions
        foreach (var transition in _currentTransitions)
            if (transition.Condition())
                return transition;

        return null;
    }
}