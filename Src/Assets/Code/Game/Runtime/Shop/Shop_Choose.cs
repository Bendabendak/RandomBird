using SadJam;
using UnityEngine;

namespace Game
{
    public class Shop_Choose : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public IGameConfig_Shop_WithChoosables Config { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public IGameConfig_Shop_Choosable ItemToChoose { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            if (Config.Choose(Owner, ItemToChoose))
            {
                Execute(Delta);
            }
        }
    }
}

