using SadJam;
using System;
using UnityEngine;

namespace Game
{
    public class Multiplayer_Player_StatisticsOwner : GameConfig_Selector
    {
        [field: SerializeField]
        public Multiplayer_Player_Owner PlayerOwner { get; private set; }
        [field: SerializeField]
        public bool GetOwnerFromParent { get; private set; }

        [field: NonSerialized]
        public Action<Statistics_Owner> OnOwnerChanged { get; set; }

        public bool IsInitialized { get; private set; } = false;

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner LocalOwner { get; }

        protected override void OnStartAndEnable()
        {
            base.OnStartAndEnable();

            PlayerOwner.OnIdentityChanged -= OnIdentityChanged;
            PlayerOwner.OnIdentityChanged += OnIdentityChanged;

            if (PlayerOwner.IsInitialized)
            {
                TrySetOwner();
            }

            IsInitialized = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerOwner.OnIdentityChanged -= OnIdentityChanged;

            IsInitialized = false;
        }

        private void OnIdentityChanged(Multiplayer_Player_Identity identity)
        {
            TrySetOwner();
        }

        public void TrySetOwner()
        {
            if (GetOwnerFromParent && (PlayerOwner == null || !transform.IsChildOf(PlayerOwner.transform)))
            {
                PlayerOwner = GetComponentInParent<Multiplayer_Player_Owner>();
            }

            Debug.Log($"try set owner {gameObject.name}", gameObject);

            if (PlayerOwner == null || PlayerOwner.Identity == null || !PlayerOwner.Identity.IsInitialized || PlayerOwner.Identity.NetworkIdentity == null)
            {
                return;
            }

            if (PlayerOwner.Identity.NetworkIdentity.isLocalPlayer)
            {
                SetAsLocalOwner();

                return;
            }

            SetAsNetworkOwner(PlayerOwner.Identity.ConnectionId.ToString());
        }

        private void SetAsLocalOwner()
        {
            if (Config != LocalOwner)
            {
                Debug.Log("local connection", gameObject);

                LocalOwner.IsNetworkOwner = false;
                Config = LocalOwner;

                OnOwnerChanged?.Invoke(LocalOwner);
            }
        }

        private void SetAsNetworkOwner(string connectionId)
        {
            Debug.Log($"new connection {connectionId}", gameObject);

            Statistics_Owner playerStatisticsOwner = ScriptableObject.CreateInstance<Statistics_Owner>();
            playerStatisticsOwner.Id = connectionId;
            playerStatisticsOwner.IsNetworkOwner = true;

            Config = playerStatisticsOwner;

            OnOwnerChanged?.Invoke(playerStatisticsOwner);
        }
    }
}
