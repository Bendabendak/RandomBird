using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    public class Transform_SetParent : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field:SerializeField]
        public GameObject Parent { get; private set; }
        [field: SerializeField]
        public Selection ParentChild { get; private set; }
        [field: Space, SerializeField]
        public bool WorldPositionStays { get; private set; } = false;
        [field: Space, SerializeField]
        public bool IfParentPrefabCreateNew { get; private set; } = false;

        private static Dictionary<GameObject, GameObject> _parentCache = new();
        [NonSerialized]
        private GameObject _prefab;
        protected override void DynamicExecutor_OnExecute()
        {
            if (transform.parent == Parent) return;

            if (Parent == null && _prefab == null)
            {
                transform.SetParent(null, WorldPositionStays);
                return;
            }

            if (!Parent.gameObject.scene.IsValid() || _prefab != null)
            {
                if (_prefab == null)
                {
                    _prefab = Parent;
                }

                if (IfParentPrefabCreateNew)
                {
                    Parent = Instantiate(_prefab);
                }
                else
                {
                    GameObject spawn;
                    if (!_parentCache.TryGetValue(_prefab, out spawn) || spawn == null)
                    {
                        spawn = Instantiate(_prefab);
                        _parentCache[_prefab] = spawn;
                    }

                    Parent = spawn;
                }
            }

            if (!string.IsNullOrWhiteSpace(ParentChild.Selected))
            {
                Transform t = Parent.transform.FindRecursive(ParentChild.Selected);

                if (t == null)
                {
                    Debug.LogWarning("Child named " + ParentChild.Selected + " not found in parent " + Parent.name + " !", gameObject);
                }

                transform.SetParent(t, WorldPositionStays);
                return;
            }

            transform.SetParent(Parent.transform, WorldPositionStays);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (Parent)
            {
                List<string> children = new();

                GetAllChildren(Parent.transform);

                void GetAllChildren(Transform p)
                {
                    foreach (Transform child in p)
                    {
                        children.Add(child.gameObject.name);
                        GetAllChildren(child);
                    }
                }

                ParentChild.ChangeCollection(children);
            }
        }
    }
}
