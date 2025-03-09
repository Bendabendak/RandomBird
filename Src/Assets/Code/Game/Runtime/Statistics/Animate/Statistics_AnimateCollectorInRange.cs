using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Statistics_AnimateCollectorInRange : DynamicExecutor
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
            public Vector2 Range;

            [NonSerialized]
            public bool Active;
        }

        [field: SerializeField]
        public Statistics_Collector Collector { get; private set; }
        [field: SerializeField]
        public Statistics_Key StatusKey { get; private set; }
        [field: Space, SerializeField]
        public List<UpdateClip> UpdateClips { get; private set; } = new();

        [NonSerialized]
        private double? _lastScore = null;
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

            if (_lastScore == stat) return;

            _lastScore = stat;

            foreach (UpdateClip updateClip in UpdateClips)
            {
                if (stat >= updateClip.Range.x && stat <= updateClip.Range.y)
                {
                    if (updateClip.Active) continue;

                    updateClip.Active = true;
                    updateClip.Clips.Play(this, (bool finished) =>
                    {
                        updateClip.Active = false;
                    });
                }
                else
                {
                    if (updateClip.Active) 
                    {
                        updateClip.Active = false;
                        updateClip.Clips.Reset();
                    }
                }
            }
        }
    }
}
