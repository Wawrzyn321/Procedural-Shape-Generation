using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Simple circle for PSG.
    /// 
    /// Colliders:
    ///     - Circle or Polygon
    /// Use CircleCollider to improve performance.
    /// 
    /// </summary>

    public class CircleMesh : MeshBase
    {
        //circle data
        public float Radius { get; protected set; }
        public int Sides { get; protected set; }

        //collider
        public bool UseCircleCollider { get; protected set; }
        public CircleCollider2D C_CC2D { get; protected set; }
        public PolygonCollider2D C_PC2D { get; protected set; }
        public Collider2D Collider
        {
            get
            {
                if (C_CC2D != null)
                {
                    return C_CC2D;
                }
                return C_PC2D;
            }
        }

        #region Static Methods - building from values and from structure

        public static CircleMesh AddCircle(Vector3 position, float radius, int sides, bool useCircleCollider, Material meshMatt = null, bool attachRigidbody = true)
        {
            GameObject circle = new GameObject();
            circle.transform.position = position;
            CircleMesh circleComponent = circle.AddComponent<CircleMesh>();
            circleComponent.Build(radius, sides, useCircleCollider, meshMatt);
            if (attachRigidbody)
            {
                circle.AddComponent<Rigidbody2D>();
            }
            return circleComponent;
        }

        public static CircleMesh AddCircle(Vector3 position, CircleStructure circleStructure, Material meshMatt = null, bool attachRigidbody = true)
        {
            return AddCircle(position, circleStructure.Radius, circleStructure.Sides, circleStructure.UseCircleCollider, meshMatt, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(float radius, int sides, bool useCircleCollider, Material meshMatt = null)
        {
            name = "Circle";
            Radius = radius;
            Sides = sides;
            UseCircleCollider = useCircleCollider;

            BuildMesh(ref meshMatt);
        }

        public void Build(CircleStructure circleStructure, Material meshMatt = null)
        {
            Build(circleStructure.Radius, circleStructure.Sides, circleStructure.UseCircleCollider, meshMatt);
        }

        #endregion

        public CircleStructure GetStructure()
        {
            return new CircleStructure
            {
                Radius = Radius,
                Sides = Sides,
                UseCircleCollider = UseCircleCollider
            };
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (Sides < 2)
            {
                Debug.LogWarning("CircleMesh::ValidateMesh: sides count can't be less than two!");
                return false;
            }
            if (Radius == 0)
            {
                Debug.LogWarning("CircleMesh::ValidateMesh: radius can't be equal to zero!");
                return false;
            }
            if (Radius < 0)
            {
                Radius = -Radius;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {
            Vertices = new Vector3[Sides + 1];
            Triangles = new int[3 * Sides];
            UVs = new Vector2[Sides + 1];

            Vertices[0] = Vector3.zero;
            //uvs are manually unwrapped here
            UVs[0] = new Vector2(0.5f, 0.5f);
            float angleDelta = deg360 / Sides;
            for (int i = 1; i < Sides + 1; i++)
            {
                Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * Radius;
                Vertices[i] = vertPos;
                UVs[i] = vertPos / 2 / Radius + new Vector3(0.5f, 0.5f, 0);
                Triangles[(i - 1) * 3 + 0] = (1 + i % Sides);
                Triangles[(i - 1) * 3 + 1] = 1 + (i - 1) % Sides;
                Triangles[(i - 1) * 3 + 2] = 0;
            }
        }

        public override void UpdateCollider()
        {
            if (UseCircleCollider)
            {
                C_CC2D.radius = Radius;
            }

            else
            {
                C_PC2D.SetPath(0, MeshHelper.ConvertVec3ToVec2(Vertices));
            }
        }

        public override void GetOrAddComponents()
        {
            if (UseCircleCollider)
            {
                C_CC2D = gameObject.GetOrAddComponent<CircleCollider2D>();
            }
            else
            {
                C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            }
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        #endregion

    }

    public struct CircleStructure
    {
        public float Radius;
        public int Sides;
        public bool UseCircleCollider;
    }
}