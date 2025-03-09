using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [ClassTypeAddress("Convertor/Transform/Rotation")]
    public class Convertor_Transform_LocalRotation : StructComponent<Quaternion>, IConvertor_Transform
    {
        public override string Label => GetLabel();

        private string GetLabel()
        {
            if (Transform == null)
            {
                return "Transform not set!";
            }

            return Transform.gameObject.name + " LocalRotation";
        }

        [field: SerializeField]
        public Transform Transform { get; set; }

        public override Quaternion Size => GetSize();

        private Quaternion GetSize()
        {
            if (Transform == null)
            {
                return new();
            }

            return Transform.localRotation;
        }
    }
}