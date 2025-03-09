using System;
using UnityEngine;
using UnityEditor;

namespace SadJamEditor
{
    public static class DropperGUI
    {
        public static void Drop(Rect pos, object holder, GameObject context, Type resultType, Action<object> onDrop, params GenericMenuItem[] contextMenuItems)
        {
            Drop(pos, null, holder, context, resultType, onDrop, contextMenuItems);
        }

        public static void Drop(Rect pos, object holder, GameObject context, Type resultType, params GenericMenuItem[] contextMenuItems)
        {
            Drop(pos, null, holder, context, resultType, null, contextMenuItems);
        }

        public static void Drop(Rect pos, object before, object holder, GameObject context, Type resultType, params GenericMenuItem[] contextMenuItems)
        {
            Drop(pos, before, holder, context, resultType, null, contextMenuItems);
        }

        public static void Drop(Rect pos, object before, object holder, GameObject context, Type resultType, Action<object> onDrop, params GenericMenuItem[] contextMenuItems)
        {
            if (pos.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.ContextClick:
                        ShowContextItemsMenu(contextMenuItems);
                        break;
                    case EventType.DragUpdated:
                        UpdateDrag();
                        break;
                    case EventType.DragPerform:
                        PerformDrag(before, holder, context, resultType, onDrop);
                        break;
                }
            }
        }
        public static void Drag(Rect pos, object value, params GenericMenuItem[] contextMenuItems)
        {
            if (pos.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.ContextClick:
                        ShowContextItemsMenu(contextMenuItems);
                        break;
                    case EventType.MouseDrag:
                        Drag(value);
                        break;
                }
            }
        }

        public static void DragAndDrop(Rect pos, object value, object holder, GameObject context, Type resultType, params GenericMenuItem[] contextMenuItems)
        {
            DragAndDrop(pos, value, holder, context, resultType, null, contextMenuItems);
        }

        public static void DragAndDrop(Rect pos, object value, object holder, GameObject context, Type resultType, Action<object> onDrop, params GenericMenuItem[] contextMenuItems)
        {
            if (pos.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.ContextClick:
                        ShowContextItemsMenu(contextMenuItems);
                        break;
                    case EventType.MouseDrag:
                        Drag(value);
                        break;
                    case EventType.DragUpdated:
                        UpdateDrag();
                        break;
                    case EventType.DragPerform:
                        PerformDrag(value, holder, context, resultType, onDrop);
                        break;
                }
            }
        }

        private static void PerformDrag(object value, object holder, GameObject context, Type resultType, Action<object> onDrop = null)
        {
            UnityEditor.DragAndDrop.AcceptDrag();

            foreach (UnityEngine.Object obj in UnityEditor.DragAndDrop.objectReferences)
            {
                Dropper.NewDrop(obj, value, holder, context, resultType, onDrop);
            }
        }

        private static void UpdateDrag()
        {
            UnityEditor.DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }

        private static void Drag(object value)
        {
            UnityEditor.DragAndDrop.PrepareStartDrag();

            UnityEditor.DragAndDrop.objectReferences = new UnityEngine.Object[1] { (UnityEngine.Object)value };

            UnityEditor.DragAndDrop.StartDrag("Dragging component");
        }

        public static void ShowContextItemsMenu(params GenericMenuItem[] contextMenuItems)
        {
            if (contextMenuItems.Length <= 0)
            {
                return;
            }

            GenericMenu menu = new GenericMenu();

            foreach (GenericMenuItem item in contextMenuItems)
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
        }
    }
}
