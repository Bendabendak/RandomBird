using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Shop/SpawnFirstChosen")]
    public class Shop_SpawnFirstChosen : SpawnPool
    {
        [GameConfigSerializeProperty]
        public IGameConfig_Shop_WithChoosables ShopConfig { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [OnGameConfigChanged(nameof(ShopConfig))]
        private void OnConfigChanged(string affected)
        {
            if (GameConfig.IsFieldAffected(affected, nameof(IGameConfig_Shop_WithChoosables.Items), nameof(IGameConfig_Shop_WithChoosables)))
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
        private List<GameObject> _prefabs = new();
        private void Reload()
        {
            CacheItems();

            ChangePrefabList(_prefabs);
        }

        private void ReloadImmediate()
        {
            CacheItems();

            ChangePrefabListImmediate(_prefabs);
        }

        private void CacheItems()
        {
            _prefabs.Clear();

            foreach (IGameConfig_Shop_Choosable c in ShopConfig.Items)
            {
                if (c is not IGameConfig_Spawnable s) continue;

                foreach(GameObject p in s.Prefabs)
                {
                    _prefabs.Add(p);
                }
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            IEnumerable<IGameConfig_Shop_Choosable> chosenList = ShopConfig.GetChosen(Owner);
            if (chosenList == null) return;

            IGameConfig_Spawnable chosen = null;
            foreach(IGameConfig_Shop_Choosable c in chosenList)
            {
                if (c is IGameConfig_Spawnable s)
                {
                    chosen = s;
                    break;
                }
            }
            if (chosen == null) return;

            GameObject prefab = chosen.Spawn(gameObject);

            Spawn(prefab);
        }
    }
}
