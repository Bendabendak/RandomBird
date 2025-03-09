using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Spawner/SpawnPool/OnActivatedAll")]
    public class SpawnPool_OnActivatedAll : DynamicExecutor
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

            Pool.OnActivatedAll += OnActivatedAll;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Pool.OnActivatedAll -= OnActivatedAll;
        }

        private void OnActivatedAll()
        {
            Execute(Time.deltaTime);
        }
    }
}
