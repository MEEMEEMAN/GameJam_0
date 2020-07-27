using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Perhaps.StateMachines
{
    /// <summary>
    /// Some standard behavior tree nodes, that have DebugEntry integrated with them.
    /// </summary>
    public static class StandardNodes
    {
#if UNITY_EDITOR
        public static List<DebugEntry> debugEntries = new List<DebugEntry>();
#endif

        public static BehaviorNodeResult SequenceNode(List<Func<BehaviorNodeResult>> functions)
        {
#if UNITY_EDITOR
            DebugEntry entry = new DebugEntry();
            entry.entryName = "Sequence Node";
            debugEntries.Add(entry);
#endif

            for (int i = 0; i < functions.Count; i++)
            {
                BehaviorNodeResult result = functions[i]();

#if UNITY_EDITOR
                DebugData debugData = new DebugData();
                debugData.funcName = functions[i].Method.Name;
                debugData.result = result;
                entry.data.Add(debugData);
#endif

                if (result == BehaviorNodeResult.FAILED || result == BehaviorNodeResult.RUNNING)
                {
                    return result;
                }
            }

            return BehaviorNodeResult.SUCCESS;
        }

        public static BehaviorNodeResult SequenceNode(params Func<BehaviorNodeResult>[] functions)
        {
            return SequenceNode(functions.ToList());
        }

        public static Func<BehaviorNodeResult> SequenceNodeFunc(params Func<BehaviorNodeResult>[] functions)
        {
            return () =>
            {
                return SequenceNode(functions);
            };
        }

        public static BehaviorNodeResult BranchNode(Func<BehaviorNodeResult> condition, Func<BehaviorNodeResult> successBranch,
                                                                                        Func<BehaviorNodeResult> failedBranch)
        {
#if UNITY_EDITOR
            DebugEntry entry = new DebugEntry();
            debugEntries.Add(entry);

            entry.entryName = "Branch - Behavior";
            DebugData debugData = new DebugData();
            entry.data.Add(debugData);
#endif

            if (condition == null)
            {
#if UNITY_EDITOR
                debugData.funcName = "Condition null";
                debugData.result = BehaviorNodeResult.FAILED;
#endif

                return BehaviorNodeResult.FAILED;
            }

            BehaviorNodeResult res = condition();

#if UNITY_EDITOR
            debugData.funcName = condition.Method.Name;
#endif

            if (res == BehaviorNodeResult.SUCCESS)
            {
                if (successBranch != null)
                {
                    res = successBranch();
#if UNITY_EDITOR
                    debugData.funcName = successBranch.Method.Name;
#endif
                }
            }
            else
            {
                if (failedBranch != null)
                {
                    res = failedBranch();

#if UNITY_EDITOR
                    debugData.funcName = failedBranch.Method.Name;
#endif
                }
            }
#if UNITY_EDITOR
            debugData.result = res;
#endif
            return res;
        }

        public static BehaviorNodeResult BranchNode(bool condition, Func<BehaviorNodeResult> successBranch,
                                                                                Func<BehaviorNodeResult> failedBranch)
        {
            BehaviorNodeResult res = BehaviorNodeResult.FAILED;

#if UNITY_EDITOR
            DebugEntry entry = new DebugEntry();
            entry.entryName = "Bool Branch - Condition: " + condition;

            DebugData debugData = new DebugData();
            entry.data.Add(debugData);
            debugEntries.Add(entry);
#endif

            if (condition)
            {
                if (successBranch != null)
                {
                    res = successBranch();
#if UNITY_EDITOR
                    debugData.funcName = successBranch.Method.Name;
#endif
                }
            }
            else
            {
                if (failedBranch != null)
                {
                    res = failedBranch();
#if UNITY_EDITOR
                    debugData.funcName = failedBranch.Method.Name;
#endif
                }
            }
#if UNITY_EDITOR
            debugData.result = res;
#endif

            return res;
        }

        public static Func<BehaviorNodeResult> BranchNodeFunc(bool condition, Func<BehaviorNodeResult> successBranch, Func<BehaviorNodeResult> failedBranch)
        {
            return () =>
            {
                return BranchNode(condition, successBranch, failedBranch);
            };
        }

        public static Func<BehaviorNodeResult> BranchNodeFunc(Func<BehaviorNodeResult> condition, Func<BehaviorNodeResult> successBranch, Func<BehaviorNodeResult> failedBranch)
        {
            return () =>
            {
                return BranchNode(condition, successBranch, failedBranch);
            };
        }
    }
}