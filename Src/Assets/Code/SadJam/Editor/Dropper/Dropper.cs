using System;
using System.Collections.Generic;
using System.Linq;
using TypeReferences;
using SadJam;
using UnityEngine;

namespace SadJamEditor
{
    public abstract class Dropper
    {
        private static List<Dropper> droppers = new();

        /// <param name="onDrop">Before, after</param>
        public abstract void DropMe(object drop, object before, object target, GameObject context, Type resultType, Action<object> onDrop = null, params object[] customData);

        /// <param name="onDrop">Before, after</param>
        public static void NewDrop(object drop, object before, object target, GameObject context, Type resultType, Action<object> onDrop = null, params object[] customData)
        {
            Type dropType = drop.GetType();

            Dropper dropper = GetDropper(dropType);

            if (dropper == null)
            {
                if (resultType.IsAssignableFrom(dropType))
                {
                    onDrop?.Invoke(drop);
                    return;
                }

                Debug.LogError("Dropper for type " + dropType.FullName + " not found!");
                return;
            }

            dropper.DropMe(drop, before, target, context, resultType, (object result) =>
            {
                if (target is not UnityEngine.Component c)
                {
                    onDrop?.Invoke(result);
                    return;
                }

                if (before != null && before is SadJam.Component beforeC)
                {
                    beforeC.RemoveReference(c);
                }

                if (result != null && result is SadJam.Component afterC)
                {
                    afterC.AddReference(c);
                }

                EditorUtilityExtensions.SetDirty(c, "New drop");

                onDrop?.Invoke(result);
            }, customData);
        }

        public static List<Dropper> GetDroppers()
        {
            List<Dropper> droppers = new();

            foreach (Type t in ClassTypeReference.GetFilteredTypes(new ClassExtendsAttribute(typeof(Dropper)) { AllowAbstract = false }))
            {
                droppers.Add((Dropper)Activator.CreateInstance(t));
            }

            return droppers;
        }

        public static Dropper GetDropper<T>() => GetDropper(typeof(T));

        public static Dropper GetDropper(Type t)
        {
            if(droppers.Count <= 0)
            {
                droppers = GetDroppers();
            }

            Dictionary<int, Dropper> pos = new();
            foreach (Dropper d in droppers.Where((Dropper d) =>
            {
                object[] atts = d.GetType().GetCustomAttributes(typeof(CustomDropper), false);

                if (atts == null || atts.Length <= 0) return false;

                return ((CustomDropper)atts[0]).targetType.IsAssignableFrom(t);
            }))
            {
                pos.Add(d.GetType().GetInheritanceHierarchy().Count(), d);
            }

            if (pos.Count <= 0)
            {
                return null;
            }

            return pos[pos.Keys.Max()];
        }
    }
}
