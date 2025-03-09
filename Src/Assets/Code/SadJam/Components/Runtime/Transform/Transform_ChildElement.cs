using UnityEngine;

namespace SadJam.Components
{
    public class Transform_ChildElement : MonoBehaviour
    {
        [field: SerializeField]
        public bool Ignore { get; private set; }
    }
}
