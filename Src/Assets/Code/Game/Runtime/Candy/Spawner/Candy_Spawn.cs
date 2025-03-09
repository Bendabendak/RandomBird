using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Candy/Spawn")]
    public class Candy_Spawn : SpawnPool
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: Space]
        [GameConfigSerializeProperty]
        public Candy_Config Config { get; }

        [GameConfigSerializeProperty]
        public Progress_Config ProgressConfig { get; }

        [OnGameConfigChanged(nameof(Config))]
        private void OnConfigChanged(string affected)
        {
            if (GameConfig.IsFieldAffected(affected, nameof(Candy_Config.Candies)))
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
        private List<GameObject> _candyPrefabs = new();
        private void Reload()
        {
            CacheCandies();

            ChangePrefabList(_candyPrefabs);
        }

        private void ReloadImmediate()
        {
            CacheCandies();

            ChangePrefabListImmediate(_candyPrefabs);
        }

        private void CacheCandies()
        {
            _candyPrefabs.Clear();

            foreach (Candy_Config.CandyData c in Config.Candies)
            {
                _candyPrefabs.Add(c.Candy.Prefab);
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            Config.Threshold = ProgressConfig.CurrentTime;

            Candy_Config.CandyData target = null;
            foreach (Candy_Config.CandyData c in Config.Candies)
            {
                if (c.SpawnThreshold >= Config.Threshold)
                {
                    target = c;
                    break;
                }
            }

            if (target == null)
            {
                if (Config.Candies.Count <= 0) return;

                target = Config.Candies[Config.Candies.Count - 1];
            }

            Spawn(target.Candy.Prefab);
        }
    }
}
