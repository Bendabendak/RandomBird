using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using SadJam.Components;

namespace SadJamEditor.Components
{
    [InitializeOnLoad]
    public static class HierarchyReorder
    {
#if UNITY_EDITOR
        static HierarchyReorder()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static Dictionary<LastInHierarchyComponent, int> lasts = new Dictionary<LastInHierarchyComponent, int>();

        private static bool reordered = false;
        private static void OnHierarchyChanged()
        {
            if (reordered)
            {
                reordered = false;
                return;
            }

            Scene activeScene = EditorSceneManager.GetActiveScene();

            foreach (LastInHierarchyComponent last in Object.FindObjectsOfType<LastInHierarchyComponent>())
            {
                int count;

                if (last.transform.parent == null)
                {
                    count = activeScene.rootCount;
                }
                else
                {
                    count = last.transform.parent.childCount;
                }

                int order = last.transform.GetSiblingIndex() + count;

                if (!lasts.ContainsKey(last))
                {
                    lasts.Add(last, order);
                }
                else if (lasts[last] == order)
                {
                    continue;
                }

                lasts[last] = order;
                last.transform.SetAsLastSibling();
                reordered = true;
            }
        }
#endif
    }
}
