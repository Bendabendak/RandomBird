using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Roulette Config", menuName = "Game/Roulette/Config")]
    public class Roulette_Config : GameConfig, IGameConfig_Spawnable
    {
        [Serializable]
        public class PrizeData : SpawnableExtensions.SpawnableWithProbability
        {
            [Space]
            public Prize Prize;
        }

        [BlendableField("Prizes"), SerializeField]
        private List<PrizeData> _prizes;
        [BlendableProperty("Prizes"),
        BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public List<PrizeData> Prizes { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs
        {
            get
            {
                foreach (PrizeData rD in Prizes)
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

        public GameObject Spawn(GameObject caller) => SpawnableExtensions.Spawn(Prizes, _rndId)?.Prefab;

        [BlendableField("SaveKey"), Space, SerializeField]
        private Statistics_Key _saveKey;
        [BlendableProperty("SaveKey")]
        public Statistics_Key SaveKey { get; set; }

        [BlendableField("ResetInterval"), Space, Tooltip("Reset interval in seconds"), SerializeField]
        private int _resetInterval = 86400;
        [BlendableProperty("ResetInterval")]
        public int ResetInterval { get; set; }

        public bool Roll(Statistics.Owner owner)
        {
            DateTime now = DateTime.Now;

            if(!Statistics.LoadStatus(owner, SaveKey, out DateTime lastRollTime, out Statistics.ErrorCodes error))
            {
                switch (error)
                {
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to roll, because " + SaveKey.Id + " status is not type of DateTime!", this);
                        return false;
                    default:
                        Statistics.UpdateAndSaveStatistics(owner, new Statistics.DataEntry(SaveKey, now));
                        return false;
                }
            }

            TimeSpan resetInterval = new(0, 0, ResetInterval);

            if (lastRollTime.Add(resetInterval) < now) 
            {
                return Statistics.UpdateAndSaveStatistics(owner, new Statistics.DataEntry(SaveKey, now)); ;
            }

            return false;
        }

        public bool GetLastRoll(Statistics.Owner owner, out DateTime lastRoll)
        {
            DateTime startTime = DateTime.Now.AddSeconds(-Time.realtimeSinceStartupAsDouble);

            if (!Statistics.LoadStatus(owner, SaveKey, out lastRoll, out Statistics.ErrorCodes error))
            {
                switch (error)
                {
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to get last roll, because " + SaveKey.Id + " status is not type of DateTime!", this);
                        return false;
                    default:
                        lastRoll = startTime;
                        Statistics.UpdateAndSaveStatistics(owner, new Statistics.DataEntry(SaveKey, startTime));
                        return true;
                }
            }

            return true;
        }

        public void CollectPrize(Statistics.Owner owner, Prize prize, Statistics_Key balanceKey) => CollectPrize(owner, prize, balanceKey.Id, balanceKey.SaveOnDevice);
        public void CollectPrize(Statistics.Owner owner, Prize prize, string balanceId, bool saveBalanceOnDevice)
        {
            prize.CollectPrize(owner, balanceId, saveBalanceOnDevice);
        }
    }
}

