using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    public class ScriptCreatorEditorWindow : EditorWindow
    {
        public string sourcePath = @"Assets\Unlabeled";
        public string destPath = @"Assets\Unlabeled";

        public List<ReplaceWith> replaces = new List<ReplaceWith>();

        [MenuItem("Assets/Create/Script")]
        public static void Open()
        {
            ScriptCreatorEditorWindow window =
                (ScriptCreatorEditorWindow)GetWindow(typeof(ScriptCreatorEditorWindow));

            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Script creator", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            sourcePath = EditorGUILayout.TextField("Source path", sourcePath);

            if (EditorGUIExtensions.DoubleClick(GUILayoutUtility.GetLastRect()))
            {
                sourcePath = EditorUtility.OpenFolderPanel("Get Source Files", "", "");
            }

            destPath = EditorGUILayout.TextField("Destination path", destPath);

            if (EditorGUIExtensions.DoubleClick(GUILayoutUtility.GetLastRect()))
            {
                destPath = EditorUtility.OpenFolderPanel("Get Destination Folder", "", "");
            }

            for (int i = 0; i < replaces.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                replaces[i] = new ReplaceWith(EditorGUILayout.TextField("Replace", replaces[i].replace),
                    EditorGUILayout.TextField("Replace with", replaces[i].replaceWith));
                if (GUILayout.Button("Remove"))
                {
                    replaces.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Add replace"))
            {
                replaces.Add(new ReplaceWith());
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Save layout"))
            {
                SaveLayout(replaces, sourcePath, destPath);
            }

            if (GUILayout.Button("Load layout"))
            {
                replaces = LoadLayout(out sourcePath, out destPath).ToList();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Create"))
            {
                ScriptCreator.Create(replaces, Application.dataPath + "/" + sourcePath, Application.dataPath + "/" + destPath, this);
            }
        }

        public static void SaveLayout(IEnumerable<ReplaceWith> replaces, string sourcePath, string destPath)
        {
            string path = EditorUtility.SaveFilePanel("Save Script Creator Layout", "",
                "ScriptCreatorSave " + DateTime.UtcNow.ToShortDateString() + ".scriptCreator", "scriptCreator");

            if (string.IsNullOrWhiteSpace(path)) return;

            string data = sourcePath + "\n" + destPath + "\n";

            foreach (ReplaceWith replace in replaces)
            {
                data += replace.ToString() + "\n";
            }

            File.WriteAllText(path, data);
        }

        public static IEnumerable<ReplaceWith> LoadLayout(out string sourcePath, out string destPath)
        {
            string path = EditorUtility.OpenFilePanel("Load Script Creator Layout", "", "scriptCreator");

            if (string.IsNullOrWhiteSpace(path))
            {
                sourcePath = "";
                destPath = "";

                return Enumerable.Empty<ReplaceWith>();
            }

            List<string> lines = File.ReadLines(path).ToList();
            List<ReplaceWith> result = new List<ReplaceWith>();

            try
            {
                sourcePath = lines[0];
                destPath = lines[1];

                for (int i = 2; i < lines.Count; i += 2)
                {
                    if (i + 1 >= lines.Count) break;

                    result.Add(new ReplaceWith(lines[i], lines[i + 1]));
                }

                return result;
            }
            catch
            {
                Debug.LogError("Unable to load script creator layout!");

                sourcePath = "";
                destPath = "";

                return Enumerable.Empty<ReplaceWith>();
            }
        }
    }
}
