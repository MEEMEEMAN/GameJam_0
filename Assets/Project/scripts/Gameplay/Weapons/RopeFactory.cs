using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public class RopeFactory : RopeObject
    {
        private RopeFactory() : base(null, 0, null)
        {

        }


        public static RopeObject ConstructRope(Vector3 start, Vector3 end, Sprite ropeSprite, RopeParameteres parameters)
        {
            int segmentCount = parameters.segmentCount;
            Vector2 segmentDimensions = parameters.segmentDimensions;
            float segmentOffset = parameters.segmentOffset;

            GameObject root = new GameObject();

            Rigidbody2D rootRb = root.AddComponent<Rigidbody2D>();
            rootRb.bodyType = RigidbodyType2D.Static;
            rootRb.name = "RopeRoot";

            RopeSegment[] segments = new RopeSegment[segmentCount];

            for (int i = 0; i < segmentCount; i++)
            {
                RopeSegment segment = new RopeSegment();
                GameObject segmentGO = new GameObject();

                segmentGO.layer = 10;//rope layer
                segmentGO.name = "RopeSegment " + i;
                segmentGO.transform.localScale = segmentDimensions;
                segmentGO.transform.SetParent(root.transform);

                SpriteRenderer sr = segmentGO.AddComponent<SpriteRenderer>();
                sr.sprite = ropeSprite;

                BoxCollider2D boxCol = segmentGO.AddComponent<BoxCollider2D>();
                Rigidbody2D rb = segmentGO.AddComponent<Rigidbody2D>();
                HingeJoint2D hinge = segmentGO.AddComponent<HingeJoint2D>();

                if(parameters.useAnglelimits)
                {
                    hinge.useLimits = true;
                    hinge.limits = parameters.hingeLimits;
                }
                else
                {
                    hinge.useLimits = false;
                }

                hinge.anchor = new Vector2(0f, 0.5f);
                hinge.connectedBody = i > 0 ? segments[i - 1].rb : rootRb;

                rb.mass = i == segmentCount - 1 ? parameters.lastSegmentMass : parameters.segmentMass;

                segment.sr = sr;
                segment.rb = rb;
                segment.hinge = hinge;
                segment.collider = boxCol;

                segments[i] = segment;
            }

            RopeObject ro = new RopeObject(segments, segmentOffset, rootRb);
            ReconfigureRope(ref ro, start, end);

            return ro;
        }

        public static void ReconfigureRope(ref RopeObject rope, Vector3 start, Vector3 end)
        {
            Vector3 tempEnd = end;
            end = start;
            start = tempEnd;

            Vector3 diff = end - start;
            Vector3 dir = diff.normalized;

            int segmentCount = rope.segments.Length;
            float dst = diff.magnitude;
            float segmentOffset = rope.segmentOffset;

            float totalOffset = segmentOffset * segmentCount;
            float yScale = (dst - totalOffset) / segmentCount;

            float segmentHeight = dst / segmentCount;

            Vector3 rootPos = start;
            rope.root.transform.position = rootPos;

            for (int i = 0; i < segmentCount; i++)
            {
                RopeSegment seg = rope.segments[i];
                seg.collider.transform.up = -dir;

                Vector2 scale = seg.collider.transform.localScale;
                scale.y = yScale;
                seg.collider.transform.localScale = scale;

                Vector3 ropePosition = start + (dir * segmentHeight * i);
                seg.collider.transform.position = ropePosition;
            }
            
            rootPos += dir * (segmentHeight / 2);
            rope.root.transform.position = rootPos;
        }
    }

    [System.Serializable]
    public class RopeParameteres
    {
        public Vector2 segmentDimensions = new Vector2(0.5f, 1f);
        public int segmentCount = 6;
        public float segmentOffset = 0f;

        public bool useAnglelimits = false;
        public Vector2 hingeAngleLimits;

        public float segmentMass = 1f;
        public float lastSegmentMass = 2f;

        public JointAngleLimits2D hingeLimits => new JointAngleLimits2D() { min = hingeAngleLimits.x, max = hingeAngleLimits.y };
    }

    [System.Serializable]
    public class RopeObject
    {
        public RopeObject(RopeSegment[] segments, float offset, Rigidbody2D root)
        {
            this.segments = segments;
            segmentOffset = offset;
            this.root = root;
        }

        public float segmentOffset { get; private set; }
        public Rigidbody2D root { get; private set; }
        public RopeSegment[] segments { get; private set; }
    }

    [System.Serializable]
    public class RopeSegment
    {
        public Rigidbody2D rb;
        public HingeJoint2D hinge;
        public BoxCollider2D collider;
        public SpriteRenderer sr;
    }

}