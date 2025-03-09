using System;
using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Trigger/Radius/OnExit")]
    public class Trigger_Radius_OnExit : DynamicExecutor
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
        private bool _exited = false;
        protected override void DynamicExecutor_OnExecute()
        {
            if (Vector3.Distance(Center.Size, Target.Size) <= Radius.Size)
            {
                _exited = false;
            }
            else
            {
                if (!_exited)
                {
                    Execute(Delta);
                    _exited = true;
                }
            }
        }
    }
}
