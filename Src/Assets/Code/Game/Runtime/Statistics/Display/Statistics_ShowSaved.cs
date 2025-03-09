using SadJam;
using TMPro;
using UnityEngine;

namespace Game
{
    public class Statistics_ShowSaved : SadJam.Component
    {
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }
        [GameConfigSerializeProperty]
        public Statistics_Key StatusKey { get; }

        [field: Space, SerializeField]
        public string DefaultValue { get; private set; } = "0";

        [field: Space, SerializeField]
        public TMP_Text Text { get; private set; }
        [field: SerializeField]
        public string Prefix { get; private set; }
        [field: SerializeField]
        public string Suffix { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (Statistics.LoadStatus(Owner, StatusKey, out object data))
            {
                Display(data.ToString());
            }
            else
            {
                Display(DefaultValue);
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
            if (ownerId != Owner.Id || !data.Verify(StatusKey)) return;

            Display(data.Value.ToString());
        }

        private void Display(string s)
        {
            Text.text = Prefix + s + Suffix;
        }
    }
}
