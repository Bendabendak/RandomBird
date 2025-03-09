using System;
#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using System.Collections;

namespace SadJam
{
    public static class EditorUtilityExtensions
    {
        public static void SafeOperation(Action trigger)
        {
#if UNITY_EDITOR
            EditorCoroutineUtility.StartCoroutineOwnerless(SafeOperationCor(trigger));
#endif
        }

        private static IEnumerator SafeOperationCor(Action trigger)
        {
            yield return new WaitForEndOfFrame();

            trigger.Invoke();
        }

        public static void SetDirty(UnityEngine.Object c)
        {
            SetDirty(c, "Unknown");
        }

        public static void SetDirty(UnityEngine.Object c, string undoMessage)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
                return;

            Undo.RecordObject(c, undoMessage);
            EditorUtility.SetDirty(c);
            PrefabUtility.RecordPrefabInstancePropertyModifications(c);
#endif
        }

        public static void SetDirty(UnityEngine.Component c)
        {
            SetDirty(c, "Unknown");
        }

        public static void SetDirty(UnityEngine.Component c, string undoMessage)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
                return;

            Undo.RecordObject(c.gameObject, undoMessage);
            EditorUtility.SetDirty(c.gameObject);
            PrefabUtility.RecordPrefabInstancePropertyModifications(c);
            EditorSceneManager.MarkSceneDirty(c.gameObject.scene);
#endif
        }

        public static void SetDirty(GameObject o)
        {
            SetDirty(o, "Unknown");
        }

        public static void SetDirty(GameObject o, string undoMessage)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
                return;

            Undo.RecordObject(o, undoMessage);
            EditorUtility.SetDirty(o);
            EditorSceneManager.MarkSceneDirty(o.scene);
#endif
        }
    }
}
