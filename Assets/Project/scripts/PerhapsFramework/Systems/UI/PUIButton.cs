using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Perhaps
{
    public class PUIButton : PUIElement, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public event Action<PUIButton, ButtonState> OnButtonChangeState;
        [Header("Settings")]
        [Tooltip("Should the button act as a toggle button?")]
        public bool actAsToggle;


        [Header("Dragging")]
        [Tooltip("Prevents interaction with other buttons while dragging this button.")]
        public bool spawnBlockerOnDrag = true;


        [Header("Info")]
        [SerializeField] ButtonState State;
        public ButtonState state => State;

        public void OnPointerClick(PointerEventData eventData)
        {
            state.isClickEvent = true;
            if (actAsToggle)
                state.isToggled = !state.isToggled;

            InvokeState();

            state.isClickEvent = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            state.isPointerWithin = true;
            state.isPointerWithinEvent = true;
            InvokeState();
            state.isPointerWithinEvent = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            state.isPointerWithin = false;
            state.isPointerWithinEvent = true;
            InvokeState();
            state.isPointerWithinEvent = false;

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            state.isDown = true;
            state.isDownEvent = true;
            InvokeState();
            state.isDownEvent = false;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            state.isDown = false;
            state.isDownEvent = true;
            InvokeState();
            state.isDownEvent = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (spawnBlockerOnDrag)
            {
                blocker.transform.position = eventData.position;
            }

        }

        Image blocker;
        public void OnBeginDrag(PointerEventData eventData)
        {
            state.isDragged = true;
            state.isDragEvent = true;
            InvokeState();
            state.isDragEvent = false;

            if (spawnBlockerOnDrag)
            {
                SpawnBlocker();
            }
        }

        void SpawnBlocker()
        {
            GameObject go = new GameObject();
            go.name = "Blocker";
            go.transform.parent = PUIManager.instance.transform;
            go.transform.SetAsLastSibling();

            blocker = go.AddComponent<Image>();
            blocker.color = Color.clear;
            blocker.raycastTarget = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            state.isDragged = false;
            state.isDragEvent = true;
            InvokeState();
            state.isDragEvent = false;

            if (spawnBlockerOnDrag)
            {
                Destroy(blocker.gameObject);
            }
        }

        void InvokeState()
        {
            if (OnButtonChangeState != null)
            {
                OnButtonChangeState(this, state);
            }
        }
    }


    [System.Serializable]
    public class ButtonState
    {
        public bool isClickEvent = false;
        public bool isToggled = false;

        public bool isDown = false;
        public bool isDownEvent = false;

        public bool isDragged = false;
        public bool isDragEvent = false;

        public bool isPointerWithin = false;
        public bool isPointerWithinEvent = false;
    }
}