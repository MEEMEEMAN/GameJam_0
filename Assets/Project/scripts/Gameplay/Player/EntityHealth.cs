using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EntityHealth : MonoBehaviour, IDamagable
    {
        [SerializeField] LivingEntity livingEntity;
        public LivingEntity entity => livingEntity;


        public void DealDamage(DamageInfo damage)
        {
            
        }

        private void OnValidate()
        {
            if (livingEntity == null)
            {
                livingEntity = GetComponent<LivingEntity>();
            }
        }
    }
}