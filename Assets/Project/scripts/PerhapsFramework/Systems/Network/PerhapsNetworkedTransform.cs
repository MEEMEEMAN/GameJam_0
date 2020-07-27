using Mirror;
using Mirror.Websocket;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    [Serializable]
    public struct NetworkRB2DState : IMessageBase
    {
        public Vector2 pos;
        public float rot;

        public void Deserialize(NetworkReader reader)
        {
            pos = reader.ReadVector2();
            rot = reader.ReadSingle();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.WriteVector2(pos);
            writer.WriteSingle(rot);
        }

        public static bool Equals(NetworkRB2DState lhs, NetworkRB2DState rhs)
        {
            Vector2 diff = rhs.pos - lhs.pos;
            float distance = diff.magnitude;

            bool val = false;

            if (distance <= PerhapsNetworkedTransform.distanceThreshold)
                val = true;

            float rot = Mathf.Abs(rhs.rot - lhs.rot);

            if (rot <= PerhapsNetworkedTransform.rotThreshold)
            {
                val &= true;
            }

            return val;
        }
    }


    public class NetworkRB2DStateList : SyncList<NetworkRB2DState> { };
    public class NetworkRB2DScaleList : SyncList<Vector2> { };

    public class PerhapsNetworkedTransform : NetworkBehaviour
    {
        public const float rotThreshold = 1f;
        public const float distanceThreshold = 0.01f;

        public readonly NetworkRB2DStateList netStates = new NetworkRB2DStateList();
        public readonly NetworkRB2DScaleList netScales = new NetworkRB2DScaleList();
        public List<Rigidbody2D> rigibodies = new List<Rigidbody2D>();

        public int maxQueuedStates = 5;

        [Range(0, 60)]
        public int UpdatesPerSecond = 10;
        public float lerpTime = 0.07f;

        public List<Transform> NetworkedTransforms;
        public bool ClientAuthority = true;

        /// <summary>
        /// Indicates if you can send position/rotation/scale updates of your NetworkedTransforms.
        /// </summary>
        public bool CanSyncTransforms => hasAuthority && ClientAuthority || isServer && !ClientAuthority;

        public void OnValidate()
        {
            if (NetworkedTransforms == null)
                return;

            rigibodies = new List<Rigidbody2D>();
            for (int i = 0; i < NetworkedTransforms.Count; i++)
            {
                Rigidbody2D rb = NetworkedTransforms[i].GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rigibodies.Add(rb);
                }
            }
        }

        void SetRigidbodiesActive(bool value)
        {
            if (value)
            {
                for (int i = 0; i < rigibodies.Count; i++)
                {
                    rigibodies[i].simulated = true;
                }
            }
            else
            {
                for (int i = 0; i < rigibodies.Count; i++)
                {
                    rigibodies[i].simulated = false;
                }
            }
        }

        public void OnEnable()
        {
            if (lerpRoutine != null)
            {
                for (int i = 0; i < lerpRoutine.Length; i++)
                {
                    if (lerpRoutine[i] != null)
                    {
                        StopCoroutine(lerpRoutine[i]);
                        lerpRoutine[i] = null;
                    }
                }
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Init();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            for (int i = 0; i < NetworkedTransforms.Count; i++)
            {
                netStates.Add(GetCurrentState((short)i));
                netScales.Add(NetworkedTransforms[i].localScale);
            }

            Init();

            if (isServer && !isClient)
                lerpTime = 0f; //lerps are graphical effects so movement isnt an eye sore, the headless server couldnt care less about it.
                               //we still want the updated position of our client on the server tho
        }

        /// <summary>
        /// Init() is called from server and client, if youre a host Init() can get called twice.
        /// This flag ensures otherwise.
        /// </summary>
        bool initialized = false;

        void Init()
        {
            if (initialized)
                return;

            initialized = true;

            SetRigidbodiesActive(CanSyncTransforms);

            lerpRoutine = new Coroutine[NetworkedTransforms.Count];
            stateUpdates = new Queue<NetworkRB2DState>[NetworkedTransforms.Count];
            for (int i = 0; i < stateUpdates.Length; i++)
            {
                stateUpdates[i] = new Queue<NetworkRB2DState>();
            }

            for (int i = 0; i < netStates.Count; i++)
            {
                Transform t = NetworkedTransforms[i];
                t.position = netStates[i].pos;

                Vector3 euler = t.eulerAngles;
                euler.z = netStates[i].rot;
                Quaternion rot = Quaternion.Euler(euler);
                t.rotation = rot;
            }

            for (int i = 0; i < netScales.Count; i++)
            {
                NetworkedTransforms[i].localScale = netScales[i];
            }

            netStates.Callback += OnNetStateUpdate;
            netScales.Callback += OnNetScaleUpdate;
        }

        public List<NetworkRB2DState> debugStates;

        Queue<NetworkRB2DState>[] stateUpdates;
        private void OnNetStateUpdate(SyncList<NetworkRB2DState>.Operation op, int itemIndex, NetworkRB2DState oldItem, NetworkRB2DState newItem)
        {
            if (debugStates == null || debugStates.Count == 0)
            {
                debugStates = new List<NetworkRB2DState>();

                for (int i = 0; i < NetworkedTransforms.Count; i++)
                {
                    debugStates.Add(default);
                }
            }
            debugStates[itemIndex] = newItem;


            //we already have the correct value
            if (hasAuthority)
                return;

            if (stateUpdates[itemIndex].Count >= maxQueuedStates)
            {
                //Debug.Log("Clearing state queue at " + transform.name);
                //stateUpdates[itemIndex].Clear();
                stateUpdates[itemIndex].Dequeue();
            }

            stateUpdates[itemIndex].Enqueue(newItem);
        }

        private void OnNetScaleUpdate(SyncList<Vector2>.Operation op, int itemIndex, Vector2 oldItem, Vector2 newItem)
        {
            //we already have the correct value
            if (hasAuthority)
                return;


            NetworkedTransforms[itemIndex].localScale = newItem;
        }



        public void OnUpdate()
        {
            if (CanSyncTransforms)
            {
                Sync();
            }
            else
            {
                Interpolate();
            }
        }

        Coroutine[] lerpRoutine;
        void Interpolate()
        {
            for (int i = 0; i < NetworkedTransforms.Count; i++)
            {
                if (lerpRoutine[i] == null)
                {
                    if (stateUpdates[i].Count > 0)
                    {
                        lerpRoutine[i] = StartCoroutine(LerpRoutine(i));
                    }
                }
            }
        }

        IEnumerator LerpRoutine(int index)
        {
            float t = 0f;
            float elapsedTime = 0f;

            NetworkRB2DState targetState = stateUpdates[index].Dequeue();
            Transform networkedTransform = NetworkedTransforms[index];

            Vector3 startPos = networkedTransform.position;
            Quaternion startRot = networkedTransform.rotation;

            Vector3 targetPos = targetState.pos;
            targetPos.z = networkedTransform.position.z;

            Vector3 targetRotEuler = networkedTransform.eulerAngles;
            targetRotEuler.z = targetState.rot;
            Quaternion targetRot = Quaternion.Euler(targetRotEuler);

            if (lerpTime > 0f)
            {
                while (t < 1f)
                {
                    elapsedTime += Time.deltaTime;
                    t = elapsedTime / lerpTime;

                    networkedTransform.position = Vector3.Lerp(startPos, targetPos, t);
                    networkedTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                    yield return null;
                }
            }

            networkedTransform.position = targetPos;
            networkedTransform.rotation = targetRot;

            lerpRoutine[index] = null;
        }

        float prevSyncTime = -100000f;
        void Sync()
        {
            float currentTime = Time.time;
            float deltaSync = currentTime - prevSyncTime;

            if (deltaSync > 1f / (float)UpdatesPerSecond)
            {
                prevSyncTime = currentTime;

                for (int i = 0; i < NetworkedTransforms.Count; i++)
                {
                    if (TransformRequiresStateSync((short)i))
                    {
                        SyncState((short)i);
                    }

                    if (TransformRequiresScaleSync((short)i))
                    {
                        SyncScale((short)i);
                    }
                }
            }
        }

        void SyncState(short index)
        {
            NetworkRB2DState state = GetCurrentState(index);

            if (hasAuthority)
            {
                CmdSyncState(state, index);
            }
            else if (isServer)
            {
                netStates[index] = state;
            }
            else
            {
                Debug.LogError("Trying to sync state but youre not a server nor do you have authority.");
            }
        }

        void SyncScale(short index)
        {
            Vector2 scale = NetworkedTransforms[index].localScale;

            if (hasAuthority)
            {
                CmdSyncScale(scale, index);
            }
            else if (isServer)
            {
                netScales[index] = scale;
            }
            else
            {
                Debug.LogError("Trying to sync scale but youre not a server nor do you have authority.");
            }
        }

        bool TransformRequiresStateSync(short index)
        {
            NetworkRB2DState localState = GetCurrentState(index);
            NetworkRB2DState netState = netStates[index];

            return !NetworkRB2DState.Equals(localState, netState);
        }

        bool TransformRequiresScaleSync(short index)
        {
            Vector2 currentScale = NetworkedTransforms[index].localScale;
            Vector2 netScale = netScales[index];

            Vector2 diff = currentScale - netScale;
            float dist = diff.magnitude;

            return dist > distanceThreshold;
        }

        NetworkRB2DState GetCurrentState(short index)
        {
            Transform netTransform = NetworkedTransforms[index];

            NetworkRB2DState state = new NetworkRB2DState();
            state.pos = netTransform.position;
            state.rot = netTransform.eulerAngles.z;

            return state;
        }

        [Command]
        void CmdSyncState(NetworkRB2DState state, short index)
        {
            netStates[index] = state;
        }

        [Command]
        void CmdSyncScale(Vector2 newScale, short index)
        {
            netScales[index] = newScale;
        }

        [Server]
        public void ForceSetPosition(Vector3 position, short index)
        {
            NetworkedTransforms[index].transform.position = position;
            NetworkRB2DState state = GetCurrentState(index);
            netStates[index] = state;

            RpcForceSetState(state, index);
        }

        [ClientRpc]
        void RpcForceSetState(NetworkRB2DState state, short index)
        {
            if (lerpRoutine[index] != null)
            {
                StopCoroutine(lerpRoutine[index]);
                lerpRoutine[index] = null;
            }

            ApplyState(state, index);
            //NetworkedTransforms[index].position = pos;
        }

        void ApplyState(NetworkRB2DState targetState, short index)
        {
            Transform networkedTransform = NetworkedTransforms[index];
            Vector3 targetRotEuler = networkedTransform.eulerAngles;
            targetRotEuler.z = targetState.rot;
            Quaternion targetRot = Quaternion.Euler(targetRotEuler);

            NetworkedTransforms[index].position = targetState.pos;
            NetworkedTransforms[index].rotation = targetRot;
        }

        public short GetIndexFromTransform(Transform t)
        {
            short index = (short)NetworkedTransforms.IndexOf(t);
            return index;
        }
    }
}
