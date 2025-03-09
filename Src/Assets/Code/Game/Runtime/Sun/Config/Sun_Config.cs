using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Game
{
    [CreateAssetMenu(fileName = "Sun Config", menuName = "Game/Sun/Config")]
    public class Sun_Config : GameConfig, IGameConfig_Spawnable, IGameConfig_SkinnableFromSpriteLibraryAsset, IGameConfig_Toggleable
    {
        [Serializable]
        public class SunData : SpawnableExtensions.SpawnableWithProbability
        {
            
        }

        [BlendableField("Enabled"), SerializeField]
        private bool _enabled = true;
        [BlendableProperty("Enabled")]
        public bool Enabled { get; set; }

        [BlendableField("Prefabs"), Space, SerializeField]
        private List<SunData> _prefabs;
        [BlendableProperty("Prefabs"),
        BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public List<SunData> Prefabs { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs
        {
            get
            {
                foreach (SunData rD in Prefabs)
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

        public GameObject Spawn(GameObject caller) => SpawnableExtensions.Spawn(Prefabs, _rndId)?.Prefab;

        [BlendableField("Skin"), Space, SerializeField]
        private SpriteLibraryAsset _skin;
        [BlendableProperty("Skin")]
        public SpriteLibraryAsset Skin { get; set; }

        [BlendableField("DayInterval"), Space, SerializeField]
        private float _dayInterval = 10f;
        [BlendableProperty("DayInterval")]
        public float DayInterval { get; set; }

        [BlendableField("NightInterval"), SerializeField]
        private float _nightInterval = 5f;
        [BlendableProperty("NightInterval")]
        public float NightInterval { get; set; }

        [BlendableField("LapCurve"), SerializeField]
        private AnimationCurve _lapCurve;
        [BlendableProperty("LapCurve")]
        public AnimationCurve LapCurve { get; set; }
    }
}
