using System;
using SadJam;
using TypeReferences;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    [CustomDropper(typeof(Renderer))]
    public class Dropper_Renderer : Dropper
    {
        public override void DropMe(object drop, object before, object target, GameObject context, Type resultType, Action<object> onDrop = null, params object[] customData)
        {
            Renderer me = (Renderer)drop;

            GenericMenu m = new();

            foreach (Type convertorT in ClassTypeReference.GetFilteredTypes(new ClassExtendsAttribute(typeof(IConvertor_Renderer)) { AllowAbstract = false }))
            {
                string label = ClassTypeReferencePropertyDrawer.FormatGroupedTypeName(convertorT, ClassGrouping.ByAddress);
                m.AddItem(new(label), true, () =>
                {
                    GameObject gC = GarbageCollector.Get(context.transform);
                    UnityEngine.Object newO = gC.GetComponent(convertorT);

                    if (newO != null)
                    {
                        IConvertor_Renderer inIC = (IConvertor_Renderer)newO;
                        if (inIC.Renderer != me)
                        {
                            newO = gC.AddComponent(convertorT);
                        }
                    }
                    else
                    {
                        newO = gC.AddComponent(convertorT);
                    }

                    SadJam.IComponent newC = (SadJam.IComponent)newO;
                    IConvertor_Renderer newIC = (IConvertor_Renderer)newC;
                    newIC.Renderer = me;

                    NewDrop(newC, before, target, context, resultType, onDrop, customData);
                });
            }

            GenericMenuExtensions.Show(m, "Renderer selection", Event.current.mousePosition);
        }
    }
}
