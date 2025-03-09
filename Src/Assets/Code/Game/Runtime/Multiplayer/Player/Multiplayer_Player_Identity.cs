using Mirror;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class Multiplayer_Player_Identity : SadJam.Component
    {
        private struct GetConnectionIdMessage : NetworkMessage
        {
            public uint NetId;
        }

        private struct ReceiveConnectionIdMessage : NetworkMessage
        {
            public bool IsHost;
            public int ConnectionId;
            public uint NetId;
        }

        public static List<Multiplayer_Player_Identity> PlayerIdentities { get; private set; } = new();
        public static Action<Multiplayer_Player_Identity> OnIdentityAdded { get; set; }
        public static Action<Multiplayer_Player_Identity> OnIdentityRemoved { get; set; }
        private static Action<ReceiveConnectionIdMessage> _onNewClientConnectionIdAdded;

        public NetworkIdentity NetworkIdentity { get; private set; }
        public Multiplayer_Player_Owner OwnedBy { get; private set; }
        public int ConnectionId { get; private set; }
        public Action OnInitialized { get; set; }
        public bool IsInitialized { get; private set; } = false;
        public bool IsHost { get; private set; } = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            Game.NetworkManager.OnServerStarted += OnServerStarted;
            Game.NetworkManager.OnClientStarted += OnClientStarted;
        }

        private static void OnServerStarted()
        {
            NetworkServer.ReplaceHandler<GetConnectionIdMessage>(ServerOnGetConnectionIdFromServerReceived);
        }

        private static void OnClientStarted()
        {
            NetworkClient.ReplaceHandler<ReceiveConnectionIdMessage>(ClientOnConnectionIdReceived);
        }

        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            NetworkIdentity = GetComponent<NetworkIdentity>();
        }

        protected override void Start()
        {
            base.Start();

            if (!NetworkClient.active)
            {
                SpawnPool.Destroy(gameObject);
                return;
            }

            _onNewClientConnectionIdAdded -= ClientOnNewConnectionIdAddedLocal;
            _onNewClientConnectionIdAdded += ClientOnNewConnectionIdAddedLocal;

            SendGetConnectionIdFromServer(NetworkIdentity.netId);

            DontDestroyOnLoad(this);
        }

        private void ClientOnNewConnectionIdAddedLocal(ReceiveConnectionIdMessage msg)
        {
            if (NetworkIdentity != null && NetworkIdentity.netId == msg.NetId)
            {
                foreach(Multiplayer_Player_Identity id in PlayerIdentities)
                {
                    if (id == null || id.NetworkIdentity == null) continue;

                    if (id.NetworkIdentity.netId == msg.NetId) return;
                }

                IsHost = msg.IsHost;
                ConnectionId = msg.ConnectionId;

                PlayerIdentities.Add(this);
                IsInitialized = true;

                OnInitialized?.Invoke();
                OnIdentityAdded?.Invoke(this);
            }
        }

        private static void SendGetConnectionIdFromServer(uint netId)
        {
            NetworkClient.Send(new GetConnectionIdMessage { NetId = netId });
        }

        private static void ClientOnConnectionIdReceived(ReceiveConnectionIdMessage msg)
        {
            _onNewClientConnectionIdAdded?.Invoke(msg);
        }

        private static void ServerOnGetConnectionIdFromServerReceived(NetworkConnectionToClient conn, GetConnectionIdMessage msg)
        {
            foreach (NetworkConnectionToClient cConn in NetworkServer.connections.Values)
            {
                if (cConn.identity.netId == msg.NetId)
                {
                    bool isHost = NetworkServer.activeHost && NetworkServer.localConnection != null && NetworkServer.localConnection.identity != null && NetworkServer.localConnection.identity.netId == msg.NetId;
                    conn.Send(new ReceiveConnectionIdMessage { NetId = msg.NetId, ConnectionId = cConn.connectionId, IsHost = isHost });
                    return;
                }
            }

            Debug.LogWarning($"Connection with netId {msg.NetId} not found in connection list!");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _onNewClientConnectionIdAdded -= ClientOnNewConnectionIdAddedLocal;

            if (OwnedBy != null)
            {
                OwnedBy.RemoveIdentity();
            }

            if (IsInitialized)
            {
                PlayerIdentities.Remove(this);
                IsInitialized = false;

                OnIdentityRemoved?.Invoke(this);
            }
        }

        public void SetOwner(Multiplayer_Player_Owner owner)
        {
            if (OwnedBy != null)
            {
                if (OwnedBy.Identity == this) return;

                OwnedBy.RemoveIdentity();
            }

            OwnedBy = owner;
        }
    }
}
