using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    [System.Serializable]
    public class NavGridPath2D
    {
        public List<NavGrid2DNode> points = new List<NavGrid2DNode>();

        public static void DrawPath(NavGridPath2D path, Color color)
        {
            for (int i = 1; i < path.points.Count; i++)
            {
                NavGrid2DNode prevNode = path.points[i - 1];
                NavGrid2DNode current = path.points[i];

                Debug.DrawLine(prevNode.worldPosition, current.worldPosition, color);
            }
        }
    }

}