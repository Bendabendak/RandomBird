using System.Collections.Generic;
using SadJam;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Shop/Buyable/OnBought")]
    public class Shop_Buyable_OnBought : DynamicExecutor
    {
        public enum BoughtType
        {
            Bought,
            BoughtOut
        }

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
        public IGameConfig_Shop_Buyable Item { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: Space, SerializeField]
        public BoughtType Bought { get; private set; } = BoughtType.Bought;

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            IEnumerable<IGameConfig_Shop_Buyable> boughtList = Config.GetBought(Owner);
            if (boughtList == null) return;

            bool bought = false;
            foreach(IGameConfig_Shop_Buyable b in boughtList)
            {
                if (b == Item)
                {
                    bought = true;
                    break;
                }
            }

            switch (Bought)
            {
                case BoughtType.Bought:
                    if (bought)
                    {
                        Execute(Delta);
                    }
                    break;
                case BoughtType.BoughtOut:
                    if (!bought)
                    {
                        Execute(Delta);
                    }
                    break;
            }
        }
    }
}
