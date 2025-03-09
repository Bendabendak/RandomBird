using SadJam;
using System;
using UnityEngine;

namespace Game
{
    public class Multiplayer_Player_HostStatisticsOwner : GameConfig_Selector
    {
        [GameConfigSerializeProperty]
        public Statistics_Owner LocalOwner { get; }

        [field: NonSerialized]
        public Multiplayer_Player_Identity HostPlayerIdentity { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            Multiplayer_Player_Identity.OnIdentityAdded -= OnPlayerIdentityAdded;
            Multiplayer_Player_Identity.OnIdentityAdded += OnPlayerIdentityAdded;

            TrySetOwner();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Multiplayer_Player_Identity.OnIdentityAdded -= OnPlayerIdentityAdded;
        }

        private void OnPlayerIdentityAdded(Multiplayer_Player_Identity identity)
        {
            TrySetOwner();
        }

        private void TrySetOwner()
        {
            if (LocalOwner == null || (Config != null && HostPlayerIdentity != null && HostPlayerIdentity.IsHost)) 
            {
                return;
            }

            foreach(Multiplayer_Player_Identity identity in Multiplayer_Player_Identity.PlayerIdentities)
            {
                if (identity.IsHost && !identity.NetworkIdentity.isLocalPlayer)
                {
                    HostPlayerIdentity = identity;

                    Statistics_Owner hostStatisticsOwner = ScriptableObject.CreateInstance<Statistics_Owner>();
                    hostStatisticsOwner.Id = HostPlayerIdentity.ConnectionId.ToString();
                    hostStatisticsOwner.IsNetworkOwner = true;

                    Config = hostStatisticsOwner;

                    return;
                }
            }

            LocalOwner.IsNetworkOwner = false;
            Config = LocalOwner;
        }
    }
}
