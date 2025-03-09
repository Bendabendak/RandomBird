using SadJam;
using UnityEngine;

namespace Game
{
    public class Level_ConfirmLevelUp : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Level_Config Config { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        protected override void DynamicExecutor_OnExecute()
        {
            Config.ConfirmLevelUp(Owner);
        }
    }
}
