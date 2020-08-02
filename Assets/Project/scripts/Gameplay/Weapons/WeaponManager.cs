using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class WeaponManager : MonoBehaviour
    {
        [Header("WeaponManager")]
        [SerializeField] WeaponBase Equipped;
        [SerializeField] LivingEntity livingEntity;

        [Header("Weapons")]
        [SerializeField] WeaponBase[] weaponRegistry;

        public WeaponBase equipped => Equipped;
        public LivingEntity entity => livingEntity;
        public bool isEquipped => equipped != null;

        private void OnValidate()
        {
            if (livingEntity == null)
            {
                livingEntity = GetComponent<LivingEntity>();
            }
        }

        public WeaponBase GetWeapon(string name)
        {
            return weaponRegistry.FirstOrDefault(x => x.GetName() == name);
        }

        public void Equip(WeaponBase wpn)
        {
            Dequip();

            Equipped = wpn;

            if (Equipped != null)
                Equipped.Equip(entity);
        }

        public void Dequip()
        {
            if(equipped != null)
            {
                Equipped.Dequip();
                Equipped = null;
            }
        }

        private void Start()
        {
            if (Equipped != null)
            {
                Equipped.Equip(entity);
            }
        }
    }

}