using System;

namespace SadJamEditor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomComponentContextItem : Attribute
    {
        public Type TargetType { get; set; }

        public CustomComponentContextItem(Type targetType)
        {
            TargetType = targetType;
        }
    }
}
