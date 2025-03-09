using System;
using System.Collections.Generic;
using System.Reflection;
using TypeReferences;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SadJam
{
    public class StaticExecutorInitializer : StaticBehaviour
    {
        public const string HOLDER_NAME = "**StaticExecutors**";
        public static Dictionary<Type, StaticExecutor> TypeExecutors => GetTypeExecutors();
        public static Dictionary<string, StaticExecutor> IdExecutors => GetIdExecutors();

        protected override void Start()
        {
            base.Start();

            Init();
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene curr, Scene next) 
        {
            Init();
        }

        private void Init() 
        {
            _typeExecutors = null;
            _idExecutors = null;

            GetTypeExecutors();
            GetIdExecutors();
        }

        private static Dictionary<Type, StaticExecutor> _typeExecutors;
        private static Dictionary<Type, StaticExecutor> GetTypeExecutors()
        {
            if(_typeExecutors == null)
            {
                _typeExecutors = new(InitializeExecutors());
            }

            return _typeExecutors;
        }

        private static Dictionary<string, StaticExecutor> _idExecutors;
        private static Dictionary<string, StaticExecutor> GetIdExecutors()
        {
            if (_idExecutors == null)
            {
                _idExecutors = new();

                foreach (KeyValuePair<Type, StaticExecutor> t in GetTypeExecutors())
                {
                    CustomStaticExecutor idAtt = t.Key.GetCustomAttribute<CustomStaticExecutor>();

                    if (idAtt == null)
                    {
                        Debug.LogError("Missing " + nameof(CustomStaticExecutor) + " attribute on " + t.Key.FullName);
                        continue;
                    }

                    _idExecutors[idAtt.Id] = t.Value;
                }
            }

            return _idExecutors;
        }


        public static bool GetExecutor<T>(out T ex) where T : StaticExecutor
        {
            if (!GetExecutor(typeof(T), out StaticExecutor e))
            {
                ex = default;
                return false;
            }

            ex = (T)e;
            return true;
        }
        public static bool GetExecutor(Type t, out StaticExecutor ex) => TypeExecutors.TryGetValue(t, out ex);
        public static bool GetExecutor(string id, out StaticExecutor ex) => IdExecutors.TryGetValue(id, out ex);

        private static IEnumerable<KeyValuePair<Type, StaticExecutor>> InitializeExecutors()
        {
            GameObject holder = GetHolder();

            foreach (Type t in ClassTypeReference.GetFilteredTypes(
                    new ClassExtendsAttribute(typeof(StaticExecutor)) { AllowAbstract = false }))
            {
                UnityEngine.Component c = holder.GetComponent(t);

                if(c == null)
                {
                    c = holder.AddComponent(t);
                }

                KeyValuePair<Type, StaticExecutor> pair = new(t, (StaticExecutor)c);

                yield return pair;
            }
        }

        public static GameObject GetHolder(params Type[] components) => TransformExtensions.GetInScene(HOLDER_NAME, true, components).gameObject;
    }
}
