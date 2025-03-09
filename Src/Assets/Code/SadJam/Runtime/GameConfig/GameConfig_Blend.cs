using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam
{
    public class GameConfig_Blend : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [Serializable]
        public class Blending
        {
            public GameConfig_Selection<GameConfig> Target;
            public GameConfig_Selection<GameConfig> Blend;
        }

        [field: SerializeField]
        public List<Blending> Blendings { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            foreach(Blending b in Blendings)
            {
                b.Target.Config.Blend(b.Blend.Config);
            }
        }
    }
}
