using System;

namespace SadJam
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DebugOnly : Attribute
    {

    }
}
