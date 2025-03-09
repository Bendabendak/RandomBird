using UnityEngine;

namespace SadJam.Components
{
    public class SpawnPool_DestroyAllImmediate : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public SpawnPool Pool { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            Pool.DestroyAll();
        }
    }
}