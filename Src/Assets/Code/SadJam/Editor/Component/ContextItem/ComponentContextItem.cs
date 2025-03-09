using SadJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace SadJamEditor
{
    public abstract class ComponentContextItem : StaticBehaviour
    {
        public static Dictionary<string, ComponentContextItem> ContextItems => GetContextItems();

        private static Dictionary<string, ComponentContextItem> _contextItems;
        private static Dictionary<string, ComponentContextItem> GetContextItems()
        {
            if (_contextItems == null)
            {
                _contextItems = new(StaticBehaviourInitializer.GetBehaviours<ComponentContextItem>());
            }

            return _contextItems;
        }

        public static IEnumerable<ComponentContextItem> GetContextItems<D>() => GetContextItems(typeof(D));
        public static IEnumerable<ComponentContextItem> GetContextItems(Type targetType)
        {
            foreach (KeyValuePair<string, ComponentContextItem> d in ContextItems.Where((KeyValuePair<string, ComponentContextItem> d) =>
            {
                object[] atts = d.Value.type.GetCustomAttributes(typeof(CustomComponentContextItem), false);

                if (atts == null || atts.Length <= 0) return false;

                Type t = ((CustomComponentContextItem)atts[0]).TargetType;

                if (targetType.IsAssignableToGenericType(t)) return true;

                return t.IsAssignableFrom(targetType);
            }))
            {
                yield return d.Value;
            }
        }

        public static ComponentContextItem GetContextItem<D>() => GetContextItem(typeof(D).FullName);
        public static ComponentContextItem GetContextItem(string fullName) => ContextItems[fullName];

        public abstract GenericMenuItem[] GetItems(SerializedProperty prop);
    }
}
