using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perhaps;
using Perhaps.StateMachines;
using System;

namespace Game
{
    public class EnemyAI : LivingEntity
    {
        public BehaviorTree tree;
        public float minRadius = 3f;
        public float spotRadius = 10f;

        public int[] range;
        public GameObject player;
        public GameObject bullet;
        public Transform m;


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

        Vector3 difference;
        BehaviorNodeResult PlayerIsClose()
        {
            difference = PlayerEntity.instance.transform.position - transform.position;

            if (difference.magnitude <= spotRadius)
            {
                return BehaviorNodeResult.SUCCESS;
            }
            WalkRand(5f, range);
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
            StartCoroutine(a());
            return BehaviorNodeResult.SUCCESS;
        }

        void WalkRand(float speed, int[] range)
        {
            var x = UnityEngine.Random.Range(range[0], range[1]);
            Vector3 t = new Vector3(x, transform.position.y, transform.position.z);
            motor.Move(t);
        }

        IEnumerator a()
        {
            yield return new WaitForSeconds(0.5f);
            Instantiate(bullet, m);
            yield return new WaitForSeconds(0.5f);
        }
    }



}