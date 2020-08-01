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



        public Vector3 velocity;
        public bool jumping = false;
        private void Update()
        {
            Vector2 wasd = input.GetWASDVector();
            wasd.y = 0;


            bool isGrounded = IsGrounded();

            if(!jumping && isGrounded && input.GetKeyTap(KeyCode.Space))
            {
                jumping = true;
                StartCoroutine(JumpRoutine());

                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            velocity = Vector2.Lerp(velocity, wasd * speed, Time.deltaTime * velocityChangeLerp);
            if (isGrounded && !jumping)
            {
                rb.velocity = velocity;
            }
            else
            {
                rb.AddForce(velocity * midairVelReductionFactor);
            }
        }

        IEnumerator JumpRoutine()
        {
            float currentPeak = transform.position.y;
            float pastPeak = float.MinValue;
            while(currentPeak > pastPeak)
            {
                pastPeak = currentPeak;
                currentPeak = transform.position.y;
                yield return null;
            }


            jumping = false;
        }

        
    }

}