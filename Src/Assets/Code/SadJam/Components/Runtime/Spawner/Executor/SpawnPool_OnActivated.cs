using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Spawner/SpawnPool/OnActivated")]
    public class SpawnPool_OnActivated : DynamicExecutor
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

            Pool.OnActivated += OnActivated;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Pool.OnActivated -= OnActivated;
        }

        private void OnActivated(SpawnPool.EntityInfo e)
        {
            Execute(Time.deltaTime);
        }
    }
}

