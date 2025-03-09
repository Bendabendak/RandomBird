using System.Collections.Generic;
using UnityEngine;

namespace SadJam.StateMachine
{
    [CreateAssetMenu(fileName = "StateCategory", menuName = "SadJam/StateMachine/StateCategory")]
    public class StateCategory : ScriptableObject 
    {
        [field: SerializeField]
        public string Id { get; private set; }

        [field: Space, SerializeField]
        public State DefaultState { get; private set; } = null;

        [field: SerializeField]
        public List<State> States { get; private set; } = new();
        [field: SerializeField]
        public List<TriggerState> Triggers { get; private set; } = new();

        protected virtual void OnValidate()
        {
            if (DefaultState == null)
            {
                foreach(State s in States)
                {
                    if (s != null)
                    {
                        DefaultState = s;
                        break;
                    }
                }
            }
            else if(!States.Contains(DefaultState)) 
            {
                DefaultState = null;

                foreach (State s in States)
                {
                    if (s != null)
                    {
                        DefaultState = s;
                        break;
                    }
                }
            }
        }
    }
}
