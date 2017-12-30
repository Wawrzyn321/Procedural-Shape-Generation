using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Simple circle for PSG.
    /// 
    /// Colliders:
    ///     - Circle or Polygon
    /// 
    /// Use CircleCollider to improve performance:
    /// PolygonCollider is advised when sides are
    /// less than 8.
    /// 
    /// </summary>

    public class CircleMesh : MeshBase
    {
        //circle data
        private float radius;
        private int sides;

        //collider
        private bool useCircleCollider;
        private Collider2D C_C2D;

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
            return AddCircle(position, circleStructure.radius, circleStructure.sides, circleStructure.useCircleCollider, meshMatt, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(float radius, int sides, bool useCircleCollider, Material meshMatt = null)
        {
            name = "Circle";
            this.radius = radius;
            this.sides = sides;
            this.useCircleCollider = useCircleCollider;

            BuildMesh(ref meshMatt);
        }

        public void Build(CircleStructure circleStructure, Material meshMatt = null)
        {
            Build(circleStructure.radius, circleStructure.sides, circleStructure.useCircleCollider, meshMatt);
        }

        #endregion

        public CircleStructure GetStructure()
        {
            return new CircleStructure
            {
                radius = radius,
                sides = sides,
                useCircleCollider = useCircleCollider
            };
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (sides < 2)
            {
                Debug.LogWarning("CircleMesh::ValidateMesh: sides count can't be less than two!");
                return false;
            }
            if (radius == 0)
            {
                Debug.LogWarning("CircleMesh::ValidateMesh: radius can't be equal to zero!");
                return false;
            }
            if (radius < 0)
            {
                radius = -radius;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {
            Vertices = new Vector3[sides + 1];
            Triangles = new int[3 * sides];
            UVs = new Vector2[sides + 1];

            Vertices[0] = Vector3.zero;
            //uvs are manually unwrapped here
            UVs[0] = new Vector2(0.5f, 0.5f);
            float angleDelta = deg360 / sides;
            for (int i = 1; i < sides + 1; i++)
            {
                Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * radius;
                Vertices[i] = vertPos;
                UVs[i] = vertPos / 2 / radius + new Vector3(0.5f, 0.5f, 0);
                Triangles[(i - 1) * 3 + 0] = (1 + i % sides);
                Triangles[(i - 1) * 3 + 1] = 1 + (i - 1) % sides;
                Triangles[(i - 1) * 3 + 2] = 0;
            }
        }

        public override void UpdateCollider()
        {
            if (useCircleCollider)
            {
                ((CircleCollider2D)C_C2D).radius = radius;
            }

            else
            {
                ((PolygonCollider2D)C_C2D).SetPath(0, MeshHelper.ConvertVec3ToVec2(Vertices));
            }
        }

        public override void GetOrAddComponents()
        {
            if (useCircleCollider)
            {
                C_C2D = gameObject.GetOrAddComponent<CircleCollider2D>();
            }
            else
            {
                C_C2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            }
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        #endregion

    }

    public struct CircleStructure{
        public float radius;
        public int sides;
        public bool useCircleCollider;
    }
}