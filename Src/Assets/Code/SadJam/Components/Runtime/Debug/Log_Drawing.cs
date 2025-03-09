using UnityEngine;

namespace SadJam.Components
{
    public abstract class Log_Drawing : DynamicExecutor
    {
        public StructComponent<Vector3> start;
        public StructComponent<Vector3> end;
        public StructComponent<Color> color;
        public StructComponent<float> duration;
        public StructComponent<bool> depthTest;
    }
}
