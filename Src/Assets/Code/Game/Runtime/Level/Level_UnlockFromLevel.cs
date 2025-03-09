using SadJam;
using SadJam.Components;
using System;
using UnityEngine;

namespace Game
{
    public class Level_UnlockFromLevel : SadJam.Component
    {
        [GameConfigSerializeProperty]
        public Level_Config Config { get; private set; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: SerializeField, Space]
        public Level Level { get; private set; }

        [field: Space, SerializeField]
        public AnimationClips UnlockClip { get; private set; }
        [field: SerializeField]
        public AnimationClips LockClip { get; private set; }

        [OnGameConfigChanged(nameof(Config))]
        [OnGameConfigChanged(nameof(Owner))]
        private void OnConfigChanged(string affected)
        {
            Enable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Enable();
        }

        [NonSerialized]
        private bool _unlocked = false;
        private void Enable()
        {
            if (!_unlocked && Config.GetCurrentProgress(Owner, out int xp))
            {
                Level_Config.LevelData currLevel = Config.GetLevel(xp);
                Level_Config.LevelData targetLevel = null;

                foreach (Level_Config.LevelData l in Config.Levels)
                {
                    if (l.Level == Level)
                    {
                        targetLevel = l;
                    }
                }

                if (targetLevel == null) return;

                if (Config.Levels.IndexOf(currLevel) >= Config.Levels.IndexOf(targetLevel))
                {
                    _unlocked = true;

                    UnlockClip.SkipToEnd();
                }
                else
                {
                    LockClip.SkipToEnd();
                }
            }

            Statistics.OnChanged -= OnChanged;
            Statistics.OnChanged += OnChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Statistics.OnChanged -= OnChanged;
        }

        private void OnChanged(string ownerId, Statistics.DataEntry data)
        {
            if (_unlocked || ownerId != Owner.Id || !data.VerifyNumeric(Config.LevelXpKey, out double dataN)) return;

            Level_Config.LevelData currLevel = Config.GetLevel((int)dataN);
            Level_Config.LevelData targetLevel = null;
            foreach(Level_Config.LevelData l in Config.Levels)
            {
                if (l.Level == Level)
                {
                    targetLevel = l;
                }
            }

            if (targetLevel == null) return;

            if (Config.Levels.IndexOf(currLevel) >= Config.Levels.IndexOf(targetLevel))
            {
                _unlocked = true;

                UnlockClip.Play(this);
            }
            else
            {
                LockClip.Play(this);
            }
        }
    }
}
