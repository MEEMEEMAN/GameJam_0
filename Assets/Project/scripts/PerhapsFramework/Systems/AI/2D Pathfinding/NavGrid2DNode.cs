using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    [System.Serializable]
    public class NavGrid2DNode : IHeapItem<NavGrid2DNode>
    {
        public bool walkable;
        public Vector3 worldPosition;
        public int gridX, gridY;
        public NavGrid2DNode parent;

        public int gCost;
        public int hCost;
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public NavGrid2DNode(bool _walkable, Vector3 _position, int _gridX, int _gridY)
        {
            walkable = _walkable;
            worldPosition = _position;
            gridX = _gridX;
            gridY = _gridY;
        }

        int mHeapIndex;
        public int heapIndex
        {
            get
            {
                return mHeapIndex;
            }
            set
            {
                mHeapIndex = value;
            }
        }

        public int CompareTo(NavGrid2DNode other)
        {
            int compare = fCost.CompareTo(other.fCost);

            if(compare == 0)
            {
                compare = hCost.CompareTo(other.hCost);
            }

            return -compare;
        }
    }

}