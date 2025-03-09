using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam
{
    public static class ComponentExtensions
    {
        public static string GetPath(this UnityEngine.Component c)
        {
            if (c is IComponent ic)
            {
                return c.transform.GetPathUntil(c.gameObject) + "/" + ic.Label + " (" + c.GetType().Name + ")";
            }

            return c.transform.GetPathUntil(c.gameObject) + "/" + c.GetType().Name;
        }

        public static IEnumerable<T> GetComponentsOnlyInChildren<T>(this GameObject c) where T : UnityEngine.Component
        {
            Transform transform = c.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                foreach (T t in transform.GetChild(i).GetComponents<T>())
                {
                    yield return t;
                }
            }
        }

        public static IEnumerable<UnityEngine.Component> GetComponentsOnlyInChildren(this GameObject c, Type type)
        {
            Transform transform = c.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                foreach (UnityEngine.Component t in transform.GetChild(i).GetComponents(type))
                {
                    yield return t;
                }
            }
        }

        public static IEnumerable<T> GetComponentsOnlyInChildren<T>(this UnityEngine.Component c) where T : UnityEngine.Component
        {
            Transform transform = c.transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                foreach(T t in transform.GetChild(i).GetComponents<T>())
                {
                    yield return t;
                }
            }
        }

        public static IEnumerable<UnityEngine.Component> GetComponentsOnlyInChildren(this UnityEngine.Component c, Type type)
        {
            Transform transform = c.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                foreach (UnityEngine.Component t in transform.GetChild(i).GetComponents(type))
                {
                    yield return t;
                }
            }
        }
    }
}
