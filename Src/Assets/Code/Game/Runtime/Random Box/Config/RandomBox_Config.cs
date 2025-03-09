using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "RandomBox Config", menuName = "Game/RandomBox/Config")]
    public class RandomBox_Config : GameConfig, IGameConfig_Spawnable
    {
        [Serializable]
        public class RandomBoxData : SpawnableExtensions.SpawnableWithProbability
        {

        }

        [BlendableField("Prefabs"), SerializeField]
        private List<RandomBoxData> _prefabs;
        [BlendableProperty("Prefabs"),
        BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public List<RandomBoxData> Prefabs { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs 
        {
            get
            {
                foreach(RandomBoxData rD in Prefabs)
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
    }
}
