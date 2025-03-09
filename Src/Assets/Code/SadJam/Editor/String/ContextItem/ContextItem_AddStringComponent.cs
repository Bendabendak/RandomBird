using SadJam;
using System;
using System.Collections.Generic;
using System.Linq;
using TypeReferences;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    [CustomComponentContextItem(typeof(StringComponent))]
    public class ContextItem_AddStringComponent : ComponentContextItem
    {
        public static string name = "Add";

        private List<Type> _stringComponents;

        protected override void Awake()
        {
            base.Awake();

            _stringComponents = ClassTypeReference.GetFilteredTypes(new ClassExtendsAttribute(typeof(StringComponent)) { AllowAbstract = false }).ToList();
            _stringComponents.Add(typeof(StringComponent));
        }

        public static IComponent AddStringComponent(Type t, UnityEngine.Component creator)
        {
            GameObject gc = GarbageCollector.Get(creator.transform);

            object[] atts = t.GetCustomAttributes(typeof(StaticStringComponent), false);

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

            foreach (Type t in _stringComponents)
            {
                if (t.GetCustomAttributes(typeof(AddFromCodeOnly), false).Length > 0) continue;

                items.Add(new(new("Add/" + ClassTypeReferencePropertyDrawer.FormatGroupedTypeName(t, ClassGrouping.ByAddress)), false, () =>
                {
                    if (prop.serializedObject.targetObject is not UnityEngine.Component obj)
                    {
                        Debug.LogError("Target must be a " + nameof(UnityEngine.Component) + "!");
                        return;
                    }

                    IComponent c = AddStringComponent(t, obj);

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
