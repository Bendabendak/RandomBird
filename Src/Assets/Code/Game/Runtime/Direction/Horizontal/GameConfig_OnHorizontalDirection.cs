using SadJam;
using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/GameConfig/Movement/Horizontal/OnDirection")]
    public class GameConfig_OnHorizontalDirection : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        public enum DirectionType
        {
            Forward,
            Backward
        }

        [GameConfigSerializeProperty]
        public IGameConfig_HorizontalDirectional Config { get; }

        [field: Space, SerializeField]
        public DirectionType Direction { get; private set; }

        private static Dictionary<DirectionType, Func<IGameConfig_HorizontalDirectional, bool>> _directionCheckMap = new(2)
        {
            {
                DirectionType.Forward,
                (t) => t.HorizontalDirection > 0
            },
            {
                DirectionType.Backward,
                (t) => t.HorizontalDirection < 0
            }
        };

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            if (_directionCheckMap[Direction](Config))
            {
                Execute(Delta);
            }
        }
    }
}
