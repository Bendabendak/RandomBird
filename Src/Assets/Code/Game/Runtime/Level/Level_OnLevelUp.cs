using SadJam;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Level/OnLevelUp")]
    public class Level_OnLevelUp : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Level_Config Config { get; private set; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: SerializeField, Space]
        public bool ExecuteOnStatisticsChange { get; private set; } = true;

        [field: SerializeField, Space]
        public bool OnAnyLevel { get; private set; }
        [field: SerializeField]
        public List<Level> Levels { get; private set; }
        
        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            if (Config.LeveledUp(Owner, out bool leveledUp) && leveledUp && CheckLevelUp())
            {
                Execute(Delta);
            }
        }

        protected override void OnStartAndEnable()
        {
            base.OnStartAndEnable();

            Statistics.OnChanged -= OnChanged;
            Statistics.OnChanged += OnChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Statistics.OnChanged -= OnChanged;
        }

        private bool CheckLevelUp()
        {
            if (OnAnyLevel)
            {
                return true;
            }
            else if (Config.GetCurrentProgress(Owner, out int xp))
            {
                Level_Config.LevelData level = Config.GetLevel(xp);

                if (Levels.Contains(level.Level))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnChanged(string ownerId, Statistics.DataEntry data)
        {
            if (!ExecuteOnStatisticsChange) return;
            if (ownerId != Owner.Id || !data.Verify<string>(Config.LevelXpKey)) return;

            Delta = Time.deltaTime;
            OnExecute();
        }
    }
}
