using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components 
{ 
    public interface IGameConfig_Spawnable : IGameConfig
    {
        public IEnumerable<GameObject> Prefabs { get; }
        public GameObject Spawn(GameObject caller);
    }
}
