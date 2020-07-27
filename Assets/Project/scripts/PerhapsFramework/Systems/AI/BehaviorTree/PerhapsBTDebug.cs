using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps.StateMachines
{
#if UNITY_EDITOR
    [System.Serializable]
    public struct DebugData
    {
        public string funcName;
        public BehaviorNodeResult result;
    }

    [System.Serializable]
    public class DebugEntry
    {
        public string entryName;

        public List<DebugData> data = new List<DebugData>();
    }
#endif
}