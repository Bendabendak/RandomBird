using UnityEngine;
using UnityEngine.Serialization;

namespace SadJam.Components
{
    public class Constant_Vector2 : StructComponent<Vector2>
    {
        [SerializeField, FormerlySerializedAs("<Size>k__BackingField")]
        private Vector2 _size;
        public override Vector2 Size => _size;

        public static Vector2 operator +(Constant_Vector2 a, StructComponent<Vector2> b)
        {
            return a.Size + b.Size;
        }
        public static Vector2 operator -(Constant_Vector2 a, StructComponent<Vector2> b)
        {
            return a.Size - b.Size;
        }
        public static Vector2 operator /(Constant_Vector2 a, StructComponent<Vector2> b)
        {
            return a.Size / b.Size;
        }
        public static Vector2 operator *(Constant_Vector2 a, StructComponent<Vector2> b)
        {
            return a.Size * b.Size;
        }
    }
}
