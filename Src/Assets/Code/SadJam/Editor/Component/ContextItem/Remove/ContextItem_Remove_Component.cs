using SadJam;
using UnityEngine;
using UnityEditor;

namespace SadJamEditor
{
    [CustomComponentContextItem(typeof(SadJam.Component))]
    public class ContextItem_Remove_Component : ComponentContextItem
    {
        public override GenericMenuItem[] GetItems(SerializedProperty prop)
        {
            GenericMenuItem[] items = new GenericMenuItem[1];

            SadJam.Component t = (SadJam.Component)prop.serializedObject.targetObject;

            items[0] = new GenericMenuItem(new GUIContent("Remove"), false, () =>
            {
                object value = prop.objectReferenceValue;

                prop.objectReferenceValue = null;
                prop.serializedObject.ApplyModifiedProperties();

                if (value != null)
                {
                    if (value is IComponent valueC)
                    {
                        valueC.RemoveReference(t);
                    }
                }

                EditorUtilityExtensions.SetDirty(t, "Drop removed");
            });

            return items;
        }
    }
}
