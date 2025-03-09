using System.Collections.Generic;
using UnityEngine;

namespace Game 
{
    public class Statistics_Collector : MonoBehaviour
    {
        public Dictionary<string, object> Stats { get; private set; } = new();
        public Dictionary<string, float> NumericalStats { get; private set; } = new();

        public static Dictionary<GameObject, Statistics_Collector> Collectors { get; private set; } = new();

        protected virtual void Awake()
        {
            AddCollector(this);
        }

        protected virtual void OnDestroy()
        {
            RemoveCollector(this);
        }

        public void RemoveStatus(Statistics_Key key) => RemoveStatus(key.Id);
        public void RemoveStatus(string id)
        {
            Stats.Remove(id);
            NumericalStats.Remove(id);
        }

        public void ChangeStatus(Statistics_Key key, object value) => ChangeStatus(key.Id, value);
        public void ChangeStatus(string id, object value) 
        {
            if (value is float f)
            {
                NumericalStats[id] = f;
            }

            Stats[id] = value;
        }

        public void ChangeStatus(Statistics_Key key, float value) => ChangeStatus(key.Id, value);
        public void ChangeStatus(string id, float value)
        {
            NumericalStats[id] = value;
            Stats[id] = value;
        }

        public bool GetStatus<T>(Statistics_Key key, out T status) => GetStatus(key.Id, out status, out _);
        public bool GetStatus<T>(Statistics_Key key, out T status, out ErrorCodes error) => GetStatus(key.Id, out status, out error);
        public bool GetStatus<T>(string id, out T status) => GetStatus(id, out status, out _);
        public bool GetStatus<T>(string id, out T status, out ErrorCodes error)
        {
            if (!Stats.TryGetValue(id, out object s))
            {
                status = default;
                error = ErrorCodes.StatusNotFound;
                return false;
            }

            if (s is not T sT)
            {
                status = default;
                error = ErrorCodes.StatusFoundWithDifferentType;
                return false;
            }

            status = sT;
            error = ErrorCodes.None;
            return true;
        }

        public bool GetNumericalStatus(Statistics_Key key, out float status) => GetNumericalStatus(key.Id, out status, out _);
        public bool GetNumericalStatus(Statistics_Key key, out float status, out ErrorCodes error) => GetNumericalStatus(key.Id, out status, out error);
        public bool GetNumericalStatus(string id, out float status) => GetNumericalStatus(id, out status, out _);
        public bool GetNumericalStatus(string id, out float status, out ErrorCodes error)
        {
            if (!NumericalStats.TryGetValue(id, out status))
            {
                status = default;
                error = ErrorCodes.StatusNotFound;
                return false;
            }

            error = ErrorCodes.None;
            return true;
        }

        public static bool AddCollector(Statistics_Collector collector)
        {
            return Collectors[collector.gameObject] = collector;
        }

        public static bool RemoveCollector(Statistics_Collector collector)
        {
            return Collectors.Remove(collector.gameObject);
        }

        public static Statistics_Collector GetCollector(GameObject collector)
        {
            Collectors.TryGetValue(collector, out Statistics_Collector score_Collector);

            return score_Collector;
        }

        public enum ErrorCodes
        {
            None,
            StatusNotFound,
            StatusFoundWithDifferentType
        }
    }
}
