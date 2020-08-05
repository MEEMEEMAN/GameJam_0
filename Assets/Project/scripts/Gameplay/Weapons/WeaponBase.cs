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
        public abstract Vector3 GetAimedPosition();
        public abstract void Shoot();
        public abstract string GetName();
        public abstract WeaponFireMode GetFireMode();
    }

    public enum WeaponFireMode
    {
        SINGLE, FULL
    }

}