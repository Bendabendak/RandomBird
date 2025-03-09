using SadJam;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Level/CurrentLevelBridge")]
    public class Level_CurrentLevelBridge : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Level_Config Config { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: SerializeField, Space]
        public List<Level> Levels { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (Config.GetCurrentProgress(Owner, out int xp))
            {
                Level_Config.LevelData currentLevel = Config.GetLevel(xp);

                if (Levels.Contains(currentLevel.Level))
                {
                    Execute(Delta);
                }
            }
        }
    }
}