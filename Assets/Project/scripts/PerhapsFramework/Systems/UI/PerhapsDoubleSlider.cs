using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Perhaps
{
    [RequireComponent(typeof(Slider))]
    public class PerhapsDoubleSlider : PUIElement
    {
        [SerializeField] Slider fgSlider;
        [SerializeField] Slider bgSlider;

        [Header("BG Tween")]
        [SerializeField] float initialWait = 0.3f;
        [SerializeField] float slideTime = 0.3f;

        public override void OnValidate()
        {
            base.OnValidate();

            if(fgSlider == null)
            {
                fgSlider = GetComponent<Slider>();
            }

            if(bgSlider == null)
            {
                GameObject bgSliderGO = new GameObject();
                bgSliderGO.name = "Background Slider";
                bgSliderGO.transform.SetParent(transform);

                Slider s = bgSliderGO.AddComponent<Slider>();
                bgSlider = s;
            }
        }

        public void SetMinMax(float min, float max)
        {
            fgSlider.minValue = min;
            fgSlider.maxValue = max;

            bgSlider.minValue = min;
            bgSlider.maxValue = max;
        }

        Coroutine ongoing;
        public void SetValue(float value, bool instant = false)
        {
            fgSlider.value = value;

            if (ongoing != null)
            {
                StopCoroutine(ongoing);
                ongoing = null;
            }

            if(instant)
            {
                bgSlider.value = value;
            }
            else
            {
                ongoing = StartCoroutine(SliderTween(value));
            }
        }

        IEnumerator SliderTween(float destination)
        {
            yield return new WaitForSeconds(initialWait);

            float startTime = Time.time;
            float t = 0;
            float initialValue = bgSlider.value;
            while(t < 1)
            {
                float delta = Time.time - startTime;

                t = delta / slideTime;
                bgSlider.value = Mathf.Lerp(initialValue, destination, t);
                yield return null;
            }

            bgSlider.value = destination;
            ongoing = null;
        }
    }

}