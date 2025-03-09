using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [ClassTypeAddress("Convertor/Renderer/Bounds/Min")]
    public class Convertor_Renderer_Bounds_Min : StructComponent<Vector3>, IConvertor_Renderer
    {
        public override string Label => GetLabel();

        private string GetLabel()
        {
            if (Renderer == null)
            {
                return "Renderer not set!";
            }

            return Renderer.gameObject.name + " Renderer Bounds Min";
        }

        [field: SerializeField]
        public Renderer Renderer { get; set; }

        public override Vector3 Size => GetSize();

        private Vector3 GetSize()
        {
            if (Renderer == null)
            {
                return new();
            }

            return Renderer.bounds.min;
        }
    }
}
