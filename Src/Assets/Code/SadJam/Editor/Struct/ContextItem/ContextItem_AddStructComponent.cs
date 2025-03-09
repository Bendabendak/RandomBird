using System;
using UnityEngine;
using SadJam;
using System.Collections.Generic;
using TypeReferences;
using UnityEditor;
using System.Linq;

namespace SadJamEditor
{
    [CustomComponentContextItem(typeof(StructComponent<>))]
    public class ContextItem_AddStructComponent : ComponentContextItem
    {
        public static string name = "Add";

        private List<Type> _structComponents;

        protected override void Awake()
        {
            base.Awake();

            _structComponents = ClassTypeReference.GetFilteredTypes(new ClassExtendsAttribute(typeof(StructComponent<>)) { AllowAbstract = false }).ToList();
        }

        public static IComponent AddStructComponent(Type t, UnityEngine.Component creator)
        {
            GameObject gc = GarbageCollector.Get(creator.transform);

            object[] atts = t.GetCustomAttributes(typeof(StaticStructComponent), false);

            if (atts == null || atts.Length <= 0)
            {
                return (IComponent)gc.AddComponent(t);
            }

            UnityEngine.Component c = gc.GetComponent(t);

            if (c == null)
            {
                c = gc.AddComponent(t);
            }

            return (IComponent)c;
        }

        public override GenericMenuItem[] GetItems(SerializedProperty prop)
        {
            List<GenericMenuItem> items = new();

            foreach (Type t in _structComponents)
            {
                if (t.GetCustomAttributes(typeof(AddFromCodeOnly), false).Length > 0) continue;

                items.Add(new(new("Add/" + ClassTypeReferencePropertyDrawer.FormatGroupedTypeName(t, ClassGrouping.ByAddress)), false, () =>
                {
                    if (prop.serializedObject.targetObject is not UnityEngine.Component obj)
                    {
                        Debug.LogError("Target must be a " + nameof(UnityEngine.Component) + "!");
                        return;
                    }

                    IComponent c = AddStructComponent(t, obj);

                    UnityEngine.Component target = (UnityEngine.Component)prop.serializedObject.targetObject;

                    Dropper.NewDrop(c, prop.objectReferenceValue, target, target.gameObject, prop.GetField().FieldType, (object result) =>
                    {
                        prop.objectReferenceValue = (UnityEngine.Object)result;
                        prop.serializedObject.ApplyModifiedProperties();
                        EditorUtilityExtensions.SetDirty(target, result.GetType().FullName + " droped");
                    });
                }));
            }

            return items.ToArray();
        }
    }
}
