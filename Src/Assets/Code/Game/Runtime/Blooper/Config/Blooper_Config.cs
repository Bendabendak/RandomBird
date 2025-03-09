using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Blooper Config", menuName = "Game/Blooper/Config")]
    public class Blooper_Config : GameConfig, IGameConfig_Toggleable, IGameConfig_Spawnable
    {
        [Serializable]
        public class BlooperData : SpawnableExtensions.SpawnableWithProbability
        {

        }

        [BlendableField("Enabled"), SerializeField]
        private bool _enabled;
        [BlendableProperty("Enabled")]
        public bool Enabled { get; set; }

        [BlendableField("Prefabs"), Space, SerializeField]
        private List<BlooperData> _prefabs;
        [BlendableProperty("Prefabs"),
        BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public List<BlooperData> Prefabs { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs
        {
            get
            {
                foreach (BlooperData rD in Prefabs)
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
