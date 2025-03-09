using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Shop/SpawnShowableItems")]
    public class Shop_SpawnShowableItems : SpawnPool
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public IGameConfig_Shop_WithShowables ShopConfig { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [OnGameConfigChanged(nameof(ShopConfig))]
        private void OnConfigChanged(string affected)
        {
            if (GameConfig.IsFieldAffected(affected, nameof(IGameConfig_Shop_WithShowables.Items), nameof(IGameConfig_Shop_WithShowables)))
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
        private IGameConfig_Spawnable _lastChosen;
        protected override void DynamicExecutor_OnExecute()
        {
            foreach(GameObject prefab in _itemPrefabs)
            {
                Spawn(prefab);
            }
        }

        [NonSerialized]
        private List<GameObject> _itemPrefabs = new();
        private void Reload()
        {
            CachePrefabs();
            if (_itemPrefabs.Count <= 0) return;

            ChangePrefabList(_itemPrefabs);
        }

        private void ReloadImmediate()
        {
            CachePrefabs();
            if (_itemPrefabs.Count <= 0) return;

            ChangePrefabListImmediate(_itemPrefabs);
        }

        private void CachePrefabs()
        {
            _itemPrefabs.Clear();

            foreach(IGameConfig_Shop_Showable item in ShopConfig.Items)
            {
                _itemPrefabs.Add(item.Prefab);
            }
        }
    }
}

