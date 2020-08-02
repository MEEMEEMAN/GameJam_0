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
        /// <summary>
        /// Like a Debug.DrawLine, but with boxes.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="halfExtent"></param>
        /// <param name="color"></param>
        /// <param name="crossLine"></param>
        public static void DrawBoxLine(Vector3 start, Vector3 end, Vector2 halfExtent, Color color, bool crossLine = false)
        {
            Bounds b1 = new Bounds(start, halfExtent);
            Bounds b2 = new Bounds(end, halfExtent);

            Vector3 b1TopLeft = b1.TopLeft();
            Vector3 b1BottomRight = b1.BottomRight();

            Vector3 b2TopLeft = b2.TopLeft();
            Vector3 b2BottomRight = b2.BottomRight();

            Debug.DrawLine(b1.min, b2.min, color);
            Debug.DrawLine(b1.max, b2.max, color);
            Debug.DrawLine(b1TopLeft, b2TopLeft, color);
            Debug.DrawLine(b1BottomRight, b2BottomRight, color);

            DrawBounds(b1, color, crossLine);
            DrawBounds(b2, color, crossLine);

            if (crossLine)
            {
                Debug.DrawLine(b1BottomRight, b2.max, color);
                Debug.DrawLine(b1.max, b2TopLeft, color);
                Debug.DrawLine(b1TopLeft, b2.min, color);
                Debug.DrawLine(b1.min, b2BottomRight, color);
            }
        }

        /// <summary>
        /// Like a Debug.DrawRay, but with boxes.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="halfExtent"></param>
        /// <param name="color"></param>
        /// <param name="crossLine"></param>
        public static void DrawBoxRay(Vector3 origin, Vector3 direction, Vector3 halfExtent, Color color, bool crossLine = false)
        {
            DrawBoxLine(origin, origin + direction, halfExtent, color, crossLine);
        }

        public static void DrawBounds(Bounds b, Color color, bool crossLine = false)
        {
            Vector3 topLeft = b.max;
            topLeft.x = b.min.x;

            Vector3 bottomRight = b.min;
            bottomRight.x = b.max.x;

            Debug.DrawLine(b.min, bottomRight, color);
            Debug.DrawLine(b.min, topLeft, color);
            Debug.DrawLine(b.max, bottomRight, color);
            Debug.DrawLine(b.max, topLeft, color);

            if (crossLine)
            {
                Debug.DrawLine(b.min, b.max, color);
            }
        }

        /*
        /// <summary>
        /// Usefull for visualizing Physics2D.BoxCast
        /// </summary>
        public static void DrawBoxLineAngled(Vector3 start, Vector3 end, float angle, Vector2 halfExtent, Color color, bool crossLine = false)
        {
            

            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

            Vector3 diff = end - start;
            Vector3 center = start + (diff / 2);
            Debug.DrawRay(center, Vector3.up);

            Matrix4x4 mat = Matrix4x4.TRS(center, rotation, Vector3.one);

            Bounds b1 = new Bounds(start, halfExtent);
            Bounds b2 = new Bounds(end, halfExtent);

            Vector3 b1TopLeft = b1.TopLeft();
            Vector3 b1BottomRight = b1.BottomRight();

            Vector3 b2TopLeft = b2.TopLeft();
            Vector3 b2BottomRight = b2.BottomRight();

            Debug.DrawLine(mat * b1.min, mat * b2.min, color);
            Debug.DrawLine(mat * b1.max, mat * b2.max, color);
            Debug.DrawLine(mat * b1TopLeft, mat * b2TopLeft, color);
            Debug.DrawLine(mat * b1BottomRight, mat * b2BottomRight, color);

            /*
            DrawBounds(b1, color, crossLine);
            DrawBounds(b2, color, crossLine);

            if (crossLine)
            {
                Debug.DrawLine(b1BottomRight, b2.max, color);
                Debug.DrawLine(b1.max, b2TopLeft, color);
                Debug.DrawLine(b1TopLeft, b2.min, color);
                Debug.DrawLine(b1.min, b2BottomRight, color);
            }
            
        }
        */
        #region
        /*
using Perhaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DrawTest : MonoBehaviour
{
    public Transform target;
    public Vector2 halfExtent;
    public Color color;
    public bool crossLine = false;
    public float angle = 0f;
    public bool drawAngled = false;

    void Update()
    {
        if(drawAngled)
        {
            PerhapsUtils.DrawBoxLineAngled(transform.position ,target.position, angle, halfExtent, color, crossLine);
        }
        else
        {
            PerhapsUtils.DrawBoxLine(transform.position, target.position, halfExtent, color, crossLine);
        }
    }
}

         */
        #endregion




        public static T GetClosest<T, E>(this E relativeTo, params T[] other) where T : Component where E : Component
        {
            float sqrClosest = float.MaxValue;
            T closest = null;

            for (int i = 0; i < other.Length; i++)
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

        static void NegateZ(Transform t)
        {
            Vector3 pos = t.localPosition;
            pos.z *= -1f;
            t.localPosition = pos;

            for (int i = 0; i < t.childCount; i++)
            {
                NegateZ(t.GetChild(i));
            }
        }

        /// <summary>
        /// Multiplies a transform's, and all of it's children local z axis by -1.
        /// </summary>
        /// <param name="t"></param>
        public static void NegateZRecursive(this Transform t)
        {
            NegateZ(t);
        }

        /// <summary>
        /// Multiplies a gameobject's, and all of it's children local z axis by -1.
        /// </summary>
        /// <param name="t"></param>
        public static void NegateZRecursive(this GameObject go)
        {
            NegateZRecursive(go.transform);
        }

        public static Vector3 Clamp(Vector3 source, Vector3 min, Vector3 max)
        {
            Vector3 clamped = new Vector3();
            clamped.x = Mathf.Clamp(source.x, min.x, max.x);
            clamped.y = Mathf.Clamp(source.y, min.y, max.y);
            clamped.z = Mathf.Clamp(source.z, min.z, max.z);

            return clamped;
        }

        public static Vector3 TopLeft(this Bounds b)
        {
            Vector3 topLeft = b.max;
            topLeft.x = b.min.x;

            return topLeft;
        }

        public static Vector3 BottomRight(this Bounds b)
        {
            Vector3 bottomRight = b.max;
            bottomRight.y = b.min.y;

            return bottomRight;
        }

        public static Vector3 TopRight(this Bounds b)
        {
            return b.max;
        }

        public static Vector3 BottomLeft(this Bounds b)
        {
            return b.min;
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