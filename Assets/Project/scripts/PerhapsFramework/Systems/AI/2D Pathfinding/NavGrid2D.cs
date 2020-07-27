using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Perhaps
{
    public class NavGrid2D : MonoBehaviour
    {
        public NavGrid2DData data;

        [HideInInspector]
        [SerializeField] NavGrid2DData cachedGrid;

        [Header("Baking")]
        [Range(0.01f, 3f)]
        [Tooltip("Beware, A value too small will yeet your performance.")]
        [SerializeField] float cellRadius = 1f;

        private void OnValidate()
        {
            if (cachedGrid != data)
            {
                cachedGrid = data;
                cellRadius = data.cellRadius;
            }
        }

        public void BakeGrid()
        {
            data.cellRadius = cellRadius;
            float nodeDiameter = cellRadius * 2;

            data.gridSizeX = Mathf.RoundToInt(data.gridRect.width / nodeDiameter);
            data.gridSizeY = Mathf.RoundToInt(data.gridRect.height / nodeDiameter);
            data.grid = new NavGrid2DNode[data.gridSizeX * data.gridSizeY];

            Vector3 worldBottomLeft = data.gridBottomLeft;
            for (int x = 0; x < data.gridSizeX; x++)
            {
                for (int y = 0; y < data.gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + cellRadius) + Vector3.up * (y * nodeDiameter + cellRadius);
                    bool walkable = Physics2D.OverlapCircle(worldPoint, cellRadius) == null ? true : false;

                    int index = data.Get1DIndex(x, y);
                    data.grid[index] = new NavGrid2DNode(walkable, worldPoint, x, y);
                }
            }
        }

        public List<NavGrid2DNode> GetNeighbors(NavGrid2DNode node)
        {
            List<NavGrid2DNode> neighbors = new List<NavGrid2DNode>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < data.gridSizeX && checkY >= 0 && checkY < data.gridSizeY)
                    {
                        neighbors.Add(data.grid[data.Get1DIndex(checkX, checkY)]);
                    }
                }
            }

            return neighbors;
        }

        public NavGrid2DNode NodeFromWorldPoint(Vector2 worldPosition)
        {
            Vector3 gridBottomLeft = data.gridBottomLeft;
            float diffX = worldPosition.x - gridBottomLeft.x;
            float diffY = worldPosition.y - gridBottomLeft.y;

            float percentX = diffX / data.gridRect.width;
            float percentY = diffY / data.gridRect.height;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((data.gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((data.gridSizeY - 1) * percentY);

            return data.grid[data.Get1DIndex(x, y)];

        }


        private void OnDrawGizmosSelected()
        {
            if (data != null && data.grid != null)
            {
                foreach (NavGrid2DNode n in data.grid)
                {
                    Gizmos.color = n.walkable ? Color.white : Color.red;

                    Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (data.cellRadius * 2 * -0.1f));
                }
            }
        }

        public NavGridPath2D GetPath(Vector2 startPos, Vector2 endPos)
        {
            NavGrid2DNode startNode = NodeFromWorldPoint(startPos);
            NavGrid2DNode endNode = NodeFromWorldPoint(endPos);

            if (!endNode.walkable)
            {
                NavGrid2DNode[] walkableNeighbors = GetNeighbors(endNode).Where(x => x.walkable).ToArray();

                if (walkableNeighbors.Length == 0)
                    return null;

                endNode = walkableNeighbors[0];
            }

            Heap<NavGrid2DNode> openSet = new Heap<NavGrid2DNode>(data.maxSize);
            HashSet<NavGrid2DNode> closedSet = new HashSet<NavGrid2DNode>();

            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                NavGrid2DNode currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == endNode)
                {
                    return GetResult(startNode, endNode);
                }

                foreach (NavGrid2DNode n in GetNeighbors(currentNode))
                {
                    if (!n.walkable || closedSet.Contains(n))
                        continue;

                    int movementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, n);

                    if (movementCostToNeighbor < n.gCost || !openSet.Contains(n))
                    {
                        n.gCost = movementCostToNeighbor;
                        n.hCost = GetDistance(n, endNode);
                        n.parent = currentNode;

                        if (!openSet.Contains(n))
                        {
                            openSet.Add(n);
                        }
                    }
                }
            }

            return null;
        }

        NavGridPath2D GetResult(NavGrid2DNode startNode, NavGrid2DNode endNode)
        {
            NavGridPath2D p = new NavGridPath2D();
            p.points = new List<NavGrid2DNode>();

            NavGrid2DNode current = endNode;
            while (current != startNode)
            {
                p.points.Add(current);
                current = current.parent;
            }

            p.points.Reverse();
            return p;
        }

        int GetDistance(NavGrid2DNode a, NavGrid2DNode b)
        {
            int dstX = Mathf.Abs(a.gridX - b.gridX);
            int dstY = Mathf.Abs(a.gridY - b.gridY);

            return dstX + dstY;

/*
if (dstX > dstY)
{
    return 14 * dstY + 10 * (dstX - dstY);
}
else
{
    return 14 * dstX + 10 * (dstY - dstX);
}

if (dstX > dstY)
{
    return 14 * dstY + 10 * (dstX - dstY);
}
else
{
    return 14 * dstX + 10 * (dstY - dstX);
}
*/
        }
    }
}