using SadJam;
using System;
using TMPro;
using UnityEngine;

namespace Game
{
    public class Statistics_ShowNumericalCollector : DynamicExecutor
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
        public TMP_Text Text { get; private set; }
        [field: SerializeField]
        public string Prefix { get; private set; } = "";
        [field: SerializeField]
        public string Suffix { get; private set; } = "";

        [NonSerialized]
        private double? _lastStatus = null;
        protected override void DynamicExecutor_OnExecute()
        {
            if (!Collector.GetNumericalStatus(StatusKey, out float stat, out Statistics_Collector.ErrorCodes error))
            {
                if (error == Statistics_Collector.ErrorCodes.StatusFoundWithDifferentType)
                {
                    Debug.LogWarning("Status is not numeric! " + StatusKey, gameObject);
                }

                return;
            }

            if (_lastStatus == stat) return;

            Text.text = Prefix + stat.ToString() + Suffix;
            _lastStatus = stat;
        }
    }
}
