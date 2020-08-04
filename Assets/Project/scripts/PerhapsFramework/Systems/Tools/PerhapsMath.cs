using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public static class PerhapsMath
    {
        /// <summary>
        /// Returns the closest round of distance / 2.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static float NearestJunction(float position, float distance)
        {
            float ceil = Mathf.Ceil(position / distance) * distance;
            float floor = Mathf.Floor(position / distance) * distance;
            float ceilDelta = Mathf.Abs(ceil - position);
            float floorDelta = Mathf.Abs(floor - position);

            return (ceilDelta < floorDelta ? ceil : floor);
        }

        public static float LerpUnclamped(float a, float b, float t)
        {
            return t * b + (1 - t) * a;
        }

        //Cantor pair courtesey of:
        //https://gist.github.com/GibsS/fdba8e3cdbd307652fc3c01336b32534

        public static int CantorPair(int x, int y)
        {
            return (((x + y) * (x + y + 1)) / 2) + y;
        }

        public static void ReverseCantorPair(int cantor, out int x, out int y)
        {
            var t = (int)Mathf.Floor((-1 + Mathf.Sqrt(1 + 8 * cantor)) / 2);
            x = t * (t + 3) / 2 - cantor;
            y = cantor - t * (t + 1) / 2;
        }

        public static int SignedCantorPair(int x, int y)
        {
            x = x >= 0 ? 2 * x : -2 * x + 1;
            y = y >= 0 ? 2 * y : -2 * y + 1;

            return (((x + y) * (x + y + 1)) / 2) + y;
        }

        public static void SignedReverseCantorPair(int cantor, out int x, out int y)
        {
            var t = (int)Mathf.Floor((-1 + Mathf.Sqrt(1 + 8 * cantor)) / 2);
            x = t * (t + 3) / 2 - cantor;
            y = cantor - t * (t + 1) / 2;

            x = x % 2 == 0 ? x / 2 : ((1 - x) / 2);
            y = y % 2 == 0 ? y / 2 : ((1 - y) / 2);
        }

        /// <summary>
        /// This exists because it can handle negative numbers.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        /// <summary>
        /// Compares floats.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static bool FloatCompare(this float lhs, float rhs, float precision = 0.001f)
        {
            float diff = Mathf.Abs(lhs - rhs);

            return diff <= precision;
        }

        //first-order intercept using absolute target position
        public static Vector3 FirstOrderIntercept
        (
            Vector3 shooterPosition,
            Vector3 shooterVelocity,
            float shotSpeed,
            Vector3 targetPosition,
            Vector3 targetVelocity
        )
        {
            Vector3 targetRelativePosition = targetPosition - shooterPosition;
            Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
            float t = FirstOrderInterceptTime
            (
                shotSpeed,
                targetRelativePosition,
                targetRelativeVelocity
            );
            return targetPosition + t * (targetRelativeVelocity);
        }

        //first-order intercept using relative target position
        static float FirstOrderInterceptTime
        (
            float shotSpeed,
            Vector3 targetRelativePosition,
            Vector3 targetRelativeVelocity
        )
        {
            float velocitySquared = targetRelativeVelocity.sqrMagnitude;
            if (velocitySquared < 0.001f)
                return 0f;

            float a = velocitySquared - shotSpeed * shotSpeed;

            //handle similar velocities
            if (Mathf.Abs(a) < 0.001f)
            {
                float t = -targetRelativePosition.sqrMagnitude /
                (
                    2f * Vector3.Dot
                    (
                        targetRelativeVelocity,
                        targetRelativePosition
                    )
                );
                return Mathf.Max(t, 0f); //don't shoot back in time
            }

            float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
            float c = targetRelativePosition.sqrMagnitude;
            float determinant = b * b - 4f * a * c;

            if (determinant > 0f)
            { //determinant > 0; two intercept paths (most common)
                float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                        t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
                if (t1 > 0f)
                {
                    if (t2 > 0f)
                        return Mathf.Min(t1, t2); //both are positive
                    else
                        return t1; //only t1 is positive
                }
                else
                    return Mathf.Max(t2, 0f); //don't shoot back in time
            }
            else if (determinant < 0f) //determinant < 0; no intercept path
                return 0f;
            else //determinant = 0; one intercept path, pretty much never happens
                return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
        }
    }

}