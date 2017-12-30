using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Circle with peak.
    /// If peak is within the circle radius, it
    /// degenerates to plain circle.
    /// 
    /// Colliders:
    ///     - Circle and Polygon (if {shift} exceeds circle)
    ///     - Circle (if shape is degenerated)
    /// </summary>
    public class PointedCircleMesh : MeshBase
    {
        //p-circle data
        private float radius;
        private Vector2 shift;
        private int sides;

        //colliders
        private CircleCollider2D C_CC2D;
        private PolygonCollider2D C_PC2D;

        #region Static Methods - building from values and from structure

        public static PointedCircleMesh AddPointedCircle(Vector3 position, float radius, int sides, Vector2 shift, Material meshMatt = null, bool attachRigidbody = true)
        {
            GameObject pointedCircle = new GameObject();
            pointedCircle.transform.position = position;
            PointedCircleMesh pointedCircleComponent = pointedCircle.AddComponent<PointedCircleMesh>();
            pointedCircleComponent.Build(radius, sides, shift, meshMatt);
            if (attachRigidbody)
            {
                pointedCircle.AddComponent<Rigidbody2D>();
            }
            return pointedCircleComponent;
        }

        public static PointedCircleMesh AddPointedCircle(Vector3 position, PointedCircleStructure pointedCircleStructure, Material meshMatt = null, bool attachRigidbody = true)
        {
            return AddPointedCircle(position, pointedCircleStructure.radius, pointedCircleStructure.sides, pointedCircleStructure.shift, meshMatt, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(float radius, int sides, Vector2 shift, Material meshMatt = null)
        {
            name = "PointedCircle";
            this.radius = radius;
            this.sides = sides;
            this.shift = shift;

            BuildMesh(ref meshMatt);
        }

        void Build(PointedCircleStructure pointedCircleStructure, Material meshMatt = null)
        {
            Build(pointedCircleStructure.radius, pointedCircleStructure.sides, pointedCircleStructure.shift, meshMatt);
        }

        #endregion

        public PointedCircleStructure GetStructure()
        {
            return new PointedCircleStructure
            {
                radius = radius,
                shift = shift,
                sides = sides
            };
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (sides < 2)
            {
                Debug.LogWarning("PointedCircleMesh::ValidateMesh: radius can't be equal to zero!");
                return false;
            }
            if (radius == 0)
            {
                Debug.LogWarning("PointedCircleMesh::ValidateMesh: radius can't be equal to zero!");
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

            float angleDelta = deg360 / sides;
            Vertices[0] = shift;
            for (int i = 1; i < sides + 1; i++)
            {
                Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * radius;
                Vertices[i] = vertPos;
                Triangles[(i - 1) * 3 + 0] = (1 + i % sides);
                Triangles[(i - 1) * 3 + 1] = 1 + (i - 1) % sides;
                Triangles[(i - 1) * 3 + 2] = 0;
            }
            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        public override void UpdateCollider()
        {
            C_CC2D.radius = radius;

            if (radius < Vertices[0].sqrMagnitude)
            {
                //not added in AddOrGetComponents
                C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();

                Vector2[] C_CC2D_vertices = new Vector2[3];

                float shiftedVertexAngle = Mathf.Atan2(Vertices[0].y, Vertices[0].x);

                C_CC2D_vertices[0] = Vertices[0];
                C_CC2D_vertices[1] = new Vector2(Mathf.Cos(shiftedVertexAngle - Mathf.PI * 0.5f), Mathf.Sin(shiftedVertexAngle - Mathf.PI * 0.5f)) * radius;
                C_CC2D_vertices[2] = new Vector2(Mathf.Cos(shiftedVertexAngle + Mathf.PI * 0.5f), Mathf.Sin(shiftedVertexAngle + Mathf.PI * 0.5f)) * radius;

                C_PC2D.SetPath(0, C_CC2D_vertices);
            }
        }

        public override void GetOrAddComponents()
        {
            C_CC2D = gameObject.GetOrAddComponent<CircleCollider2D>();
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        public override void SetCollidersEnabled(bool enable)
        {
            Collider2D[] cs = GetComponents<Collider2D>();
            foreach (Collider2D c in cs)
            {
                c.enabled = enable;
            }
        }

        #endregion

    } 

    public struct PointedCircleStructure
    {
        public float radius;
        public Vector2 shift;
        public int sides;
    }
}