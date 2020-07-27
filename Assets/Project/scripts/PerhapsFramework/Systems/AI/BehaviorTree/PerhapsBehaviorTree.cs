using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Perhaps.StateMachines
{
    public enum BehaviorNodeResult
    {
        SUCCESS = 0,
        RUNNING = 10,
        FAILED = 20
    }

    /// <summary>
    /// Useful for AI Programming.
    /// </summary>
    [System.Serializable]
    public class BehaviorTree
    {
        public List<Func<BehaviorNodeResult>> nodes;

#if UNITY_EDITOR
        public List<DebugEntry> branchEntries = new List<DebugEntry>();
#endif

        public void ExecuteTree()
        {
            if (nodes != null)
            {
#if UNITY_EDITOR
                branchEntries.Clear();
#endif
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i] != null)
                        nodes[i]();
                }

#if UNITY_EDITOR
                branchEntries = StandardNodes.debugEntries;
#endif
            }
        }
    }
}