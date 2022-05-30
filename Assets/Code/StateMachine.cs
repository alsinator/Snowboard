using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Snowboard.Controllers;

namespace Snowboard
{
    public class StateMachine : GameActor
    {
        protected StateBase currentState;

        protected readonly Queue<StateBase> stateQueue = new(3);

        public Action<StateBase> OnStateChange;

        public Action<StateBase> OnStateFinished;


        public override void Update()
        {
            if (currentState != null)
            {
                currentState.OnUpdate();

                if (currentState.IsFinished)
                {
                    OnStateFinished?.Invoke(currentState);

                    if (stateQueue.Count > 0)
                    {
                        ChangeState(stateQueue.Dequeue());
                    }
                }
            }
        }

        public void ChangeState(StateBase newState)
        {
            if (currentState != null)
            {
                currentState.OnExit();
            }

            currentState = newState;

            if (currentState != null)
            {
                currentState.StateMachine = this;
                currentState.OnEnter();

                OnStateChange?.Invoke(currentState);
            }
        }

        public void EnqueueState(StateBase newState)
        {
            stateQueue.Enqueue(newState);
        }
    }
}