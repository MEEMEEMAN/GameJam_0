using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Perhaps
{
    public class ButtonState
    {
        public bool PointerWithinBounds;
        public bool IsClickEvent;
    }

    [System.Serializable]
    public class ButtonGraphic
    {
        public Image image;
        public Color defaultColor;
        public Color pressedColor;
    }

    public enum FadeMode
    {
        DEFAULT, PRESSED
    }

    public class UIButtonPerhaps : UIElementPerhaps
    {
        [SerializeField] ButtonGraphic[] graphics;

        [Header("Effects")]
        [SerializeField] float cloclLerpTime = 0.3f;
        [SerializeField] float onClickScaleLerpTime = 0.3f;
        [SerializeField] float onClickScaleMultiplier = 1.2f;

        public override void OnValidate()
        {
            base.OnValidate();
            if(graphics == null || graphics.Length == 0)
            {
                Image[] images = GetComponentsInChildren<Image>();
                graphics = new ButtonGraphic[images.Length];

                for (int i = 0; i < images.Length; i++)
                {
                    graphics[i] = new ButtonGraphic();
                    graphics[i].image = images[i];
                    graphics[i].defaultColor = images[i].color;

                    Color c = images[i].color;
                    c -= new Color(0.3f, 0.3f, 0.3f, 0f);

                    graphics[i].pressedColor = c;
                }
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            rect.localScale = rect.localScale * onClickScaleMultiplier;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            rect.localScale = Vector3.one;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            clickAnim = true;

            Lerp(FadeMode.PRESSED, () =>
            {
                clickAnim = false;

                if(state.cursorEntered)
                {
                    Lerp(FadeMode.PRESSED);
                }
                else
                {
                    Lerp(FadeMode.DEFAULT);
                }
            });
        }

        bool clickAnim;
        Coroutine lerpRoutine;
        void Lerp(FadeMode mode, Action onComplete = null)
        {
            if(lerpRoutine != null)
            {
                StopCoroutine(lerpRoutine);
            }


            lerpRoutine = StartCoroutine(ColorLerp(mode, onComplete));
        }

        IEnumerator ColorLerp(FadeMode mode, Action onComplete)
        {
            float startTime = Time.time;

            float t = 0;
            while(t < 1)
            {
                for (int i = 0; i < graphics.Length; i++)
                {
                    LerpColors(graphics[i], mode, t);
                }

                yield return null;
                t = (Time.time - startTime) / cloclLerpTime;
            }

            for (int i = 0; i < graphics.Length; i++)
            {
                LerpColors(graphics[i], mode, 1f);
            }

            lerpRoutine = null;

            if (onComplete != null)
                onComplete();
        }

        void LerpColors(ButtonGraphic graphic, FadeMode mode, float t)
        {
            switch (mode)
            {
                case FadeMode.DEFAULT:
                    graphic.image.color = Color.Lerp(graphic.pressedColor, graphic.defaultColor, t);
                    break;
                case FadeMode.PRESSED:
                    graphic.image.color = Color.Lerp(graphic.defaultColor, graphic.pressedColor, t);
                    break;
                default:
                    break;
            }
        }
    }

}