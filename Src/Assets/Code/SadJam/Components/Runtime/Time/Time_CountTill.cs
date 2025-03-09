using System;
using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Time/CountTill")]
    public class Time_CountTill : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public float Seconds { get; private set; }

        [NonSerialized]
        private float _count = 0;
        protected override void DynamicExecutor_OnExecute()
        {
            if (_count >= Seconds)
            {
                _count = Delta;
                Execute(Delta);
            }
            else
            {
                _count += Delta;
            }
        }
    }
}
