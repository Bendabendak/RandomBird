using Mirror;
using SadJam.Components;
using System;

namespace Game
{
    public class ClientNotHostOnly : SadJam.Component
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

            Game.NetworkManager.OnClientStarted -= OnClientStarted;
            Game.NetworkManager.OnClientStarted += OnClientStarted;
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
            Game.NetworkManager.OnClientStarted -= OnClientStarted;
        }

        private void OnClientStarted()
        {
            CheckActive();
        }

        private void OnServerStarted()
        {
            CheckActive();
        }

        private void CheckActive()
        {
            if (_isPartOfSpawnPool)
            {
                if (!NetworkServer.active && NetworkClient.active)
                {
                    if (_poolEntityInfo.IsNotEnabled)
                    {
                        SpawnPool.Activate(_poolEntityInfo);
                    }
                }
                else
                {
                    if (_poolEntityInfo.IsNotDisabled)
                    {
                        SpawnPool.Destroy(_poolEntityInfo);
                    }
                }
            }
            else
            {
                if (!NetworkServer.active && NetworkClient.active)
                {
                    if (!gameObject.activeSelf)
                    {
                        gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (gameObject.activeSelf)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
