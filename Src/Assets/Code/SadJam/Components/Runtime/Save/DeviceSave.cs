using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

namespace SadJam.Components
{
    public static class DeviceSave
    {
        public static string SavePath { get; set; } = Application.persistentDataPath;

        private static BinaryFormatter _binaryFormatter = new();

        private struct SaveQueue
        {
            public string fileName;
            public object toSave;
            public Action<ErrorCodes> done;
        }

        public enum ErrorCodes
        {
            None,
            FileNotExists,
            SerializationFailed
        }

        private static List<SaveQueue> _saveQueue = new List<SaveQueue>();
        public static void SaveAsync(string fileName, object toSave, Action<ErrorCodes> done = null)
        {
            _saveQueue.Add(new() { fileName = fileName, toSave = toSave, done = done });
            if (_saveQueue.Count <= 1)
            {
                FinishQueue();
            }

            void FinishQueue()
            {
                if (_saveQueue.Count <= 0) return;

                SaveQueue q = _saveQueue[0];
                StaticCoroutine.Start(SaveCoroutine(q.fileName, q.toSave, (ErrorCodes s) =>
                {
                    _saveQueue.RemoveAt(0);
                    q.done?.Invoke(s);

                    FinishQueue();
                }));
            }
        }

        public static ErrorCodes Save(string fileName, object toSave)
        {
            string path = Path.Combine(SavePath, fileName);

            string dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            FileStream stream = new(path, FileMode.Create);
            try
            {
                _binaryFormatter.Serialize(stream, toSave);
            }
            catch
            {
                stream.Close();
                return ErrorCodes.SerializationFailed;
            }

            stream.Close();
            return ErrorCodes.None;
        }

        private static IEnumerator SaveCoroutine(string fileName, object toSave, Action<ErrorCodes> done = null)
        {
            bool complete = false;
            ErrorCodes result = ErrorCodes.None;
            Task task = Task.Run(() =>
            {
                result = Save(fileName, toSave);
                complete = true;
            });

            while (!complete)
            {
                yield return null;
            }
            
            done?.Invoke(result);
        }
        private struct LoadQueue
        {
            public string fileName;
            public Action<object, ErrorCodes> done;
        }

        private static List<LoadQueue> _loadQueue = new List<LoadQueue>();
        public static void LoadAsync(string fileName, Action<object, ErrorCodes> done = null)
        {
            _loadQueue.Add(new() { fileName = fileName, done = done });
            if (_loadQueue.Count <= 1)
            {
                FinishQueue();
            }

            void FinishQueue()
            {
                if (_loadQueue.Count <= 0) return;

                LoadQueue q = _loadQueue[0];
                StaticCoroutine.Start(LoadCoroutine(q.fileName, (object s, ErrorCodes error) =>
                {
                    _loadQueue.RemoveAt(0);
                    q.done?.Invoke(s, error);

                    FinishQueue();
                }));
            }
        }

        public static ErrorCodes Load(string fileName, out object loaded)
        {
            string path = Path.Combine(SavePath, fileName);

            if (File.Exists(path) && new FileInfo(path).Length != 0)
            {
                FileStream stream = new(path, FileMode.Open);
                object data;
                try
                {
                    data = _binaryFormatter.Deserialize(stream);
                }
                catch
                {
                    loaded = null;
                    stream.Close();

                    return ErrorCodes.SerializationFailed;
                }

                stream.Close();

                loaded = data;
                return ErrorCodes.None;
            }

            loaded = null;
            return ErrorCodes.FileNotExists;
        }

        private static IEnumerator LoadCoroutine(string fileName, Action<object, ErrorCodes> done = null)
        {
            bool complete = false;
            object result = false;
            ErrorCodes error = ErrorCodes.None;
            Task task = Task.Run(() =>
            {
                error = Load(fileName, out result);
                complete = true;
            });

            while (!complete)
            {
                yield return null;
            }

            done?.Invoke(result, error);
        }
    }
}
