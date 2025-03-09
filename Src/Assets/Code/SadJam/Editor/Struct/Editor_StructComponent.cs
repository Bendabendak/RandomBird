using System;
using System.Reflection;

namespace SadJamEditor
{
    public abstract class Editor_StructComponent<T> : ComponentEditor where T : struct
    {
        [NonSerialized]
        protected Func<T> _getter;
        protected virtual void OnEnable() 
        {
            PropertyInfo sizePropInfo = target.GetType().GetProperty("Size");

            if (sizePropInfo == null) return;

            _getter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), target, sizePropInfo.GetGetMethod(true));
        }
    }
}
