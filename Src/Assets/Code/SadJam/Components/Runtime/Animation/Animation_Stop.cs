using UnityEngine;

namespace SadJam.Components
{
    public class Animation_Stop : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Animation Animation { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            Animation.Stop();
        }
    }
}
