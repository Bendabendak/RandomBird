using SadJam;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomPropertyDrawer(typeof(StructComponent<Quaternion>), true)]
    public class PropertyDrawer_QuaternionComponent : PropertyDrawer_StructComponent<Quaternion> { }
}
