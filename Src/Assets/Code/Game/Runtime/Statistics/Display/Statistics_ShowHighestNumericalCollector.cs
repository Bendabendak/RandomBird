using UnityEngine;
using TMPro;
using SadJam;
using System;

namespace Game 
{
    public class Statistics_ShowHighestCollector : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Statistics_Key StatusKey { get; private set; }
        [field: Space, SerializeField]
        public TMP_Text Text { get; private set; }
        [field: SerializeField]
        public string Prefix { get; private set; } = "";
        [field: SerializeField]
        public string Suffix { get; private set; } = "";

        [NonSerialized]
        private double? _lastStatus = null;
        protected override void DynamicExecutor_OnExecute()
        {
            if (Statistics_Collector.Collectors.Count > 0) 
            {
                double maxStat = double.MinValue;
                foreach (Statistics_Collector collector in Statistics_Collector.Collectors.Values)
                {
                    if (!collector.GetNumericalStatus(StatusKey, out float stat)) continue;

                    if (stat > maxStat)
                    {
                        maxStat = stat;
                    }
                }

                if (maxStat == double.MinValue) return;

                if (_lastStatus == maxStat) return;

                Text.text = Prefix + maxStat.ToString() + Suffix;
                _lastStatus = maxStat;
            }
        }
    }
}
