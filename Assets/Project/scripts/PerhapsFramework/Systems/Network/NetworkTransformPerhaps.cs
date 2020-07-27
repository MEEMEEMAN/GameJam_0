using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public class NetworkTransformPerhaps : NetworkBehaviour
    {
        [SerializeField] List<Transform> networkedTransforms;
        public bool ClientAuthority = false;
        [Range(0f, 1f)]
        public float LerpTime = 0.0166666f * 2;
        [Range(1, 60)]
        public int UpdatesPerSecond = 10;

        bool CanSend => hasAuthority && ClientAuthority || isServer && !ClientAuthority;

        float lastUpdate = float.MinValue;
        public void Update()
        {
            if(CanSend)
            {
                AsSender();
            }
            else
            {
                AsReceiver();
            }
        }

        void AsSender()
        {
            float delta = Time.time - lastUpdate;
            float t = 1 / UpdatesPerSecond;
            if (delta >= t)
            {
                lastUpdate = Time.time;
                SendUpdates();
            }
        }

        Coroutine receiverRoutine;
        Queue<NetTransformPosition[]> receiveQueue = new Queue<NetTransformPosition[]>();
        void AsReceiver()
        {
            if(receiverRoutine == null)
            {
                receiverRoutine = StartCoroutine(lerpRoutine());
            }
        }

        IEnumerator lerpRoutine()
        {
            while(!CanSend)
            {
                if(receiveQueue.Count == 0)
                {
                    yield return null;
                    continue;
                }

                NetTransformPosition[] positionsToLerp = receiveQueue.Dequeue();
                for (int i = 0; i < positionsToLerp.Length; i++)
                {
                    NetTransformPosition pos = positionsToLerp[i];
                    GameObject go = networkedTransforms[pos.index].gameObject;
                    Vector3 targetPos = pos.position;
                    targetPos.z = go.transform.position.z;
                    Vector3 targetEuler = go.transform.eulerAngles;
                    targetEuler.z = pos.angle;

                    go.transform.position = targetPos;
                    go.transform.eulerAngles = targetEuler;
                    /*
                    NetTransformPosition pos = positionsToLerp[i];
                    GameObject go = networkedTransforms[pos.index].gameObject;
                    iTween.Stop(go);

                    Vector3 targetPos = pos.position;
                    targetPos.z = go.transform.position.z;
                    Vector3 targetEuler = go.transform.eulerAngles;
                    targetEuler.z = pos.angle;

                    iTween.MoveTo(go, pos.position, LerpTime);
                    iTween.RotateTo(go, targetEuler, LerpTime);
                    */
                }

                yield return new WaitForSeconds(LerpTime);
            }

            receiverRoutine = null;
        }

        NetTransformPosition[] msgBuffer;
        void SendUpdates()
        {
            if (msgBuffer == null)
                msgBuffer = new NetTransformPosition[networkedTransforms.Count]; 

            for (int i = 0; i < networkedTransforms.Count; i++)
            {
                msgBuffer[i].index = i;
                msgBuffer[i].position = networkedTransforms[i].position;
                msgBuffer[i].angle = networkedTransforms[i].eulerAngles.z;
            }

            if(isServer)
            {
                RpcUpdatePositions(msgBuffer);   
            }
            else
            {
                CmdUpdatePositions(msgBuffer);
            }
        }

        [Server]
        public void ForceSetPosition(Vector2 position)
        {
            
        }

        [Command]
        void CmdUpdatePositions(NetTransformPosition[] positions)
        {
            RpcUpdatePositions(positions);
        }

        [ClientRpc]
        void RpcUpdatePositions(NetTransformPosition[] positions)
        {
            if (hasAuthority)
                return;

            receiveQueue.Enqueue(positions);
        }
    }

    public struct NetTransformPosition : IMessageBase
    {
        public Vector2 position;
        public float angle;
        public int index;

        public void Deserialize(NetworkReader reader)
        {
            position = reader.ReadVector2();
            angle = reader.ReadSingle();
            index = reader.ReadInt32();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.WriteVector2(position);
            writer.WriteSingle(angle);
            writer.WriteInt32(index);
        }
    }

}