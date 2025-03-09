using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game 
{
    public class Statistics_AnimateHighestCollectorInInterval : DynamicExecutor
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
        public Statistics_Key StatusKey { get; private set; }
        [field: Space, SerializeField]
        public AnimationClips ShowClips { get; private set; }
        [field: SerializeField]
        public int ShowThreshold { get; private set; } = 0;
        [field: Space, SerializeField]
        public List<UpdateClip> UpdateClips { get; private set; } = new();

        protected override void OnValidate()
        {
            base.OnValidate();

            UpdateClips.Sort((x, y) => y.Interval.CompareTo(x.Interval));
        }

        [NonSerialized]
        private double _lastScore = 0;
        [NonSerialized]
        private bool _shown = false;
        protected override void DynamicExecutor_OnExecute()
        {
            if (Statistics_Collector.Collectors.Count > 0)
            {
                double max = double.MinValue;

                foreach (Statistics_Collector collector in Statistics_Collector.Collectors.Values)
                {
                    if (!collector.GetNumericalStatus(StatusKey, out float stat)) continue;

                    if (stat > max)
                    {
                        max = stat;
                    }
                }

                if (max == double.MinValue) return;

                if (_lastScore == max) return;

                _lastScore = max;

                if (!_shown)
                {
                    if (ShowClips.Clips.Count <= 0)
                    {
                        _shown = true;
                    }
                    else if (max > ShowThreshold)
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
                        if (updateClip.Interval == 0 || max % updateClip.Interval == 0)
                        {
                            updateClip.Clips.Play(this);
                            break;
                        }
                    }
                }
            }
        }
    }
}
