using SadJam;
using SadJam.Components;
using System;
using UnityEngine;

namespace Game
{
    public class Day_LoadSkin : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public AnimationClips DayTransitionClip { get; private set; }
        [field: SerializeField]
        public AnimationClips NightTransitionClip { get; private set; }

        [NonSerialized]
        private Transform _nightSkin;
        [NonSerialized]
        private Transform _daySkin;

        protected override void OnEnable()
        {
            base.OnEnable();

            _nightSkin = transform.FindRecursive("Night");
            _daySkin = transform.FindRecursive("Day");

            _lastDayTransformer = Day_Transformer.GetTransformer();

            if (_lastDayTransformer == null)
            {
                Debug.LogError("Day transformer not found in scene!", gameObject);
                return;
            }

            switch (_lastDayTransformer.TransformStatus)
            {
                case Day_TransformStatus.Day:
                    DaySetActive(true);
                    NightSetActive(false);
                    break;
                case Day_TransformStatus.Night:
                    DaySetActive(false);
                    NightSetActive(true);
                    break;
            }
        }

        [NonSerialized]
        private Day_Transformer _lastDayTransformer;
        protected override void Start()
        {
            base.Start();

            if (_lastDayTransformer != null) return;

            _lastDayTransformer = Day_Transformer.GetTransformer();

            if (_lastDayTransformer == null)
            {
                Debug.LogError("Day transformer not found in scene!", gameObject);
                return;
            }

            switch (_lastDayTransformer.TransformStatus)
            {
                case Day_TransformStatus.Day:
                    DaySetActive(true);
                    NightSetActive(false);
                    break;
                case Day_TransformStatus.Night:
                    DaySetActive(false);
                    NightSetActive(true);
                    break;
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            _lastDayTransformer = Day_Transformer.GetTransformer();

            if (_lastDayTransformer == null)
            {
                Debug.LogError("Day transformer not found in scene!", gameObject);
                return;
            }

            switch (_lastDayTransformer.TransformStatus)
            {
                case Day_TransformStatus.Day:
                    DayTransitionClip.Play(this, (bool finished) =>
                    {
                        DaySetActive(true);
                        NightSetActive(false);
                    });
                    break;
                case Day_TransformStatus.Night:
                    NightTransitionClip.Play(this, (bool finished) =>
                    {
                        DaySetActive(false);
                        NightSetActive(true);
                    });
                    break;
            }
        }

        private void DaySetActive(bool value)
        {
            if (_daySkin != null)
            {
                if (_nightSkin == null)
                {
                    _daySkin.gameObject.SetActive(true);
                    return;
                }

                _daySkin.gameObject.SetActive(value);
            }
        }

        private void NightSetActive(bool value)
        {
            if (_nightSkin != null)
            {
                if (_daySkin == null)
                {
                    _nightSkin.gameObject.SetActive(true);
                    return;
                }

                _nightSkin.gameObject.SetActive(value);
            }
        }
    }
}
