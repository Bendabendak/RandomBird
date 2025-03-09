using System;
using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Trigger/Radius/OnEnter")]
    public class Trigger_Radius_OnEnter : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public StructComponent<Vector3> Target { get; private set; }
        [field: SerializeField]
        public StructComponent<Vector3> Center { get; private set; }
        [field: SerializeField]
        public StructComponent<float> Radius { get; private set; }

        [NonSerialized]
        private bool _entered = false;
        protected override void DynamicExecutor_OnExecute()
        {
            if (Vector3.Distance(Center.Size, Target.Size) <= Radius.Size)
            {
                if (!_entered)
                {
                    Execute(Delta);
                    _entered = true;
                }
            }
            else
            {
                _entered = false;
            }
        }
    }
}
