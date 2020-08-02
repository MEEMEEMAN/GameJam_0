using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perhaps;

namespace Game
{
    public class EntityMotor : MonoBehaviour
    {
        [SerializeField] LivingEntity livingEntity;
        public LivingEntity entity => livingEntity;

        [SerializeField] EntityMotorState MotorState;
        public EntityMotorState state => MotorState;

        [Header("Ground Probing")]
        public float groundRayLength = 1f;
        public Vector2 groundCastSize = new Vector2(1, 1);

        [Header("Movement Settings")]
        public float speed = 3f;
        public float velocityChangeLerp = 10f;
        public float jumpForce = 3f;
        public float midairVelReductionFactor = 0.5f;

        Vector3 velocity;
        bool jumping = false;
        Vector3 prevFramePos;

        private void OnValidate()
        {
            if(livingEntity == null)
            {
                livingEntity = GetComponent<LivingEntity>();
            }
        }

        private void Start()
        {
            MotorState = new EntityMotorState();
        }

        public void Move(Vector3 direction)
        {
            if(direction.x > 0)
            {
                moveDir = Vector3.Project(direction, Vector3.right);
            }
            else
            {
                moveDir = Vector3.Project(direction, Vector3.left);
            }

            Orientation();
        }

        public void Jump()
        {
            if (!jumping && state.isGrounded)
            {
                jumping = true;
                entity.rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        void Orientation()
        {
            float angle = moveDir.x > 0 ? 0 : 180f;
            Vector3 euler = transform.eulerAngles;

            if (!euler.y.FloatCompare(angle))
            {
                euler.y = angle;
                transform.eulerAngles = euler;
            }
        }

        Vector3 moveDir;
        private void Update()
        {

            state.isGrounded = IsGrounded();

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

            prevFramePos = transform.position;
            velocity = Vector2.Lerp(velocity, moveDir * speed, Time.deltaTime * velocityChangeLerp);

            if (state.isGrounded && !jumping)
            {
                entity.rb.velocity = velocity;
            }
            else
            {
                entity.rb.AddForce(velocity * midairVelReductionFactor);

                Vector3 range = new Vector3(speed, speed, speed) * 1.2f;
                entity.rb.velocity = PerhapsUtils.Clamp(entity.rb.velocity, -range, range);
            }
        }

        public const int hitBufferSize = 5;
        public RaycastHit2D[] groundBuffer = new RaycastHit2D[hitBufferSize];
        public bool IsGrounded()
        {
            int layer = gameObject.layer;
            //ignore raycast
            gameObject.SetLayerRecursive(2);

            int hitCount = Physics2D.BoxCastNonAlloc(transform.position, groundCastSize,
                0f, Vector2.down, groundBuffer, groundRayLength);

            Debug.DrawRay(transform.position, Vector2.down * groundRayLength, Color.red);

            //restore
            gameObject.SetLayerRecursive(layer);

            return hitCount > 0;
        }

        static float CalculateAngle(Vector3 current, Vector3 prev)
        {
            Vector3 diff = current - prev;
            diff.Normalize();

            Debug.DrawRay(current, diff, Color.magenta);
            return Vector3.Angle(Vector3.up, diff);
        }
    }

    public class EntityMotorState
    {
        public bool isJumping;
        public bool isGrounded;
    }

}