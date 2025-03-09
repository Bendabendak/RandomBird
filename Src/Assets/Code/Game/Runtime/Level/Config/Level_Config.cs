using SadJam;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Level Config", menuName = "Game/Level/Config")]
    public class Level_Config : GameConfig
    {
        [Serializable]
        public class LevelData
        {
            public Level Level;
            public int XpThreshold = 0;
        }

        [BlendableField("LastLevelKey"), SerializeField]
        private Statistics_Key _lastLevelKey;
        [BlendableProperty("LastLevelKey")]
        public Statistics_Key LastLevelKey { get; set; }

        [BlendableField("LevelXpKey"), SerializeField]
        private Statistics_Key _levelXpKey;
        [BlendableProperty("LevelXpKey")]
        public Statistics_Key LevelXpKey { get; set; }

        [BlendableField("Levels"), SerializeField]
        private List<LevelData> _levels;
        [BlendableProperty("Levels")]
        public List<LevelData> Levels { get; set; }

        private static Comparison<LevelData> _levelXpComparison => (x, y) => x.XpThreshold.CompareTo(y.XpThreshold);
        protected override void OnValidate()
        {
            base.OnValidate();

            if (_levels.Count <= 0)
            {
                _levels.Add(new());
                return;
            }

            _levels.Sort(_levelXpComparison);
        }

        public void AddLevel(LevelData level)
        {
            _levels.Add(level);
            _levels.Sort(_levelXpComparison);
        }

        public bool RemoveLevel(LevelData level) => _levels.Remove(level);

        public bool GetCurrentProgress(Statistics.Owner owner, out int progress)
        {
            if (!Statistics.LoadNumericalStatus(owner, LevelXpKey, out double xpD, out Statistics.ErrorCodes loadError))
            {
                switch (loadError)
                {
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to get current progress, because " + LevelXpKey + " status is not type of double! Statistics owner: " + owner.Id, this);
                        progress = default;
                        return false;
                    case Statistics.ErrorCodes.StatusNotFound:
                        progress = 0;
                        return true;
                    default:
                        Debug.LogError("Unable to get current progress, because " + LevelXpKey + " status cannot be loaded! Error: " + loadError + " Statistics owner: " + owner.Id, this);
                        progress = default;
                        return false;
                }
            }

            progress = (int)xpD;
            return true;
        }

        public LevelData GetLevel(int xp)
        {
            LevelData level = _levels[0];
            foreach (LevelData l in  _levels)
            {
                if (l.XpThreshold > xp)
                {
                    break;
                }

                level = l;
            }

            return level;
        }

        public bool LeveledUp(Statistics.Owner owner, out bool leveledUp)
        {
            if(!Statistics.LoadStatus(owner, LastLevelKey, out string lastLevel, out Statistics.ErrorCodes loadError))
            {
                switch (loadError)
                {
                    case Statistics.ErrorCodes.StatusNotFound:
                        lastLevel = Levels[0].Level.Id;
                        break;
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to check if leveled up, because " + LastLevelKey.Id + " status is not type of boolean! Statistics owner: " + owner.Id    , this);
                        leveledUp = default;
                        return false;
                    default:
                        Debug.LogError("Unable to check if leveled up, because " + LastLevelKey.Id + " status cannot be loaded! Error: " + loadError + " Statistics owner: " + owner.Id, this);
                        leveledUp = default;
                        return false;
                }
            }

            if (GetCurrentProgress(owner, out int xp))
            {
                leveledUp = lastLevel != GetLevel(xp).Level.Id;
                return true;
            }

            leveledUp = default;
            return false;
        }

        public bool ConfirmLevelUp(Statistics.Owner owner)
        {
            if (GetCurrentProgress(owner, out int xp))
            {
                return Statistics.UpdateAndSaveStatistics(owner, new Statistics.DataEntry(LastLevelKey, GetLevel(xp).Level.Id));
            }

            return false;
        }
    }
}
