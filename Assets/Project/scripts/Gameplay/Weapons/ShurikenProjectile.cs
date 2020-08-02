using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ShurikenProjectile : ProjectileBase
    {
        public float torque = 10f;
        public override void Deploy(LivingEntity shooter, Vector3 direction, float force)
        {
            base.Deploy(shooter, direction, force);

            float angle = Vector2.SignedAngle(Vector3.down, direction);

            float appliedTorque = torque;
            if(angle > 0)
            {
                appliedTorque *= -1;
            }

            rb.AddTorque(appliedTorque);

        }

        public override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);

            rb.gravityScale = 1f;
        }
    }

}