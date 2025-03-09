using UnityEngine;

namespace SadJam.Components
{
    public class Input_MousePos_3D: Input_MousePos
    {
        public LayerMask layerMask;

        public override Vector3 Size => Pos(base.Size);

        private Vector3 Pos(Vector3 mousePos)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask))
            {
                return hit.point;
            }

            return Vector3.zero;
        }
    }
}
