using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam.StateMachine
{
    [CreateAssetMenu(fileName = "State", menuName = "SadJam/StateMachine/State")]
    public class State : ScriptableObject 
    {
        [field: SerializeField]
        public string Id { get; private set; }

        public struct Data : IEquatable<Data>
        {
            public State State;
            public Dictionary<string, object> CustomData;

            public Data(State state, Dictionary<string, object> customData)
            {
                State = state;
                CustomData = customData;
            }

            public Data(State state)
            {
                State = state;
                CustomData = null;
            }

            public override int GetHashCode() => State.GetHashCode();
            public override bool Equals(object obj) => State.Equals(obj);
            public bool Equals(Data other) => State.Equals(other.State);
        }
    }
}
