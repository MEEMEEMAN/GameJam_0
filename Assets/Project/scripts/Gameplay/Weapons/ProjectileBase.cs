using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        public abstract void Deploy(LivingEntity shooter, Vector3 direction);
    }
}