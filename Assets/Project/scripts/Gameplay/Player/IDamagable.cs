using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IDamagable
    {
        void DealDamage(DamageInfo damage);
    }

    public struct DamageInfo
    {
        public EntityBase damageDealer;
        public RaycastHit2D hitPoint;
        public int damageAmount;
    }

}