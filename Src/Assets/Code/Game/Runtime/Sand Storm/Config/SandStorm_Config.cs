using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Sand Storm Config", menuName = "Game/SandStorm/Config")]
    public class SandStorm_Config : GameConfig, IGameConfig_Toggleable, IGameConfig_HorizontalDirectional, IGameConfig_Spawnable
    {
        [Serializable]
        public class SandStormData : SpawnableExtensions.SpawnableWithProbability
        {

        }

        [BlendableField("Enabled"), SerializeField]
        private bool _enabled;
        [BlendableProperty("Enabled")]
        public bool Enabled { get; set; }

        [BlendableField("Prefabs"), Space, SerializeField]
        private List<SandStormData> _prefabs;
        [BlendableProperty("Prefabs"),
        BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public List<SandStormData> Prefabs { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs
        {
            get
            {
                foreach (SandStormData rD in Prefabs)
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

        [BlendableField("HorizontalDirection"), Space, Range(-1f, 1f), SerializeField]
        private int _horizontalDirection;
        [BlendableProperty("HorizontalDirection")]
        public int HorizontalDirection { get; set; }
    }
}
