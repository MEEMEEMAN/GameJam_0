using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    [System.Serializable]
    public class PUIPreset
    {
        public string presetName = "A Preset";
        [Header("Tree")]
        public PUINode root;

        public Dictionary<int, PUINode> idToNode;
        [Header("Nodes")]
        public List<PUINode> serializedNodes;

        public PUINode GetNode(int id)
        {
            if(idToNode == null)
            {
                idToNode = new Dictionary<int, PUINode>();

                for (int i = 0; i < serializedNodes.Count; i++)
                {
                    idToNode.Add(serializedNodes[i].id, serializedNodes[i]);
                }
            }

            if (idToNode.TryGetValue(id, out PUINode node))
            {
                return node;
            }

            return null;
        }

        void AddNode(PUINode node)
        {
            if (serializedNodes == null)
                serializedNodes = new List<PUINode>();

            serializedNodes.Add(node);
        }

        [SerializeField]
        [HideInInspector]
        int idReel = 0;

        public static PUIPreset SavePreset(PUIManager manager)
        {
            PUIPreset preset = new PUIPreset();
            preset.root = preset.Traverse(manager.transform);

            return preset;
        }

        PUINode Traverse(Transform t)
        {
            //we only deal with UI Nodes
            if (!(t is RectTransform) )
                return null;

            RectTransform rect = (RectTransform)t;
            PUINode node = new PUINode();

            node.position = rect.position;
            node.rotation = rect.rotation;
            node.scale = rect.localScale;

            PUIElement element = t.GetComponent<PUIElement>();
            if (element != null)
            {
                PAnimatedElement animated = t.GetComponent<PAnimatedElement>();

                node.isAnimated = animated != null;
                if (node.isAnimated)
                {
                    node.transitionPosition = animated.currentTransition;
                }
            }

            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                PUINode childNode = Traverse(child);

                childNode.id = ++idReel;
                AddNode(childNode);

                if (childNode == null)
                    continue;

                if (node.children == null)
                    node.children = new List<int>();

                node.children.Add(childNode.id);
            }

            return node;
        }

        public static void ApplyPreset(PUIManager manager, PUIPreset preset)
        {
            RecursiveApply(preset.root, manager.transform, preset);
        }

        static void RecursiveApply(PUINode node, Transform t, PUIPreset preset)
        {
            if (node.isAnimated)
            {
                PAnimatedElement animated = t.GetComponent<PAnimatedElement>();
                if (animated != null)
                {
                    bool instant = !Application.isPlaying;
                    animated.Transition(node.transitionPosition, instant);
                }
                else
                {
                    Debug.LogError("Node PAnimatedElement Mistmatch");
                }
            }
            else
            {
                t.position = node.position;
                t.rotation = node.rotation;
                t.localScale = node.scale;
            }

            if (node.children == null)
                return;

            if (node.children.Count != t.childCount)
            {
                Debug.LogError("Node Childcount mismatch");
            }

            for (int i = 0; i < node.children.Count; i++)
            {
                PUINode childNode = preset.GetNode(node.children[i]);
                RecursiveApply(childNode, t.GetChild(i), preset);
            }

        }
    }

    [System.Serializable]
    public class PUINode
    {
        /*
         * Unity cannot serialize recursive nodes that well, there is a depth limit of 7.
         * we can circumvent this limitation by using ids.
         */

        public int id;

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public bool isAnimated;
        public TransitionFlag transitionPosition;
        public List<int> children;
    }

}