using UnityEngine;
using UnityEngine.Serialization;

namespace SadJam.Components
{
    public class Constant_Quaternion : StructComponent<Quaternion>
    {
        [SerializeField, FormerlySerializedAs("<Size>k__BackingField")]
        private Quaternion _size;
        public override Quaternion Size => _size;

        public static Vector3 operator *(Constant_Quaternion a, StructComponent<Vector3> b)
        {
            return a.Size * b.Size;
        }

        public static Quaternion operator *(Constant_Quaternion a, StructComponent<Quaternion> b)
        {
            return a.Size * b.Size;
        }
    }
}
