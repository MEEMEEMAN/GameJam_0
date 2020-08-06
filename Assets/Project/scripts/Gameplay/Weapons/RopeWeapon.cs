using Perhaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RopeWeapon : WeaponBase
    {
        public Sprite ropeSprite;
        public RopeParameteres ropeParameters = new RopeParameteres()
        {
            segmentDimensions = new Vector2(1f, 0.5f),
            segmentCount = 6,
            segmentOffset = 0.1f
        };
        
        public override void Aim(Vector3 aimPosition)
        {
            aimPos = aimPosition;
        }

        public override void Dequip()
        {
            
        }

        private void FixedUpdate()
        {
            if (entity == null)
                return;

            if(entity.input.GetMouseHeld(1))
            {
                rope.root.position = MainCamera.instance.WSCursorPosition;
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

        public override void Shoot()
        {
            Vector3 ropeOrigin = transform.position;
            ropeOrigin.z -= 1;
            aimPos.z = ropeOrigin.z;

            RopeFactory.ReconfigureRope(ref rope, ropeOrigin, aimPos);
        }
    }

}