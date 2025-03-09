using UnityEngine;

namespace SadJam.Components
{
    public class DynamicPoint_CameraViewport : DynamicPoint
    {
        [field: SerializeField]
        public Vector2 Viewport { get; private set; }

        public override Vector3 Point_OnExecute()
        {
            Vector3 cameraViewport = Camera.main.ViewportToWorldPoint(Viewport);

            return new(cameraViewport.x + transform.position.x + Offset.x, cameraViewport.y + transform.position.y + Offset.y, transform.position.z + Offset.z);
        }
    }
}
