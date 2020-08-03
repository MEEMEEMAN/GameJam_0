using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ShurikensWeapon : WeaponBase
    {
        [Header("Setup")]
        public float aimLerpFactor = 10f;
        public Transform shuriken;
        public Transform hands;
        public Transform throwingHand;
        public Animator animator;

        [Header("Settings")]
        public float throwForce = 10f;
        public float fireRate = 10f;

        static int throwHash = Animator.StringToHash("throw");



        public override void Aim(Vector3 aimPosition)
        {
            Vector3 diff = aimPosition - transform.position;
            diff.z = 0;
            diff.Normalize();

            transform.right = Vector3.Lerp(transform.right, diff, Time.deltaTime * aimLerpFactor);

            float angle = Vector2.SignedAngle(Vector2.down, transform.right);


            float scale = 0f;
            if (angle < 0)
            {
                scale = -1;
            }
            else
            {
                scale = 1;
            }

            Vector3 localScale = hands.localScale;
            localScale.y = scale;
            hands.localScale = localScale;
        }

        void ShootProjectile()
        {

            ShurikenProjectile shurikenPrefab = GameConstants.instance.shurikenPrefab;
            ShurikenProjectile shurikenProj = Instantiate(shurikenPrefab, throwingHand.position, shuriken.transform.rotation);
            shurikenProj.Deploy(equipper, transform.right, throwForce);

            shuriken.gameObject.SetActive(false);
        }

        void ShowShuriken()
        {
            shuriken.gameObject.SetActive(true);
            canShoot = true;
        }

        bool canShoot = true;
        public override void Dequip()
        {
            shuriken.transform.localPosition = shurikenPos;
        }

        Vector3 shurikenPos;
        LivingEntity equipper;
        public override void Equip(LivingEntity equipper)
        {
            this.equipper = equipper;
            shurikenPos = shuriken.transform.localPosition;
        }

        public const string wpnName = "Shurikens";
        public override string GetName()
        {
            return wpnName;
        }

        public override void Shoot()
        {
            if (!canShoot)
                return;

            canShoot = false;
            animator.SetTrigger(throwHash);
        }

        public override WeaponFireMode GetFireMode()
        {
            return WeaponFireMode.FULL;
        }
    }

}