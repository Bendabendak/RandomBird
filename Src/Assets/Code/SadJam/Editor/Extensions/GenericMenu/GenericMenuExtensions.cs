using BinaryEgo.Editor.UI;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    public class GenericMenuItem
    {
        public GUIContent content;
        public bool on;
        public GenericMenu.MenuFunction func;
        public GenericMenu.MenuFunction2 func2;
        public object userData;

        public GenericMenuItem() { }
        public GenericMenuItem(GUIContent content, bool on, GenericMenu.MenuFunction func)
        {
            this.content = content;
            this.on = on;
            this.func = func;
        }

        public GenericMenuItem(GUIContent content, bool on, GenericMenu.MenuFunction2 func2, object userData)
        {
            this.content = content;
            this.on = on;
            this.func2 = func2;
            this.userData = userData;
        }
    }

    public static class GenericMenuExtensions
    {
        public static void Show(GenericMenu menu, string title, Vector2 pos)
        {
            GenericMenuPopup m = new(menu, title);

            UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(PopupWindow));
            foreach(UnityEngine.Object o in array)
            {
                ((PopupWindow)o).Close();
            }

            m.Show(pos);
        }

        public static IEnumerable<GUIContent> ConcatMenus(params IEnumerable<GUIContent>[] menus)
        {
            foreach (GUIContent c in GetResult("", -1, menus))
            {
                yield return c;
            }

            static IEnumerable<GUIContent> GetResult(string result, int index, IEnumerable<GUIContent>[] menus)
            {
                if (index >= menus.Length - 1)
                {
                    yield return new(result.Remove(result.Length - 1));
                    yield break;
                }

                index++;

                foreach (GUIContent content in menus[index])
                {
                    string root = result + content + "/";

                    foreach (GUIContent r in GetResult(root, index, menus))
                    {
                        yield return r;
                    }
                }
            }
        }
    }
}
