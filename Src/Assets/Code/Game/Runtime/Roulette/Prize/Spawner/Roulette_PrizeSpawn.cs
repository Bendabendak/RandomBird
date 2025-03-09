using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Roulette/Prize/Spawn")]
    public class Roulette_PrizeSpawn : SpawnPool
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Roulette_Config Config { get; }

        [OnGameConfigChanged(nameof(Config))]
        private void OnConfigChanged(string affected)
        {
            if (GameConfig.IsFieldAffected(affected, nameof(Roulette_Config.Prizes)))
            {
                Reload();
            }
        }

        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            ReloadImmediate();
        }

        [NonSerialized]
        private List<GameObject> _prizePrefabs = new();
        private void Reload()
        {
            CachePrizes();

            ChangePrefabList(_prizePrefabs);
        }

        private void ReloadImmediate()
        {
            CachePrizes();

            ChangePrefabListImmediate(_prizePrefabs);
        }

        private void CachePrizes()
        {
            _prizePrefabs.Clear();

            foreach (Roulette_Config.PrizeData c in Config.Prizes)
            {
                foreach(GameObject p in c.Prize.Prefabs)
                {
                    _prizePrefabs.Add(p);
                }
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            foreach (Roulette_Config.PrizeData c in Config.Prizes)
            {
                Spawn(c.Prize.Spawn(Prefab));
            }
        }
    }
}
