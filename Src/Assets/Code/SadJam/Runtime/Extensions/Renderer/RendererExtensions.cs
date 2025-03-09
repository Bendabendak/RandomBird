using System;
using UnityEngine;

namespace SadJam
{
    public static class RendererExtensions
    {
        public static Bounds GetRendererBounds(this GameObject gameObject, bool exceptColliders = false)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            Collider[] colliders = null;
            Collider2D[] colliders2D = null;

            if (!exceptColliders)
            {
                colliders = gameObject.GetComponentsInChildren<Collider>();
                colliders2D = gameObject.GetComponentsInChildren<Collider2D>();
            }

            Bounds bounds;

            if (renderers.Length > 0)
            {
                bounds = renderers[0].bounds;
            }
            else if (exceptColliders) return new(gameObject.transform.position, Vector3.zero);
            else if (colliders.Length > 0)
            {
                bounds = colliders[0].bounds;
            }
            else if (colliders2D.Length > 0)
            {
                bounds = colliders2D[0].bounds;
            }
            else
            {
                return new(gameObject.transform.position, Vector3.zero);
            }

            foreach (Renderer r in renderers)
            {
                bounds.Encapsulate(r.bounds);
            }

            if (!exceptColliders)
            {
                foreach (Collider r in colliders)
                {
                    bounds.Encapsulate(r.bounds);
                }

                foreach (Collider2D r in colliders2D)
                {
                    bounds.Encapsulate(r.bounds);
                }
            }

            return bounds;
        }

        public static Bounds GetRendererLocalBounds(this GameObject gameObject, bool exceptColliders = false)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            Collider[] colliders = null;
            Collider2D[] colliders2D = null;

            if (!exceptColliders)
            {
                colliders = gameObject.GetComponentsInChildren<Collider>();
                colliders2D = gameObject.GetComponentsInChildren<Collider2D>();
            }

            Bounds bounds;

            if (renderers.Length > 0)
            {
                bounds = renderers[0].localBounds;
            }
            else if(exceptColliders) return new(gameObject.transform.position, Vector3.zero);
            else if(colliders.Length > 0)
            {
                Collider c = colliders[0];
                Bounds b = c.bounds;
                b.size = GetBoundingBox(c);

                bounds = b;
            }
            else if(colliders2D.Length > 0)
            {
                Collider2D c = colliders2D[0];
                Bounds b = colliders2D[0].bounds;
                b.size = GetBoundingBox(c);

                bounds = b;
            }
            else
            {
                return new (gameObject.transform.position, Vector3.zero);
            }

            foreach (Renderer r in renderers)
            {
                Bounds b = r.localBounds;

                bounds.Encapsulate(b);
            }

            if (!exceptColliders)
            {
                foreach (Collider c in colliders)
                {
                    Bounds b = c.bounds;
                    b.size = GetBoundingBox(c);

                    bounds.Encapsulate(b);
                }

                foreach (Collider2D c in colliders2D)
                {
                    Bounds b = c.bounds;
                    b.size = GetBoundingBox(c);

                    bounds.Encapsulate(b);
                }
            }

            return bounds;
        }

        [NonSerialized]
        private static Vector3[] _directionArray = new Vector3[] { Vector3.right, Vector3.up, Vector3.forward };
        private static Vector3 GetBoundingBox(Collider collider)
        {
            if (collider is BoxCollider boxColl)
            {
                return boxColl.size;
            }
            else if (collider is SphereCollider sphereColl)
            {
                return new Vector3(sphereColl.radius * 2, sphereColl.radius * 2, sphereColl.radius * 2);
            }
            else if (collider is CapsuleCollider capsuleColl)
            {
                Vector3 result = new();
                for (int i = 0; i < 3; i++)
                {
                    if (i == capsuleColl.direction)
                    {
                        result += _directionArray[i] * capsuleColl.height;
                    }
                    else
                    {
                        result += _directionArray[i] * capsuleColl.radius * 2;
                    }
                }
                return result;
            }
            else if (collider is MeshCollider meshColl)
            {
                return meshColl.sharedMesh.bounds.size;
            }

            return collider.bounds.size;
        }

        private static Vector2 GetBoundingBox(Collider2D collider)
        {
            if (collider is BoxCollider2D boxColl)
            {
                return boxColl.size;
            }
            else if (collider is CircleCollider2D sphereColl)
            {
                return new Vector2(sphereColl.radius * 2, sphereColl.radius * 2);
            }
            else if (collider is CapsuleCollider2D capsuleColl)
            {
                return capsuleColl.size;
            }

            return collider.bounds.size;
        }
    }
}
