using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public struct RelativeVectors
    {
        public Vector3 forwardVector;
        public Vector3 rightVector;
    }

    public static class PerhapsUtils
    {
        public static T GetClosest<T, E>(this E relativeTo, params T[] other) where T : Component where E : Component
        {
            float sqrClosest = float.MaxValue;
            T closest = null;

            for (int i = 0; i < other.Length; i++)
            {
                float sqrDist = (other[i].transform.position - relativeTo.transform.position).sqrMagnitude;
                if(sqrDist < sqrClosest)
                {
                    sqrClosest = sqrDist;
                    closest = other[i];
                }
            }

            return closest;
        }

        public static T GetClosest<T, E>(this E relativeTo, List<T> other) where T : Component where E : Component
        {
            float sqrClosest = float.MaxValue;
            T closest = null;

            for (int i = 0; i < other.Count; i++)
            {
                float sqrDist = (other[i].transform.position - relativeTo.transform.position).sqrMagnitude;
                if (sqrDist < sqrClosest)
                {
                    sqrClosest = sqrDist;
                    closest = other[i];
                }
            }

            return closest;
        }

        public static void IgnoreCollisionsArray(Collider2D[] colliders, Collider2D ignoredCollider, bool ignore = true)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Physics2D.IgnoreCollision(colliders[i], ignoredCollider, ignore);
            }
        }

        public static void IgnoreCollisionsArray(Collider2D[] colliders, bool ignore = true)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider2D colA = colliders[i];

                for (int j = 1; j < colliders.Length; j++)
                {
                    Collider2D colB = colliders[j];

                    Physics2D.IgnoreCollision(colA, colB, ignore);
                }
            }
        }

        public static Vector3 ToVec3(this Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }

        public static RelativeVectors GetRelativeVectors(this RaycastHit hit, Transform relativeTo)
        {
            RelativeVectors vectors = new RelativeVectors();
            vectors.forwardVector = Vector3.Cross(hit.normal, -relativeTo.right);
            vectors.rightVector = Vector3.Cross(hit.normal, relativeTo.forward);

            return vectors;
        }

        public static float GetSurfaceAngle(this RaycastHit hit)
        {
            return Vector3.Angle(hit.normal, Vector3.up);
        }

        public static void SetLayerRecursive(this GameObject go, int layer)
        {
            go.layer = layer;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                go.transform.GetChild(i).gameObject.SetLayerRecursive(layer);
            }
        }

        public static Rect RectTransformToScreenSpace(this RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            return new Rect((Vector2)transform.position - (size * 0.5f), size);
        }

        static Vector3[] WorldCorners = new Vector3[4];
        public static Bounds GetWorldSpaceBounds(this RectTransform transform)
        {

            transform.GetWorldCorners(WorldCorners);
            Bounds bounds = new Bounds(WorldCorners[0], Vector3.zero);
            for (int i = 1; i < 4; ++i)
            {
                bounds.Encapsulate(WorldCorners[i]);
            }
            return bounds;
        }

        /// <summary>
        /// In some cases, the collider we hit is a child of the component we are searching for.
        /// We just simply check the parent aswell if the target does not have the component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T GetComponentFromTargetOrParent<T>(this Transform target)
        {
            return target.gameObject.GetComponentFromTargetOrParent<T>();
        }


        /// <summary>
        /// In some cases, the collider we hit is a child of the component we are searching for.
        /// We just simply check the parent aswell if the target does not have the component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T GetComponentFromTargetOrParent<T>(this GameObject target)
        {
            T component = target.GetComponent<T>();
            if(component == null)
            {
                if(target.transform.parent != null)
                {
                    component = target.transform.parent.GetComponent<T>();
                }
            }

            return component;
        }

        public static T[] GetComponentsInParentAndChildren<T>(this Transform transform)
        {
            return GetComponentsInParentAndChildren<T>(transform.gameObject);
        }

        public static T[] GetComponentsInParentAndChildren<T>(this GameObject go)
        {
            List<T> components = new List<T>();
            T parentComponent = go.GetComponent<T>();

            if(parentComponent != null)
            {
                components.Add(parentComponent);
            }
            components.AddRange(go.GetComponentsInChildren<T>(true));

            return components.ToArray();
        }

    }

}