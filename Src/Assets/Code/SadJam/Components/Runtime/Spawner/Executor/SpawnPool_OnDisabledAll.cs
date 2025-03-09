using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Spawner/SpawnPool/OnDisabledAll")]
    public class SpawnPool_OnDisabledAll : DynamicExecutor
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

            Pool.OnDisabledAll += OnDisabledAll;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Pool.OnDisabledAll -= OnDisabledAll;
        }

        private void OnDisabledAll()
        {
            Execute(Time.deltaTime);
        }
    }
}
