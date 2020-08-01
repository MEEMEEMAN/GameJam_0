using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Perhaps
{
    public struct RelativeVectors
    {
        public Vector3 forwardVector;
        public Vector3 rightVector;
    }

    /// <summary>
    /// General Purpose Runtime Utillity.
    /// </summary>
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

        public static Vector3 Clamp(Vector3 source, Vector3 min, Vector3 max)
        {
            Vector3 clamped = new Vector3();
            clamped.x = Mathf.Clamp(source.x, min.x, max.x);
            clamped.y = Mathf.Clamp(source.y, min.y, max.y);
            clamped.z = Mathf.Clamp(source.z, min.z, max.z);

            return clamped;
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


        /// <summary>
        /// Returns the forward/right vectors of a transform relative to the surface hit.
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="relativeTo"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Set the layer of a gameobject and all of it's children.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        public static void SetLayerRecursive(this GameObject go, int layer)
        {
            go.layer = layer;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                go.transform.GetChild(i).gameObject.SetLayerRecursive(layer);
            }
        }

        static Vector3[] WorldCorners = new Vector3[4];
        /// <summary>
        /// Returns the area that the rect transform occupies.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns>A 3D Bounding Box</returns>
        public static Bounds GetScreenSpaceBounds(this RectTransform transform)
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
        /// Returns the area that the rect transform occupies.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns>A 2D Bounding Box</returns>
        public static Rect GetScreenSpaceRect(this RectTransform transform)
        {
            /*
            * GetWorldCorners returns 4 UI Space points that go in a clockwise fashion.
            * Bottom Left, Top Left, Top Right, Bottom Right
            */
            transform.GetWorldCorners(WorldCorners);

            Rect r = new Rect()
            {
                //Bottom Left
                min = WorldCorners[0],

                //Top Right
                max = WorldCorners[2]
            };

            return r;
        }

        /// <summary>
        /// Searches the parents for a component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="upCount">Amount of parents to search.</param>
        /// <returns></returns>
        public static T GetComponentFromParent<T>(this Transform t, int upCount, bool searchInChild = true) where T : Component
        {
            if (searchInChild)
            {
                T component = t.GetComponent<T>();
                if (component != null)
                    return component;
            }

            Transform parent = t.parent;
            for (int i = 0; i < upCount; i++)
            {
                if (parent == null)
                    break;

                T component = parent.GetComponent<T>();
                if (component != null)
                    return component;

                parent = parent.parent;
            }

            return null;
        }


    }

    /// <summary>
    /// Helps with flag enums.
    /// </summary>
    public static class EnumHelperClass
    {
        public static bool IsSet<T>(this T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (flagsValue & flagValue) != 0;
        }

        public static void Set<T>(ref this T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            flags = (T)(object)(flagsValue | flagValue);
        }

        public static void Unset<T>(ref this T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            flags = (T)(object)(flagsValue & (~flagValue));
        }
    }

}