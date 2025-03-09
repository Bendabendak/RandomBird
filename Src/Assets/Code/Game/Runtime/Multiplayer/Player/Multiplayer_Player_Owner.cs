using SadJam.Components;
using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class Multiplayer_Player_Owner : SadJam.Component
    {
        public Multiplayer_Player_Identity Identity { get; private set; }

        [field: NonSerialized]
        public Action<Multiplayer_Player_Identity> OnIdentityChanged { get; set; }

        public bool IsInitialized { get; private set; } = false;

        [NonSerialized]
        private bool _isPartOfSpawnPool = false;
        [NonSerialized]
        private SpawnPool.EntityInfo _spawnPoolData;

        protected override void OnStartAndEnable()
        {
            base.OnStartAndEnable();

            _destroying = false;

            _isPartOfSpawnPool = SpawnPool.GetEntityInfo(gameObject, out _spawnPoolData);

            foreach (Multiplayer_Player_Identity id in Multiplayer_Player_Identity.PlayerIdentities)
            {
                if (TrySetIdentity(id))
                {
                    break;
                }
            }

            IsInitialized = true;

            StartCoroutine(DestroyWhenIdentityNull());
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            StopAllCoroutines();

            IsInitialized = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _destroying = false;
        }

        [NonSerialized]
        private WaitForEndOfFrame _waitForEndOfFrame = new();
        [NonSerialized]
        private bool _destroying = false;
        private IEnumerator DestroyWhenIdentityNull()
        {
            while (true)
            {
                if (this == null || gameObject == null || !gameObject.activeSelf)
                {
                    yield break;
                }

                if (Identity == null)
                {
                    if (_isPartOfSpawnPool)
                    {
                        if (!_destroying && _spawnPoolData.IsNotDisabled)
                        {
                            _destroying = true;
                            SpawnPool.Destroy(gameObject);
                        }
                    }
                    else if (gameObject.activeSelf)
                    {
                        gameObject.SetActive(false);
                    }
                }

                yield return _waitForEndOfFrame;
            }
        }

        public void SetIdentity(Multiplayer_Player_Identity identity)
        {
            if (identity == null)
            {
                RemoveIdentity();
                return;
            }

            if (Identity == identity) return;

            Identity = identity;

            identity.SetOwner(this);

            if (_isPartOfSpawnPool)
            {
                if (_spawnPoolData.IsNotEnabled)
                {
                    SpawnPool.Activate(gameObject);
                }
            }
            else
            {
                gameObject.SetActive(true);
            }

            OnIdentityChanged?.Invoke(identity);
        }

        public void RemoveIdentity()
        {
            if (Identity == null) return;

            Identity = null;

            OnIdentityChanged?.Invoke(null);
        }

        private bool TrySetIdentity(Multiplayer_Player_Identity identity)
        {
            if (Identity != null)
            {
                return true;
            }

            if (Identity == null && identity != null && identity.IsInitialized && identity.OwnedBy == null)
            {
                SetIdentity(identity);
                return true;
            }

            return false;
        }
    }
}
