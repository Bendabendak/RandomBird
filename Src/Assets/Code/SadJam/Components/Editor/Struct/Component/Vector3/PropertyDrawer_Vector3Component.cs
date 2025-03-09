using UnityEngine;
using UnityEditor;
using SadJam;

namespace SadJamEditor.Components
{
    [CustomPropertyDrawer(typeof(StructComponent<Vector3>), true)]
    public class PropertyDrawer_Vector3Component : PropertyDrawer_StructComponent<Vector3> { }
}
