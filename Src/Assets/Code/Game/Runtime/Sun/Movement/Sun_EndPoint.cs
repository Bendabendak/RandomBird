using SadJam.Components;
using UnityEngine;

namespace Game
{
    public class Sun_EndPoint : DynamicPoint
    {
        public override Vector3 Point_OnExecute()
        {
            Vector3 cameraRightDown = Camera.main.ViewportToWorldPoint(new(1, 0));

            return new(cameraRightDown.x + Offset.x, cameraRightDown.y + Offset.y);
        }
    }
}

