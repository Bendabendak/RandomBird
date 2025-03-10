/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BinaryEgo.UI
{
    public class MenuItemNode2
    {
        public GUIContent content;
        public Action func;
        public Action<object> func2;
        public object userData;
        public bool separator;
        public bool on;

        public string name { get; }
        public MenuItemNode2 parent { get; }
        
        public List<MenuItemNode2> Nodes { get; private set; }

        public MenuItemNode2(string p_name = "", MenuItemNode2 p_parent = null)
        {
            name = p_name;
            parent = p_parent;
            Nodes = new List<MenuItemNode2>();
        }

        public MenuItemNode2 CreateNode(string p_name)
        {
            var node = new MenuItemNode2(p_name, this);
            Nodes.Add(node);
            return node;
        }

        // TODO Optimize
        public MenuItemNode2 GetOrCreateNode(string p_name)
        {
            var node = Nodes.Find(n => n.name == p_name);
            if (node == null)
            {
                node = CreateNode(p_name);
            }

            return node;
        }

        public List<MenuItemNode2> Search(string p_search)
        {
            p_search = p_search.ToLower();
            List<MenuItemNode2> result = new List<MenuItemNode2>();
            
            foreach (var node in Nodes)
            {
                if (node.Nodes.Count == 0 && node.name.ToLower().Contains(p_search))
                {
                    result.Add(node);
                }
                
                result.AddRange(node.Search(p_search));
            }

            return result;
        }

        public string GetPath()
        {
            return parent == null ? "" : parent.GetPath() + "/" + name;
        }

        public void Execute()
        {
            if (func != null)
            {
                func?.Invoke();
            }
            else
            {
                func2?.Invoke(userData);
            }
        }
    }
    
    public class MenuPopup : AbstractPopup
    {
        public static MenuPopup Get(RuntimeGenericMenu p_menu, string p_title)
        {
            var popup = new MenuPopup(p_menu, p_title);
            return popup;
        }
        
        public static MenuPopup Show(RuntimeGenericMenu p_menu, string p_title, Vector2 p_position) {
            var popup = new MenuPopup(p_menu, p_title);
            AbstractPopup.Show(new Rect(p_position.x, p_position.y, 0, 0), popup);
            return popup;
        }

        private GUIStyle _backStyle;
        public GUIStyle BackStyle 
        {
            get
            {
                if (_backStyle == null)
                {
                    _backStyle = new GUIStyle();
                    _backStyle.alignment = TextAnchor.MiddleLeft;
                    _backStyle.normal.background = Texture2D.grayTexture;
                    _backStyle.hover.background = Texture2D.whiteTexture;
                    _backStyle.normal.textColor = Color.black;
                }

                return _backStyle;
            }
        }

        private GUIStyle _plusStyle;
        public GUIStyle PlusStyle
        {
            get {
                if (_plusStyle == null)
                {
                    _plusStyle = new GUIStyle();
                    _plusStyle.fontStyle = FontStyle.Bold;
                    _plusStyle.normal.textColor = Color.white;
                    _plusStyle.fontSize = 16;
                }

                return _plusStyle;
            }
        }
        
        private string _title;
        private Vector2 _scrollPosition;
        private MenuItemNode2 _rootNode;
        private MenuItemNode2 _currentNode;
        private MenuItemNode2 _hoverNode;
        private bool _repaint;
        private string _search;
        private int _contentHeight;
        private bool _useScroll;
        private GUISkin _skin;
        
        public int width = 200;
        public int height = 200;
        public int maxHeight = 300;
        public bool resizeToContent = false;
        public bool showOnStatus = true;
        public bool showSearch = true;
        public bool showTooltip = false;
        public bool showTitle = false;


        public MenuPopup(RuntimeGenericMenu p_menu, string p_title)
        {
            _skin = Resources.Load<GUISkin>("MenuPopup");   
            
            _title = p_title;
            showTitle = !string.IsNullOrWhiteSpace(_title);
            _currentNode = _rootNode = GenerateMenuItemNodeTree(p_menu);
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(width, height);
        }

        public override void OnGUI(Rect p_rect)
        {
            GUI.skin = _skin;
            
            if (Event.current.type == EventType.Layout)
                _useScroll = _contentHeight > maxHeight || (!resizeToContent && _contentHeight > height);

            Vector2 size = GetWindowSize();
            p_rect = new Rect(rect.x, rect.y, size.x, size.y);
            
            _contentHeight = 0;
            GUIStyle style = new GUIStyle();
            style.normal.background = Texture2D.whiteTexture;
            GUI.color = new Color(0.1f, 0.1f, 0.1f, 1);
            GUI.Box(p_rect, string.Empty, style);
            GUI.color = Color.white;
            
            if (showTitle)
            {
                DrawTitle(new Rect(p_rect.x, p_rect.y, p_rect.width, 24));
            }

            if (showSearch)
            {
                DrawSearch(new Rect(p_rect.x, p_rect.y + (showTitle ? 24 : 0), p_rect.width, 20));
            }

            DrawMenuItems(new Rect(p_rect.x, p_rect.y + (showTitle ? 24 : 0) + (showSearch ? 22 : 0), p_rect.width, p_rect.height - (showTooltip ? 60 : 0) - (showTitle ? 24 : 0) - (showSearch ? 22 : 0)));

            if (showTooltip)
            {
                DrawTooltip(new Rect(p_rect.x + 5, p_rect.y + p_rect.height - 58, p_rect.width - 10, 56));
            }

            if (resizeToContent)
            {
                height = Mathf.Min(_contentHeight, maxHeight);
            }
            GUI.FocusControl("Search");
        }

        private void DrawTitle(Rect p_rect)
        {
            _contentHeight += 24;
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 16;
            style.alignment = TextAnchor.LowerCenter;
            GUI.Label(p_rect, _title, style);
        }

        private void DrawSearch(Rect p_rect)
        {
            _contentHeight += 22;
            GUI.SetNextControlName("Search");
            _search = GUI.TextArea(p_rect, _search);
        }

        private void DrawTooltip(Rect p_rect)
        {
            _contentHeight += 60;
            if (_hoverNode == null || _hoverNode.content == null || string.IsNullOrWhiteSpace(_hoverNode.content.tooltip))
                return;

            GUIStyle style = new GUIStyle();
            style.fontSize = 9;
            style.wordWrap = true;
            style.normal.textColor = Color.white;
            GUI.Label(p_rect, _hoverNode.content.tooltip, style);
        }

        private void DrawMenuItems(Rect p_rect)
        {
            GUILayout.BeginArea(p_rect);
            if (_useScroll) 
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUIStyle.none, _skin.verticalScrollbar);
            }

            GUILayout.BeginVertical();

            if (string.IsNullOrWhiteSpace(_search) || _search.Length<2)
            {
                DrawNodeTree(p_rect);
            }
            else
            {
                DrawNodeSearch(p_rect);
            }

            GUILayout.EndVertical();
            if (_useScroll)
            {
                GUILayout.EndScrollView();
            }

            GUILayout.EndArea();
        }
        
        private void DrawNodeSearch(Rect p_rect)
        {
            List<MenuItemNode2> search = _rootNode.Search(_search);
            search.Sort((n1, n2) =>
            {
                string p1 = n1.parent.GetPath();
                string p2 = n2.parent.GetPath();
                if (p1 == p2)
                    return n1.name.CompareTo(n2.name);

                return p1.CompareTo(p2);
            });

            string lastPath = "";
            foreach (var node in search)
            {
                string nodePath = node.parent.GetPath();
                if (nodePath != lastPath)
                {
                    _contentHeight += 20;
                    GUILayout.Label(nodePath, GUILayout.Height(20));
                    lastPath = nodePath;
                }

                _contentHeight += 20;
                GUI.color = _hoverNode == node ? Color.white : Color.gray;
                GUIStyle style = new GUIStyle();
                style.normal.background = Texture2D.grayTexture;
                GUILayout.BeginHorizontal(style);

                if (showOnStatus)
                {
                    style = new GUIStyle();
                    style.normal.background = Texture2D.whiteTexture;
                    GUI.color = node.on ? new Color(0, .6f, .8f) : new Color(.2f, .2f, .2f);
                    GUILayout.Box("", style, GUILayout.Width(14), GUILayout.Height(14));
                }

                GUI.color = _hoverNode == node ? Color.white : Color.white;
                style = new GUIStyle();
                style.normal.textColor = Color.white;
                GUILayout.Label(node.name, style, GUILayout.Height(20));
                
                GUILayout.EndHorizontal();
                
                var nodeRect = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.Repaint || Event.current.isMouse)
                {
                    if (nodeRect.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            if (node.Nodes.Count > 0)
                            {
                                _currentNode = node;
                                _repaint = true;
                            }
                            else
                            {
                                node.Execute();
                                Close();
                            }

                            break;
                        }
                        
                        if (_hoverNode != node)
                        {
                            _hoverNode = node;
                            _repaint = true;
                        }
                    }
                    else if (_hoverNode == node)
                    {
                        _hoverNode = null;
                        _repaint = true;
                    }
                }
            }

            if (search.Count == 0)
            {
                GUILayout.Label("No result found for specified search.");
            }
        }

        private void DrawNodeTree(Rect p_rect)
        {
            if (_currentNode != _rootNode)
            {
                _contentHeight += 20;
                if (GUILayout.Button(_currentNode.GetPath(), BackStyle, GUILayout.Height(20)))
                {
                    _currentNode = _currentNode.parent;
                }
            }
            
            foreach (var node in _currentNode.Nodes)
            {
                if (node.separator)
                {
                    GUILayout.Space(4);
                    _contentHeight += 4;
                    continue;
                }

                _contentHeight += 20;
                GUI.color = _hoverNode == node ? Color.white : Color.gray;
                GUIStyle style = new GUIStyle();
                style.normal.background = Texture2D.grayTexture;
                GUILayout.BeginHorizontal(style);

                if (showOnStatus)
                {
                    style = new GUIStyle();
                    style.normal.background = Texture2D.whiteTexture;
                    GUI.color = node.on ? new Color(0, .6f, .8f, .5f) : new Color(.2f, .2f, .2f, .2f);
                    GUILayout.Box("", style, GUILayout.Width(14), GUILayout.Height(14));
                }

                GUI.color = _hoverNode == node ? Color.white : Color.white;
                style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.fontStyle = node.Nodes.Count > 0 ? FontStyle.Bold : FontStyle.Normal;
                GUILayout.Label(node.name, style, GUILayout.Height(20));
                
                GUILayout.EndHorizontal();
                var nodeRect = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.Repaint || Event.current.isMouse)
                {
                    if (nodeRect.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            if (node.Nodes.Count > 0)
                            {
                                _currentNode = node;
                                _repaint = true;
                            }
                            else
                            {
                                node.Execute();
                                Close();
                            }

                            break;
                        }
                        
                        if (_hoverNode != node)
                        {
                            _hoverNode = node;
                            _repaint = true;
                        }
                    }
                    else if (_hoverNode == node)
                    {
                        _hoverNode = null;
                        _repaint = true;
                    }
                }

                if (node.Nodes.Count > 0)
                {
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    GUI.Label(new Rect(lastRect.x+lastRect.width-16, lastRect.y-2, 20, 20), "+", PlusStyle);
                }
            }
        }

        public static MenuItemNode2 GenerateMenuItemNodeTree(RuntimeGenericMenu p_menu)
        {
            MenuItemNode2 rootNode = new MenuItemNode2();
            if (p_menu == null)
                return rootNode;

            foreach (var menuItem in p_menu.Items)
            {
                GUIContent content = menuItem.content;
                
                string path = content.text;
                string[] splitPath = path.Split('/');
                MenuItemNode2 currentNode = rootNode;
                for (int i = 0; i < splitPath.Length; i++)
                {
                    currentNode = (i < splitPath.Length - 1)
                        ? currentNode.GetOrCreateNode(splitPath[i])
                        : currentNode.CreateNode(splitPath[i]);
                }

                if (menuItem.separator)
                {
                    currentNode.separator = true;
                }
                else
                {
                    currentNode.content = content;
                    currentNode.func = menuItem.callback1;
                    currentNode.func2 = menuItem.callback2;
                    currentNode.userData = menuItem.data;
                    currentNode.on = menuItem.state;
                }
            }

            return rootNode;
        }
        
        public void Show(float p_x, float p_y)
        {
            AbstractPopup.Show(new Rect(p_x, p_y, 0, 0), this);
        }
        
        public void Show(Vector2 p_position)
        {
            AbstractPopup.Show(new Rect(p_position.x, p_position.y, 0, 0), this);
        }
    }
}