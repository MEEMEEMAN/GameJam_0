using Perhaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GrapplingHookWeapon : WeaponBase
    {
        public Sprite ropeSprite;
        public float castSpeed = 3f;
        public float maxCastDistance = 10f;
        public Vector2 grappleCastWidth = Vector2.one;
        public LayerMask grappleCastMask;
        public Vector3 bodyRopeOffset;
        float dashForce = 10f;

        [Header("Swinging")]
        public float newMidairVelocityFactor = 60f;

        [Header("Hands")]
        public Transform leftHand;
        public Transform handPivot;

        public RopeParameteres ropeParameters = new RopeParameteres()
        {
            segmentDimensions = new Vector2(1f, 0.5f),
            segmentCount = 6,
            segmentOffset = 0.1f
        };

        private void Start()
        {
            leftHandOriginalPos = leftHand.transform.localPosition;
        }

        public override void Aim(Vector3 aimPosition)
        {
            aimPos = aimPosition;

            Vector3 diff = aimPos - handPivot.transform.position;
            diff.z = 0;

            handPivot.right = diff.normalized;
        }

        public override void Dequip()
        {
            if(lr != null)
            {
                Destroy(lr.gameObject);
                lr = null;
            }
        }


        RopeObject rope;
        LivingEntity entity;
        public override void Equip(LivingEntity equipper)
        {
            entity = equipper;

            rope = RopeFactory.ConstructRope(Vector2.zero, Vector2.zero, ropeSprite, ropeParameters);
        }

        Vector3 aimPos;
        public override Vector3 GetAimedPosition()
        {
            return aimPos;
        }

        public override WeaponFireMode GetFireMode()
        {
            return WeaponFireMode.SINGLE;
        }

        public const string wpnName = "Rope";
        public override string GetName()
        {
            return wpnName;
        }

        private void Update()
        {
            if(ropeDeployed)
            {
                Vector3 pos = rope.segments[rope.segments.Length - 1].rb.transform.position;
                pos.z = leftHand.transform.position.z;
                leftHand.transform.position = pos;

                if(entity.input.GetMouseTap(1))
                {
                    Vector3 diff = rope.root.transform.position - transform.position;
                    diff.z = 0;
                    Vector3 dir = diff.normalized;

                    entity.rb.AddForce(dir * dashForce, ForceMode2D.Impulse);
                }
            }
        }

        public override void Shoot()
        {
            if(ropeDeployed)
            {
                UndeployRope();
                return; 
            }
            else
            {
                if (ropeRoutine != null)
                    StopCoroutine(ropeRoutine);

                ropeRoutine = StartCoroutine(RopeRoutine());
            }
        }

        Vector3 leftHandOriginalPos;
        float midairVelFactor;
        void DeployRope(Vector3 start, Vector3 end)
        {
            ropeDeployed = true;

            start.z = entity.transform.position.z;
            end.z = entity.transform.position.z;
            //entity.transform.position = start + bodyRopeOffset;

            midairVelFactor = entity.motor.midairVelFactor;
            entity.motor.midairVelFactor = newMidairVelocityFactor;

            RopeFactory.ReconfigureRope(ref rope, start, end);
            RopeSegment lastSegment = rope.segments[rope.segments.Length - 1];

            entity.hinge.connectedBody = lastSegment.rb;
            entity.hinge.enabled = true;
        }

        void UndeployRope()
        {
            entity.motor.midairVelFactor = midairVelFactor;
            leftHand.transform.localPosition = leftHandOriginalPos;

            ropeDeployed = false;
            entity.hinge.enabled = false;
            entity.hinge.connectedBody = null;
        }

        public bool ropeDeployed = false;
        Coroutine ropeRoutine;
        static RaycastHit2D[] buffer = new RaycastHit2D[1];

        LineRenderer lr;
        IEnumerator RopeRoutine()
        {
            if(lr == null)
            {
                GameObject go = new GameObject();
                go.name = "GrapplingHookLine";
                lr = go.AddComponent<LineRenderer>();
                lr.startWidth = grappleCastWidth.x;
                lr.endWidth = grappleCastWidth.x;
                lr.positionCount = 2;
            }
            lr.enabled = true;

            Vector3 ropeOrigin = leftHand.transform.position;
            ropeOrigin.z -= 1;
            aimPos.z = ropeOrigin.z;

            Vector3 diff = ropeOrigin - aimPos;
            diff.z = 0;
            Vector3 dir = -diff.normalized;

            float castDistance = 0f;

            Vector3 ropeHead = ropeOrigin;
            while(castDistance < maxCastDistance)
            {
                ropeOrigin = leftHand.transform.position;

                Vector3 add = dir * castSpeed * Time.deltaTime;
                ropeHead += add;
                ropeHead.z = ropeOrigin.z;

                castDistance += add.magnitude;
                lr.SetPosition(0, ropeOrigin);
                lr.SetPosition(1, ropeHead);

                float dst = Vector2.Distance(ropeOrigin, ropeHead);

                int hit = Physics2D.BoxCastNonAlloc(ropeOrigin, grappleCastWidth, 0f, dir, buffer, dst, grappleCastMask);
                if(hit > 0)
                {
                    DeployRope(ropeOrigin, buffer[0].point);
                    break;
                }

                ropeRoutine = null;
                yield return null;
            }

            lr.enabled = false;
        }
    }

}