using System;
using UnityEngine.Serialization;
using UnityEngine;

namespace SadJam.Components
{
    public class Constant_Float : StructComponent<Single>
    {
        [SerializeField, FormerlySerializedAs("<Size>k__BackingField")]
        private Single _size;
        public override Single Size => _size;

        public static Single operator +(Constant_Float a, StructComponent<Single> b)
        {
            return a.Size + b.Size;
        }
        public static Single operator -(Constant_Float a, StructComponent<Single> b)
        {
            return a.Size - b.Size;
        }
        public static Single operator /(Constant_Float a, StructComponent<Single> b)
        {
            return a.Size / b.Size;
        }
        public static Single operator *(Constant_Float a, StructComponent<Single> b)
        {
            return a.Size * b.Size;
        }
    }
}
