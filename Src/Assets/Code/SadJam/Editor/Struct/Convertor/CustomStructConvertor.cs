using System;

namespace SadJamEditor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomStructConvertor : Attribute
    {
        public Type targetType;

        public CustomStructConvertor(Type targetType)
        {
            this.targetType = targetType;
        }
    }
}
