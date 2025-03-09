using System;

namespace SadJam
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OnGameConfigChangedAttribute : Attribute
    {
        public string GameConfigPropertyName { get; private set; }

        public OnGameConfigChangedAttribute(string gameConfigPropertyName)
        {
            GameConfigPropertyName = gameConfigPropertyName;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OnNewGameConfigSetAttribute : Attribute
    {
        public string GameConfigPropertyName { get; private set; }

        public OnNewGameConfigSetAttribute(string gameConfigPropertyName)
        {
            GameConfigPropertyName = gameConfigPropertyName;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GameConfigSerializePropertyAttribute : Attribute
    {
        public Attribute[] FieldAttributes { get; private set; }

        public GameConfigSerializePropertyAttribute()
        {

        }

        public GameConfigSerializePropertyAttribute(params Attribute[] fieldAttributes)
        {
            FieldAttributes = fieldAttributes;
        }
    }
}
