using SadJam;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    [CustomEditor(typeof(GameConfig), true)]
    [CanEditMultipleObjects]
    public class GameConfig_Editor : ComponentEditor
    {
        public override void OnInspectorGUI()
        {
            GameConfig target = (GameConfig)serializedObject.targetObject;

            if (target.FieldsToBlend == null) return;

            serializedObject.Update();

            foreach (SerializedProperty prop in serializedObject.GetVisibleSerializedProperties(false))
            {
                FieldInfo fieldInfo = prop.GetField();
                GameConfig.BlendableField fieldAtt = fieldInfo.GetCustomAttribute<GameConfig.BlendableField>();

                if (fieldAtt == null)
                {
                    EditorGUIExtensions.DrawProperty(prop, new GUIContent(prop.displayName), true);
                    continue;
                }

                PropertyInfo targetPropertyInfo = null;
                GameConfig.BlendableProperty targetPropertyAtt = null;
                foreach (PropertyInfo propInfo in serializedObject.targetObject.GetType().GetProperties())
                {
                    GameConfig.BlendableProperty at = propInfo.GetCustomAttribute<GameConfig.BlendableProperty>();

                    if (at == null || at.Id != fieldAtt.Id) continue;

                    targetPropertyInfo = propInfo;
                    targetPropertyAtt = at;
                    break;
                }

                if (targetPropertyInfo == null)
                {
                    Debug.LogError("Property with same id as BlendableField attribute id " + fieldAtt.Id + " not found!", serializedObject.targetObject);
                    continue;
                }

                GameConfig.BlendField field = target.FieldsToBlend.FirstOrDefault(f => f.PropertyId == targetPropertyInfo.Name);
                if (field != null)
                {
                    string displayName = prop.displayName;
                    switch (field.Operation)
                    {
                        case '+':
                            displayName += " [+]";
                            break;
                        case '-':
                            displayName += " [-]";
                            break;
                        case ':':
                            displayName += " [/]";
                            break;
                        case '*':
                            displayName += " [*]";
                            break;
                    }

                    if (EditorGUIExtensions.DrawProperty(prop, new GUIContent(displayName), true))
                    {
                        Rect pos = GUILayoutUtility.GetLastRect();

                        EditorGUI.DrawRect(pos, new(255, 0, 0, 0.1f));

                        if (pos.Contains(Event.current.mousePosition))
                        {
                            switch (Event.current.type)
                            {
                                case EventType.MouseDown:
                                    GenericMenu menu = new();

                                    menu.AddItem(new("Remove Blend"), false, () =>
                                    {
                                        target.RemoveFieldFromBlend(field);
                                        EditorUtility.SetDirty(target);
                                    });

                                    menu.ShowAsContext();
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (EditorGUIExtensions.DrawProperty(prop, true))
                    {
                        Rect pos = GUILayoutUtility.GetLastRect();

                        if (pos.Contains(Event.current.mousePosition))
                        {
                            switch (Event.current.type)
                            {
                                case EventType.MouseDown:
                                    GenericMenu menu = new();

                                    field = new(targetPropertyAtt.Id);
                                    field.NumericType = (GameConfig.PropertyNumericType)(int)prop.numericType;

                                    if (prop.numericType != SerializedPropertyNumericType.Unknown)
                                    {
                                        menu.AddItem(new("Blend/_"), false, () =>
                                        {
                                            field.Operation = '_';
                                            target.AddFieldToBlend(field);
                                            EditorUtility.SetDirty(target);
                                        });

                                        menu.AddItem(new("Blend/+"), false, () =>
                                        {
                                            field.Operation = '+';
                                            target.AddFieldToBlend(field);
                                            EditorUtility.SetDirty(target);
                                        });

                                        menu.AddItem(new("Blend/-"), false, () =>
                                        {
                                            field.Operation = '-';
                                            target.AddFieldToBlend(field);
                                            EditorUtility.SetDirty(target);
                                        });

                                        menu.AddItem(new("Blend/*"), false, () =>
                                        {
                                            field.Operation = '*';
                                            target.AddFieldToBlend(field);
                                            EditorUtility.SetDirty(target);
                                        });

                                        menu.AddItem(new("Blend/:"), false, () =>
                                        {
                                            field.Operation = ':';
                                            target.AddFieldToBlend(field);
                                            EditorUtility.SetDirty(target);
                                        });
                                    }
                                    else
                                    {
                                        menu.AddItem(new("Blend"), false, () =>
                                        {
                                            field.Operation = '_';
                                            target.AddFieldToBlend(field);
                                            EditorUtility.SetDirty(target);
                                        });
                                    }

                                    menu.ShowAsContext();
                                    break;
                            }
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
