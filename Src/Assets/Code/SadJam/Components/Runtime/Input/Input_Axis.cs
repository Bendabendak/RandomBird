using UnityEngine;

namespace SadJam.Components
{
    /// <summary>
    /// X - Horizontal, Y - Vertical
    /// </summary>
    public class Input_Axis : StructComponent<Vector2>
    {
        public override Vector2 Size => new (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
