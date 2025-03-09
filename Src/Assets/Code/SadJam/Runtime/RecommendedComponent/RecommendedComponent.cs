using System;

namespace SadJam
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RecommendedComponent : Attribute
    {
        public Type type;

        public RecommendedComponent(Type type)
        {
            this.type = type;
        }
    }
}
