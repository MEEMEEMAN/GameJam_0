using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perhaps;
using Perhaps.StateMachines;
using System;

namespace Game
{
    public class AIBehavior : LivingEntity
    {
        public BehaviorTree tree;
        public float minRadius = 3f;
        public float spotRadius = 10f;


        public void Start()
        {
            List<Func<BehaviorNodeResult>> mainBranch = new List<Func<BehaviorNodeResult>>()
            {
                StandardNodes.SequenceNodeFunc(PlayerIsClose, ApproachPlayer, ShootPlayer),
            };

            tree.nodes = mainBranch;
        }

        public override void Update()
        {
            base.Update();
            tree.ExecuteTree();
        }

        Vector3 playerPosition;
        Vector3 difference;
        BehaviorNodeResult PlayerIsClose()
        {
            playerPosition = PlayerEntity.instance.transform.position;
            difference = PlayerEntity.instance.transform.position - transform.position;

            return BehaviorNodeResult.FAILED;
        }

        BehaviorNodeResult ApproachPlayer()
        {
            if (difference.magnitude <= minRadius)
            {
                return BehaviorNodeResult.SUCCESS;
            }

            motor.Move(difference.normalized);
            return BehaviorNodeResult.RUNNING;
        }

        BehaviorNodeResult ShootPlayer()
        {
            Debug.Log("shot");
            return BehaviorNodeResult.SUCCESS;
        }
    }

}