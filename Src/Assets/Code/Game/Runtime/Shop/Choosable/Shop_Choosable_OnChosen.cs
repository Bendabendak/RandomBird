using SadJam;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Shop/Choosable/OnChosen")]
    public class Shop_Choosable_OnChosen : DynamicExecutor
    {
        public enum ChooseType
        {
            Chosen,
            ChosenOut
        }

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
        public IGameConfig_Shop_Choosable Item { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: Space, SerializeField]
        public ChooseType Choose { get; private set; } = ChooseType.Chosen;

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            IEnumerable<IGameConfig_Shop_Choosable> chosenList = Config.GetChosen(Owner);
            if (chosenList == null) return;

            bool chosen = false;
            foreach (IGameConfig_Shop_Choosable b in chosenList)
            {
                if (b == Item)
                {
                    chosen = true;
                    break;
                }
            }

            switch (Choose)
            {
                case ChooseType.Chosen:
                    if (chosen)
                    {
                        Execute(Delta);
                    }
                    break;
                case ChooseType.ChosenOut:
                    if (!chosen)
                    {
                        Execute(Delta);
                    }
                    break;
            }
        }
    }
}

