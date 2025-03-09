using SadJam;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Spike/OnHidden")]
    public class Spike_OnHidden : DynamicExecutor
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

            VerticalMovement.OnHidden -= OnHidden;
            VerticalMovement.OnHidden += OnHidden;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            VerticalMovement.OnHidden -= OnHidden;
        }

        private void OnHidden()
        {
            Execute(Time.deltaTime);
        }
    }
}
