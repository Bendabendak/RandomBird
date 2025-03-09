using SadJam;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Level", menuName = "Game/Level/Create")]
    public class Level : GameConfig
    {
        [BlendableField("Id"), SerializeField]
        private string _id;
        [BlendableProperty("Id")]
        public string Id { get; set; }

        [BlendableField("DisplayName"), Space, SerializeField]
        private string _displayName;
        [BlendableProperty("DisplayName")]
        public string DisplayName { get; set; }
    }
}
