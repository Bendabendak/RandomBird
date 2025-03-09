using Mirror;
using SadJam;
using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class NetworkManager : Mirror.NetworkManager
    {
        private struct MakeThisClientNotReadyMessage : NetworkMessage
        {

        }

        private struct StartGameMessage : NetworkMessage
        {
            public long StartTime;
            public bool SoloStart;
        }

        [field: SerializeField]
        public float StartGameWaitTime { get; private set; } = 3;

        [field: NonSerialized]
        public static float StartGameCounter { get; private set; } = 0;

        public static Action OnClientConnectedEvent { get; set; }
        public static Action OnServerStarted { get; set; }
        public static Action OnClientStarted { get; set; }
        public static Action OnGameStart { get; set; }
        public static Action OnGameStarted { get; set; }
        public static Action OnGameStartFailed { get; set; }

        public static bool ReadyToEnterTheGame { get; private set; } = false;

        public static void SetReadyToEnterTheGame()
        {
            Debug.Log("Ready to enter the game");

            ReadyToEnterTheGame = true;

            if (NetworkClient.active && !NetworkClient.ready)
            {
                NetworkClient.Ready();
            }
        }

        public static void SetNotReadyToEnterTheGame()
        {
            Debug.Log("Not ready to enter the game");

            ReadyToEnterTheGame = false;

            if (NetworkClient.active)
            {
                NetworkClient.Send(new MakeThisClientNotReadyMessage());
            }
        }

        private static void ServerOnClientNotReadyMessageReceived(NetworkConnectionToClient conn, MakeThisClientNotReadyMessage msg)
        {
            NetworkServer.SetClientNotReady(conn);
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();

            OnClientConnectedEvent?.Invoke();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.ReplaceHandler<MakeThisClientNotReadyMessage>(ServerOnClientNotReadyMessageReceived);

            OnServerStarted?.Invoke();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            NetworkClient.ReplaceHandler<StartGameMessage>(OnClientStartGame);

            OnClientStarted?.Invoke();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            Debug.Log("Client add player");
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);

            Debug.Log("Client disconnected");
        }

        [ServerCallback]
        public static void StartGame()
        {
            if (!NetworkServer.active || !ReadyToEnterTheGame) return;

            int readyConnections = 0;
            foreach(NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (conn.isReady)
                {
                    readyConnections++;
                }
            }

            NetworkServer.SendToReady(new StartGameMessage() { StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), SoloStart = readyConnections <= 1 });
        }

        private static bool _isStarting = false;
        private static void OnClientStartGame(StartGameMessage msg)
        {
            Debug.Log("Client start game " + ReadyToEnterTheGame);

            if (_isStarting || !ReadyToEnterTheGame) return;
            _isStarting = true;

            OnGameStart?.Invoke();

            if (msg.SoloStart)
            {
                _isStarting = false;

                StartGameCounter = 0;
                OnGameStarted?.Invoke();
            }
            else
            {
                StaticCoroutine.Start(StartGameCoroutine(msg.StartTime));
            }
        }

        private static IEnumerator StartGameCoroutine(long gameStartTime)
        {
            float startWaitTime;
            if (NetworkManager.singleton is not Game.NetworkManager networkManager)
            {
                startWaitTime = 3;
            }
            else
            {
                startWaitTime = networkManager.StartGameWaitTime;
            }

            long timeNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            float diff = (timeNow - gameStartTime) / 1000f;

            StartGameCounter = startWaitTime - diff;
            if (StartGameCounter < 0)
            {
                NetworkClient.Disconnect();

                StartGameCounter = 0;
                _isStarting = false;

                OnGameStartFailed?.Invoke();
                OnGameStarted?.Invoke();

                yield break;
            }

            while (StartGameCounter > 0)
            {
                StartGameCounter -= Time.deltaTime;

                yield return null;
            }

            StartGameCounter = 0;
            _isStarting = false;

            OnGameStarted?.Invoke();
        }
    }
}
