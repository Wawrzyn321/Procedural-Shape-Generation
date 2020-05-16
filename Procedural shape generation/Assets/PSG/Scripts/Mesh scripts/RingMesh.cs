using UnityEngine;

namespace PSG
{
    /// <summary>
    /// A ring. If both radiuses are equal, it
    /// degenerates to circle.
    /// 
    /// Colliders:
    ///     - Polygon
    /// </summary>
    public class RingMesh : MeshBase
    {
        //ring data
        public float InnerRadius { get; protected set; }
        public float OuterRadius { get; protected set; }
        public int Sides { get; protected set; }

        //colliders
        public PolygonCollider2D C_PC2D { get; protected set; }

        #region Static Methods - building from values and from structure

        public static RingMesh AddRing(Vector3 position, float innerRadius, float outerRadius, int sides, Material meshMat = null, bool attachRigidbody = true)
        {
            GameObject ring = new GameObject();
            ring.transform.position = position;
            RingMesh ringComponent = ring.AddComponent<RingMesh>();
            ringComponent.Build(innerRadius, outerRadius, sides, meshMat);
            if (attachRigidbody)
            {
                ring.AddComponent<Rigidbody2D>();
            }
            return ringComponent;
        }

        public static RingMesh AddRing(Vector3 position, RingStructure ringStructure, Material meshMat = null, bool attachRigidbody = false)
        {
            return AddRing(position, ringStructure.InnerRadius, ringStructure.OuterRadius, ringStructure.Sides, meshMat, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(float innerRadius, float outerRadius, int sides, Material meshMat = null)
        {
            name = "Ring";
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
            Sides = sides;

            BuildMesh(ref meshMat);
        }

        public void Build(RingStructure ringStructure, Material meshMat = null)
        {
            Build(ringStructure.InnerRadius, ringStructure.OuterRadius, ringStructure.Sides, meshMat);
        }

        #endregion

        public RingStructure GetStructure()
        {
            return new RingStructure
            {
                InnerRadius = InnerRadius,
                OuterRadius = OuterRadius,
                Sides = Sides
            };
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (Sides < 2)
            {
                Debug.LogWarning("RingMesh::ValidateMesh: sides count can't be less than two!");
                return false;
            }
            if (InnerRadius == 0 && OuterRadius == 0)
            {
                Debug.LogWarning("RingMesh::ValidateMesh: radius can't be equal to zero!");
                return false;
            }
            if (InnerRadius < 0)
            {
                InnerRadius = -InnerRadius;
            }
            if (OuterRadius < 0)
            {
                OuterRadius = -OuterRadius;
            }
            //swap radiuses if inner one is greater than outer
            if (InnerRadius > OuterRadius)
            {
                float tempRadius = InnerRadius;
                InnerRadius = OuterRadius;
                OuterRadius = tempRadius;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {
            bool isCircle = InnerRadius == 0;

            Vertices = new Vector3[isCircle ? Sides + 1 : 2 * Sides];
            Triangles = new int[isCircle ? 3 * Sides : 6 * Sides];

            if (isCircle)
            {
                //build ordinary circle
                Vertices[0] = Vector3.zero;
                float angleDelta = deg360 / Sides;
                for (int i = 1; i < Sides + 1; i++)
                {
                    Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * OuterRadius;
                    Vertices[i] = vertPos;
                    Triangles[(i - 1) * 3 + 0] = 0;
                    Triangles[(i - 1) * 3 + 1] = 1 + (i - 1) % Sides;
                    Triangles[(i - 1) * 3 + 2] = 1 + i % Sides;
                }
            }
            else
            {
                //build a ring!
                float angleDelta = deg360 / Sides;
                int triangleIndex = 0;
                for (int i = 0; i < Sides; i++)
                {
                    Vector3 vertPosInner = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * InnerRadius;
                    Vector3 vertPosOuter = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * OuterRadius;
                    Vertices[i] = vertPosInner;
                    Vertices[i + Sides] = vertPosOuter;
                    Triangles[triangleIndex++] = i;
                    Triangles[triangleIndex++] = (i + 1) % (Sides);
                    Triangles[triangleIndex++] = (i + Sides);
                    Triangles[triangleIndex++] = (i + 1) % (Sides * 2);
                    Triangles[triangleIndex++] = (i + Sides);
                    Triangles[triangleIndex++] = (i + Sides + 1) % (Sides * 2);
                }
            }
            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        public override void UpdateCollider()
        {
            bool isHollow = InnerRadius > 0;
            int numberOfPoints = isHollow ? (Sides+1) * 2 : Sides;
            Vector2[] colliderPoints = new Vector2[numberOfPoints];

            float angleDelta = deg360 / Sides;
            colliderPoints[0] = new Vector2(Mathf.Cos(angleDelta), Mathf.Sin(angleDelta)) * OuterRadius;
            for (int i = 0; i < Sides; i++)
            {
                colliderPoints[i] = new Vector2(Mathf.Cos((i + 1) * angleDelta), Mathf.Sin((i + 1) * angleDelta)) * OuterRadius;
            }

            if (isHollow)
            {
                colliderPoints[Sides] = colliderPoints[0];
                for (int i = 0; i < Sides; i++)
                {
                    colliderPoints[i + Sides+1] = new Vector2(Mathf.Cos((i + 1) * angleDelta), Mathf.Sin((i + 1) * angleDelta)) * InnerRadius;
                }
                colliderPoints[numberOfPoints - 1] = colliderPoints[Sides+1];
            }

            C_PC2D.points = colliderPoints;
        }

        public override void GetOrAddComponents()
        {
            C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        #endregion

    }

    [System.Serializable]
    public struct RingStructure
    {
        public float InnerRadius;
        public float OuterRadius;
        public int Sides;
    }
}