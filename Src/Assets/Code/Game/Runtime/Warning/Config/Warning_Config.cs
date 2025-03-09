using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Warning Config", menuName = "Game/Warning/Config")]
    public class Warning_Config : GameConfig, IGameConfig_Spawnable
    {
        [Serializable]
        public class WarningData : SpawnableExtensions.SpawnableWithProbability
        {

        }

        [BlendableField("Prefabs"), SerializeField]
        private List<WarningData> _prefabs;
        [BlendableProperty("Prefabs"),
        BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public List<WarningData> Prefabs { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs
        {
            get
            {
                foreach (WarningData rD in Prefabs)
                {
                    yield return rD.Prefab;
                }
            }
        }

        [BlendableField("Duration"), Space, SerializeField]
        private float _duration;
        [BlendableProperty("Duration")]
        public float Duration { get; set; }

        [NonSerialized]
        private Game.Random.Identifier _rndId;
        protected override void OnConfigResetedToDefault()
        {
            _rndId = Game.Random.GetIdentifier();
        }

        public GameObject Spawn(GameObject caller) => SpawnableExtensions.Spawn(Prefabs, _rndId)?.Prefab;
    }
}
