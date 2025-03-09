using UnityEngine;

namespace SadJam
{
    [CreateAssetMenu(fileName = "GlobalSettingsMember", menuName = "SadJam/Settings/Global/Member")]
    public class GlobalSettingsMember : ScriptableObject
    {
        [field:SerializeField]
        public string Label { get; private set; }
        [field:SerializeField]
        public SettingsMemberType Type { get; private set; }
        public object Value { get; set; }
    }
}
