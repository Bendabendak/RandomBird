using Mirror;
using SadJam.Components;
using System;

namespace Game
{
    public class ServerOnly : SadJam.Component
    {
        [NonSerialized]
        private SpawnPool.EntityInfo _poolEntityInfo;
        [NonSerialized]
        private bool _isPartOfSpawnPool = false;

        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            _isPartOfSpawnPool = SpawnPool.GetEntityInfo(gameObject, out _poolEntityInfo);

            Game.NetworkManager.OnServerStarted -= OnServerStarted;
            Game.NetworkManager.OnServerStarted += OnServerStarted;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            CheckActive();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Game.NetworkManager.OnServerStarted -= OnServerStarted;
        }

        private void OnServerStarted()
        {
            CheckActive();
        }

        private void CheckActive()
        {
            if (_isPartOfSpawnPool)
            {
                if (!NetworkServer.active)
                {
                    if (_poolEntityInfo.IsNotDisabled)
                    {
                        SpawnPool.Destroy(_poolEntityInfo);
                    }
                }
                else
                {
                    if (_poolEntityInfo.IsNotEnabled)
                    {
                        SpawnPool.Activate(_poolEntityInfo);
                    }
                }
            }
            else
            {
                if (!NetworkServer.active)
                {
                    if (gameObject.activeSelf)
                    {
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (!gameObject.activeSelf)
                    {
                        gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
