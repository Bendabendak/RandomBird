using System;
using UnityEngine;

namespace SadJam
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class Abs : PropertyAttribute
    {
        public Abs() { }
    }
}
