using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public class NavGrid2DAgent : MonoBehaviour
    {
        [SerializeField] NavGridPath2D currentPath;
        [SerializeField] float thresholdRadius = 0.1f;

        public void SetPath(NavGridPath2D path)
        {
            currentPath = path;
            Update();
        }

        public Vector2 GetTravelDirection()
        {
            return mTravelDir;
        }

        Vector2 mTravelDir;
        public void Update()
        {
            if (currentPath != null && currentPath.points != null)
            {
                if (currentPath.points.Count > 0)
                {
                    while (currentPath.points.Count != 0)
                    {
                        NavGrid2DNode current = currentPath.points[0];

                        Vector3 diff = current.worldPosition - transform.position;
                        diff.z = 0;

                        float dst = diff.magnitude;
                        if (dst <= thresholdRadius)
                        {
                            currentPath.points.RemoveAt(0);
                        }
                        else
                        {
                            mTravelDir = diff.normalized;
                            break;
                        }
                    }

#if UNITY_EDITOR
                    NavGridPath2D.DrawPath(currentPath, Color.black);
#endif
                }
                else
                {
                    mTravelDir = Vector2.zero;
                }
            }

            Debug.DrawRay(transform.position, mTravelDir, Color.magenta);
        }
    }

}