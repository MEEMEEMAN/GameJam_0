using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public class PAnimatedElement : PUIElement
    {
        public TransitionFlag currentTransition;
        [Header("Data")]
        public AnimatedElementData startPosition;
        public AnimatedElementData endPosition;
        public float animateTime = 0.5f;

        [Header("Curves")]
        public AnimatedCurveData curves;

        [Header("Settings")]
        public bool ignorePresets = false;

        public override void OnValidate()
        {
            if (startPosition == null)
            {
                startPosition = WriteCurrentPosition();
            }

            if (endPosition == null)
            {
                endPosition = WriteCurrentPosition();
            }
        }

        AnimatedElementData GetData(TransitionFlag flag)
        {
            if (flag == TransitionFlag.START)
            {
                return startPosition;
            }
            else
            {
                return endPosition;
            }
        }

        public void Transition(TransitionFlag flag, bool instant = false)
        {
            currentTransition = flag;
            var to = GetData(flag);

            Animate(to, instant);
        }

        public void TransitionOpposite(bool instant = false)
        {
            TransitionFlag f = GetOpposite(currentTransition);
            Transition(f, instant);
        }

        static TransitionFlag GetOpposite(TransitionFlag flag)
        {
            TransitionFlag ret = flag == TransitionFlag.START ? TransitionFlag.END : TransitionFlag.START;
            return ret;
        }

        public AnimatedElementData WriteCurrentPosition()
        {
            AnimatedElementData data = new AnimatedElementData();
            data.position = rect.anchoredPosition;
            data.rotation = rect.localEulerAngles.z;
            data.scale = rect.localScale;

            return data;
        }

        Coroutine routine;
        void Animate(AnimatedElementData to, bool instant = false)
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }

            routine = StartCoroutine(TransitionRoutine(to, curves, instant ? 0 : animateTime));
        }

        IEnumerator TransitionRoutine(AnimatedElementData to, AnimatedCurveData curve, float time)
        {
            if (time > 0)
            {
                float t = 0;
                float startTime = Time.time;
                while (t < 1)
                {
                    t = (Time.time - startTime) / time;

                    float posT = curve.positionCurve.Evaluate(t) * t;
                    float rotT = curve.rotationCurve.Evaluate(t) * t;
                    float scaleT = curve.scaleCurve.Evaluate(t) * t;


                    TransitionLerp(to, posT, rotT, scaleT);
                    yield return null;
                }
            }

            TransitionLerp(to, 1, 1, 1);
        }

        void TransitionLerp(AnimatedElementData to, float posT, float rotT, float scaleT)
        {
            /*
             * LerpUnclamped so we can lerp to a value beyond 0 and 1
             */

            Vector3 pos = Vector3.LerpUnclamped(rect.anchoredPosition, to.position, posT);
            float rot = Mathf.LerpUnclamped(rect.localEulerAngles.z, to.rotation, rotT);
            Vector3 scale = Vector3.LerpUnclamped(rect.localScale, to.scale, scaleT);

            rect.anchoredPosition = pos;
            Vector3 euler = rect.localEulerAngles;
            euler.z = rot;
            rect.localEulerAngles = euler;

            rect.localScale = scale;
        }
    }

    public enum TransitionFlag
    {
        START, END
    }

    [System.Serializable]
    public class AnimatedElementData
    {
        public Vector3 position;
        public float rotation;
        public Vector3 scale;
    }

    [System.Serializable]
    public class AnimatedCurveData
    {
        public AnimationCurve positionCurve = AnimationCurve.Constant(0, 1, 1);
        public AnimationCurve rotationCurve = AnimationCurve.Constant(0, 1, 1);
        public AnimationCurve scaleCurve = AnimationCurve.Constant(0, 1, 1);
    }

}