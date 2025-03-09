using SadJam;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Spike/OnShown")]
    public class Spike_OnShown : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Spike_VerticalMovement VerticalMovement { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            VerticalMovement.OnShown -= OnShown;
            VerticalMovement.OnShown += OnShown;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            VerticalMovement.OnShown -= OnShown;
        }

        private void OnShown()
        {
            Execute(Time.deltaTime);
        }
    }
}
