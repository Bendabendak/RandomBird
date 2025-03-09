using SadJam;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Statistics/LoadToCollector")]
    public class Statistics_LoadToCollector : DynamicExecutor
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
        public List<string> StatisticsToLoad { get; private set; }
        [field: SerializeField]
        public bool LoadAll { get; private set; } = true;

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (!Statistics.LoadStatistics(Owner, out Dictionary<string, object> data)) return;

            if (LoadAll)
            {
                foreach (KeyValuePair<string, object> d in data)
                {
                    if (!double.TryParse(d.ToString(), out double dataS))
                    {
                        continue;
                    }

                    Collector.ChangeStatus(d.Key, dataS);
                }
            }
            else
            {
                foreach (string s in StatisticsToLoad)
                {
                    if (!data.TryGetValue(s, out object d)) continue;

                    if (!double.TryParse(d.ToString(), out double dataS))
                    {
                        continue;
                    }

                    Collector.ChangeStatus(s, dataS);
                }
            }
        }
    }
}
