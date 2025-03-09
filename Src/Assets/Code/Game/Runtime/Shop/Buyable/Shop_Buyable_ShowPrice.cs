using SadJam;
using TMPro;
using UnityEngine;

namespace Game
{
    public class Shop_Buyable_ShowPrice : SadJam.Component
    {
        [GameConfigSerializeProperty]
        public IGameConfig_Shop_Buyable Item { get; }

        [field: Space, SerializeField]
        public TMP_Text Text { get; private set; }
        [field: SerializeField]
        public string TextPrefix { get; private set; } = "";
        [field: SerializeField]
        public string TextSuffix { get; private set; } = "";

        [OnGameConfigChanged(nameof(Item))]
        private void OnConfigChanged(string affected)
        {
            if (GameConfig.IsFieldAffected(affected, nameof(IGameConfig_Shop_Buyable.Price), nameof(IGameConfig_Shop_Buyable)))
            {
                ShowPrice();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            ShowPrice();
        }

        public void ShowPrice()
        {
            Text.text = TextPrefix + Item.Price + TextSuffix;
        }
    }
}
