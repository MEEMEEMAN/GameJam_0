using Perhaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ProjectileBase : MonoBehaviour
    {
        [SerializeField] protected Collider2D[] projectileColliders;
        [SerializeField] protected Rigidbody2D rb;
        public float lifeTime = 10f;
        public Collider2D[] colliders => projectileColliders;
        public int damageAmount = 25;


        [Header("Death Animation")]
        public float deathAnimTime = 1f;
        public float delayDeath = 1f;


        void OnValidate()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();

            if (projectileColliders == null || projectileColliders.Length == 0)
                projectileColliders = GetComponentsInChildren<Collider2D>();
        }

        public virtual void Deploy(LivingEntity shooter, Vector3 direction, float force)
        {
            this.shooter = shooter;
            Collider2D[] cols = projectileColliders.Concat(shooter.colliders).ToArray();
            PerhapsUtils.IgnoreCollisionsArray(cols, true);

            rb.AddForce(direction * force, ForceMode2D.Impulse);

            StartCoroutine(DeathTimer());
        }

        IEnumerator DeathTimer()
        {
            yield return new WaitForSeconds(lifeTime);
            DestroyProjectile();
        }

        bool destroyed = false;
        void DestroyProjectile()
        {
            if (destroyed)
                return;

            destroyed = true;
            LeanTween.scale(gameObject, Vector2.zero, deathAnimTime).setOnComplete(() =>
            {
                Destroy(gameObject);
            });
        }

        IEnumerator DelayedDeath()
        {
            yield return new WaitForSeconds(delayDeath);
            DestroyProjectile();
        }


        public LivingEntity shooter;
        public bool usedUp = false;
        public virtual void OnCollisionEnter2D(Collision2D collision)
        {
            usedUp = true;
            StartCoroutine(DelayedDeath());

            IDamagable damagable = collision.transform.GetComponentFromParent<IDamagable>(3);
            if (damagable == null)
                return;

            DamageInfo info = new DamageInfo();
            info.damageAmount = damageAmount;
            info.damageDealer = shooter;
            damagable.DealDamage(info);
        }
    }
}