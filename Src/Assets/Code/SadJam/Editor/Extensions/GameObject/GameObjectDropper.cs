using System;
using UnityEditor;
using UnityEngine;
using SadJam;

namespace SadJamEditor
{
    [CustomDropper(typeof(GameObject))]
    public class GameObjectDropper : Dropper
    {
        public override void DropMe(object drop, object before, object target, GameObject context, Type resultType, Action<object> onDrop = null, params object[] customData)
        {
            GameObject me = (GameObject)drop;

            GenericMenu m = new();

            foreach (UnityEngine.Component o in me.GetComponentsInChildren<UnityEngine.Component>())
            {
                if (o == null) continue;

                m.AddItem(new(o.GetPath()), true, () =>
                {
                    NewDrop(o, before, target, context, resultType, onDrop, customData);
                });
            }

            GenericMenuExtensions.Show(m, "Drop selection", Event.current.mousePosition);
        }
    }
}
