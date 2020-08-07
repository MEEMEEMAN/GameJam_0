using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perhaps;

namespace Game
{
    [RequireComponent(typeof(PerhapsInputNode), typeof(EntityMotor), typeof(EntityHealth))]
    [RequireComponent(typeof(WeaponManager))]
    public class LivingEntity : EntityBase
    {
        [Header("LivingEntity")]
        [Tooltip("The main graphic to rotate on face switch.")]
        [SerializeField] Transform mainGraphic;
        [SerializeField] PerhapsInputNode InputNode;
        [SerializeField] EntityMotor Motor;
        [SerializeField] EntityHealth Health;
        [SerializeField] WeaponManager WeaponManager;
        [SerializeField] HingeJoint2D Hinge;

        public HingeJoint2D hinge => Hinge;
        public PerhapsInputNode input => InputNode;
        public EntityMotor motor => Motor;
        public EntityHealth health => Health;
        public WeaponManager weapons => WeaponManager;

        public override void OnValidate()
        {
            base.OnValidate();

            if (InputNode == null)
            {
                InputNode = GetComponent<PerhapsInputNode>();
            }

            if (Motor == null)
            {
                Motor = GetComponent<EntityMotor>();
            }

            if (Health == null)
            {
                Health = GetComponent<EntityHealth>();
            }

            if (WeaponManager == null)
            {
                WeaponManager = GetComponent<WeaponManager>();
            }

            if (Hinge == null)
            {
                Hinge = GetComponent<HingeJoint2D>();
            }
        }

        private void Start()
        {
            motor.OnFaceSwitch += OnFaceSwitch;
        }

        private void OnFaceSwitch(EntityMotor motor, EntityFacing face)
        {
            return;

            float angle = face == EntityFacing.RIGHT ? 0f : 180f;
            Vector3 euler = mainGraphic.transform.localEulerAngles;
            euler.y = angle;

            mainGraphic.localEulerAngles = euler;
        }

        public override void Update()
        {
            base.Update();
        }


    }

}