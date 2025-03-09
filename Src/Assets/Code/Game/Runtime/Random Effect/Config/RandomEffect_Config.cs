using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "RandomEffect Config", menuName = "Game/RandomEffect/Config")]
    public class RandomEffect_Config : GameConfig, IGameConfig_Spawnable
    {
        [Serializable]
        public class RandomEffectData : SpawnableExtensions.SpawnableWithProbability
        {
            [Space]
            public Effect Effect;
        }

        [BlendableField("Effects"), SerializeField]
        private List<RandomEffectData> _effects;
        [BlendableProperty("Effects"),
        BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public List<RandomEffectData> Effects { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs
        {
            get
            {
                foreach (RandomEffectData rD in Effects)
                {
                    yield return rD.Prefab;
                }
            }
        }

        [NonSerialized]
        private Game.Random.Identifier _rndId;
        protected override void OnConfigResetedToDefault()
        {
            _rndId = Game.Random.GetIdentifier();
        }

        public GameObject Spawn(GameObject caller) => SpawnableExtensions.Spawn(Effects, _rndId)?.Prefab;

        public void DisableAllEffects(IEnumerable<RandomEffectData> except = null)
        {
            if (except == null)
            {
                foreach (RandomEffectData e in Effects)
                {
                    e.Effect.Enabled = false;
                }
            }
            else
            {
                foreach (RandomEffectData e in Effects)
                {
                    bool skip = false;
                    foreach (RandomEffectData d in except)
                    {
                        if (d == e)
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (skip) continue;

                    e.Effect.Enabled = false;
                }
            }
        }

        public void DisableAllEffects(IEnumerable<Effect> except = null)
        {
            if (except == null)
            {
                foreach (RandomEffectData e in Effects)
                {
                    e.Effect.Enabled = false;
                }
            }
            else
            {
                foreach (RandomEffectData e in Effects)
                {
                    bool skip = false;
                    foreach (Effect d in except)
                    {
                        if (d == e.Effect)
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (skip) continue;

                    e.Effect.Enabled = false;
                }
            }
        }

        public void EnableAllEffects(IEnumerable<RandomEffectData> except = null)
        {
            if (except == null)
            {
                foreach (RandomEffectData e in Effects)
                {
                    e.Effect.Enabled = true;
                }
            }
            else
            {
                foreach (RandomEffectData e in Effects)
                {
                    bool skip = false;
                    foreach (RandomEffectData d in except)
                    {
                        if (d == e)
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (skip) continue;

                    e.Effect.Enabled = true;
                }
            }
        }

        public void EnableAllEffects(IEnumerable<Effect> except = null)
        {
            if (except == null)
            {
                foreach (RandomEffectData e in Effects)
                {
                    e.Effect.Enabled = true;
                }
            }
            else
            {
                foreach(RandomEffectData e in Effects)
                {
                    bool skip = false;
                    foreach(Effect d in except)
                    {
                        if (d == e.Effect)
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (skip) continue;

                    e.Effect.Enabled = true;
                }
            }
        }
    }
}
