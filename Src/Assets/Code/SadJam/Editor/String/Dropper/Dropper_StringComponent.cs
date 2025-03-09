using SadJam;
using System;
using UnityEngine;

namespace SadJamEditor
{
    [CustomDropper(typeof(StringComponent))]
    public class Dropper_StringComponent : Dropper
    {
        public override void DropMe(object drop, object before, object target, GameObject context, Type resultType, Action<object> onDrop = null, params object[] customData)
        {
            StringComponent result = Convertor_ToString.Convert(context, (StringComponent)drop, before);

            EditorUtilityExtensions.SetDirty(result, "New string dropped");

            onDrop?.Invoke(result);
        }
    }
}
