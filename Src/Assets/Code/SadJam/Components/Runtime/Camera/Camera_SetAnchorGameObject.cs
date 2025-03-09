using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(0)]
    public class Camera_SetAnchorGameObject : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        public enum AnchorType
        {
            Left,
            Center,
            Right,
            Bottom,
            Middle,
            Top,
            BottomLeft,
            BottomCenter,
            BottomRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            TopLeft,
            TopCenter,
            TopRight,
        };

        [field: SerializeField]
        public AnchorType Anchor { get; private set; }
        [field: SerializeField]
        public Vector3 AnchorOffset { get; private set; }
        [field: Space, SerializeField]
        public Vector3 BoundsOffset { get; private set; }
        [field: SerializeField]
        public GameObjectBounds_Size BoundsComponent { get; private set; }
        [field: Space, SerializeField]
        public Vector2 ScreenSpaceOffset { get; private set; }
        [field: SerializeField]
        public bool UseMaxOffset { get; private set; } = false;
        [field: SerializeField]
        public Vector3 MaxOffset { get; private set; }
        [field: Space, SerializeField]
        public Camera Camera { get; private set; }

        [field: Space, SerializeField]
        public bool ParentLess { get; private set; } = false;
        [field: SerializeField]
        public bool DestroyWithParent { get; private set; } = false;

        [field: Space, SerializeField]
        public bool Preview { get; private set; }

        [NonSerialized]
        private Transform _parent;
        protected override void Awake()
        {
            if (!Application.isPlaying) return;

            if (DestroyWithParent)
            {
                if (transform.parent == null)
                {
                    _parent = transform;
                }
                else
                {
                    _parent = transform.parent;
                    transform.parent = null;
                }
            }

            if (ParentLess)
            {
                transform.parent = null;
            }

            base.Awake();
        }

        protected override void DynamicExecutor_Update()
        {
            if (Application.isPlaying && DestroyWithParent && _parent == null)
            {
                SpawnPool.Destroy(gameObject);
            }

#if UNITY_EDITOR
            if (Application.isPlaying) return;

            if (Preview)
            {
                OnExecute();
            }
#endif
        }

        private static Dictionary<AnchorType, Action<float, float, float, float, float, float, Camera_SetAnchorGameObject>> _setAnchorMap = new(15)
        {
            {
                AnchorType.Left,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(leftX, 0, 0), t, AnchorType.Left)
            },
            {
                AnchorType.Center,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(cameraX, 0, 0), t, AnchorType.Center)
            },
            {
                AnchorType.Right,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(rightX, 0, 0), t, AnchorType.Right)
            },
            {
                AnchorType.Bottom,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(0, bottomY, bottomY), t, AnchorType.Bottom)
            },
            {
                AnchorType.Middle,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(0, cameraY, cameraY), t, AnchorType.Middle)
            },
            {
                AnchorType.Top,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(0, topY, topY), t, AnchorType.Top)
            },
            {
                AnchorType.BottomLeft,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(leftX, bottomY, bottomY), t, AnchorType.BottomLeft)
            },
            {
                AnchorType.BottomCenter,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(cameraX, bottomY, bottomY), t, AnchorType.BottomCenter)
            },
            {
                AnchorType.BottomRight,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(rightX, bottomY, bottomY), t, AnchorType.BottomRight)
            },
            {
                AnchorType.MiddleLeft,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(leftX, cameraY, cameraY), t, AnchorType.MiddleLeft)
            },
            {
                AnchorType.MiddleCenter,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(cameraX, cameraY, cameraY), t, AnchorType.MiddleCenter)
            },
            {
                AnchorType.MiddleRight,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(rightX, cameraY, cameraY), t, AnchorType.MiddleRight)
            },
            {
                AnchorType.TopLeft,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(leftX, topY, topY), t, AnchorType.TopLeft)
            },
            {
                AnchorType.TopCenter,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(cameraX, topY, topY), t, AnchorType.TopCenter)
            },
            {
                AnchorType.TopRight,
                (leftX, rightX, topY, bottomY, cameraX, cameraY, t) => SetAnchor(new(rightX, topY, topY), t, AnchorType.TopRight)
            }
        };

        private void UpdateAnchor()
        {
            Camera cam;
            if (Camera == null)
            {
                cam = Camera.main;
            }
            else
            {
                cam = Camera;
            }

            float _height = 2f * cam.orthographicSize;
            float _width = _height * cam.aspect;

            float cameraX, cameraY;
            cameraX = cam.transform.position.x;
            cameraY = cam.transform.position.y;

            float leftX = cameraX - _width / 2;
            float rightX = cameraX + _width / 2;
            float topY = cameraY + _height / 2;
            float bottomY = cameraY - _height / 2;

            if (BoundsComponent != null)
            { 
                _boundsCache[gameObject] = BoundsComponent;
            }
            else
            {
                _boundsCache.Remove(gameObject);
            }

            _setAnchorMap[Anchor](leftX, rightX, topY, bottomY, cameraX, cameraY, this);
        }

        private static Dictionary<GameObject, Bounds_Element> _boundsCache = new();
        private static void SetAnchor(Vector3 anchor, Camera_SetAnchorGameObject target, AnchorType type)
        {
            Vector3 sP = Vector3.zero;
            Camera cam;
            if (target.ScreenSpaceOffset != Vector2.zero)
            {
                if (target.Camera == null)
                {
                    cam = Camera.main;
                }
                else
                {
                    cam = target.Camera;
                }

                float worldHeight = cam.orthographicSize * 2;
                float worldWidth = worldHeight * cam.aspect;

                sP = new(worldWidth * target.ScreenSpaceOffset.x, worldHeight * target.ScreenSpaceOffset.y, 0);
            }

            Vector3 offset = target.AnchorOffset + sP;

            if (target.BoundsOffset != Vector3.zero)
            {
                if (!_boundsCache.TryGetValue(target.gameObject, out Bounds_Element bounds) || bounds == null)
                {
                    if (target.gameObject.TryGetBoundsComponent(out bounds))
                    {
                        _boundsCache[target.gameObject] = bounds;
                    }
                    else
                    {
                        Debug.LogError("Bounds component not found!", target.gameObject);
                        return;
                    }
                }

                Bounds b = bounds.Bounds;
                offset += new Vector3(b.size.x * target.BoundsOffset.x, b.size.y * target.BoundsOffset.y, b.size.z * target.BoundsOffset.z);
            }

            if (target.UseMaxOffset)
            {
                offset = new(Mathf.Clamp(offset.x, -target.MaxOffset.x, target.MaxOffset.x), Mathf.Clamp(offset.y, -target.MaxOffset.y, target.MaxOffset.y), Mathf.Clamp(offset.z, -target.MaxOffset.z, target.MaxOffset.z));
            }

            switch (type)
            {
                case AnchorType.Left:
                case AnchorType.Center:
                case AnchorType.Right:
                    anchor.y = target.gameObject.transform.position.y;
                    anchor.x += offset.x;
                    anchor.z += offset.z;
                    break;
                case AnchorType.Bottom:
                case AnchorType.Middle:
                case AnchorType.Top:
                    anchor.x = target.gameObject.transform.position.x;
                    anchor.y += offset.y;
                    anchor.z += offset.z;
                    break;
                default:
                    anchor += offset;
                    break;
            }

            if (target.gameObject.transform.position != anchor)
            {
                target.gameObject.transform.position = anchor;
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            UpdateAnchor();
        }
    }
}
