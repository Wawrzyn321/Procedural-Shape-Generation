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
        private float innerRadius;
        private float outerRadius;
        private int sides;

        //colliders
        private PolygonCollider2D C_PC2D;

        #region Static Methods - building from values and from structure

        public static RingMesh AddRing(Vector3 position, float innerRadius, float outerRadius, int sides, Material meshMatt = null, bool attachRigidbody = true)
        {
            GameObject ring = new GameObject();
            ring.transform.position = position;
            RingMesh ringComponent = ring.AddComponent<RingMesh>();
            ringComponent.Build(innerRadius, outerRadius, sides, meshMatt);
            if (attachRigidbody)
            {
                ring.AddComponent<Rigidbody2D>();
            }
            return ringComponent;
        }

        public static RingMesh AddRing(Vector3 position, RingStructure ringStructure, Material meshMatt = null, bool attachRigidbody = false)
        {
            return AddRing(position, ringStructure.innerRadius, ringStructure.outerRadius, ringStructure.sides, meshMatt, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(float innerRadius, float outerRadius, int sides, Material meshMatt = null)
        {
            name = "Ring";
            this.innerRadius = innerRadius;
            this.outerRadius = outerRadius;
            this.sides = sides;

            BuildMesh(ref meshMatt);
        }

        public void Build(RingStructure ringStructure, Material meshMatt = null)
        {
            Build(ringStructure.innerRadius, ringStructure.outerRadius, ringStructure.sides, meshMatt);
        }

        #endregion

        public RingStructure GetStructure()
        {
            return new RingStructure
            {
                innerRadius = innerRadius,
                outerRadius = outerRadius,
                sides = sides
            };
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (sides < 2)
            {
                Debug.LogWarning("RingMesh::ValidateMesh: sides count can't be less than two!");
                return false;
            }
            if (innerRadius == 0 && outerRadius == 0)
            {
                Debug.LogWarning("RingMesh::ValidateMesh: radius can't be equal to zero!");
                return false;
            }
            if (innerRadius < 0)
            {
                innerRadius = -innerRadius;
            }
            if (outerRadius < 0)
            {
                outerRadius = -outerRadius;
            }
            //swap radiuses if inner one is greater than outer
            if (innerRadius > outerRadius)
            {
                float tempRadius = innerRadius;
                innerRadius = outerRadius;
                outerRadius = tempRadius;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {
            bool isCircle = innerRadius == 0;

            Vertices = new Vector3[isCircle ? sides + 1 : 2 * sides];
            Triangles = new int[isCircle ? 3 * sides : 6 * sides];

            if (isCircle)
            {
                //build ordinary circle
                Vertices[0] = Vector3.zero;
                float angleDelta = deg360 / sides;
                for (int i = 1; i < sides + 1; i++)
                {
                    Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * outerRadius;
                    Vertices[i] = vertPos;
                    Triangles[(i - 1) * 3 + 0] = 0;
                    Triangles[(i - 1) * 3 + 1] = 1 + (i - 1) % sides;
                    Triangles[(i - 1) * 3 + 2] = 1 + i % sides;
                }
            }
            else
            {
                //build a ring!
                float angleDelta = deg360 / sides;
                int triangleIndex = 0;
                for (int i = 0; i < sides; i++)
                {
                    Vector3 vertPosInner = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * innerRadius;
                    Vector3 vertPosOuter = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * outerRadius;
                    Vertices[i] = vertPosInner;
                    Vertices[i + sides] = vertPosOuter;
                    Triangles[triangleIndex++] = i;
                    Triangles[triangleIndex++] = (i + 1) % (sides);
                    Triangles[triangleIndex++] = (i + sides);
                    Triangles[triangleIndex++] = (i + 1) % (sides * 2);
                    Triangles[triangleIndex++] = (i + sides);
                    Triangles[triangleIndex++] = (i + sides + 1) % (sides * 2);
                }
            }
            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        public override void UpdateCollider()
        {
            bool isHollow = innerRadius > 0;
            int numberOfPoints = isHollow ? (sides+1) * 2 : sides;
            Vector2[] colliderPoints = new Vector2[numberOfPoints];

            float angleDelta = deg360 / sides;
            colliderPoints[0] = new Vector2(Mathf.Cos(angleDelta), Mathf.Sin(angleDelta)) * outerRadius;
            for (int i = 0; i < sides; i++)
            {
                colliderPoints[i] = new Vector2(Mathf.Cos((i + 1) * angleDelta), Mathf.Sin((i + 1) * angleDelta)) * outerRadius;
            }

            if (isHollow)
            {
                colliderPoints[sides] = colliderPoints[0];
                for (int i = 0; i < sides; i++)
                {
                    colliderPoints[i + sides+1] = new Vector2(Mathf.Cos((i + 1) * angleDelta), Mathf.Sin((i + 1) * angleDelta)) * innerRadius;
                }
                colliderPoints[numberOfPoints - 1] = colliderPoints[sides+1];
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
        public float innerRadius;
        public float outerRadius;
        public int sides;
    }
}