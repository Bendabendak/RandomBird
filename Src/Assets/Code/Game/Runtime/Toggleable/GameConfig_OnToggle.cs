using SadJam;
using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/GameConfig/Toggleable/OnToggle")]
    public class GameConfig_OnToggle : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        public enum ToggleableType
        {
            Enabled,
            Disabled
        }

        [GameConfigSerializeProperty]
        public IGameConfig_Toggleable Config { get; }

        [field: Space, SerializeField]
        public ToggleableType Toggleable { get; private set; }

        private static Dictionary<ToggleableType, Func<IGameConfig_Toggleable, bool>> _toggleableCheckMap = new(2)
        {
            {
                ToggleableType.Enabled,
                (t) => t.Enabled
            },
            {
                ToggleableType.Disabled,
                (t) => !t.Enabled
            }
        };

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            if (_toggleableCheckMap[Toggleable](Config))
            {
                Execute(Delta);
            }
        }
    }
}