using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    [RequireComponent(typeof(PerhapsInputNode))]
    public class LivingEntity : EntityBase
    {
        [Header("LivingEntity")]
        public PerhapsInputNode input;

        public override void OnValidate()
        {
            base.OnValidate();

            if (input == null)
                input = GetComponent<PerhapsInputNode>();
        }
    }

}