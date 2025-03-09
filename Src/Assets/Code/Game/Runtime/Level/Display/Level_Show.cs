using SadJam;
using TMPro;
using UnityEngine;

namespace Game
{
    public class Level_Show : SadJam.Component
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
        public string Suffix { get; private set; } = "";

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

        private void Enable()
        {
            if (Config.GetCurrentProgress(Owner, out int xp))
            {
                Display(Config.GetLevel(xp).Level.DisplayName);
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
            if (ownerId != Owner.Id || !data.VerifyNumeric(Config.LevelXpKey, out double dataN)) return;

            Display(Config.GetLevel((int)dataN).Level.DisplayName);
        }

        private void Display(string s)
        {
            Text.text = Prefix + s + Suffix;
        }
    }
}
