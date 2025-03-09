using UnityEditor;
using SadJam;

namespace SadJamEditor.Components
{
    [CustomPropertyDrawer(typeof(StructComponent<float>), true)]
    public class PropertyDrawer_FloatComponent : PropertyDrawer_StructComponent<float> { }
}
