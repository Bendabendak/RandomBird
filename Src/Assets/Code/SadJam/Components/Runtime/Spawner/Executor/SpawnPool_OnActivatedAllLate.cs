using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Spawner/SpawnPool/OnActivatedAllLate")]
    public class SpawnPool_OnActivatedAllLate : DynamicExecutor
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

            Pool.OnActivatedAllLate += OnActivatedAllLate;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Pool.OnActivatedAllLate -= OnActivatedAllLate;
        }

        private void OnActivatedAllLate()
        {
            Execute(Time.deltaTime);
        }
    }
}

