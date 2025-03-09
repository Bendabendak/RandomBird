using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SadJam
{
    [ExecuteAlways]
    public static class GarbageCollector
    {
        public static string GarbageObjectName = "**garbageCollector**";

#if UNITY_EDITOR
        [MenuItem("SadJam/Garbage/Clean Up")]
        public static void CleanAllGarbage()
        {
            foreach (GameObject garbage in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (garbage == null || garbage.name != GarbageObjectName) continue;

                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(garbage);
                if (prefab != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(prefab);
                    GameObject contentsRoot = PrefabUtility.LoadPrefabContents(assetPath);
                    CleanGarbageIn(contentsRoot.transform);
                    PrefabUtility.SaveAsPrefabAsset(contentsRoot, assetPath);
                    PrefabUtility.UnloadPrefabContents(contentsRoot);
                }
                else
                {
                    CleanGarbage(garbage);
                }
            }

            foreach (SadJam.Component o in Resources.FindObjectsOfTypeAll<SadJam.Component>())
            {
                o.AssignedTo.RemoveAll(c => c == null);
            }

            Debug.LogWarning("Garbage cleaned");
        }
#endif
        public static void CleanGarbageIn(Transform t)
        {
            foreach(Transform c in t.GetComponentsInChildren<Transform>().Where(t => t.gameObject.name == GarbageObjectName))
            {
                CleanGarbage(c.gameObject);
            }
        }

        public static GameObject Get(Transform t) => t.GetIn(GarbageObjectName, true).gameObject;

        public static void CleanGarbage(GameObject garbageCollector)
        {
            IComponent[] components = garbageCollector.GetComponents<IComponent>();

            foreach (IComponent droppable in components)
            {
                Clean(droppable);

                foreach (IComponent droppable1 in components)
                {
                    Clean(droppable1);
                }
            }

            void Clean(IComponent d)
            {
                if (d is not UnityEngine.Component c) return;
                if (c == null) return;

                d.AssignedTo.RemoveAll(c => c == null);

                if (d.AssignedTo.Count <= 0)
                {
                    if (d is DebugOnlyComponent) return;

                    UnityEngine.Object.DestroyImmediate(c, true);
                }
            }

#if UNITY_EDITOR
            EditorUtilityExtensions.SetDirty(garbageCollector, "Component copied");
#endif

            if (garbageCollector.GetComponents<UnityEngine.Component>().Length <= 2)
            {
                UnityEngine.Object.DestroyImmediate(garbageCollector, true);
            }
        }
    }
}
