using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [ClassTypeAddress("Convertor/Transform/LocalPosition")]
    public class Convertor_Transform_LocalPosition : StructComponent<Vector3>, IConvertor_Transform
    {
        public override string Label => GetLabel();

        private string GetLabel()
        {
            if (Transform == null)
            {
                return "Transform not set!";
            }

            return Transform.gameObject.name + " LocalPosition";
        }

        [field: SerializeField]
        public Transform Transform { get; set; }

        public override Vector3 Size => GetSize();

        private Vector3 GetSize()
        {
            if (Transform == null)
            {
                return new();
            }

            return Transform.localPosition;
        }
    }
}
