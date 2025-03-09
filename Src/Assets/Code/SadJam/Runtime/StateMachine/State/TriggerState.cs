using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam.StateMachine
{
    [CreateAssetMenu(fileName = "TriggerState", menuName = "SadJam/StateMachine/TriggerState")]
    public class TriggerState : ScriptableObject 
    {
        [field: SerializeField]
        public string Id { get; private set; }

        public struct Data : IEquatable<Data>
        {
            public TriggerState Trigger;
            public Dictionary<string, object> CustomData;

            public Data(TriggerState trigger, Dictionary<string, object> customData)
            {
                Trigger = trigger;
                CustomData = customData;
            }

            public Data(TriggerState trigger)
            {
                Trigger = trigger;
                CustomData = null;
            }

            public override int GetHashCode() => Trigger.GetHashCode();
            public override bool Equals(object obj) => Trigger.Equals(obj);
            public bool Equals(Data other) => Trigger.Equals(other.Trigger);
        }
    }
}
