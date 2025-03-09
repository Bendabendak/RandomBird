using System.Collections.Generic;
using UnityEditor;

namespace SadJamEditor
{
    public static class SerializedObjectExtensions
    {
        public static IEnumerable<SerializedProperty> GetSerializedProperties(this SerializedObject obj)
        {
            return GetSerializedProperties(obj, true);
        }

        public static IEnumerable<SerializedProperty> GetSerializedProperties(this SerializedObject obj, bool enterChildren)
        {
            SerializedProperty source = obj.GetIterator();
            source.Next(true);

            while (source.Next(enterChildren))
            {
                yield return source;
            }
        }

        public static IEnumerable<SerializedProperty> GetVisibleSerializedProperties(this SerializedObject obj)
        {
            return GetVisibleSerializedProperties(obj, true);
        }

        public static IEnumerable<SerializedProperty> GetVisibleSerializedProperties(this SerializedObject obj, bool enterChildren)
        {
            SerializedProperty source = obj.GetIterator();
            source.NextVisible(true);

            while (source.NextVisible(enterChildren))
            {
                yield return source;
            }
        }
    }
}
