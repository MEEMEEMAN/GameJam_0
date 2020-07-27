using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Perhaps
{
    public class UIElementPerhaps : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public event Action<UIElementPerhaps> OnStateChange;

        static HashSet<UIElementPerhaps> blockingElements = new HashSet<UIElementPerhaps>();

        [Header("Element Settings")]
        public bool blockOnDrag = false;
        public bool RegisterBlockingOnStart = false;
        [SerializeField] RectTransform Rect;
        [SerializeField] ElementState State = new ElementState();
        public RectTransform rect => Rect;
        public ElementState state => State;


        public virtual void OnValidate()
        {
            if (Rect == null)
            {
                Rect = GetComponent<RectTransform>();
            }
        }

        public virtual void Start()
        {
            if (RegisterBlockingOnStart)
            {
                if (Rect == null)
                {
                    Rect = GetComponent<RectTransform>();
                }

                if (Rect != null)
                    blockingElements.Add(this);
            }

        }

        private void OnDestroy()
        {
            if (RegisterBlockingOnStart && rect != null)
            {
                blockingElements.Remove(this);
            }
        }

        public static bool IsPointBlocked(Vector2 screenSpacePoint)
        {
            foreach (UIElementPerhaps element in blockingElements)
            {
                Bounds bounds = GetScreenSpaceBounds(element.rect);
                if (bounds.Contains(screenSpacePoint))
                    return true;
            }

            return false;
        }

        private static Vector3[] WorldCorners = new Vector3[4];
        public static Bounds GetScreenSpaceBounds(RectTransform transform)
        {
            transform.GetWorldCorners(WorldCorners);
            Bounds bounds = new Bounds(WorldCorners[0], Vector3.zero);
            for (int i = 1; i < 4; ++i)
            {
                bounds.Encapsulate(WorldCorners[i]);
            }
            return bounds;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            state.cursorEntered = true;
            DispatchEvent(eventData);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            state.cursorEntered = false;
            DispatchEvent(eventData);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (blockOnDrag)
            {
                if (blockingRect.TryGetValue(eventData.pointerId, out UIElementPerhaps blocker))
                {
                    blocker.transform.position = eventData.position;
                }
            }

            DispatchEvent(eventData);
        }

        Dictionary<int, UIElementPerhaps> blockingRect = new Dictionary<int, UIElementPerhaps>();
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            state.dragged = true;
            DispatchEvent(eventData);

            if (blockOnDrag)
            {
                GameObject blockerGo = new GameObject();
                blockerGo.name = "Blocker Element";
                blockerGo.transform.SetParent(transform);
                UIElementPerhaps blocker = blockerGo.AddComponent<UIElementPerhaps>();
                Image i = blocker.gameObject.AddComponent<Image>();
                blocker.RegisterBlockingOnStart = true;
                i.raycastTarget = true;
                i.color = Color.clear;


                blockingRect.Add(eventData.pointerId, blocker);
                blocker.transform.position = eventData.position;
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            state.dragged = false;
            DispatchEvent(eventData);

            if (blockOnDrag)
            {
                if (blockingRect.TryGetValue(eventData.pointerId, out UIElementPerhaps blocker))
                {
                    Destroy(blocker.gameObject);
                    blockingRect.Remove(eventData.pointerId);
                }
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            state.clicked = true;
            DispatchEvent(eventData);
            state.clicked = false;
        }

        void DispatchEvent(PointerEventData data)
        {
            state.latestData = data;

            if (OnStateChange != null)
            {
                OnStateChange(this);
            }
        }
    }

    [System.Serializable]
    public class ElementState
    {
        public PointerEventData latestData;
        public bool clicked = false;
        public bool dragged = false;
        public bool cursorEntered = false;
        public bool held = false;
        public List<int> cursorIds = new List<int>();
    }

}