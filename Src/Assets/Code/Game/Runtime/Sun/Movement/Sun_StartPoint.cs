using SadJam.Components;
using UnityEngine;

namespace Game
{
    public class Sun_StartPoint : DynamicPoint
    {
        public override Vector3 Point_OnExecute()
        {
            Vector3 cameraLeftDown = Camera.main.ViewportToWorldPoint(new(0, 0));

            return new(cameraLeftDown.x + Offset.x, cameraLeftDown.y + Offset.y);
        }
    }
}
