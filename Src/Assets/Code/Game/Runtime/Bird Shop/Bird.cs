using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Bird", menuName = "Game/Bird/Create")]
    public class Bird : GameConfig, IGameConfig_Spawnable, IGameConfig_Shop_Buyable, IGameConfig_Shop_Choosable, IGameConfig_Shop_Showable
    {
        [Serializable]
        public class Bought
        {
            public string Id = "";
            public bool Chosen = false;
        }

        [BlendableField("GamePrefab"), SerializeField]
        private GameObject _gamePrefab;
        [BlendableProperty("GamePrefab")]
        [BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public GameObject GamePrefab { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs { get { yield return GamePrefab; } }
        public GameObject Spawn(GameObject caller) => GamePrefab;

        [BlendableField("ShopPrefab"), SerializeField]
        private GameObject _shopPrefab;
        [BlendableProperty("ShopPrefab")]
        [BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Shop_Showable), nameof(IGameConfig_Shop_Showable.Prefab))]
        public GameObject ShopPrefab { get; set; }
        GameObject IGameConfig_Shop_Showable.Prefab => ShopPrefab;

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
