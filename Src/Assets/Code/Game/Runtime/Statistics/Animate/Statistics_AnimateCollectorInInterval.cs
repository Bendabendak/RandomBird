using SadJam.Components;
using SadJam;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Statistics_AnimateCollectorInInterval : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [Serializable]
        public class UpdateClip
        {
            public AnimationClips Clips;
            public int Interval = 0;
        }

        [field: SerializeField]
        public Statistics_Collector Collector { get; private set; }
        [field: SerializeField]
        public Statistics_Key StatusKey { get; private set; }
        [field: Space, SerializeField]
        public AnimationClips ShowClips { get; private set; }
        [field: SerializeField]
        public int ShowThreshold { get; private set; } = 0;
        [field: Space, SerializeField]
        public List<UpdateClip> UpdateClips { get; private set; } = new();

        private static Comparison<UpdateClip> _updateClipComparison => (x, y) => y.Interval.CompareTo(x.Interval);
        protected override void OnValidate()
        {
            base.OnValidate();

            UpdateClips.Sort(_updateClipComparison);
        }

        [NonSerialized]
        private double? _lastScore = null;
        [NonSerialized]
        private bool _shown = false;
        protected override void DynamicExecutor_OnExecute()
        {
            if (!Collector.GetNumericalStatus(StatusKey, out float stat, out Statistics_Collector.ErrorCodes error))
            {
                if (error == Statistics_Collector.ErrorCodes.StatusFoundWithDifferentType)
                {
                    Debug.LogWarning("Status is not numeric! " + StatusKey.Id, gameObject);
                }

                return;
            }

            if (_lastScore == stat) return;

            _lastScore = stat;

            if (!_shown)
            {
                if (ShowClips.Clips.Count <= 0)
                {
                    _shown = true;
                }
                else if (stat > ShowThreshold)
                {
                    ShowClips.Play(this, (bool finished) =>
                    {
                        _shown = true;
                    });
                }
            }
            else
            {
                foreach (UpdateClip updateClip in UpdateClips)
                {
                    if (updateClip.Interval == 0 || stat % updateClip.Interval == 0)
                    {
                        updateClip.Clips.Play(this);
                        break;
                    }
                }
            }
        }
    }
}
