using System;
using UnityEditor;
using UnityEngine;
using TypeReferences;
using SadJam;

namespace SadJamEditor
{
    [CustomDropper(typeof(Transform))]
    public class Dropper_Transform : Dropper
    {
        public override void DropMe(object drop, object before, object target, GameObject context, Type resultType, Action<object> onDrop = null, params object[] customData)
        {
            Transform me = (Transform)drop;

            GenericMenu m = new();

            foreach(Type convertorT in ClassTypeReference.GetFilteredTypes(new ClassExtendsAttribute(typeof(IConvertor_Transform)) { AllowAbstract = false }))
            {
                string label = ClassTypeReferencePropertyDrawer.FormatGroupedTypeName(convertorT, ClassGrouping.ByAddress);
                m.AddItem(new(label), true, () =>
                {
                    GameObject gC = GarbageCollector.Get(context.transform);
                    UnityEngine.Object newO = gC.GetComponent(convertorT);

                    if(newO != null)
                    {
                        IConvertor_Transform inIC = (IConvertor_Transform)newO;
                        if(inIC.Transform != me)
                        {
                            newO = gC.AddComponent(convertorT);
                        }
                    }
                    else
                    {
                        newO = gC.AddComponent(convertorT);
                    }

                    SadJam.IComponent newC = (SadJam.IComponent)newO;
                    IConvertor_Transform newIC = (IConvertor_Transform)newC;
                    newIC.Transform = me;

                    NewDrop(newC, before, target, context, resultType, onDrop, customData);
                });
            }

            GenericMenuExtensions.Show(m, "Transform selection", Event.current.mousePosition);
        }
    }
}
