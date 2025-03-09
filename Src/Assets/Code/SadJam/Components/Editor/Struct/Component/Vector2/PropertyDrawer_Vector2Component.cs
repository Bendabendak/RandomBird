using UnityEngine;
using UnityEditor;
using SadJam;

namespace SadJamEditor.Components
{
    [CustomPropertyDrawer(typeof(StructComponent<Vector2>), true)]
    public class PropertyDrawer_Vector2Component : PropertyDrawer_StructComponent<Vector2> { }
}
