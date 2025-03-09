using SadJam;
using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/GameConfig/Toggleable/OnChanged")]
    public class GameConfig_OnToggleChanged : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        public enum ToggleableType
        {
            Both,
            Enabled,
            Disabled
        }

        [GameConfigSerializeProperty]
        public IGameConfig_Toggleable Config { get; }

        [field: Space, SerializeField]
        public ToggleableType Toggleable { get; private set; } = ToggleableType.Enabled;

        [field: Space, SerializeField]
        public bool CheckOnStart { get; private set; } = false;

        private static Dictionary<ToggleableType, Func<IGameConfig_Toggleable, bool>> _toggleableCheckMap = new(3)
        {
            {
                ToggleableType.Both,
                (t) => true
            },
            {
                ToggleableType.Enabled,
                (t) => t.Enabled
            },
            {
                ToggleableType.Disabled,
                (t) => !t.Enabled
            }
        };

        [OnGameConfigChanged(nameof(Config))]
        private void OnConfigChanged(string affected)
        {
            if (GameConfig.IsFieldAffected(affected, nameof(IGameConfig_Toggleable.Enabled), nameof(IGameConfig_Toggleable)))
            {
                if (_toggleableCheckMap[Toggleable](Config))
                {
                    Execute(Time.deltaTime);
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            if (!CheckOnStart) return;

            if (_toggleableCheckMap[Toggleable](Config))
            {
                Execute(Time.deltaTime);
            }
        }
    }
}