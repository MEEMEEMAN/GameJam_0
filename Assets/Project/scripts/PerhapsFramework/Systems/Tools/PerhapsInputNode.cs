using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif

namespace Perhaps
{
    public class PerhapsInputNode : MonoBehaviour
    {
#if ENABLE_INPUT_SYSTEM
        public virtual bool GetKeyHeld(Key key)
        {
            if (!allowInput)
                return false;

            return PerhapsInput.GetKeyHeld(key);
        }

        public virtual bool GetKeyTap(Key key)
        {
            if (!allowInput)
                return false;

            return PerhapsInput.GetKeyTap(key);
        }

        public virtual bool GetTouch(out Touch touch)
        {
            if (!allowInput)
            {
                touch = default;
                return false;
            }


            return PerhapsInput.GetTouch(out touch);
        }

        public virtual Touch[] GetAllTouches()
        {
            if (!allowInput)
            {
                return null;
            }

            return PerhapsInput.GetAllTouches();
        }
#else

        public virtual bool GetKeyHeld(KeyCode key)
        {
            if (!allowInput)
                return false;

            return PerhapsInput.GetKeyHeld(key);
        }

        public virtual bool GetKeyTap(KeyCode key)
        {
            if (!allowInput)
                return false;

            return PerhapsInput.GetKeyTap(key);
        }

        public virtual bool GetTouch(out Touch touch)
        {
            return PerhapsInput.GetTouch(out touch);
        }

        public virtual Touch[] GetAllTouches()
        {
            return PerhapsInput.GetAllTouches();
        }
#endif

        /// <summary>
        /// Returns a non normalized WASD vector. where y is forward/backwards and x is left/right
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetWASDVector()
        {
            if (!allowInput)
                return Vector2.zero;

            return PerhapsInput.GetWASDVector();
        }

        public virtual Vector2 GetMouseDelta(float sensitivity = 1f, float responsiveness = 60f)
        {
            if (!allowInput)
                return Vector2.zero;
            return PerhapsInput.GetMouseDelta(sensitivity, responsiveness);
        }

        public virtual bool GetMouseTap(int index)
        {
            if (!allowInput)
                return false;

            return PerhapsInput.GetMouseTap(index);
        }

        public virtual bool GetMouseDoubleTap()
        {
            if (!allowInput)
                return false;

            return PerhapsInput.GetMouseDoubleTap();
        }

        public virtual bool GetMouseReleased(int index)
        {
            if (!allowInput)
                return false;

            return PerhapsInput.GetMouseReleased(index);
        }

        public virtual bool GetMouseHeld(int index)
        {
            if (!allowInput)
                return false;

            return PerhapsInput.GetMouseHeld(index);
        }

        public virtual Vector2 MouseScreenPosition
        {
            get
            {
                if (!allowInput)
                    return Vector2.zero;

                return PerhapsInput.MouseScreenPosition;
            }
        }

        [SerializeField] bool AllowInput = true;

        public bool allowInput => AllowInput;
        public virtual void AllowInputCollection(bool value)
        {
            AllowInput = value;
        }

        public Vector2 StickDirection
        {
            get
            {
                return PerhapsInput.StickDirection;
            }
        }
    }
}