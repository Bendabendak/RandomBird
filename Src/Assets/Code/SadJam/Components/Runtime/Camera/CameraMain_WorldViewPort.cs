using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    public class CameraMain_WorldViewPort : StructComponent<Vector2>
    {
        public override Vector2 Size 
        {
            get 
            {
                float h = Camera.main.orthographicSize * 2;
                return new(h * Camera.main.aspect, h);
            }
        }
    }
}
