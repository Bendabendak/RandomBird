using System;

namespace SadJamEditor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomDropper : Attribute
    {
        public Type targetType;

        public CustomDropper(Type targetType)
        {
            this.targetType = targetType;
        }
    }
}
