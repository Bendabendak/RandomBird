using SadJam;
using Tymski;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Map", menuName = "Game/Map/Create")]
    public class Map : GameConfig, IGameConfig_Shop_Buyable, IGameConfig_Shop_Choosable
    {
        [BlendableField("MenuScene"), SerializeField]
        private SceneReference _menuScene;
        [BlendableProperty("MenuScene")]
        public SceneReference MenuScene { get; set; }

        [BlendableField("GameScene"), SerializeField]
        private SceneReference _gameScene;
        [BlendableProperty("GameScene")]
        public SceneReference GameScene { get; set; }

        [BlendableField("Id"), Space, SerializeField]
        private string _id;
        [BlendableProperty("Id")]
        public string Id { get; set; }

        [BlendableField("Price"), Space, SerializeField]
        private int _price;
        [BlendableProperty("Price")]
        public int Price { get; set; }
    }
}
