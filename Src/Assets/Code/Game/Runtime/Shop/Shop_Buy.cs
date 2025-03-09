using SadJam;
using UnityEngine;

namespace Game
{
    public class Shop_Buy : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public IGameConfig_Shop_WithBuyables Config { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public IGameConfig_Shop_Buyable ItemToBuy { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            if (Config.Buy(Owner, ItemToBuy))
            {
                Execute(Delta);
            }
        }
    }
}
