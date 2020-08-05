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

        public event Action<EntityMotor, EntityFacing> OnFaceSwitch;

        public EntityFacing facing { get; private set; }

        [Header("Ground Probing")]
        public float groundRayLength = 1f;
        public Vector2 groundCastSize = new Vector2(1, 1);

        [Header("Movement Settings")]
        public float speed = 3f;
        public float velocityChangeLerp = 10f;
        public float jumpForce = 3f;
        public float midairVelFactor = 0.5f;

        public Vector3 velocity { get; private set; }
        Vector3 prevFramePos;

        private void OnValidate()
        {
            if (livingEntity == null)
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
            if (direction.sqrMagnitude.FloatCompare(0))
            {
                moveDir = Vector3.zero;
                return;
            }

            EntityFacing face = facing;
            if (direction.x > 0)
            {
                moveDir = Vector3.Project(direction, Vector3.right);
                facing = EntityFacing.RIGHT;
            }
            else
            {
                moveDir = Vector3.Project(direction, Vector3.left);
                facing = EntityFacing.LEFT;
            }

            Debug.DrawRay(transform.position, moveDir, Color.blue);

            if (face != facing)
            {
                if (OnFaceSwitch != null)
                {
                    OnFaceSwitch(this, facing);
                }
            }

        }

        public bool Jump()
        {
            if (!state.isJumping && state.isGrounded)
            {
                state.isJumping = true;
                entity.rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                return true;
            }

            return false;
        }

        Vector3 moveDir;
        private void Update()
        {
            state.isGrounded = IsGrounded();

            if (state.isJumping)
            {
                /*
                 * Jumping is an arc motion, we check if we begin to descend from our initial jump.
                */

                float angle = CalculateAngle(transform.position, prevFramePos);
                if (angle > 90f)
                {
                    state.isJumping = false;
                }
            }

            prevFramePos = transform.position;
            velocity = Vector2.Lerp(velocity, moveDir * speed, Time.deltaTime * velocityChangeLerp);

            if (state.isGrounded && !state.isJumping)
            {
                entity.rb.velocity = velocity;
            }
            else
            {
                entity.rb.AddForce(velocity * midairVelFactor * Time.deltaTime);
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

            PerhapsUtils.DrawBoxRay(transform.position, Vector3.down * groundRayLength,
                                groundCastSize, hitCount > 0 ? Color.green : Color.red);

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

    public enum EntityFacing
    {
        RIGHT = 1,
        LEFT,
    }

    [System.Serializable]
    public class EntityMotorState
    {
        public bool isJumping;
        public bool isGrounded;
    }

}