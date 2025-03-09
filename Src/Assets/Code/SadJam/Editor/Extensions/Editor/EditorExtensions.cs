using SadJam;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    public static class EditorExtensions
    {
        private static Dictionary<UnityEngine.Object, Editor> _caschedEditors = new();
        public static Editor GetCachedEditor(UnityEngine.Object obj)
        {
            if (_caschedEditors.ContainsKey(obj))
            {
                Editor e = _caschedEditors[obj];

                if(e != null)
                {
                    return e;
                }
            }

            _caschedEditors[obj] = Editor.CreateEditor(obj);

            return _caschedEditors[obj];
        }

        public static void CleanUpCachedEditor(UnityEngine.Object target)
        {
            CleanUpCachedEditor(target, true);
        }

        public static void CleanUpCachedEditor(UnityEngine.Object target, bool safe)
        {
            if (safe)
            {
                EditorUtilityExtensions.SafeOperation(() =>
                {
                    Clean();
                });
            }
            else
            {
                Clean();
            }

            void Clean()
            {
                CleanUpEmptyEditors();

                if (!_caschedEditors.ContainsKey(target)) return;

                if(_caschedEditors[target] != null)
                {
                    UnityEngine.Object.DestroyImmediate(_caschedEditors[target], true);
                }

                _caschedEditors.Remove(target);
            }
        }

        private static void CleanUpEmptyEditors()
        {
            foreach (KeyValuePair<UnityEngine.Object, Editor> k in _caschedEditors.Where(k => k.Key == null || k.Value == null).ToList())
            {
                _caschedEditors.Remove(k.Key);

                if (k.Value != null)
                {
                    UnityEngine.Object.DestroyImmediate(k.Value, true);
                }
            }
        }

        public static void CleanUpAllEditors(UnityEngine.Object target)
        {
            foreach (UnityEngine.Object o in Resources.FindObjectsOfTypeAll(typeof(Editor)).Where(o => ((Editor)o).target == target))
            {
                EditorUtilityExtensions.SafeOperation(() =>
                {
                    if(o != null)
                    {
                        UnityEngine.Object.DestroyImmediate(o, true);
                    }
                });
            }
        }
    }
}
