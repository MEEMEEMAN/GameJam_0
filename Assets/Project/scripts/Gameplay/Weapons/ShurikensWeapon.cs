using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ShurikensWeapon : WeaponBase
    {
        [Header("Setup")]
        public SpriteRenderer model;
        public float aimLerpFactor = 10f;
        public Transform hands;

        [Header("Settings")]
        public float fireRate = 10f;

        public override void Aim(Vector3 aimPosition)
        {
            Vector3 diff = aimPosition - transform.position;
            diff.z = 0;
            diff.Normalize();

            transform.right = Vector3.Lerp(transform.right, diff, Time.deltaTime * aimLerpFactor);

            float angle = Vector2.SignedAngle(Vector2.down, transform.right);

            float zAngle = 0f;
            if(angle < 0)
            {
                zAngle = 180f;
            }
            else
            {
                zAngle = 0f;
            }

            Vector3 euler = hands.localEulerAngles;
            euler.z = zAngle;
            hands.localEulerAngles = euler;
        }

        public override void Dequip()
        {

        }

        public override void Equip(LivingEntity equipper)
        {

        }

        public const string wpnName = "Shurikens";
        public override string GetName()
        {
            return wpnName;
        }

        public override void Shoot()
        {
            
        }
    }

}