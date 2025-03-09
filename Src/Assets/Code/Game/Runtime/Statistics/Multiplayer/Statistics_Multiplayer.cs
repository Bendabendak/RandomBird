using Mirror;
using SadJam.Components;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game
{
    public static class Statistics_Multiplayer
    {
        private struct UpdateStatisticsMessage : NetworkMessage
        {
            public bool IsLoading;
            public string DataId;
            public string DataValue;
        }

        private struct ReceiveStatisticsMessage : NetworkMessage
        {
            public bool IsLoading;
            public int ConnectionId;
            public string DataId;
            public string DataValue;
        }

        public static Action<int, Statistics.DataEntry> OnChanged { get; set; }

        private static Dictionary<int, Dictionary<string, object>> _statisticsData = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            Game.NetworkManager.OnServerStarted += OnServerStarted;
            Game.NetworkManager.OnClientStarted += OnClientStarted;

            if (NetworkClient.active)
            {
                LoadStatistics();
            }

            Game.NetworkManager.OnClientConnectedEvent += OnClientConnected;
        }

        private static void OnServerStarted()
        {
            NetworkServer.ReplaceHandler<UpdateStatisticsMessage>(ServerOnUpdateStatisticsReceived);
        }

        private static void OnClientStarted() 
        {
            NetworkClient.ReplaceHandler<ReceiveStatisticsMessage>(ClientOnStatisticsReceived);
        }

        private static void OnClientConnected()
        {
            LoadStatistics();
        }

        private static void LoadStatistics()
        {
            if (!NetworkClient.active) return;

            string dirPath = Path.Combine(DeviceSave.SavePath, Statistics.SAVE_PATH);
            string searchPattern = "*" + Statistics.FILE_EXTENSION;

            if (Directory.Exists(dirPath))
            {
                foreach(string file in Directory.GetFiles(dirPath, searchPattern, SearchOption.TopDirectoryOnly))
                {
                    string fileName = Path.GetFileName(file);

                    DeviceSave.ErrorCodes error = DeviceSave.Load(Path.Combine(Statistics.SAVE_PATH, fileName), out object d);

                    if (d is not string dataRaw || error != DeviceSave.ErrorCodes.None)
                    {
                        continue;
                    }

                    Statistics.DeserializeStatisticsData(dataRaw, out Dictionary<string, object> data);

                    foreach(KeyValuePair<string, object> pair in data)
                    {
                        SendStatisticsToServerFromLoad(pair.Key, Statistics.SerializeStatisticsDataValue(pair.Value));
                    }

                    break;
                }
            }
        }

        public static bool TryGetStatisticsData(Statistics.Owner owner, out Dictionary<string, object> data)
        {
            if (!owner.IsNetworkOwner)
            {
                return Statistics.LoadStatistics(owner, out data);
            }

            if (!int.TryParse(owner.Id, out int connectionId))
            {
                data = default;
                return false;
            }

            return _statisticsData.TryGetValue(connectionId, out data);
        }

        public static bool TryGetStatisticsData(string ownerId, out Dictionary<string, object> data)
        {
            if (!int.TryParse(ownerId, out int connectionId))
            {
                return Statistics.LoadStatistics(new() { Id = ownerId, IsNetworkOwner = false }, out data);
            }

            return _statisticsData.TryGetValue(connectionId, out data);
        }

        public static bool TryGetStatisticsData(int connectionId, out Dictionary<string, object> data)
        {
            return _statisticsData.TryGetValue(connectionId, out data);
        }

        public static void UpdateStatisticsData(Statistics.DataEntry data)
        {
            if (!NetworkClient.active) return;

            SendStatisticsToServer(data.Id, Statistics.SerializeStatisticsDataValue(data.Value));
        }

        private static void SendStatisticsToServerFromLoad(string dataId, string dataValue)
        {
            NetworkClient.Send(new UpdateStatisticsMessage { DataId = dataId, DataValue = dataValue, IsLoading = true });
        }

        private static void SendStatisticsToServer(string dataId, string dataValue)
        {
            NetworkClient.Send(new UpdateStatisticsMessage { DataId = dataId, DataValue = dataValue, IsLoading = false });
        }

        private static void ClientOnStatisticsReceived(ReceiveStatisticsMessage msg)
        {
            if (!_statisticsData.TryGetValue(msg.ConnectionId, out Dictionary<string, object> statistics))
            {
                statistics = new();
                _statisticsData[msg.ConnectionId] = statistics;
            }

            object dataDes = Statistics.DeserializeStatisticsDataValue(msg.DataValue);
            statistics[msg.DataId] = dataDes;

            if (!msg.IsLoading)
            {
                OnChanged?.Invoke(msg.ConnectionId, new(msg.DataId, dataDes, false));
            }
        }

        private static void ServerOnUpdateStatisticsReceived(NetworkConnectionToClient conn, UpdateStatisticsMessage msg)
        {
            NetworkServer.SendToAll(new ReceiveStatisticsMessage { DataId = msg.DataId, DataValue = msg.DataValue, ConnectionId = conn.connectionId, IsLoading = msg.IsLoading });
        }
    }
}
