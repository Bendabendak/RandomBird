using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SadJam;
using TypeReferences;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    [CustomEditor(typeof(DynamicExecutor), true)]
    [CanEditMultipleObjects]
    public class DynamicExecutor_Editor : ComponentEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDynamicExecutor();

            EditorGUILayout.Space();

            DrawDefaultComponentInspector(false);

            serializedObject.ApplyModifiedProperties();
        }

        public void DrawDynamicExecutor()
        {
            DrawLabel();

            DynamicExecutor t = (DynamicExecutor)target;

            if (t.Behaviour.Type != DynamicExecutor.ExecutorBehaviourType.OnlyExecutor)
            {
                SerializedProperty executionOrderProp = serializedObject.FindProperty(TypeExtensions.GetBackingFieldName(nameof(DynamicExecutor.ExecutionOrder)));

                EditorGUILayout.PropertyField(executionOrderProp);

                EditorGUILayout.Space();

                SerializedProperty delayInProp = serializedObject.FindProperty(TypeExtensions.GetBackingFieldName(nameof(DynamicExecutor.DelayIn)));

                EditorGUILayout.PropertyField(delayInProp);

                SerializedProperty seqDelayInProp = serializedObject.FindProperty(TypeExtensions.GetBackingFieldName(nameof(DynamicExecutor.SequentialDelayIn)));

                EditorGUILayout.PropertyField(seqDelayInProp);

                EditorGUILayout.Space();
            }

            SerializedProperty delayOutProp = serializedObject.FindProperty(TypeExtensions.GetBackingFieldName(nameof(DynamicExecutor.DelayOut)));

            EditorGUILayout.PropertyField(delayOutProp);

            SerializedProperty seqDelayOutProp = serializedObject.FindProperty(TypeExtensions.GetBackingFieldName(nameof(DynamicExecutor.SequentialDelayOut)));

            EditorGUILayout.PropertyField(seqDelayOutProp);

            EditorGUILayout.Space();

            if (t.Behaviour.Type != DynamicExecutor.ExecutorBehaviourType.OnlyExecutor)
            {
                DrawExecutable(serializedObject, t);

                EditorGUILayout.Space();
            }

            DrawExecutor(serializedObject, t);
        }

        private static Dictionary<string, Type> _staticExectorCache;
        public static void DrawExecutable(SerializedObject serializedObject, DynamicExecutor e)
        {
            SerializedProperty staticExecutorsProp = serializedObject.FindProperty(TypeExtensions.GetBackingFieldName(nameof(DynamicExecutor.StaticExecutors)));

            e.StaticExecutors.RemoveAll(e => string.IsNullOrWhiteSpace(e));
            e.DynamicExecutors.RemoveAll(e => e == null);

            if (e.StaticExecutors.Count <= 0 && e.DynamicExecutors.Count <= 0)
            {
                GUIStyle style = new();
                style.normal.textColor = Color.red;
                EditorGUILayout.LabelField("No Executor!", style);
            }

            if (e.StaticExecutors.Count > 0 || staticExecutorsProp.prefabOverride)
            {
                staticExecutorsProp.isExpanded = EditorGUILayout.Foldout(staticExecutorsProp.isExpanded, "Static Executors");
                EditorGUI.BeginProperty(GUILayoutUtility.GetLastRect(), null, staticExecutorsProp);

                if (staticExecutorsProp.isExpanded)
                {
                    if (_staticExectorCache == null)
                    {
                        _staticExectorCache = new();

                        foreach (Type t in ClassTypeReference.GetFilteredTypes(new ClassExtendsAttribute(typeof(StaticExecutor)) { AllowAbstract = false }))
                        {
                            CustomStaticExecutor att = t.GetCustomAttribute<CustomStaticExecutor>();

                            if (att != null)
                            {
                                _staticExectorCache[att.Id] = t;
                            }
                        }
                    }

                    foreach (string ex in e.StaticExecutors.ToList())
                    {
                        string name;
                        if (_staticExectorCache.TryGetValue(ex, out Type staticExType))
                        {
                            name = staticExType.FullName;
                        }
                        else
                        {
                            name = "Unknown (" + ex + ")";
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(15);
                        if (GUILayout.Button("Remove " + name))
                        {
                            if (Event.current.button == 0)
                            {
                                if (RemoveExecutableDialog(name))
                                {
                                    e.StaticExecutors.Remove(ex);
                                    EditorUtilityExtensions.SetDirty(serializedObject.targetObject);
                                }
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                EditorGUI.EndProperty();
            }

            SerializedProperty localExecutorsProp = serializedObject.FindProperty(TypeExtensions.GetBackingFieldName(nameof(DynamicExecutor.DynamicExecutors)));

            if (e.DynamicExecutors.Count > 0 || localExecutorsProp.prefabOverride)
            {
                localExecutorsProp.isExpanded = EditorGUILayout.Foldout(localExecutorsProp.isExpanded, "Local Executors");
                EditorGUI.BeginProperty(GUILayoutUtility.GetLastRect(), null, localExecutorsProp);

                if (localExecutorsProp.isExpanded)
                {
                    int index = 0;
                    foreach (DynamicExecutor ex in e.DynamicExecutors.ToList())
                    {
                        if (ex == null)
                        {
                            continue;
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(15);

                        string buttonLabel;

                        if (ex.gameObject == ((UnityEngine.Component)serializedObject.targetObject).gameObject)
                        {
                            buttonLabel = "Remove " + ex.Label;
                        }
                        else
                        {
                            buttonLabel = "Remove " + ex.gameObject.name + " " + ex.Label;
                        }

                        if (GUILayout.Button(buttonLabel))
                        {
                            if (Event.current.button == 1)
                            {
                                EditorGUIUtility.PingObject(ex);
                            }
                            else
                            {
                                if (RemoveExecutableDialog(ex.Label))
                                {
                                    DynamicExecutor l = e.DynamicExecutors[index];
                                    l.RemoveExecutable(e);
                                    e.DynamicExecutors.Remove(l);

                                    EditorUtilityExtensions.SetDirty(serializedObject.targetObject, "Removed executor " + l.Label + " from " + e.Label);
                                }
                            }
                        }
                        GUILayout.EndHorizontal();

                        index++;
                    }
                }

                EditorGUI.EndProperty();
            }

            if (GUILayout.Button("Add Executor"))
            {
                GenericMenu m = new();

                foreach (Type t in ClassTypeReference.GetFilteredTypes(new ClassExtendsAttribute(typeof(Executor)) { AllowAbstract = false }))
                {
                    m.AddItem(new(ClassTypeReferencePropertyDrawer.FormatGroupedTypeName(t, ClassGrouping.ByAddress)), true, () =>
                    {
                        AddStaticExecutor(e, t);
                        EditorUtilityExtensions.SetDirty(serializedObject.targetObject);
                    });
                }

                GenericMenuExtensions.Show(m, "Executor selection", Event.current.mousePosition);
            }

            DropperGUI.Drop(GUILayoutUtility.GetLastRect(), e, ((UnityEngine.Component)serializedObject.targetObject).gameObject, typeof(DynamicExecutor), (object result) =>
            {
                if (result == null) return;

                AddExecutable((DynamicExecutor)result, e);
            });
        }

        private static bool RemoveExecutorDialog(string label) => EditorUtility.DisplayDialog("Remove Executor?", "Are you sure you want to remove " + label + " ?", "Remove", "Cancel");

        public static void AddStaticExecutor(IExecutable e, Type t)
        {
            SadJam.Component c = (SadJam.Component)e;

            if (typeof(DynamicExecutor).IsAssignableFrom(t))
            {
                DynamicExecutor ex;
                GameObject parent = c.gameObject;

                ex = (DynamicExecutor)parent.AddComponent(t);

                if (ex.Behaviour.InGarbage)
                {
                    DestroyImmediate(ex);

                    parent = GarbageCollector.Get(c.transform);

                    ex = (DynamicExecutor)parent.AddComponent(t);
                }

                if (ex.Behaviour.OnlyOnePerObject && parent.TryGetComponent(t, out UnityEngine.Component component))
                {
                    DestroyImmediate(ex);

                    ex = (DynamicExecutor)component;
                }

                AddExecutable(ex, e);
            }
            else
            {
                CustomStaticExecutor idAtt = t.GetCustomAttribute<CustomStaticExecutor>();

                if (idAtt == null)
                {
                    Debug.LogError("Missing " + nameof(CustomStaticExecutor) + " attribute on " + t.FullName, c);
                    return;
                }

                e.StaticExecutors.Add(idAtt.Id);

                EditorUtilityExtensions.SetDirty(c, "Added static executor " + t.FullName + " to " + c.Label);
            }
        }
        public static void DrawExecutor(SerializedObject serializedObject, DynamicExecutor e)
        {
            SerializedProperty executablesProp = serializedObject.FindProperty(TypeExtensions.GetBackingFieldName(nameof(DynamicExecutor.Executables)));

            e.Executables.RemoveAll(e => e == null);

            if (e.Executables.Count > 0 || executablesProp.prefabOverride)
            {
                executablesProp.isExpanded = EditorGUILayout.Foldout(executablesProp.isExpanded, "Executables");
                EditorGUI.BeginProperty(GUILayoutUtility.GetLastRect(), null, executablesProp);

                if (executablesProp.isExpanded)
                {
                    int index = 0;
                    foreach (Behaviour executable in e.Executables.ToList())
                    {
                        if (executable == null)
                        {
                            continue;
                        }

                        if (executable is not IExecutable executableI || executable is not SadJam.Component executableC)
                        {
                            e.AssignedTo.Remove(executable);
                            e.Executables.Remove(executable);
                            continue;
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(15);


                        string buttonLabel;

                        if (executableC.gameObject == ((UnityEngine.Component)serializedObject.targetObject).gameObject)
                        {
                            buttonLabel = "Remove " + executableC.Label;
                        }
                        else
                        {
                            buttonLabel = "Remove " + executableC.gameObject.name + " " + executableC.Label;
                        }

                        if (GUILayout.Button(buttonLabel))
                        {
                            if (Event.current.button == 1)
                            {
                                EditorGUIUtility.PingObject(executableC);
                            }
                            else
                            {
                                if (RemoveExecutableDialog(executableC.Label))
                                {
                                    e.RemoveExecutable(executableI);

                                    if (executable is DynamicExecutor executableD)
                                    {
                                        executableD.DynamicExecutors.Remove(e);
                                        EditorUtilityExtensions.SetDirty(executableD, "Removed executor " + e.Label + " from " + executableD.Label);
                                    }

                                    EditorUtilityExtensions.SetDirty(serializedObject.targetObject, "Removed executable " + executableC.Label + " from " + e.Label);
                                }
                            }
                        }
                        GUILayout.EndHorizontal();

                        index++;
                    }
                }

                EditorGUI.EndProperty();
            }

            if (GUILayout.Button("Add Executable"))
            {
                GenericMenu m = new();

                foreach (Type t in ClassTypeReference.GetFilteredTypes(new ClassExtendsAttribute(typeof(IExecutable)) { AllowAbstract = false }))
                {
                    m.AddItem(new(ClassTypeReferencePropertyDrawer.FormatGroupedTypeName(t, ClassGrouping.ByAddress)), true, () =>
                    {
                        AddNewExecutable(e, t);
                        EditorUtilityExtensions.SetDirty(serializedObject.targetObject);
                    });
                }

                GenericMenuExtensions.Show(m, "Executable selection", Event.current.mousePosition);
            }

            DropperGUI.Drop(GUILayoutUtility.GetLastRect(), e, ((UnityEngine.Component)serializedObject.targetObject).gameObject, typeof(IExecutable), (object result) =>
            {
                if (result == null) return;

                AddExecutable(e, (IExecutable)result);
            });
        }

        private static bool RemoveExecutableDialog(string label) => EditorUtility.DisplayDialog("Remove Executable?", "Are you sure you want to remove " + label + " ?", "Remove", "Cancel");

        public static void AddExecutable(DynamicExecutor e, IExecutable c)
        {
            if (!e.AddExecutable(c)) return;
            c.DynamicExecutors.Add(e);

            UnityEngine.Object oC = (UnityEngine.Object)c;

            EditorUtilityExtensions.SetDirty(e, "Added executable " + oC.name + " to " + e.Label);
            EditorUtilityExtensions.SetDirty(oC, "Added executor " + e.Label + " to " + oC.name);
        }

        public static void AddNewExecutable(DynamicExecutor e, Type t)
        {
            if (typeof(IExecutable).IsAssignableFrom(t) && typeof(SadJam.Component).IsAssignableFrom(t))
            {
                GameObject parent = e.gameObject;

                UnityEngine.Component ex = parent.AddComponent(t);

                if (ex is DynamicExecutor exD)
                {
                    if (exD.Behaviour.InGarbage)
                    {
                        DestroyImmediate(ex);

                        parent = GarbageCollector.Get(e.transform);

                        ex = parent.AddComponent(t);
                        exD = (DynamicExecutor)ex;
                    }

                    if (exD.Behaviour.OnlyOnePerObject && parent.TryGetComponent(t, out UnityEngine.Component component))
                    {
                        DestroyImmediate(ex);

                        ex = component;
                    }
                } 

                AddExecutable(e, (IExecutable)ex);
            }
            else
            {
                Debug.LogError("Tried to add non executable object!", e);
            }
        }
    }
}
