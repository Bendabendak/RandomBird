using UnityEngine;
using SadJam;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace SadJamEditor
{
    [CustomComponentContextItem(typeof(SadJam.Component))]
    public class ContextItem_Edit_Component : ComponentContextItem
    {
        public override GenericMenuItem[] GetItems(SerializedProperty prop)
        {
            GenericMenuItem[] items = new GenericMenuItem[1];

            items[0] = new GenericMenuItem(new GUIContent("Edit"), false, () =>
            {
                Type t = prop.objectReferenceValue.GetType();
                List<Editor> editors = new();

                if (t.IsAssignableToGenericType(typeof(StructAdapterComponent<>)))
                {
                    foreach (UnityEngine.Component c in (IEnumerable<UnityEngine.Component>)t.GetMethod(nameof(StructAdapterComponent<float>.GetSourceInputs)).Invoke(prop.objectReferenceValue, null))
                    {
                        editors.Add(EditorExtensions.GetCachedEditor(c));
                    }
                }
                else
                {
                    editors.Add(EditorExtensions.GetCachedEditor(prop.objectReferenceValue));
                }

                EditorWindow_Edit_Component.Open(editors.ToArray());
            });

            return items;
        }
    }
}
