using SadJam.Components;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomEditor(typeof(GameObjectAllBounds_Size), true)]
    [CanEditMultipleObjects]
    public class GameObjectAllBoundsEditor_Size : Editor_Vector3Component
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GameObjectAllBounds_Size t = (GameObjectAllBounds_Size)target;

            if (GUILayout.Button("Recalculate"))
            {
                t.Recalculate();
            }
        }
    }
}
