using UnityEngine;

namespace SadJam.Components
{
    public class Input_MousePos_World : Input_MousePos
    {
        public override Vector3 Size => Camera.main.ScreenToWorldPoint(base.Size);
    }
}
