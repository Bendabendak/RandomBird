using SadJam;
using TMPro;
using UnityEngine;

namespace Game
{
    public class Level_ShowProgress : SadJam.Component
    {
        [GameConfigSerializeProperty]
        public Level_Config Config { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: Space, SerializeField]
        public TMP_Text Text { get; private set; }
        [field: SerializeField]
        public string Prefix { get; private set; } = "";
        [field: SerializeField]
        public string Separator { get; private set; } = "/";
        [field: SerializeField]
        public string MaximumLabel { get; private set; } = "maximum";
        [field: SerializeField]
        public string Suffix { get; private set; } = "";

        [OnGameConfigChanged(nameof(Owner))]
        [OnGameConfigChanged(nameof(Config))]
        private void OnConfigChanged(string affected)
        {
            Enable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Enable();
        }

        private void Enable()
        {
            if (Config.GetCurrentProgress(Owner, out int xp))
            {
                ShowProgress(xp);
            }

            Statistics.OnChanged -= OnChanged;
            Statistics.OnChanged += OnChanged;
        }

        private void ShowProgress(int xp)
        {
            Level_Config.LevelData level = Config.GetLevel(xp);
            int index = Config.Levels.IndexOf(level);

            if (index < 0 || index + 1 >= Config.Levels.Count)
            {
                Display(MaximumLabel);
            }
            else
            {
                Display(xp - level.XpThreshold + Separator + (Config.Levels[index + 1].XpThreshold - level.XpThreshold));
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Statistics.OnChanged -= OnChanged;
        }

        private void OnChanged(string ownerId, Statistics.DataEntry data)
        {
            if (ownerId != Owner.Id || !data.VerifyNumeric(Config.LevelXpKey, out double dataN)) return;

            ShowProgress((int)dataN);
        }

        private void Display(string s)
        {
            Text.text = Prefix + s + Suffix;
        }
    }
}
