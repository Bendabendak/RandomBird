using SadJam;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Statistics Key", menuName = "Game/Statistics/Key")]
    public class Statistics_Key : GameConfig
    {
        [BlendableField("Id"), SerializeField]
        private string _id;
        [BlendableProperty("Id")]
        public string Id { get; set; }

        [BlendableField("SaveOnDevice"), Space, SerializeField]
        private bool _saveOnDevice = true;
        [BlendableProperty("SaveOnDevice")]
        public bool SaveOnDevice { get; set; }
    }
}
