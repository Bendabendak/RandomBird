using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeReferences;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SadJam
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class StaticBehaviourInitializer
    {
        public static Dictionary<Type, StaticBehaviour> typeBehaviours;
        public static Dictionary<string, StaticBehaviour> nameBehaviours;

        static StaticBehaviourInitializer()
        {
            Init();
        }

        private static bool _initialized = false;
        private static bool _started = false;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (_initialized) return;

            _initialized = true;

            if (typeBehaviours != null) return;

            typeBehaviours = new(InitializeStaticBehaviours());
            nameBehaviours = typeBehaviours.ToDictionary(x => x.Key.FullName, x => x.Value);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Start()
        {
            if (_started) return;
            _started = true;

            Init();

            foreach (KeyValuePair<Type, StaticBehaviour> pair in typeBehaviours)
            {
                pair.Key.GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(pair.Value, null);
            }
        }

        /// <typeparam name="T">Generic type, e.g. A<></typeparam>
        public static IEnumerable<KeyValuePair<string, T>> GetBehavioursFromGeneric<T>() where T : StaticBehaviour
        {
            Type t = typeof(T);

            foreach (KeyValuePair<Type, StaticBehaviour> pair in typeBehaviours)
            {
                if (pair.Key.IsAssignableToGenericType(t))
                {
                    yield return new(pair.Key.FullName, (T)pair.Value);
                }
            }
        }
        /// <param name="genericType">e.g. A<></param>
        public static IEnumerable<KeyValuePair<string, StaticBehaviour>> GetBehavioursFromGeneric(Type genericType)
        {
            foreach (KeyValuePair<Type, StaticBehaviour> pair in typeBehaviours)
            {
                if (pair.Key.IsAssignableToGenericType(genericType))
                {
                    yield return new(pair.Key.FullName, pair.Value);
                }
            }
        }

        public static IEnumerable<KeyValuePair<string, T>> GetBehaviours<T>() where T : StaticBehaviour
        {
            Type t = typeof(T);

            foreach (KeyValuePair<Type, StaticBehaviour> pair in typeBehaviours)
            {
                if (t.IsAssignableFrom(pair.Key))
                {
                    yield return new(pair.Key.FullName, (T)pair.Value);
                }
            }
        }

        public static IEnumerable<KeyValuePair<string, StaticBehaviour>> GetBehaviours(Type t)
        {
            foreach(KeyValuePair<Type, StaticBehaviour> pair in typeBehaviours)
            {
                if (t.IsAssignableFrom(pair.Key))
                {
                    yield return new(pair.Key.FullName, pair.Value);
                }
            }
        }

        public static T GetBehaviour<T>() where T : StaticBehaviour => (T)GetBehaviour(typeof(T));
        public static StaticBehaviour GetBehaviour(Type t) => typeBehaviours[t];
        public static StaticBehaviour GetBehaviour(string fullName) => nameBehaviours[fullName];

        private static IEnumerable<KeyValuePair<Type, StaticBehaviour>> InitializeStaticBehaviours()
        {
            foreach (Type t in ClassTypeReference.GetFilteredTypes(
                    new ClassExtendsAttribute(typeof(StaticBehaviour)) { AllowAbstract = false }))
            {
                KeyValuePair<Type, StaticBehaviour> pair = new(t, ActivatorExtensions.GetActivator<StaticBehaviour>(t.GetConstructors().First())());

                pair.Value.type = t;

                pair.Key.GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(pair.Value, null);

                yield return pair;
            }
        }
    }
}
