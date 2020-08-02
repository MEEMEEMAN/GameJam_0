using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public abstract void Equip(LivingEntity equipper);
        public abstract void Dequip();
        public abstract void Aim(Vector3 aimPosition);
        public abstract void Shoot();
        public abstract string GetName();
    }

}