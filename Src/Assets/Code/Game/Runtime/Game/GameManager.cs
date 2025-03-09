using System;
using System.Collections;
using Mirror;
using SadJam;
using UnityEngine;

namespace Game
{
    public static class GameManager
    {
        private struct SeedMessage : NetworkMessage
        {
            public int Seed;
            public string Id;
        }

        private struct SeedRequestMessage : NetworkMessage
        {

        }

        public const float SEED_RECEIVE_TIMEOUT = 3;

        public static IGameConfig_GameManager Config { get; private set; }
        public static Action OnConfigChanged { get; set; }

        private static Action _onSeedReceived;
        private static int? _seedReceived = null;
        private static string _seedReceivedId = "";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            Game.NetworkManager.OnServerStarted += OnServerStarted;
            Game.NetworkManager.OnClientStarted += OnClientStarted;
        }

        private static void OnClientStarted()
        {
            NetworkClient.ReplaceHandler<SeedMessage>(ClientOnSeedReceived);
        }

        private static void OnServerStarted()
        {
            NetworkServer.ReplaceHandler<SeedRequestMessage>(ServerOnSeedRequestReceived);
        }

        private static void ClientOnSeedReceived(SeedMessage msg)
        {
            _seedReceivedId = msg.Id;
            _seedReceived = msg.Seed;

            _onSeedReceived?.Invoke();
        }

        private static void ServerOnSeedRequestReceived(NetworkConnectionToClient conn, SeedRequestMessage msg)
        {
            conn.Send(new SeedMessage { Seed = Config.Seed, Id = System.Guid.NewGuid().ToString() });
        }

        private static string _lastLoadedSeedId = "";
        private static Coroutine _seedReceiveTimeoutCoroutine;
        public static void ChangeConfig(IGameConfig_GameManager config)
        {
            if (_seedReceiveTimeoutCoroutine != null)
            {
                StaticCoroutine.Stop(_seedReceiveTimeoutCoroutine);
            }

            Application.targetFrameRate = config.TargetFPS;

            Config = config;

            if (Config.RandomSeed)
            {
                Config.Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            }

            if (NetworkServer.active)
            {
                GameConfig.ResetAllToDefault();
                OnConfigChanged?.Invoke();
                return;
            }

            if (!NetworkClient.active)
            {
                Game.NetworkManager.OnClientStarted -= RetryConfigChange;
                Game.NetworkManager.OnClientStarted += RetryConfigChange;

                void RetryConfigChange()
                {
                    Game.NetworkManager.OnClientStarted -= RetryConfigChange;

                    ChangeConfig(config);
                }
                return;
            }

            if (_lastLoadedSeedId == _seedReceivedId)
            {
                _seedReceiveTimeoutCoroutine = StaticCoroutine.Start(SeedReceiveTimeoutCoroutine(Timeout));

                _onSeedReceived -= RetryConfigChange;
                _onSeedReceived += RetryConfigChange;

                NetworkClient.Send(new SeedRequestMessage());

                void RetryConfigChange()
                {
                    _onSeedReceived -= RetryConfigChange;

                    ChangeConfig(config);
                }

                void Timeout()
                {
                    if (NetworkClient.active)
                    {
                        Debug.LogWarning("Receiving seed from server timed out! Disconnecting...");
                        NetworkClient.Disconnect();
                    }

                    GameConfig.ResetAllToDefault();
                    OnConfigChanged?.Invoke();
                }

                return;
            }

            _lastLoadedSeedId = _seedReceivedId;

            Config.Seed = _seedReceived.Value;

            GameConfig.ResetAllToDefault();
            OnConfigChanged?.Invoke();
        }

        private static WaitForSeconds _waitForSeedReceiveTimeout = new(SEED_RECEIVE_TIMEOUT);
        private static IEnumerator SeedReceiveTimeoutCoroutine(Action timeout)
        {
            yield return _waitForSeedReceiveTimeout;

            timeout.Invoke();
        }
    }
}
