using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perhaps;

namespace Game
{
    [RequireComponent(typeof(PerhapsInputNode), typeof(EntityMotor))]
    public class LivingEntity : EntityBase
    {
        [Header("LivingEntity")]
        [SerializeField] PerhapsInputNode InputNode;
        [SerializeField] EntityMotor Motor;

        public PerhapsInputNode input => InputNode;
        public EntityMotor motor => Motor;

        public override void OnValidate()
        {
            base.OnValidate();

            if (input == null)
                InputNode = GetComponent<PerhapsInputNode>();
        }

        public override void Update()
        {
            base.Update();

            Vector2 wasd = input.GetWASDVector();
            
            if(!wasd.x.FloatCompare(0))
            {
                float angle = wasd.x > 0 ? 0 : 180f;
                Vector3 euler = transform.eulerAngles;

                if(!euler.y.FloatCompare(angle))
                {
                    euler.y = angle;
                    transform.eulerAngles = euler;
                }
            }
        }

    }

}