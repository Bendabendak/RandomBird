using SadJam;
using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/GameConfig/Movement/Horizontal/OnDirectionChanged")]
    public class GameConfig_OnHorizontalDirectionChanged : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        public enum DirectionType
        {
            Both,
            Forward,
            Backward
        }

        [GameConfigSerializeProperty]
        public IGameConfig_HorizontalDirectional Config { get; }

        [field: Space, SerializeField]
        public DirectionType Direction { get; private set; }

        [field: Space, SerializeField]
        public bool CheckOnStart { get; private set; } = false;

        private static Dictionary<DirectionType, Func<IGameConfig_HorizontalDirectional, bool>> _directionCheckMap = new(3)
        {
            {
                DirectionType.Both,
                (t) => true
            },
            {
                DirectionType.Forward,
                (t) => t.HorizontalDirection > 0
            },
            {
                DirectionType.Backward,
                (t) => t.HorizontalDirection < 0
            }
        };

        [OnGameConfigChanged(nameof(Config))]
        private void OnConfigChanged(string affected)
        {
            if (GameConfig.IsFieldAffected(affected, nameof(IGameConfig_HorizontalDirectional.HorizontalDirection), nameof(IGameConfig_HorizontalDirectional)))
            {
                if (_directionCheckMap[Direction](Config))
                {
                    Execute(Time.deltaTime);
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            if (!CheckOnStart) return;

            if (_directionCheckMap[Direction](Config))
            {
                Execute(Time.deltaTime);
            }
        }
    }
}

