using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perhaps
{
    public class GridLayoutExtended : GridLayoutGroup
    {
        [HideInInspector]
        [SerializeField] RectTransform rect;

        new void OnValidate()
        {
            #if UNITY_EDITOR
            base.OnValidate(); 
            #endif

            if (rect == null)
                rect = GetComponent<RectTransform>();
        }

        [Header("GridLayoutExtended")]
        public int desiredRowCount;
        public int desiredColumnCount;

        public void Build()
        {     
            float totalWidth = rect.rect.width;
            float totalHeight = rect.rect.height;

            //we simply divide the total rect sizes by our desired row and column count
            float widthPerButton = totalWidth / desiredRowCount;
            float heightPerButton = totalHeight / desiredColumnCount;

            //the calculation above does not account for padding, without padding the rects get all smudged up into one single blob.
            //hence, we must account for padding.
            widthPerButton -= ((float)desiredRowCount - 1) / (float)desiredRowCount * spacing.x;
            heightPerButton -= ((float)desiredColumnCount - 1) / (float)desiredColumnCount * spacing.y;

            //we set the final cell size for the GridLayoutGroup to do it's magic.
            cellSize = new Vector2(widthPerButton, heightPerButton);
        }
    }

}