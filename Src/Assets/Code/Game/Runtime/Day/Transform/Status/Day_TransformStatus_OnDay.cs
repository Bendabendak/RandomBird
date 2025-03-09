using SadJam;
using System;
using System.Collections;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Day/TransformStatus/OnDay")]
    [CustomStaticExecutor("fRK61TQJlUmBi96rGpqG-Q")]
    public class Day_TransformStatus_OnDay : StaticExecutor
    {
        [NonSerialized]
        private Day_Transformer _transformer;
        protected override void Start()
        {
            base.Start();

            if (_transformer != null)
            {
                _transformer.OnExecuted -= OnChange;
            }

            _transformer = Day_Transformer.GetTransformer();

            if (_transformer == null)
            {
                StartCoroutine(WaitTillTransformer(() =>
                {
                    Done();
                }));
            }
            else
            {
                Done();
            }

            void Done()
            {
                if (_transformer == null) return;

                _transformer.OnExecuted += OnChange;

                if (_transformer.TransformStatus == Day_TransformStatus.Day)
                {
                    Execute(Time.deltaTime);
                }
            }
        }

        private IEnumerator WaitTillTransformer(Action done = null)
        {
            if (Day_Transformer.GetTransformer() == null)
            {
                yield return null;
            }

            _transformer = Day_Transformer.GetTransformer();
            done?.Invoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_transformer != null)
            {
                _transformer.OnExecuted -= OnChange;
            }
        }

        private void OnChange()
        {
            if (_transformer.TransformStatus == Day_TransformStatus.Day)
            {
                Execute(Time.deltaTime);
            }
        }
    }
}
