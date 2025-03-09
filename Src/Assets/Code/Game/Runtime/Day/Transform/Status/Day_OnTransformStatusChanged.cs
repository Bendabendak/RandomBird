using SadJam;
using System;
using System.Collections;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Day/TransformStatus/OnChanged")]
    [CustomStaticExecutor("_UEGXhWH8kKh7tjGy2zS8g")]
    public class Day_OnTransformStatusChanged : StaticExecutor
    {
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
            Execute(Time.deltaTime);
        }
    }
}

