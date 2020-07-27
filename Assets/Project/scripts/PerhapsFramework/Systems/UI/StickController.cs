using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Perhaps
{
    public class StickController : UIElementPerhaps
    {
        public float maxRadius = 70f;
        public float lerpFactor = 20;
        [SerializeField] RectTransform stickAnchor;
        [SerializeField] RectTransform stickCircle;

        bool mDragged = false;
        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            mDragged = true;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            mDragged = false;
        }


        public void Update()
        {
            if(mDragged)
            {
                Vector3 mousePos = Input.mousePosition;
                Vector3 diff = mousePos - stickAnchor.transform.position;
                float dst = Mathf.Clamp(diff.magnitude, 0, maxRadius);
                diff.Normalize();

                float factor = Mathf.Clamp01(dst / maxRadius);
                PerhapsInput.SetStickStrength(factor);
                PerhapsInput.SetStickVector(diff);

                stickCircle.transform.position = stickAnchor.transform.position + diff * dst;
            }
            else
            {
                PerhapsInput.SetStickVector(Vector2.zero);
                PerhapsInput.SetStickStrength(0f);
                stickCircle.transform.localPosition = Vector3.zero;
            }
        }
    }

}