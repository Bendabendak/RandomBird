using SadJam;
using UnityEngine;

namespace Game
{
    public class Statistics_SetCollector : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Statistics_Collector Collector { get; private set; }
        [field: SerializeField]
        public Statistics_Key StatusKey { get; private set; }
        [field: Space, SerializeField]
        public float Value { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            Collector.ChangeStatus(StatusKey, Value);
        }
    }
}
