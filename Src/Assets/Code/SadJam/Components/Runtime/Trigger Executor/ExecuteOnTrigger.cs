using System;
using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/TriggerExecutor/ExecuteOnTrigger")]
    public class ExecuteOnTrigger : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public bool Trigger { get; private set; } = false;

        [NonSerialized]
        private bool _lastTrigger = false;
        protected override void DynamicExecutor_Update()
        {
            if (Trigger && Trigger != _lastTrigger)
            {
                Execute(Time.deltaTime);
            }

            _lastTrigger = Trigger;
        }
    }
}
