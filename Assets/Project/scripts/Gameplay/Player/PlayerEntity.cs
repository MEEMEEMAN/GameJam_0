using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public class PlayerEntity : LivingEntity
    {
        [Header("PlayerEntity")]
        public float speed = 3f;
        public float velocityChangeLerp = 10f;
        public float jumpForce = 3f;
        public float midairVelReductionFactor = 0.5f;



        Vector3 velocity;
        public bool jumping = false;
        Vector3 prevFramePos;
        private void Update()
        {
            Vector2 wasd = input.GetWASDVector();
            wasd.y = 0;

            bool isGrounded = IsGrounded();

            if (jumping)
            {
                /*
                    * Jumping is an arc motion, we check if we begin to descend from our initial jump.
                */

                float angle = CalculateAngle(transform.position, prevFramePos);
                if (angle > 90f)
                {
                    jumping = false;
                }
            }

            if (!jumping && isGrounded && input.GetKeyTap(KeyCode.Space))
            {
                jumping = true;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            prevFramePos = transform.position;

            velocity = Vector2.Lerp(velocity, wasd * speed, Time.deltaTime * velocityChangeLerp);
            if (isGrounded && !jumping)
            {
                rb.velocity = velocity;
            }
            else
            {
                rb.AddForce(velocity * midairVelReductionFactor);

                Vector3 range = new Vector3(speed, speed, speed) * 1.2f;
                rb.velocity = PerhapsUtils.Clamp(rb.velocity, -range, range);
            }
        }

        static float CalculateAngle(Vector3 current, Vector3 prev)
        {
            Vector3 diff = current - prev;
            diff.Normalize();

            Debug.DrawRay(current, diff, Color.magenta);
            return Vector3.Angle(Vector3.up, diff);
        }


    }

}