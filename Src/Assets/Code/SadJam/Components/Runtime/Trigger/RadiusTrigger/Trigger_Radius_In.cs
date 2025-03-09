using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Trigger/Radius/In")]
    public class Trigger_Radius_In : DynamicExecutor
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

        protected override void DynamicExecutor_OnExecute()
        {
            if (Vector3.Distance(Center.Size, Target.Size) <= Radius.Size)
            {
                Execute(Delta);
            }
        }
    }
}
