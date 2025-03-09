using System;
using UnityEngine;

namespace SadJam.Components
{
    public class IndependentObject : SadJam.Component
    {
        [NonSerialized]
        private bool _isPartOfSpawnPool = false;
        [NonSerialized]
        private SpawnPool.EntityInfo _spawnPoolEntityInfo;
        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            transform.SetParent(null, false);

            _isPartOfSpawnPool = GetSpawnPoolEntityInfo(gameObject, out _spawnPoolEntityInfo);

            if (_isPartOfSpawnPool)
            {
                if (_spawnPoolEntityInfo.IsNotEnabled)
                {
                    gameObject.SetActive(false);
                }

                _spawnPoolEntityInfo.OnAboutToActivate -= OnEntityAboutToActivate;
                _spawnPoolEntityInfo.OnAboutToActivate += OnEntityAboutToActivate;

                _spawnPoolEntityInfo.OnDisabled -= OnEntityDisabled;
                _spawnPoolEntityInfo.OnDisabled += OnEntityDisabled;

                _spawnPoolEntityInfo.OnCleared -= OnEntityCleard;
                _spawnPoolEntityInfo.OnCleared += OnEntityCleard;
            }
        }

        private void OnEntityAboutToActivate()
        {
            gameObject.SetActive(true);
        }

        private void OnEntityDisabled()
        {
            gameObject.SetActive(false);
        }

        private void OnEntityCleard()
        {
            Destroy(gameObject);
        }

        private static bool GetSpawnPoolEntityInfo(GameObject obj, out SpawnPool.EntityInfo info)
        {
            if (SpawnPool.GetEntityInfo(obj, out info)) return true;

            if (obj.transform.parent == null)
            {
                info = default;
                return false;
            }

            return GetSpawnPoolEntityInfo(obj.transform.parent.gameObject, out info);
        }
    }
}
