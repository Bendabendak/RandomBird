using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Reflection;
using SadJam;
using System.Linq;

namespace SadJamEditor
{
    [Flags]
    public enum EditorListOption
    {
        None = 0,
        ListSize = 1,
        ListLabel = 2,
        ElementLabels = 4,
        Buttons = 8,
        Default = ListSize | ListLabel | ElementLabels,
        NoElementLabels = ListSize | ListLabel,
        All = Default | Buttons
    }

    public static class EditorGUIExtensions
    {
        public static void DrawPrefabOverridden(Rect pos)
        {
            EditorGUI.DrawRect(pos, new Color(0.059f, 0.506f, 0.745f));
        }

        private static Dictionary<Editor, bool> _expanded = new();

        /// <returns>expanded; position</returns>
        public static IEnumerable<Tuple<bool, Rect>> DrawEditors(IEnumerable<Editor> editors)
        {
            int lastIndentLevel = EditorGUI.indentLevel;
            GameObject currGameObject = null;

            foreach (Editor e in editors)
            {
                if (e == null || e.target == null) continue;

                if (!_expanded.ContainsKey(e))
                {
                    _expanded[e] = false;
                }

                if (e.target is UnityEngine.Component c)
                {
                    if(currGameObject == null)
                    {
                        EditorGUILayout.LabelField(c.gameObject.name);
                        currGameObject = c.gameObject;
                    }

                    if (currGameObject != c.gameObject)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.Space(8);
                        EditorGUILayout.LabelField(c.gameObject.name);
                        currGameObject = c.gameObject;
                    }

                    if (c is SadJam.IComponent iC)
                    {
                        _expanded[e] = EditorGUILayout.Foldout(_expanded[e], iC.Label);
                    }
                    else
                    {
                        _expanded[e] = EditorGUILayout.Foldout(_expanded[e], c.GetType().Name);
                    }
                }
                else
                {
                    _expanded[e] = EditorGUILayout.Foldout(_expanded[e], e.target.GetType().Name);
                }

                if (_expanded[e])
                {
                    EditorGUI.indentLevel += 1;
                    e.OnInspectorGUI();
                    EditorGUI.indentLevel -= 1;

                    yield return new(true, GUILayoutUtility.GetLastRect());
                }

                yield return new(false, GUILayoutUtility.GetLastRect());
            }

            EditorGUI.indentLevel = lastIndentLevel;
        }

        private static Dictionary<object, List<string>> _debugOnly = new();
        public static bool DrawProperty(SerializedProperty property, GUIContent label, bool includeChildren, params GUILayoutOption[] options)
        {
            if (!_debugOnly.ContainsKey(property.serializedObject.targetObject))
            {
                _debugOnly.Add(property.serializedObject.targetObject, property.serializedObject.targetObject.GetType().GetAllFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).Where(f => f.IsDefined(typeof(DebugOnly), true)).Select(f => f.Name).ToList());
                _debugOnly[property.serializedObject.targetObject].Add("m_Script");
            }

            if (_debugOnly[property.serializedObject.targetObject].Contains(property.name))
            {
                if ((bool)GlobalSettings.Get("Debug").Value)
                {
                    EditorGUILayout.PropertyField(property, new("**" + label.text + "**"), includeChildren, options);
                    return true;
                }

                return false;
            }

