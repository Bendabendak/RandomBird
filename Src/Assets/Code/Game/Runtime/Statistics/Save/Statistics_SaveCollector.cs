using SadJam;
using UnityEngine;

namespace Game
{
    public class Statistics_SaveCollector : DynamicExecutor
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

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (!Collector.GetStatus(StatusKey, out object stat)) return;

            Statistics.UpdateAndSaveStatistics(Owner, new Statistics.DataEntry(StatusKey, stat));
        }
    }
}