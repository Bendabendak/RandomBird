using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Spawner/SpawnPool/OnDisabled")]
    public class SpawnPool_OnDisabled : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public SpawnPool Pool { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Pool.OnDisabled += OnDisabled;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Pool.OnDisabled -= OnDisabled;
        }

        private void OnDisabled(SpawnPool.EntityInfo e)
        {
            Execute(Time.deltaTime);
        }
    }
}
