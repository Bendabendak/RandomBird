using SadJam;
using SadJam.Components;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Game
{
    public abstract class Prize : GameConfig, IGameConfig_Spawnable
    {
        [Serializable]
        public class PrizeData : SpawnableExtensions.SpawnableWithProbability
        {

        }

        [BlendableField("Prefabs"), SerializeField]
        private List<PrizeData> _prefabs;
        [BlendableProperty("Prefabs"),
        BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public List<PrizeData> Prefabs { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs
        {
            get
            {
                foreach (PrizeData rD in Prefabs)
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

        public abstract void CollectPrize(Statistics.Owner owner, Statistics_Key balanceKey);
        public abstract void CollectPrize(Statistics.Owner owner, string balanceId, bool saveBalanceOnDevice);
    }
}