            EditorGUILayout.PropertyField(property, label, includeChildren, options);
            return true;
        }
        public static bool DrawProperty(SerializedProperty property, params GUILayoutOption[] options)
        {
            return DrawProperty(property, new(property.displayName), true, options);
        }
        public static bool DrawProperty(SerializedProperty property, bool includeChildren, params GUILayoutOption[] options)
        {
            return DrawProperty(property, new(property.displayName), includeChildren, options);
        }
        public static bool DrawProperty(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            return DrawProperty(property, label, false, options);
        }

        private static GUIContent _moveButtonListContent = new GUIContent("\u21b4", "move down");
        private static GUIContent _duplicateButtonListContent = new GUIContent("+", "duplicate");
        private static GUIContent _deleteButtonListContent = new GUIContent("-", "delete");
        private static GUIContent _addButtonListContent = new GUIContent("+", "add element");

        private static GUILayoutOption _miniButtonListWidth = GUILayout.Width(20f);

        public static Dictionary<SerializedProperty, Rect> DrawList(SerializedProperty list, EditorListOption options = EditorListOption.All)
        {
            if (!list.isArray)
            {
                EditorGUILayout.HelpBox(list.name + " is neither an array nor a list!", MessageType.Error);
                return new();
            }

            bool
                showListLabel = (options & EditorListOption.ListLabel) != 0,
                showListSize = (options & EditorListOption.ListSize) != 0;

            if (showListLabel)
            {
                EditorGUILayout.PropertyField(list);
                EditorGUI.indentLevel += 1;
            }

            if (!showListLabel || list.isExpanded)
            {
                SerializedProperty size = list.FindPropertyRelative("Array.size");
                if (showListSize)
                {
                    EditorGUILayout.PropertyField(size);
                }
                if (size.hasMultipleDifferentValues)
                {
                    EditorGUILayout.HelpBox("Not showing lists with different sizes.", MessageType.Info);
                }
                else
                {
                    return ShowElements(list, options);
                }
            }

            if (showListLabel)
            {
                EditorGUI.indentLevel -= 1;
            }

            return new();
        }

        private static Dictionary<SerializedProperty, Rect> ShowElements(SerializedProperty list, EditorListOption options)
        {
            bool
                showElementLabels = (options & EditorListOption.ElementLabels) != 0,
                showButtons = (options & EditorListOption.Buttons) != 0;

            Dictionary<SerializedProperty, Rect> l = new();
            for (int i = 0; i < list.arraySize; i++)
            {
                if (showButtons)
                {
                    EditorGUILayout.BeginHorizontal();
                }
                SerializedProperty p = list.GetArrayElementAtIndex(i);
                if (showElementLabels)
                {
                    EditorGUILayout.PropertyField(p);
                }
                else
                {
                    EditorGUILayout.PropertyField(p, GUIContent.none);
                }

                l[p] = GUILayoutUtility.GetLastRect();

                if (showButtons)
                {
                    ShowButtons(list, i);
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (showButtons && list.arraySize == 0 && GUILayout.Button(_addButtonListContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
            }

            return l;
        }

        private static void ShowButtons(SerializedProperty list, int index)
        {
            if (GUILayout.Button(_moveButtonListContent, EditorStyles.miniButtonLeft, _miniButtonListWidth))
            {
                list.MoveArrayElement(index, index + 1);
            }
            if (GUILayout.Button(_duplicateButtonListContent, EditorStyles.miniButtonMid, _miniButtonListWidth))
            {
                list.InsertArrayElementAtIndex(index);
            }
            if (GUILayout.Button(_deleteButtonListContent, EditorStyles.miniButtonRight, _miniButtonListWidth))
            {
                int oldSize = list.arraySize;
                list.DeleteArrayElementAtIndex(index);
                if (list.arraySize == oldSize)
                {
                    list.DeleteArrayElementAtIndex(index);
                }
            }
        }

        public static void DrawContextItemsSelector(Rect pos, SerializedProperty prop)
        {
            DrawContextItemsSelector(pos, prop, new GUIContent(""));
        }

        public static void DrawContextItemsSelector(Rect pos, SerializedProperty prop, GUIContent label)
        {
            DrawContextItemsSelector(pos, prop.GetField().FieldType, prop, label);
        }

        public static void DrawContextItemsSelector(Rect pos, SerializedProperty prop, string label)
        {
            DrawContextItemsSelector(pos, prop.GetField().FieldType, prop, new GUIContent(label));
        }

        public static void DrawContextItemsSelector(Rect pos, Type targetType, SerializedProperty prop)
        {
            DrawContextItemsSelector(pos, targetType, prop, new GUIContent(""));
        }

        public static void DrawContextItemsSelector(Rect pos, Type targetType, SerializedProperty prop, string label)
        {
            DrawContextItemsSelector(pos, targetType, prop, new GUIContent(label));
        }

        public static void DrawContextItemsSelector(Rect pos, Type targetType, SerializedProperty prop, GUIContent label)
        {
            pos.x += EditorStyles.label.CalcSize(label).x;

            List<GenericMenuItem> items = new();
            foreach (ComponentContextItem c in ComponentContextItem.GetContextItems(targetType))
            {
                items.AddRange(c.GetItems(prop));
            }

            if (pos.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.ContextClick:
                        GenericMenu menu = new GenericMenu();

                        foreach (GenericMenuItem item in items)
                        {
                            if (item == null) continue;

                            if (item.func2 == null || item.userData == null)
                            {
                                menu.AddItem(item.content, item.on, item.func);
                                continue;
                            }

                            menu.AddItem(item.content, item.on, item.func2, item.userData);
                        }

                        GenericMenuExtensions.Show(menu, "Context items", Event.current.mousePosition);
                        break;
                }
            }
        }

        public static float Foldout(Rect pos, GUIContent label, SerializedProperty prop, params SerializedProperty[] content)
        {
            prop.isExpanded = EditorGUI.Foldout(new Rect(pos.min.x, pos.min.y, pos.min.x, EditorGUIUtility.singleLineHeight), prop.isExpanded, label);

            EditorGUI.indentLevel++;
            float height = EditorGUIUtility.singleLineHeight;
            if (prop.isExpanded)
            {
                int index = 0;
                foreach(SerializedProperty p in content)
                {
                    index++;
                    float h = EditorGUI.GetPropertyHeight(p);
                    Rect r = new(pos.min.x, pos.min.y + height, pos.size.x, h);
                    height += h;
                    EditorGUI.PropertyField(r, p);
                }
            }
            EditorGUI.indentLevel--;

            return height;
        }

        private static Dictionary<string, float> _propertiesHeight = new();
        public static float GetPropertyHeight(SerializedProperty property)
        {
            string id = GetPropertyID(property);
            if (!_propertiesHeight.TryGetValue(id, out float height))
            {
                return 0;
            }

            return height;
        }

        public static void SetPropertyHeight(SerializedProperty property, float height)
        {
            _propertiesHeight[GetPropertyID(property)] = height;
        }

        private static string GetPropertyID(SerializedProperty property) => property.propertyPath + " " + property.serializedObject.targetObject.GetInstanceID();

        public static void List(IList list, Type type)
        {
            int newCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("size", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(null);

            for (int i = 0; i < list.Count; i++)
            {
                list[i] = EditorGUILayout.ObjectField((UnityEngine.Object)list[i], type, true);
            }
        }

        public static string Selection(Rect position, GUIContent label, List<string> selection, string value)
        {
            int selectedIndex = selection.IndexOf(value);

            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, selection.ToArray());

            if (selectedIndex > selection.Count - 1) return null;

            if (selectedIndex < 0)
            {
                if (selection.Count > 0)
                {
                    selectedIndex = 0;
                }
                else
                {
                    return null;
                }
            }

            return selection[selectedIndex];
        }

        public static void DoubleOperation(string prop1, string prop2, string operationOperator)
        {
            GUIStyle propStyle = new GUIStyle(GUI.skin.textField);
            propStyle.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.TextField(prop1, propStyle, GUILayout.Width(200), GUILayout.MinWidth(0));
            GUILayout.Space(20);
            GUILayout.Label(operationOperator);
            GUILayout.Space(20);
            EditorGUILayout.TextField(prop2, propStyle, GUILayout.Width(200), GUILayout.MinWidth(0));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        public static void DoubleOperation(SerializedProperty prop1, SerializedProperty prop2, string operationOperator)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.PropertyField(prop1, GUIContent.none, GUILayout.Width(200), GUILayout.MinWidth(0));
            GUILayout.Space(20);
            GUILayout.Label(operationOperator);
            GUILayout.Space(20);
            EditorGUILayout.PropertyField(prop2, GUIContent.none, GUILayout.Width(200), GUILayout.MinWidth(0));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        public static bool DoubleClick(Rect pos)
        {
            Event e = Event.current;

            return e.isMouse && e.clickCount >= 2 && pos.Contains(e.mousePosition);
        }

        #region Result
        public static void Result<T>(T result)
        {
            switch (result)
            {
                case Vector3 v:
                    Result(v);
                    break;
                case Vector2 v:
                    Result(v);
                    break;
                case float f:
                    Result(f);
                    break;
                case int i:
                    Result(i);
                    break;
                case double d:
                    Result(d);
                    break;
                case AnimationCurve c:
                    Result(c);
                    break;
                case Color c:
                    Result(c);
                    break;
            }
        }

        public static void Result(Color result)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.ColorField("", result);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        public static void Result(AnimationCurve result)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.CurveField("", result);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        public static void Result(string result)
        {
            GUIStyle style = new GUIStyle(GUI.skin.textField);
            style.alignment = TextAnchor.MiddleCenter;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(result, style);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        public static void Result(Vector3 result)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.Vector3Field("", result);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        public static void Result(Vector2 result)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.Vector2Field("", result);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        public static void Result(double result)
        {
            GUIStyle style = new GUIStyle(GUI.skin.textField);
            style.alignment = TextAnchor.MiddleCenter;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.DoubleField("", result, style);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        public static void Result(int result)
        {
            GUIStyle style = new GUIStyle(GUI.skin.textField);
            style.alignment = TextAnchor.MiddleCenter;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.IntField("", result, style);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        public static void Result(float result)
        {
            GUIStyle style = new GUIStyle(GUI.skin.textField);
            style.alignment = TextAnchor.MiddleCenter;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.FloatField("", result, style);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }
        #endregion
    }
}
