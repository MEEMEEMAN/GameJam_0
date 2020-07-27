using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    [CreateAssetMenu(menuName = "Perhaps/NavGrid2D Data")]
    public class NavGrid2DData : ScriptableObject
    {
        public Color outlineColor = Color.green;
        public Color fillColor = Color.yellow;

        public Rect gridRect;
        public float cellRadius;
        public NavGrid2DNode[] grid;

        public int gridSizeX, gridSizeY;
        public float cellDiameter => cellRadius * 2;
        public int maxSize => gridSizeX * gridSizeY;

        public Vector3 gridBottomLeft => gridRect.min - new Vector2(gridRect.width / 2, gridRect.height / 2);

        public int Get1DIndex(int x, int y)
        {
            return y * gridSizeX + x;
        }
    }
}