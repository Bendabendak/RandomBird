using SadJam;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Day Config", menuName = "Game/Day/Config")]
    public class Day_Config : GameConfig
    {
        public Day_TransformStatus TransformStatus;

        [Space]
        public float DayInterval = 10f;
        public float NightInterval = 5f;
    }
}
