using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Spawner/SpawnPool/OnActivatedLate")]
    public class SpawnPool_OnActivatedLate : DynamicExecutor
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

            Pool.OnActivatedLate += OnActivatedLate;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Pool.OnActivatedLate -= OnActivatedLate;
        }

        private void OnActivatedLate(SpawnPool.EntityInfo e)
        {
            Execute(Time.deltaTime);
        }
    }
}


