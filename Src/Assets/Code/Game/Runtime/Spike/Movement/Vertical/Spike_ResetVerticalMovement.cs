using SadJam;
using UnityEngine;

namespace Game
{
    public class Spike_ResetVerticalMovement : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Spike_VerticalMovement VerticalMovement { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            VerticalMovement.ResetMovement();
        }
    }
}
