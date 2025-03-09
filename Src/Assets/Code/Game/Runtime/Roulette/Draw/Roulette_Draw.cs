using SadJam;
using SadJam.Components;
using System;
using System.Collections;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Roulette/Draw")]
    public class Roulette_Draw : SpawnPool
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: Space]
        [GameConfigSerializeProperty]
        public Roulette_Config Config { get; }

        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: Space,SerializeField]
        public GameObject Wheel { get; private set; }

        [field: Space, SerializeField]
        public bool Clockshape { get; private set; }

        [field: Space, SerializeField]
        public int NumberOfSpins { get; private set; } = 1;
        [field: SerializeField]
        public int Speed { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (!Config.Roll(Owner)) return;

            StartCoroutine(DrawCoroutine((Roulette_Config.PrizeData chosen) =>
            {
                if (Spawn(chosen.Prefab))
                {
                    Execute(Delta);
                }
            }));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            StopAllCoroutines();
        }

        private IEnumerator DrawCoroutine(Action<Roulette_Config.PrizeData> done = null)
        {
            GameObject prefabToSpawn = ((IGameConfig_Spawnable)Config).Spawn(gameObject);

            Roulette_Config.PrizeData target = null;
            foreach (Roulette_Config.PrizeData e in Config.Prizes)
            {
                if (e.Prefab == prefabToSpawn)
                {
                    target = e;
                    break;
                }
            }

            if (target == null) yield break;

            Wheel.transform.eulerAngles = new(Wheel.transform.eulerAngles.x, Wheel.transform.eulerAngles.y, 0);

            float angleAddUp = 360f / Config.Prizes.Count;
            float shift = 0;

            if (Clockshape)
            {
                shift = angleAddUp / 2f;
            }

            float time = Time.deltaTime;
            float targetAngle = shift + (Config.Prizes.IndexOf(target) * angleAddUp) + (NumberOfSpins * 360f);
            float lerp = 0;
            while (lerp < targetAngle)
            {
                lerp = Mathf.SmoothStep(0, targetAngle, time / 100f * Speed);
                Wheel.transform.eulerAngles = new(Wheel.transform.eulerAngles.x, Wheel.transform.eulerAngles.y, lerp);

                time += Time.deltaTime;
                yield return null;
            }

            Wheel.transform.eulerAngles = new(Wheel.transform.eulerAngles.x, Wheel.transform.eulerAngles.y, targetAngle);

            done?.Invoke(target);
        }
    }
}
