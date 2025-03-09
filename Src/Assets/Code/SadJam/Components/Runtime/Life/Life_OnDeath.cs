using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Life/OnDeath")]
    public class Life_OnDeath : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public StructComponent<float> Health { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (Health.Size <= 0)
            {
                Execute(Delta);
            }
        }
    }
}
