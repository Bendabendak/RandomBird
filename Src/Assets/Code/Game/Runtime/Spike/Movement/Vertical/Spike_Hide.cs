using SadJam;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Spike/Hide")]
    public class Spike_Hide : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Spike_VerticalMovement VerticalMovement { get; private set; }

        [field: SerializeField, Space]
        public bool WithFreeze { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (WithFreeze)
            {
                VerticalMovement.FreezeMovement();
            }

            VerticalMovement.HideSpike((bool completed) =>
            {
                Execute(Time.deltaTime);
            });
        }
    }
}
