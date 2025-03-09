using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SadJam
{
    public static class TransformExtensions
    {
        public static int GetAllChildCount(this Transform self)
        {
            return GetAllChildCountHelper(self, 0);
        }

        private static int GetAllChildCountHelper(Transform t, int count)
        {
            count += t.childCount;

            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                count += GetAllChildCountHelper(child, 0);
            }

            return count;
        }

        public static Transform FindRecursive(this Transform self, string exactName) => self.FindRecursive(child => child.name == exactName);
        public static Transform FindRecursive(this Transform self, Func<Transform, bool> selector)
        {
            for (int i = 0; i < self.childCount; i++)
            {
                Transform child = self.GetChild(i);

                if (selector(child))
                {
                    return child;
                }

                Transform finding = child.FindRecursive(selector);

                if (finding != null)
                {
                    return finding;
                }
            }

            return null;
        }

        public static string GetPathUntil(this Transform t, string until)
        {
            StringBuilder path = new(t.name);

            while (t.parent != null)
            {
                t = t.parent;

                if (t.name == until) break;
                
                path.Insert(0, t.name + "/");
            }

            return path.ToString();
        }

        public static string GetPathUntil(this Transform t, GameObject until)
        {
            StringBuilder path = new(t.name);

            while (t.parent != null)
            {
                t = t.parent;

                if (t.gameObject == until) break;

                path.Insert(0, t.name + "/");
            }

            return path.ToString();
        }

        public static string GetPath(this Transform t)
        {
            StringBuilder path = new(t.name);

            while (t.parent != null)
            {
                t = t.parent;
                path.Insert(0, t.name + "/");
            }

            return path.ToString();
        }

        public static Transform GetIn(this Transform target, string name, params Type[] components) => GetIn(target, name, false, components);
        public static Transform GetIn(this Transform target, string name, bool debugOnly, params Type[] components)
        {
            if (target.gameObject.name == name) return target;

            Transform holder = target.Find(name);
            if (holder == null)
            {
                holder = new GameObject(name, components).transform;

                if (debugOnly)
                {
                    holder.gameObject.AddComponent<DebugOnlyComponent>();
                }

                holder.parent = target;
                holder.localPosition = Vector3.zero;
                holder.localRotation = Quaternion.identity;
            }

            return holder;
        }

        public static Transform GetInRoot(this Transform target, string name, params Type[] components) => GetInRoot(target, name, false, components);
        public static Transform GetInRoot(this Transform target, string name, bool debugOnly, params Type[] components)
        {
            Transform holder = target.root.Find(name);
            if (holder == null)
            {
                holder = new GameObject(name, components).transform;

                if (debugOnly)
                {
                    holder.gameObject.AddComponent<DebugOnlyComponent>();
                }

                holder.parent = target.root;
            }

            return holder;
        }

        public static Transform GetInScene(string name, params Type[] components) => GetInScene(name, false, components);
        public static Transform GetInScene(string name, bool debugOnly, params Type[] components)
        {
            GameObject holder = null;
            try
            {
                foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects()) 
                {
                    if (g.name == name)
                    {
                        holder = g;
                        break;
                    }
                }
            }
            catch
            {
                holder = null;
            }

            if (holder == null)
            {
                holder = new GameObject(name, components);

                if (debugOnly)
                {
                    holder.AddComponent<DebugOnlyComponent>();
                }
            }

            return holder.transform;
        }
    }
}
