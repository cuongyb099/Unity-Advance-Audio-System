using System;
using System.Collections.Generic;

namespace Tech.State_Machine
{
    public class StateMachine<StateID, BState> where StateID : Enum where BState : BaseState
    {
        public readonly Dictionary<StateID, BState> _states = new ();
        
        public BState CurrentState { get; private set; }

        public void AddNewState(StateID stateID, BState newState)
        {
            _states.Add(stateID, newState);
        }
        
        public virtual void Initialize(StateID startedState)
        {
            CurrentState = _states[startedState];
            CurrentState.Enter();
        }

        public void ChangeState(StateID newState)
        {
            BState state = _states[newState]; 
            if(CurrentState == state) return;
            CurrentState.Exit();
            CurrentState = state;
            state.Enter();
        }
    }
}