using SadJam;
using SadJam.Components;
using System;
using System.Collections;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Effect/Apply")]
    public class Effect_Apply : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public IGameConfig_Effect EffectToApply { get; }

        [field: SerializeField]
        public Transform Root { get; private set; }

        [field: SerializeField, Space]
        public bool DestroyIfNotActivated { get; private set; } = true;

        protected override void DynamicExecutor_OnExecute()
        {
            ApplyEffect(EffectToApply, () =>
            {
                Execute(Delta);
            });
        }

        private void ApplyEffect(IGameConfig_Effect effect, Action done = null)
        {
            if (!effect.Enabled)
            {
                StartCoroutine(WaitTillEnabled(effect, Enabled));
            }
            else
            {
                Enabled();
            }

            void Enabled()
            {
                if (Root.parent == null)
                {
                    Debug.LogError("Root parent cannot be null! " + Root.name, gameObject);
                    return;
                }

                if (!EffectToApply.ActivateEffect(Root.parent.gameObject))
                {
                    if (DestroyIfNotActivated)
                    {
                        SpawnPool.Destroy(gameObject);
                    }

                    return;
                }

                StopAllCoroutines();

                if (effect.DurationUnscaled)
                {
                    StartCoroutine(UnscaledDelayCoroutine(EffectToApply, DelayDone));
                }
                else
                {
                    StartCoroutine(DelayCoroutine(EffectToApply, DelayDone));
                }

                void DelayDone()
                {
                    if (!effect.Enabled)
                    {
                        StartCoroutine(WaitTillEnabled(effect, Done));
                    }
                    else
                    {
                        Done();
                    }

                    void Done()
                    {
                        StopEffect(effect);

                        done?.Invoke();
                    }
                }
            }
        }

        public void StopEffect(IGameConfig_Effect effect)
        {
            StopAllCoroutines();

            if (Root.parent == null)
            {
                Debug.LogError("Root parent cannot be null! " + Root.name, gameObject);
                return;
            }

            effect.RemoveEffect(Root.parent.gameObject);
        }

        private IEnumerator WaitTillEnabled(IGameConfig_Effect effect, Action done = null)
        {
            while (!effect.Enabled)
            {
                yield return null;
            }

            done?.Invoke();
        }

        public float TimeElapsed { get; private set; }

        private IEnumerator UnscaledDelayCoroutine(IGameConfig_Effect effect, Action done = null)
        {
            TimeElapsed = 0;

            while (TimeElapsed < effect.Duration)
            {
                TimeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            TimeElapsed = 0;

            done?.Invoke();
        }

        private IEnumerator DelayCoroutine(IGameConfig_Effect effect, Action done = null)
        {
            TimeElapsed = 0;

            while (TimeElapsed < effect.Duration)
            {
                TimeElapsed += Time.deltaTime;

                yield return null;
            }

            TimeElapsed = 0;

            done?.Invoke();
        }
    }
}
