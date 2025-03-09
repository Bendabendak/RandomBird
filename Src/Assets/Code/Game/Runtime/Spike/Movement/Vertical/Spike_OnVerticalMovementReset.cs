using SadJam;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Spike/OnVerticalMovementReset")]
    public class Spike_OnVerticalMovementReset : DynamicExecutor
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

            VerticalMovement.OnMovementReset -= OnMovementReset;
            VerticalMovement.OnMovementReset += OnMovementReset;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            VerticalMovement.OnMovementReset -= OnMovementReset;
        }

        private void OnMovementReset()
        {
            Execute(Time.deltaTime);
        }
    }
}
