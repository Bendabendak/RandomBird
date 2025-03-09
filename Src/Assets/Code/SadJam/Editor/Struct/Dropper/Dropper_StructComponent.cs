using SadJam;
using System;
using UnityEngine;

namespace SadJamEditor
{
    public abstract class Dropper_StructComponent<T> : Dropper where T : struct
    {
        public override void DropMe(object drop, object before, object target, GameObject context, Type resultType, Action<object> onDrop = null, params object[] customData)
        {
            StructConvertor<StructComponent<T>> convertor = StructConvertor<StructComponent<T>>.GetConvertor(resultType);

            if (convertor == null)
            {
                Debug.LogError("Convertor of type " + resultType.FullName + " on " + typeof(T).FullName + " dropper not found!");
                return;
            }

            convertor.ConvertMeAs(context, resultType, (StructComponent<T>)drop, before, Done, customData);

            void Done(object result)
            {
                onDrop?.Invoke(result);
            }
        }
    }
}
