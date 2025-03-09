using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SadJam
{
    public static class ScriptableObjectExtensions
    {
#if UNITY_EDITOR
        public static IEnumerable<UnityEngine.Object> GetAllInstances(Type t)
        {
            return AssetDatabase.FindAssets($"t: {t.Name}").Select(AssetDatabase.GUIDToAssetPath).Select(s => AssetDatabase.LoadAssetAtPath(s, t));
        }
#endif
#if UNITY_EDITOR
        public static IEnumerable<T> GetAllInstances<T>() where T : ScriptableObject
        {
            return AssetDatabase.FindAssets($"t: {typeof(T).Name}").Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<T>);
        }
#endif
    }
}
