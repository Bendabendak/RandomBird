using System.Collections.Generic;
using UnityEngine;

namespace SadJam
{
    public class GameConfig_Reset : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public List<GameConfig_Selection<GameConfig>> Configs { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            foreach(GameConfig_Selection<GameConfig> config in Configs)
            {
                config.Config.ResetToDefault();
            }
        }
    }
}
