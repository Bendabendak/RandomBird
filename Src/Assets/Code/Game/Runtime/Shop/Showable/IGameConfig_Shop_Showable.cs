using SadJam;
using UnityEngine;

namespace Game
{
    public interface IGameConfig_Shop_Showable : IGameConfig
    {
        public GameObject Prefab { get; }
    }
}
