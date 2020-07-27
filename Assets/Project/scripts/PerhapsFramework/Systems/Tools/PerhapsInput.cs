using System.Collections.Generic;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
#else
using TouchPhase = UnityEngine.TouchPhase;
#endif

namespace Perhaps
{
    public static class PerhapsInput
    {
        public static bool AllowPerhapsUIInteraction = true;

#if ENABLE_INPUT_SYSTEM
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            EnhancedTouchSupport.Enable();
        }


        /// <summary>
        /// Returns a non normalized WASD vector. where y is forward/backwards and x is left/right
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetWASDVector()
        {
            if (!mInputAllowed)
                return Vector2.zero;


            int w = GetKeyHeld(Key.W) ? 1 : 0;
            int s = GetKeyHeld(Key.S) ? -1 : 0;

            int d = GetKeyHeld(Key.D) ? 1 : 0;
            int a = GetKeyHeld(Key.A) ? -1 : 0;

            return new Vector2(a + d, w + s);
        }

        public static bool GetKeyTap(Key key)
        {
            if (!mInputAllowed)
                return false;

            return Keyboard.current[key].wasPressedThisFrame;
        }
        public static bool GetKeyHeld(Key key)
        {
            if (!mInputAllowed)
                return false;

            return Keyboard.current[key].isPressed;
        }

        public static bool GetTouch(out Touch t)
        {
            if (Touch.activeTouches.Count == 0)
            {
                t = default;
                return false;
            }

            t = Touch.activeTouches[0];
            return true;
        }

        public static Touch[] GetAllTouches()
        {
            return Touch.activeTouches.ToArray();
        }

#else
        public static bool GetKeyHeld(KeyCode key)
        {
            if (!mInputAllowed)
                return false;

            return Input.GetKey(key);
        }

        public static bool GetKeyTap(KeyCode key)
        {
            if (!mInputAllowed)
                return false;

            return Input.GetKeyDown(key);
        }
    
        public static bool GetTouch(out Touch touch)
        {
#if UNITY_EDITOR || UNITY_STANDALONE //simulate touch
            Touch t = new Touch();
            t.fingerId = 0;
            t.position = MouseScreenPosition;


            if (GetMouseTap(0))
            {
                t.phase = TouchPhase.Began;
            }
            else if (GetMouseHeld(0))
            {
                Vector2 delta = GetMouseDelta();

                if (delta.sqrMagnitude > 0)
                {
                    t.phase = TouchPhase.Moved;
                    t.deltaPosition = delta;
                }
                else
                {
                    t.phase = TouchPhase.Stationary;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                t.phase = TouchPhase.Ended;
            }
            else
            {
                touch = t;
                return false;
            }

            touch = t;
            return true;
#else

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);

                touch = t;
                return true;
            }

            touch = default;
            return false;
#endif
        }

        public static Touch[] GetAllTouches()
        {
            Touch[] touches = new Touch[Input.touchCount];

            for (int i = 0; i < Input.touchCount; i++)
            {
                touches[i] = Input.GetTouch(i);
            }

            return touches;
        }

        /// <summary>
        /// Returns a non normalized WASD vector. where y is forward/backwards and x is left/right
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetWASDVector()
        {
            if (!mInputAllowed)
                return Vector2.zero;


            int w = GetKeyHeld(KeyCode.W) ? 1 : 0;
            int s = GetKeyHeld(KeyCode.S) ? -1 : 0;

            int d = GetKeyHeld(KeyCode.D) ? 1 : 0;
            int a = GetKeyHeld(KeyCode.A) ? -1 : 0;

            return new Vector2(a + d, w + s);
        }
#endif

        public static void SetStickVector(Vector2 direction)
        {
            mStickVector = direction;
        }

        public static void SetStickStrength(float factor)
        {
            stickVectorStrength = factor;
        }

        static float stickVectorStrength;
        static Vector2 mStickVector;
        /// <summary>
        /// Get the stick controller set by the UI
        /// </summary>
        public static Vector2 StickDirection
        {
            get
            {
                if (!mInputAllowed)
                    return Vector2.zero;

                return mStickVector;
            }
        }

        public static float StickVectorStrength
        {
            get
            {
                if (!mInputAllowed)
                    return 0f;

                return stickVectorStrength;
            }
        }

        static Vector2 LerpDelta = Vector2.zero;
        public static Vector2 GetMouseDelta(float sensitivity = 1f, float responsiveness = 60f)
        {
            if (!mInputAllowed)
                return Vector2.zero;

            Vector2 mouseDelta;
#if ENABLE_INPUT_SYSTEM
            mouseDelta = Mouse.current.delta.ReadValue();
#else
            mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * sensitivity;
#endif

            LerpDelta = Vector2.Lerp(LerpDelta, mouseDelta, Time.deltaTime * responsiveness);
            return LerpDelta;
        }

        public static Vector2 MouseScreenPosition
        {
            get
            {
                if (!mInputAllowed)
                    return Vector2.zero;

#if ENABLE_INPUT_SYSTEM
                return Mouse.current.position.ReadValue();
#else
            return Input.mousePosition;
#endif
            }
        }

        public static bool GetMouseTap(int index, bool ignoreUI = false)
        {
            if (!mInputAllowed)
                return false;

            if (!ignoreUI && UIElementPerhaps.IsPointBlocked(MouseScreenPosition))
            {
                return false;
            }


#if ENABLE_INPUT_SYSTEM
            switch (index)
            {
                case 0:
                    return Mouse.current.leftButton.wasPressedThisFrame;
                case 1:
                    return Mouse.current.rightButton.wasPressedThisFrame;
                case 2:
                    return Mouse.current.middleButton.wasPressedThisFrame;
                case 3:
                    return Mouse.current.forwardButton.wasPressedThisFrame;
                case 4:
                    return Mouse.current.backButton.wasPressedThisFrame;
                default:
                    return false;
            }
#else
            return Input.GetMouseButtonDown(index);
#endif
        }

        public static bool GetMouseReleased(int index)
        {
            if (!mInputAllowed)
                return false;


#if ENABLE_INPUT_SYSTEM
            switch (index)
            {
                case 0:
                    return Mouse.current.leftButton.wasReleasedThisFrame;
                case 1:
                    return Mouse.current.rightButton.wasReleasedThisFrame;
                case 2:
                    return Mouse.current.middleButton.wasReleasedThisFrame;
                case 3:
                    return Mouse.current.forwardButton.wasReleasedThisFrame;
                case 4:
                    return Mouse.current.backButton.wasReleasedThisFrame;
                default:
                    return false;
            }
#else
            return Input.GetMouseButtonUp(index);
#endif
        }

        public static bool GetMouseHeld(int index, bool ignoreUI = false)
        {
            if (!mInputAllowed)
                return false;

            if (!ignoreUI && UIElementPerhaps.IsPointBlocked(MouseScreenPosition))
            {
                return false;
            }


#if ENABLE_INPUT_SYSTEM
            switch (index)
            {
                case 0:
                    return Mouse.current.leftButton.isPressed;
                case 1:
                    return Mouse.current.rightButton.isPressed;
                case 2:
                    return Mouse.current.middleButton.isPressed;
                case 3:
                    return Mouse.current.forwardButton.isPressed;
                case 4:
                    return Mouse.current.backButton.isPressed;
                default:
                    return false;
            }
#else
            return Input.GetMouseButton(index);
#endif
        }

        public static void SetCursorLocked(bool value)
        {
            if (value)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        static bool mInputAllowed = true;

        public static void AllowInputCollection(bool value)
        {
            mInputAllowed = value;
        }
    }

}