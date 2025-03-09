using SadJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeReferences;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    public abstract class EditorWindow_StructCalculatorComponent<T> : EditorWindow where T : struct
    {
        public StructCalculatorComponent<T> target;
        public GameObject structsHodler;

        public int maxInRow = 3;

        private GameObject temp;

        public void Open(StructCalculatorComponent<T> target)
        {
            EditorWindow_StructCalculatorComponent<T> window =
                (EditorWindow_StructCalculatorComponent<T>)GetWindow(GetType());

            window.target = target;

            window.OnOpen();

            window.Show();
        }

        public void RemoveMember(StructCalculatorMember<T> member)
        {
            target.members.Remove(member);

            RemoveComponentFromEditing(member.component);

            if(!target.members.Any(m => m.component == member.component))
            {
                member.component.RemoveReference(target);
            }
        }

        public void AddMember(IComponent component, StructCalculatorOperator<T> operation, params string[] customData)
        {
            if (component is UnityEngine.Component c)
            {
                if (target.members.Any(m => m.component == c))
                {
                    return;
                }
                GameObject garbage = GarbageCollector.Get(target.transform);
 
                StructCalculatorMember<T> member = new() { operation = operation };

                FieldInfo field = member.GetType().GetField(nameof(member.component));

                UnityEngine.Component copy = garbage.CopyComponent(c);

                Dropper.NewDrop(copy, field.GetValue(member), member, target.gameObject, field.FieldType, (object result) =>
                {
                    member.component = (StructComponent<T>)result;
                    target.members.Add(member);
                }, customData);

                return;
            }

            Debug.LogError(component.GetType().FullName + " must be a component!");
        }

        public void EditComponent(IComponent component)
        {
            if(component is not StructAdapterComponent<T> adapter)
            {
                _componentEditorsToEdit.Add(EditorExtensions.GetCachedEditor((UnityEngine.Object)component));
                return;
            }

            foreach(UnityEngine.Component c in adapter.GetSourceInputs())
            {
                _componentEditorsToEdit.Add(EditorExtensions.GetCachedEditor(c));
            }
        }

        public void RemoveComponentFromEditing(IComponent component)
        {
            if (component == null)
            {
                _componentEditorsToEdit.RemoveAll(e => e.target == null);

                return;
            }

            if (component is StructAdapterComponent<T> adapter)
            {
                foreach (UnityEngine.Component target in _componentEditorsToEdit.Select(e => e.target).Intersect(adapter.Inputs).ToList())
                {
                    _componentEditorsToEdit.RemoveAll(e => e.target == target);
                }

                return;
            }

            Editor componentEditor = _componentEditorsToEdit.FirstOrDefault(e => e.target == (UnityEngine.Object)component);

            if (componentEditor != null)
            {
                _componentEditorsToEdit.Remove(componentEditor);
            }
        }

        protected virtual void OnOpen()
        {
            temp = target.transform.GetInRoot("**temp** " + Guid.NewGuid()).gameObject;
        }

        protected virtual void OnDestroy()
        {
            DestroyImmediate(temp);
        }

        private List<Editor> _componentEditorsToEdit = new List<Editor>();
        protected virtual void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int index = 0;
            foreach (StructCalculatorMember<T> member in target.members.ToList())
            {
                if (member.component == null)
                {
                    RemoveMember(member);
                    continue;
                }

                if (index >= maxInRow)
                {
                    index = 0;
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                }

                index++;

                FieldInfo componentField = member.GetType().GetField(nameof(member.component));

                StructComponent<T> before = member.component;
                StructComponentGUI<T>.StructComponentLayoutLabel(before);
                DropperGUI.DragAndDrop(GUILayoutUtility.GetLastRect(),
                    before, member, target.gameObject, componentField.FieldType, (object result) =>
                    {
                        if (before is IComponent beforeC && !target.members.Any(m => m.component == (UnityEngine.Object)before))
                        {
                            beforeC.RemoveReference(target);
                        }

                        if(result is IComponent resultC) 
                        {
                            resultC.AddReference(target);
                        }

                        componentField.SetValue(member, result);
                        EditorUtilityExtensions.SetDirty(target, "Calculator member changed on " + target.GetType().FullName);
                    }, new GenericMenuItem(new GUIContent("Edit"), false, () =>
                    {
                        EditComponent(member.component);
                    }), new GenericMenuItem(new GUIContent("Remove"), false, () =>
                    {
                        RemoveMember(member);
                    }));

                if (target.members.IndexOf(member) < target.members.Count - 1)
                {
                    float textWidth = GUI.skin.label.CalcSize(new GUIContent(member.operation.Symbol)).x;
                    List<StructCalculatorOperator<T>> operators = StructCalculatorComponent<T>.Operators.Values.ToList();
                    member.operation = operators[EditorGUILayout.Popup(operators.IndexOf(member.operation), operators.Select(t => t.Symbol).
                        ToArray(), GUILayout.Width(textWidth + 20))];
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("=", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUIExtensions.Result(target.Size);

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("", GUILayout.MaxWidth(250));
            EditorGUI.BeginChangeCheck();
            string newMember = ClassTypeReferencePropertyDrawer.DrawTypeSelectionControl(GUILayoutUtility.GetLastRect(),
                new GUIContent(""), "Add member", new ClassExtendsAttribute(typeof(StructComponent<>)));
            if (EditorGUI.EndChangeCheck())
            {
                EditComponent((IComponent)temp.AddComponent(Type.GetType(newMember)));
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            foreach (Editor editor in _componentEditorsToEdit.ToList())
            {
                GUILayout.Space(50);
                editor.OnInspectorGUI();

                if (GUILayout.Button("Done"))
                {
                    RemoveComponentFromEditing((IComponent)editor.target);
                }

                if (GUILayout.Button("Add"))
                {
                    IComponent component = (IComponent)editor.target;

                    RemoveComponentFromEditing(component);

                    AddMember(component, StructCalculatorComponent<T>.Operators.FirstOrDefault().Value);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtilityExtensions.SetDirty(target, "Struct calculator edited");
            }
        }
    }
}
