using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using JacksonDunstanIterator;

namespace Game 
{
    public static class LineupChildrenHorizontally
    {
        public enum LineUpType
        {
            ToRight,
            ToLeft,
        }

        private struct ChildData
        {
            public Transform Transform;
            public Bounds_Element Bounds;
            public LineupElement LineupElement;
        }

        private static Dictionary<GameObject, ChildData> _lineupElements = new();
        private static List<ChildData> _childrenSortedByPosition = new();

        private static Dictionary<LineUpType, Func<ChildData, ChildData, bool>> _sortMap = new(2)
        {
            {
                LineUpType.ToRight,
                (ChildData t1, ChildData t2) =>
                {
                    if (t1.LineupElement.Linedup == t2.LineupElement.Linedup) return t1.Transform.position.x < t2.Transform.position.x;

                    if (!t1.LineupElement.Linedup) return false;

                    return true;
                }
            },
            {
                LineUpType.ToLeft,
                (ChildData t1, ChildData t2) =>
                {
                    if (t1.LineupElement.Linedup == t2.LineupElement.Linedup) return t1.Transform.position.x > t2.Transform.position.x;

                    if (!t1.LineupElement.Linedup) return false;

                    return true;
                }
            }
        };

        private static Dictionary<LineUpType, Action<List<ChildData>, float>> _lineUpByElementPositionMap = new(2)
        {
            {
                LineUpType.ToRight,
                (sortedList, space) =>
                {
                    ChildData d0 = sortedList[0];
                    float shift = d0.Transform.position.x;
                    float lastBoundsExtents = 0;
                    if (d0.Bounds != null)
                    {
                        lastBoundsExtents = d0.Bounds.Bounds.extents.x;
                    }

                    d0.LineupElement.Linedup = true;

                    for (int i = 1; i < sortedList.Count; i++)
                    {
                        ChildData d = sortedList[i];

                        shift += space + lastBoundsExtents;

                        if (d.Bounds != null)
                        {
                            lastBoundsExtents = d.Bounds.Bounds.extents.x;
                            shift += lastBoundsExtents;
                        }
                        else
                        {
                            lastBoundsExtents = 0;
                        }

                        if (d.Transform.position.x != shift)
                        {
                            d.Transform.position = new(shift, d.Transform.position.y);
                        }

                        d.LineupElement.Linedup = true;
                    }
                }
            },
            {
                LineUpType.ToLeft,
                (sortedList, space) =>
                {
                    ChildData d0 = sortedList[0];
                    float shift = d0.Transform.position.x;
                    float lastBoundsExtents = 0;
                    if (d0.Bounds != null)
                    {
                        lastBoundsExtents = d0.Bounds.Bounds.extents.x;
                    }

                    d0.LineupElement.Linedup = true;

                    for (int i = 1; i < sortedList.Count; i++)
                    {
                        ChildData d = sortedList[i];

                        shift -= space + lastBoundsExtents;

                        if (d.Bounds != null)
                        {
                            lastBoundsExtents = d.Bounds.Bounds.extents.x;
                            shift -= lastBoundsExtents;
                        }
                        else
                        {
                            lastBoundsExtents = 0;
                        }

                        if (d.Transform.position.x != shift)
                        {
                            d.Transform.position = new(shift, d.Transform.position.y);
                        }

                        d.LineupElement.Linedup = true;
                    }
                }
            }
        };

        public static void LineupByElementPosition(IGameConfig_HorizontalLineupable config, GameObject lineupParent, LineUpType lineupType, bool ignoreDisablingObjects)
        {
            if (lineupParent.transform.childCount <= 0) return;

            _childrenSortedByPosition.Clear();
            CacheLineupElements(config, lineupParent, ignoreDisablingObjects);

            foreach (ChildData childD in _lineupElements.Values)
            {
                _childrenSortedByPosition.Add(childD);
            }

            if (_childrenSortedByPosition.Count <= 1) return;

            _childrenSortedByPosition.Begin().Sort(_childrenSortedByPosition.End(), _sortMap[lineupType]);

            _lineUpByElementPositionMap[lineupType](_childrenSortedByPosition, config.SpaceBetween);
        }

        public static void LineupByPosition(IGameConfig_HorizontalLineupable config, GameObject lineupParent, float position, LineUpType lineupType, bool ignoreDisablingObjects)
        {
            if (lineupParent.transform.childCount <= 0) return;

            _childrenSortedByPosition.Clear();
            CacheLineupElements(config, lineupParent, ignoreDisablingObjects);

            foreach (KeyValuePair<GameObject, ChildData> childD in _lineupElements)
            {
                childD.Value.LineupElement.Linedup = false;
                _childrenSortedByPosition.Add(childD.Value);
            }

            if (_childrenSortedByPosition.Count <= 1) return;

            _childrenSortedByPosition.Begin().Sort(_childrenSortedByPosition.End(), _sortMap[lineupType]);

            ChildData firstD = _childrenSortedByPosition[0];
            firstD.Transform.position = new(position, firstD.Transform.position.y, firstD.Transform.position.z);

            _lineUpByElementPositionMap[lineupType](_childrenSortedByPosition, config.SpaceBetween);
        }

        private static List<GameObject> _lineupElementsToRemove = new();
        public static void CacheLineupElements(IGameConfig_HorizontalLineupable config, GameObject lineupParent, bool ignoreDisablingObjects)
        {
            foreach (GameObject g in _lineupElements.Keys)
            {
                if (g == null || g.transform.parent != lineupParent.transform)
                {
                    _lineupElementsToRemove.Add(g);
                }
            }

            foreach (GameObject g in _lineupElementsToRemove)
            {
                _lineupElements.Remove(g);
            }

            _lineupElementsToRemove.Clear();

            for (int i = 0; i < lineupParent.transform.childCount; i++)
            {
                Transform t = lineupParent.transform.GetChild(i);

                if (ignoreDisablingObjects && SpawnPool.GetEntityInfo(t.gameObject, out SpawnPool.EntityInfo info))
                {
                    if (info.IsNotEnabled) continue;
                }

                if (t.name.StartsWith("**") && t.name.EndsWith("**")) continue;

                if (!_lineupElements.ContainsKey(t.gameObject))
                {
                    ChildData data = new();

                    data.Transform = t;

                    if (!t.gameObject.TryGetComponent(out LineupElement lineupElement))
                    {
                        lineupElement = t.gameObject.AddComponent<LineupElement>();
                    }

                    if (config.WithBounds)
                    {
                        if (t.gameObject.TryGetBoundsComponent(out Bounds_Element bounds))
                        {
                            data.Bounds = bounds;
                        }
                    }

                    data.LineupElement = lineupElement;

                    _lineupElements[t.gameObject] = data;
                }
            }
        }
    }
}
