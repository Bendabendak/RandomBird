using UnityEngine;
using UnityEngine.Serialization;

namespace SadJam.Components
{
    public class Constant_Vector3 : StructComponent<Vector3>
    {
        [SerializeField, FormerlySerializedAs("<Size>k__BackingField")]
        private Vector3 _size;
        public override Vector3 Size => _size;

        public static Vector3 operator +(Constant_Vector3 a, StructComponent<Vector3> b)
        {
            return a.Size + b.Size;
        }
        public static Vector3 operator -(Constant_Vector3 a, StructComponent<Vector3> b)
        {
            return a.Size - b.Size;
        }
    }
}
