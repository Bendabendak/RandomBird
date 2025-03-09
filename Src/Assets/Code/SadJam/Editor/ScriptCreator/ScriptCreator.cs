using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.Linq;
using System.Collections.Generic;

namespace SadJamEditor
{
    public static class ScriptCreator
    {
        public static void Create(IEnumerable<ReplaceWith> replace, string sourcePath, string destPath, object owner)
        {
            EditorCoroutineUtility.StartCoroutine(CreateScripts(replace, sourcePath, destPath), owner);
        }

        private static IEnumerator CreateScripts(IEnumerable<ReplaceWith> replace, string sourcePath, string destPath)
        {
            Task task = Task.Run(() =>
            {
                CreateScripts(sourcePath, destPath);

                foreach (string dir in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    CreateScripts(dir, string.Join(Path.DirectorySeparatorChar.ToString(),
                        destPath + new string(dir.Skip(dir.IndexOf(sourcePath) + sourcePath.Length).ToArray())));
                }

                void CreateScripts(string sourcePath, string destPath)
                {
                    if (!destPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        destPath += Path.DirectorySeparatorChar;
                    }

                    if (!sourcePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        sourcePath += Path.DirectorySeparatorChar;
                    }

                    if (!Directory.Exists(sourcePath))
                    {
                        Debug.LogError("Source path doesn't exists " + sourcePath);
                        return;
                    }

                    if (!Directory.Exists(destPath))
                    {
                        Debug.LogWarning("Creating missing destination path " + destPath);
                        Directory.CreateDirectory(destPath);
                    }

                    DirectoryInfo directoryInfo = new DirectoryInfo(sourcePath);

                    foreach (FileInfo file in directoryInfo.GetFiles("*.txt"))
                    {
                        FileInfo copy = file.CopyTo(destPath + file.Name);

                        StreamReader reader = new StreamReader(copy.OpenRead());

                        string fileContent = reader.ReadToEnd();

                        reader.Close();

                        string newName = file.Name.Substring(0, file.Name.Length - file.Extension.Length);

                        foreach (ReplaceWith r in replace)
                        {
                            fileContent = fileContent.Replace(r.replace, r.replaceWith);

                            newName = newName.Replace(r.replace, r.replaceWith);
                        }

                        File.WriteAllText(copy.FullName, "");

                        StreamWriter writer = new StreamWriter(copy.OpenWrite());

                        writer.Write(fileContent);

                        writer.Close();

                        copy.MoveTo(destPath + newName + ".cs");
                    }
                }
            });
            yield return new WaitUntil(() => task.IsCompleted);

            AssetDatabase.Refresh();
        }
    }
}
