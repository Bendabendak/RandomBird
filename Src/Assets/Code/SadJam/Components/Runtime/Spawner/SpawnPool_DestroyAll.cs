using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Spawner/DestroyAll")]
    public class SpawnPool_DestroyAll : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public SpawnPool Pool { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            Pool.DestroyAll(() =>
            {
                Execute(Delta);
            });
        }
    }
}
