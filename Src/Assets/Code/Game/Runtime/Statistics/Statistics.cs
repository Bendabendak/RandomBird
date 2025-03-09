using Newtonsoft.Json;
using SadJam.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Game
{
    public static class Statistics
    {
        public const string SAVE_PATH = "Statistics";
        public const string FILE_EXTENSION = ".randomBird";

        public enum ErrorCodes
        {
            None,
            StatusNotFound,
            StatusFoundWithDifferentType,
            LoadFailed,
            SaveFailed,
            CannotUpdateNetworkOwner
        }

        [Serializable]
        private struct Data
        {
            public string TypeName;
            public string ValueRaw;

            [NonSerialized]
            private object _value;

            [JsonIgnore]
            public object Value 
            {
                get
                {
                    return _value;
                }
                set
                {
                    Type t = value.GetType();
                    TypeName = t.FullName + ", " + t.Assembly.FullName;

                    _value = value;
                }
            }

            public Data(object value)
            {
                _value = value;
                ValueRaw = "";

                Type t = value.GetType();
                TypeName = t.FullName + ", " + t.Assembly.FullName;
            }

            public void Serialize()
            {
                ValueRaw = JsonConvert.SerializeObject(Value);
            }

            public void Deserialize()
            {
                if (ValueRaw == "" || ValueRaw == null) return;

                Value = JsonConvert.DeserializeObject(ValueRaw, Type.GetType(TypeName));
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is not Data d) return false;

                return d.Value == Value;
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        public struct DataEntry
        {
            public string Id;
            public object Value;
            public bool SaveOnDevice;

            public DataEntry(string id, object value, bool saveOnDevice)
            {
                Id = id;
                Value = value;
                SaveOnDevice = saveOnDevice;
            }

            public DataEntry(Statistics_Key key, object value)
            {
                Id = key.Id;
                Value = value;
                SaveOnDevice = key.SaveOnDevice;
            }

            public bool VerifyNumeric(Statistics_Key key) => VerifyNumeric(key.Id);
            public bool VerifyNumeric(Statistics_Key key, out double value) => VerifyNumeric(key.Id, out value);
            public bool VerifyNumeric(string Id) => VerifyNumeric(Id, out _);
            public bool VerifyNumeric(string Id, out double value)
            {
                if (!double.TryParse(Value.ToString(), out value))
                {
                    return false;
                }

                return this.Id == Id;
            }

            public bool Verify(Statistics_Key key) => Verify(key.Id);
            public bool Verify<T>(Statistics_Key key) => Verify<T>(key.Id, out _);
            public bool Verify<T>(Statistics_Key key, out T value)=> Verify(key.Id, out value);
            public bool Verify(Statistics_Key key, Type type) => Verify(key.Id, type);
            public bool Verify(string Id)
            {
                return this.Id == Id;
            }
            public bool Verify<T>(string Id) => Verify<T>(Id, out _);
            public bool Verify<T>(string Id, out T value)
            {
                if (Value is not T vT)
                {
                    value = default;
                    return false;
                }

                value = vT;

                return this.Id == Id;
            }
            public bool Verify(string Id, Type type)
            {
                return this.Id == Id && Value.GetType() == type;
            }

            public static implicit operator KeyValuePair<string, object>(DataEntry v)
            {
                return new(v.Id, v.Value);
            }
        }

        public struct Owner
        {
            public string Id;
            public bool IsNetworkOwner;

            public Owner(string id, bool isNetworkOwner)
            {
                Id = id;
                IsNetworkOwner = isNetworkOwner;
            }
        }

        public static Action<string, DataEntry> OnChanged { get; set; }

        private static HashSet<Type> _isNumeric = new()
        {
            typeof(byte), typeof(sbyte), typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(Int16), typeof(Int32), typeof(Int64), typeof(Decimal), typeof(Double), typeof(Single)
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            Statistics_Multiplayer.OnChanged += OnStatisticsFromServerChanged;
        }

        private static Dictionary<int, string> _connectionIdsAsStringCache = new();
        private static void OnStatisticsFromServerChanged(int connectionId, DataEntry entry)
        {
            if (!_connectionIdsAsStringCache.TryGetValue(connectionId, out string connAsString))
            {
                connAsString = connectionId.ToString();
                _connectionIdsAsStringCache[connectionId] = connAsString;
            }

            OnChanged?.Invoke(connAsString, entry);
        }

        public static bool UpdateAndSaveStatistics(Owner owner, params DataEntry[] data) => UpdateAndSaveStatistics(owner, out _, data);
        public static bool UpdateAndSaveStatistics(Owner owner, out ErrorCodes error, params DataEntry[] data)
        {
            if (owner.IsNetworkOwner)
            {
                error = ErrorCodes.CannotUpdateNetworkOwner;
                return false;
            }

            DeviceSave.ErrorCodes loadError = Load(owner, out Dictionary<string, object> loadedData);

            DeviceSave.ErrorCodes saveError;
            switch (loadError)
            {
                case DeviceSave.ErrorCodes.FileNotExists:
                    Dictionary<string, object> d = new();

                    foreach (DataEntry e in data)
                    {
                        if (!e.SaveOnDevice) continue;

                        d[e.Id] = e.Value;
                    }

                    saveError = Save(owner, d);

                    foreach (DataEntry e in data)
                    {
                        OnChanged?.Invoke(owner.Id, e);
                    }

                    break;
                case DeviceSave.ErrorCodes.None:
                    foreach(DataEntry e in data)
                    {
                        if (!e.SaveOnDevice) continue;

                        loadedData[e.Id] = e.Value;
                    }

                    saveError = Save(owner, loadedData);

                    foreach (DataEntry e in data)
                    {
                        Statistics_Multiplayer.UpdateStatisticsData(e);
                    }

                    foreach (DataEntry e in data)
                    {
                        OnChanged?.Invoke(owner.Id, e);
                    }

                    break;
                default:
                    error = ErrorCodes.LoadFailed;
                    return false;
            }

            if (saveError != DeviceSave.ErrorCodes.None)
            {
                error = ErrorCodes.SaveFailed;
                return false;
            }

            error = ErrorCodes.None;
            return true;
        }

        public static bool LoadStatistics(Owner owner, out Dictionary<string, object> data) => LoadStatistics(owner, out data, out _);
        public static bool LoadStatistics(Owner owner, out Dictionary<string, object> data, out ErrorCodes error)
        {
            DeviceSave.ErrorCodes loadError = Load(owner, out data);

            if (loadError != DeviceSave.ErrorCodes.None)
            {
                if (loadError == DeviceSave.ErrorCodes.FileNotExists)
                {
                    data = new();
                    error = ErrorCodes.None;
                    return true;
                }

                error = ErrorCodes.LoadFailed;
                return false;
            }

            error = ErrorCodes.None;
            return true;
        }

        public static bool LoadNumericalStatus(Owner owner, Statistics_Key key, out double status) => LoadNumericalStatus(owner, key.Id, out status, out _);
        public static bool LoadNumericalStatus(Owner owner, Statistics_Key key, out double status, out ErrorCodes error) => LoadNumericalStatus(owner, key.Id, out status, out error);
        public static bool LoadNumericalStatus(Owner owner, string statisticId, out double status) => LoadNumericalStatus(owner, statisticId, out status, out _);
        public static bool LoadNumericalStatus(Owner owner, string statisticId, out double status, out ErrorCodes error)
        {
            if(!LoadStatus(owner, statisticId, out object load, out error))
            {
                status = default;
                return false;
            }

            if (!double.TryParse(load.ToString(), out status))
            {
                status = default;
                error = ErrorCodes.StatusFoundWithDifferentType;
                return false;
            }

            return true;
        }

        public static bool LoadStatus<T>(Owner owner, Statistics_Key key, out T status) => LoadStatus(owner, key.Id, out status, out _);
        public static bool LoadStatus<T>(Owner owner, Statistics_Key key, out T status, out ErrorCodes error) => LoadStatus(owner, key.Id, out status, out error);
        public static bool LoadStatus<T>(Owner owner, string statisticId, out T status) => LoadStatus(owner, statisticId, out status, out _);
        public static bool LoadStatus<T>(Owner owner, string statisticId, out T status, out ErrorCodes error)
        {
            DeviceSave.ErrorCodes loadError = Load(owner, out Dictionary<string, object> data);

            if (loadError != DeviceSave.ErrorCodes.None)
            {
                status = default;

                if (loadError == DeviceSave.ErrorCodes.FileNotExists)
                {
                    error = ErrorCodes.StatusNotFound;
                }
                else
                {
                    error = ErrorCodes.LoadFailed;
                }

                return false;
            }

            if(!data.TryGetValue(statisticId, out object statusD))
            {
                error = ErrorCodes.StatusNotFound;
                status = default;
                return false;
            }

            if (statusD is not T statusT)
            {
                error = ErrorCodes.StatusFoundWithDifferentType;
                status = default;
                return false;
            }

            status = statusT;
            error = ErrorCodes.None;
            return true;
        }

        private static Dictionary<string, Dictionary<string, object>> _dataCache = new();
        private static DeviceSave.ErrorCodes Save(Owner owner, Dictionary<string, object> data)
        {
            Dictionary<string, Data> dataSave = new(data.Count);
            for (int i = 0; i < data.Count; i++)
            {
                KeyValuePair<string, object> dd = data.ElementAt(i);

                Data d = new(dd.Value);

                d.Serialize();

                dataSave[dd.Key] = d;
            }

            string dataRaw = JsonConvert.SerializeObject(dataSave);

            DeviceSave.ErrorCodes error = DeviceSave.Save(Path.Combine(SAVE_PATH, owner.Id + FILE_EXTENSION), dataRaw);

            if (error == DeviceSave.ErrorCodes.None)
            {
                _dataCache[owner.Id] = data;
            }

            return error;
        }

        private static DeviceSave.ErrorCodes Load(Owner owner, out Dictionary<string, object> data)
        {
            if (owner.IsNetworkOwner)
            {
                if (!int.TryParse(owner.Id, out int connectionId))
                {
                    Debug.LogError("Network owner id is not connection id! " + owner.Id);

                    data = default;
                    return DeviceSave.ErrorCodes.SerializationFailed;
                }

                if (!Statistics_Multiplayer.TryGetStatisticsData(connectionId, out data))
                {
                    data = default;
                    return DeviceSave.ErrorCodes.FileNotExists;
                }

                return DeviceSave.ErrorCodes.None;
            }

            if (_dataCache.TryGetValue(owner.Id, out data))
            {
                return DeviceSave.ErrorCodes.None;
            }

            DeviceSave.ErrorCodes error = DeviceSave.Load(Path.Combine(SAVE_PATH, owner.Id + FILE_EXTENSION), out object d);

            if (d is not string dataRaw || error == DeviceSave.ErrorCodes.FileNotExists)
            {
                data = default;
                return error;
            }

            DeserializeStatisticsData(dataRaw, out data);

            return error;
        }

        public static void DeserializeStatisticsData(string dataRaw, out Dictionary<string, object> data)
        {
            Dictionary<string, Data> dataLoaded = JsonConvert.DeserializeObject<Dictionary<string, Data>>(dataRaw);
            data = new(dataLoaded.Count);

            foreach (KeyValuePair<string, Data> dd in dataLoaded)
            {
                Data ddd = dd.Value;

                ddd.Deserialize();

                data[dd.Key] = ddd.Value;
            }
        }

        public static string SerializeStatisticsDataValue(object dataValue)
        {
            Data d = new(dataValue);
            d.Serialize();

            return JsonConvert.SerializeObject(d);
        }

        public static object DeserializeStatisticsDataValue(string dataValue)
        {
            Data d = JsonConvert.DeserializeObject<Data>(dataValue);
            d.Deserialize();

            return d.Value;
        }
    }
}
