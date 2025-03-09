using SadJam;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Game Config", menuName = "Game/Config")]
    public class Game_Config : GameConfig, IGameConfig_GameManager
    {
        [field: SerializeField]
        public int Seed { get; set; }

        [BlendableField("RandomSeed"), SerializeField]
        private bool _randomSeed;
        [BlendableProperty("RandomSeed")]
        public bool RandomSeed { get; set; }

        [BlendableField("TargetFPS"), Space, SerializeField]
        private int _targetFPS = 60;
        [BlendableProperty("TargetFPS")]
        public int TargetFPS { get; set; }
    }
}
