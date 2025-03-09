using System;
using System.Collections.Generic;
using SadJam;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Candy Config", menuName = "Game/Candy/Config")]
    public class Candy_Config : GameConfig, IGameConfig_HorizontalDirectional
    {
        [Serializable]
        public class CandyData
        {
            [Space]
            public Game.Candy Candy;
            [Space]
            public int SpawnThreshold;
        }

        [BlendableField("Candies"), SerializeField]
        private List<CandyData> _candies;
        [BlendableProperty("Candies")]
        public List<CandyData> Candies { get; set; }

        [BlendableField("HorizontalDirection"), Space, Range(-1, 1), SerializeField]
        private int _horizontalDirection = 1;
        [BlendableProperty("HorizontalDirection")]
        public int HorizontalDirection { get; set; }

        public int Threshold { get; set; } = 0;

        protected override void OnValidate() 
        {
            base.OnValidate();

            _candies.Sort((x, y) => x.SpawnThreshold.CompareTo(y.SpawnThreshold));
        }
    }
}
