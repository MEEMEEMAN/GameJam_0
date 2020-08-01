using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Perhaps
{
    [RequireComponent(typeof(RectTransform))]
    public class PUIElement : MonoBehaviour
    {
        [SerializeField] protected RectTransform rect;
        public virtual void OnValidate()
        {
            if(rect == null)
            {
                rect = GetComponent<RectTransform>();
            }
        }

        public bool IsPointOverlapped(Vector2 screenSpace)
        {
            return rect.GetScreenSpaceRect().Contains(screenSpace);
        }

        public static bool IsPointBlocked(Vector3 point)
        {
            return false;
        }

    }

}