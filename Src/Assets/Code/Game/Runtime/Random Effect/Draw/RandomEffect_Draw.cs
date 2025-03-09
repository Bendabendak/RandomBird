using SadJam;
using SadJam.Components;
using System;
using System.Collections;
using TypeReferences;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [ClassTypeAddress("Executor/Game/RandomEffect/Draw")]
    public class RandomEffect_Draw : SpawnPool
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public RandomEffect_Config Config { get; }

        [field: Space, SerializeField]
        public Image Image { get; protected set; }

        [field: Space, SerializeField]
        public AnimationClips EffectClip { get; protected set; }
        [field: SerializeField]
        public AnimationClips ChosenEffectClip { get; protected set; }

        [field: SerializeField, Space]
        public float DrawDuration { get; protected set; } = 5;

        [field: Space, SerializeField]
        public float SpeedMultiplier { get; private set; }
        [field: SerializeField]
        public AnimationCurve DrawSpeedCurve { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            StartCoroutine(DrawCoroutine((RandomEffect_Config.RandomEffectData chosen) =>
            {
                if (Spawn(chosen.Prefab))
                {
                    Execute(Delta);
                }
            }));
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            StopAllCoroutines();
        }

        private IEnumerator DrawCoroutine(Action<RandomEffect_Config.RandomEffectData> done = null)
        {
            if(Config.Effects.Count <= 0) yield break;

            int index = UnityEngine.Random.Range(0, Config.Effects.Count);

            Image.sprite = Config.Effects[index].Effect.Icon;
            float time = 0;
            RandomEffect_Config.RandomEffectData lastEffect;
            float lastSpeed;
            int i = 0;
            while (true)
            {
                if (index >= Config.Effects.Count)
                {
                    index = 0;
                }

                lastEffect = Config.Effects[index];

                Image.sprite = lastEffect.Effect.Icon;

                float length = Time.realtimeSinceStartup;
                float e = time / DrawDuration;

                lastSpeed = DrawSpeedCurve.Evaluate(e) * SpeedMultiplier;

                EffectClip.Play(this, lastSpeed);
                while (EffectClip.IsPlaying) yield return null;

                length = Time.realtimeSinceStartup - length;

                time += length;
                index++;
                i++;

                if (Time.timeScale > 0 && time + length > DrawDuration / Time.timeScale)
                {
                    break;
                }
            }

            GameObject prefabToSpawn = ((IGameConfig_Spawnable)Config).Spawn(gameObject);

            RandomEffect_Config.RandomEffectData target = null;
            foreach (RandomEffect_Config.RandomEffectData e in Config.Effects)
            {
                if (e.Prefab == prefabToSpawn)
                {
                    target = e;
                    break;
                }
            }

            if (target == null) yield break;

            RandomEffect_Config.RandomEffectData next = null;
            foreach (RandomEffect_Config.RandomEffectData e in Config.Effects)
            {
                if (e != target && e != lastEffect)
                {
                    next = e;
                    break;
                }
            }

            if (next != null)
            {
                Image.sprite = next.Effect.Icon;

                EffectClip.Play(this, lastSpeed);
                while (EffectClip.IsPlaying) yield return null;
            }

            Image.sprite = target.Effect.Icon;

            ChosenEffectClip.Play(this, lastSpeed);
            while (ChosenEffectClip.IsPlaying) yield return null;

            done?.Invoke(target);
        }
    }
}
