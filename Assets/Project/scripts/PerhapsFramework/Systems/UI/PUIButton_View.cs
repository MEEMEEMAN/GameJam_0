using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perhaps
{
    [RequireComponent(typeof(PUIButton))]
    public class PUIButton_View : PUIElement
    {
        [Header("Settings")]
        [SerializeField]
        PUIButton btn;

        [Header("Inflation")]
        [Tooltip("Briefly increase the scale of the element.")]
        public ButtonChangeFlag inflateFlag;
        public float inflateFactor = 1.1f;
        public float inflateTime = 0.1f;

        [SerializeField]
        [HideInInspector]
        Vector3 defaultScale;

        [Header("Color")]
        public ImageColor[] images;
        public float colorTransitionTime = 0.1f;
        [Tooltip("The default color factor.")]
        [Range(0, 1)]
        [SerializeField] float defaultColorFactor = 1f;

        [Header("OnClick")]
        public bool colorTransitionOnClick = true;
        [Range(0, 1)]
        public float onClickColorFactor = 0.7f;

        [Header("OnPointerEnter")]
        public bool colorTransitionOnCursorEnter = true;
        [Range(0, 1)]
        public float onCursorEnterFactor = 0.8f;


        public override void OnValidate()
        {
            base.OnValidate();

            if (btn == null)
                btn = GetComponent<PUIButton>();

            if (images == null || images.Length == 0)
            {
                Image[] imgs = GetComponentsInChildren<Image>();
                images = new ImageColor[imgs.Length];

                for (int i = 0; i < imgs.Length; i++)
                {
                    ImageColor image = new ImageColor();
                    images[i] = image;

                    image.image = imgs[i];
                    image.defaultColor = image.image.color;
                }
            }

            defaultScale = transform.localScale;
        }

        private void Start()
        {
            btn.OnButtonChangeState += OnStateChange;
            currentFactor = defaultColorFactor;

            SetImageColors(currentFactor);
        }

        private void OnStateChange(PUIButton btn, ButtonState state)
        {
            if (state.isClickEvent)
            {
                OnClick();
                OnToggle(state.isToggled);
            }

            if (state.isDownEvent)
            {
                if (state.isDown)
                {
                    OnPointerDown();
                }
                else
                {
                    OnPointerUp();
                }
            }

            if (state.isDragEvent)
            {
                if (state.isDragged)
                {
                    OnBeginDrag();
                }
                else
                {
                    OnEndDrag();
                }
            }

            if (state.isPointerWithinEvent)
            {
                if (state.isPointerWithin)
                {
                    OnPointerEnter();
                }
                else
                {
                    OnPointerExit();
                }
            }
        }

        void OnClick()
        {
            if (inflateFlag.IsSet(ButtonChangeFlag.ON_CLICK))
            {
                LeanTween.scale(rect, defaultScale * inflateFactor, inflateTime).setOnComplete(() =>
                {
                    LeanTween.scale(rect, defaultScale, inflateTime);
                });
            }

            if (colorTransitionOnClick)
            {
                AnimateColors(onClickColorFactor, colorTransitionTime, () =>
                {
                    /*
                     * If we transition colors on click AND on cursor enter,
                     * we must transition back to the correct color factor -
                     * provided that the cursor has entered the element.
                     * 
                     * If the buttons acts as a toggle, and it is toggled,
                     * we simply want to keep the click color.
                     */

                    float factor;
                    if(btn.actAsToggle && btn.state.isToggled)
                    {
                        factor = onClickColorFactor;
                    }
                    else if (colorTransitionOnCursorEnter && btn.state.isPointerWithin)
                    {
                        factor = onCursorEnterFactor;
                    }
                    else
                    {
                        factor = defaultColorFactor;
                    }

                    AnimateColors(factor, colorTransitionTime);
                });
            }
        }

        void OnPointerDown()
        {

        }

        void OnPointerUp()
        {

        }

        void OnBeginDrag()
        {

        }

        void OnEndDrag()
        {

        }
        void OnToggle(bool isToggled)
        {

        }

        void OnPointerEnter()
        {
            if (inflateFlag.IsSet(ButtonChangeFlag.ON_CURSOR_ENTER))
            {
                LeanTween.scale(rect, defaultScale * inflateFactor, inflateTime);
            }

            bool notToggled = !(btn.actAsToggle && btn.state.isToggled);
            if (colorTransitionOnCursorEnter && notToggled)
            {
                AnimateColors(onCursorEnterFactor, colorTransitionTime);
            }
        }

        void OnPointerExit()
        {
            if (inflateFlag.IsSet(ButtonChangeFlag.ON_CURSOR_ENTER))
            {
                LeanTween.scale(rect, defaultScale, inflateTime);
            }

            bool notToggled = !(btn.actAsToggle && btn.state.isToggled);
            if (colorTransitionOnCursorEnter && notToggled)
            {
                AnimateColors(defaultColorFactor, colorTransitionTime);
            }
        }


        Coroutine colorRoutine;
        void AnimateColors(float targetFactor, float lerpTime, Action onDone = null)
        {
            if (colorRoutine != null)
            {
                StopCoroutine(colorRoutine);
            }

            colorRoutine = StartCoroutine(LerpImageColors(targetFactor, lerpTime, onDone));
        }

        float currentFactor = 1f;
        IEnumerator LerpImageColors(float colorFactor, float lerpTime, Action onDone = null)
        {
            /*
             * This function supports instant transitions,
             * We dont want to divide by zero.
             */
            if (lerpTime > 0)
            {
                float t = 0f;
                float startTime = Time.time;

                while (t < 1)
                {
                    t = (Time.time - startTime) / lerpTime;
                    currentFactor = Mathf.Lerp(currentFactor, colorFactor, t);
                    SetImageColors(currentFactor);
                    yield return null;
                }
            }

            currentFactor = colorFactor;
            SetImageColors(currentFactor);
            colorRoutine = null;

            if (onDone != null)
            {
                onDone();
            }
        }

        void SetImageColors(float factor)
        {
            for (int i = 0; i < images.Length; i++)
            {
                ImageColor col = images[i];
                col.image.color = col.defaultColor * factor;
            }
        }
    }

    [System.Serializable]
    [Flags]
    public enum ButtonChangeFlag
    {
        NEVER = 0,
        ON_CURSOR_ENTER,
        ON_CLICK
    }

    [System.Serializable]
    public class ImageColor
    {
        public Image image;
        public Color defaultColor;
    }
}
