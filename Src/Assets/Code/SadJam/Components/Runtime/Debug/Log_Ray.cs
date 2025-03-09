using System;
using UnityEngine;

namespace SadJam.Components
{
    public class Log_Ray : Log_Drawing
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void DynamicExecutor_OnExecute()
        {
            Color c;
            float d;
            bool dT;

            if (color == null) c = Color.white;
            else c = color;

            if (duration == null) d = 0;
            else d = duration;

            if (depthTest == null) dT = true;
            else dT = depthTest;

            Debug.DrawRay(start.Size, end.Size, c, d, dT);
        }
    }
}
