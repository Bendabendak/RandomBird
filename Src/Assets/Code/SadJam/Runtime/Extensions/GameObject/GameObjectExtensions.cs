using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SadJam
{
    public static class GameObjectExtensions
    {
        public static T CopyComponent<T>(this GameObject gameObject, T component) where T : UnityEngine.Component
        {
            Type type = component.GetType();

            T target = (T)gameObject.AddComponent(type);

            foreach (FieldInfo field in type.GetFields())
            {
                if (field.IsStatic) continue;

                field.SetValue(target, field.GetValue(component));
            }

            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;

                prop.SetValue(target, prop.GetValue(component, null), null);
            }

#if UNITY_EDITOR
            EditorUtilityExtensions.SetDirty(target, "Component copied");
#endif

            return target;
        }
        #region AddExceptedComponents
        public static List<UnityEngine.Component> AddExceptedComponents(this GameObject target, Type type, IEnumerable<Type> exceptWith)
        {
            List<UnityEngine.Component> list = new();

            IEnumerable<Type> componentsIn = target.GetComponents(type).Select(c => c.GetType());

            foreach (Type t in exceptWith.Except(componentsIn))
            {
                list.Add(target.AddComponent(t));
            }

            return list;
        }
        public static List<UnityEngine.Component> AddExceptedComponents(this GameObject target, Type type, IEnumerable<Type> exceptWith, out UnityEngine.Component[] componentsIn)
        {
            List<UnityEngine.Component> list = new();

            componentsIn = target.GetComponents(type);

            foreach (Type t in exceptWith.Except(componentsIn.Select(c => c.GetType()).ToList()))
            {
                list.Add(target.AddComponent(t));
            }

            return list;
        }
        public static List<UnityEngine.Component> AddExceptedComponents<T>(this GameObject target, IEnumerable<Type> exceptWith)
        {
            List<UnityEngine.Component> list = new();

            List<Type> componentsIn = target.GetComponents<T>().Select(c => c.GetType()).ToList();

            foreach (Type t in exceptWith.Except(componentsIn))
            {
                list.Add(target.AddComponent(t));
            }

            return list;
        }
        public static List<UnityEngine.Component> AddExceptedComponents<T>(this GameObject target, IEnumerable<Type> exceptWith, out T[] componentsIn)
        {
            List<UnityEngine.Component> list = new();

            componentsIn = target.GetComponents<T>();

            foreach (Type t in exceptWith.Except(componentsIn.Select(c => c.GetType())))
            {
                list.Add(target.AddComponent(t));
            }

            return list;
        }
        public static List<UnityEngine.Component> AddExceptedComponents(this GameObject target, Type type, IEnumerable<UnityEngine.Component> exceptWith)
        {
            List<UnityEngine.Component> list = new();

            UnityEngine.Component[] componentsIn = target.GetComponents(type);

            foreach (UnityEngine.Component c in exceptWith.Except(componentsIn))
            {
                list.Add(target.AddComponent(c.GetType()));
            }

            return list;
        }
        public static List<UnityEngine.Component> AddExceptedComponents(this GameObject target, Type type, IEnumerable<UnityEngine.Component> exceptWith, out UnityEngine.Component[] componentsIn)
        {
            List<UnityEngine.Component> list = new();

            componentsIn = target.GetComponents(type);

            foreach (UnityEngine.Component c in exceptWith.Except(componentsIn))
            {
                list.Add(target.AddComponent(c.GetType()));
            }

            return list;
        }
        public static List<UnityEngine.Component> AddExceptedComponents<T>(this GameObject target, IEnumerable<UnityEngine.Component> exceptWith) where T : UnityEngine.Component
        {
            List<UnityEngine.Component> list = new();

            UnityEngine.Component[] componentsIn = target.GetComponents<T>();

            foreach (T c in exceptWith.Except(componentsIn))
            {
                list.Add(target.AddComponent(c.GetType()));
            }

            return list;
        }
        public static List<UnityEngine.Component> AddExceptedComponents<T>(this GameObject target, IEnumerable<UnityEngine.Component> exceptWith, out UnityEngine.Component[] componentsIn) where T : UnityEngine.Component
        {
            List<UnityEngine.Component> list = new ();

            componentsIn = target.GetComponents<T>();

            foreach (T c in exceptWith.Except(componentsIn))
            {
                list.Add(target.AddComponent(c.GetType()));
            }

            return list;
        }
        #endregion
    }
}
