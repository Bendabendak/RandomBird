using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Candy", menuName = "Game/Candy/Create")]
    public class Candy : ScriptableObject
    {
        public GameObject Prefab;
    }
}
