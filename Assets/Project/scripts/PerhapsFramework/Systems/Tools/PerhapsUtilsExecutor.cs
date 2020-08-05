using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public class PerhapsUtilsExecutor : MonoBehaviour
    {
        public static PerhapsUtilsExecutor instance { get; private set; }

        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                go.name = "PerhapsUtilExecutor";

                go.AddComponent<PerhapsUtilsExecutor>();
            }
        }

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        class ActionPair
        {
            public Func<bool> action;
            public float time;
        }

        static Queue<ActionPair> actionsQueue = new Queue<ActionPair>();
        public static void ExecuteTimed(float time, Func<bool> a)
        {
            if (a == null)
                return;

            lock (actionsQueue)
            {
                ActionPair p = new ActionPair();
                p.action = a;
                p.time = time;

                actionsQueue.Enqueue(p);
            }
        }

        private void Update()
        {
            lock (actionsQueue)
            {
                while (actionsQueue.Count != 0)
                {
                    ActionPair pair = actionsQueue.Dequeue();

                    StartCoroutine(ExecTimed(pair.time, pair.action));
                }
            }
        }

        [SerializeField] int executedCount = 0;
        IEnumerator ExecTimed(float time, Func<bool> a)
        {
            executedCount++;
            float timer = 0f;
            while (timer < time)
            {
                bool continueExectue = a();

                if (!continueExectue)
                    break;

                timer += Time.deltaTime;
                yield return null;
            }
            executedCount--;
        }
    }

}