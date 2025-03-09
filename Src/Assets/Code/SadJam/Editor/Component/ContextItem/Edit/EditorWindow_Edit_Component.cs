using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    public class EditorWindow_Edit_Component : EditorWindow
    {
        public Editor[] Editors { get; private set; }

        public static EditorWindow_Edit_Component Open(params Editor[] editors)
        {
            EditorWindow_Edit_Component window = GetWindow<EditorWindow_Edit_Component>();

            window.Editors = editors;
            window.OnOpen();

            window.Show();

            return window;
        }

        protected virtual void OnOpen()
        {

        }

        protected virtual void OnGUI()
        {
            foreach(Editor e in Editors)
            {
                e.OnInspectorGUI();
                GUILayout.Space(50);
            }
        }
    }
}
