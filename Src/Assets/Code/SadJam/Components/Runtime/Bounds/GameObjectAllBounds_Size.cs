using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    public class GameObjectAllBounds_Size : GameObjectBounds_Size
    {
        public override Vector3 Size => GetBounds().size;

        public override Bounds Bounds => GetBounds();

        [field: SerializeField]
        public Vector3 Offset { get; private set; } = Vector3.zero;

        [NonSerialized]
        private bool _childrenChanged = true;
        public Bounds Recalculate()
        {
            _childrenChanged = true;
            return GetBounds();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            Recalculate();
        }

        protected override void StartOnce()
        {
            base.StartOnce();

            GetBounds();
        }

        [NonSerialized]
        private List<GameObjectBounds_Size> _elements = new();
        [NonSerialized]
        private List<GameObjectBounds_Size> _elementsToRemove = new();
        private Bounds GetBounds()
        {
            if (_childrenChanged)
            {
                _elements.Clear();
                foreach(GameObjectBounds_Size element in gameObject.GetComponentsInChildren<GameObjectBounds_Size>(true))
                {
                    _elements.Add(element);
                }

                _childrenChanged = false;
            }

            if (_elements.Count <= 0) 
            {
                return new(transform.position, Offset);
            }

            bool allNull = true;
            Bounds bounds = new();
            foreach (GameObjectBounds_Size e in _elements)
            {
                if (e == null) continue;

                if (e == this || !e.transform.IsChildOf(transform))
                {
                    _elementsToRemove.Add(e);
                    continue;
                }

                bounds = e.Bounds;
                allNull = false;
                break;
            }

            if (_elementsToRemove.Count > 0)
            {
                foreach (GameObjectBounds_Size e in _elementsToRemove)
                {
                    _elements.Remove(e);
                }

                _elementsToRemove.Clear();
            }

            if (allNull)
            {
                return new(transform.position, Offset);
            }

            foreach (GameObjectBounds_Size e in _elements)
            {
                bounds.Encapsulate(e.Bounds);
            }

            bounds.size += Offset;

            return bounds;
        }

        protected virtual void OnTransformChildrenChanged()
        {
            _childrenChanged = true;
        }
    }
}
