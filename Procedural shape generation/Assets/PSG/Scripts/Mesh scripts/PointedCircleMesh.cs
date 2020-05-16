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
        public float Radius { get; protected set; }
        public Vector2 Shift { get; protected set; }
        public int Sides { get; protected set; }

        //colliders
        public CircleCollider2D C_CC2D { get; protected set; }
        public PolygonCollider2D C_PC2D { get; protected set; }

        #region Static Methods - building from values and from structure

        public static PointedCircleMesh AddPointedCircle(Vector3 position, float radius, int sides, Vector2 shift, Material meshMat = null, bool attachRigidbody = true)
        {
            GameObject pointedCircle = new GameObject();
            pointedCircle.transform.position = position;
            PointedCircleMesh pointedCircleComponent = pointedCircle.AddComponent<PointedCircleMesh>();
            pointedCircleComponent.Build(radius, sides, shift, meshMat);
            if (attachRigidbody)
            {
                pointedCircle.AddComponent<Rigidbody2D>();
            }
            return pointedCircleComponent;
        }

        public static PointedCircleMesh AddPointedCircle(Vector3 position, PointedCircleStructure pointedCircleStructure, Material meshMat = null, bool attachRigidbody = true)
        {
            return AddPointedCircle(position, pointedCircleStructure.Radius, pointedCircleStructure.Sides, pointedCircleStructure.Shift, meshMat, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(float radius, int sides, Vector2 shift, Material meshMat = null)
        {
            name = "PointedCircle";
            Radius = radius;
            Sides = sides;
            Shift = shift;

            BuildMesh(ref meshMat);
        }

        void Build(PointedCircleStructure pointedCircleStructure, Material meshMat = null)
        {
            Build(pointedCircleStructure.Radius, pointedCircleStructure.Sides, pointedCircleStructure.Shift, meshMat);
        }

        #endregion

        public PointedCircleStructure GetStructure()
        {
            return new PointedCircleStructure
            {
                Radius = Radius,
                Shift = Shift,
                Sides = Sides
            };
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (Sides < 2)
            {
                Debug.LogWarning("PointedCircleMesh::ValidateMesh: radius can't be equal to zero!");
                return false;
            }
            if (Radius == 0)
            {
                Debug.LogWarning("PointedCircleMesh::ValidateMesh: radius can't be equal to zero!");
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

            float angleDelta = deg360 / Sides;
            Vertices[0] = Shift;
            for (int i = 1; i < Sides + 1; i++)
            {
                Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * Radius;
                Vertices[i] = vertPos;
                Triangles[(i - 1) * 3 + 0] = (1 + i % Sides);
                Triangles[(i - 1) * 3 + 1] = 1 + (i - 1) % Sides;
                Triangles[(i - 1) * 3 + 2] = 0;
            }
            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        public override void UpdateCollider()
        {
            C_CC2D.radius = Radius;

            if (Radius < Vertices[0].sqrMagnitude)
            {
                //not added in AddOrGetComponents
                C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();

                Vector2[] C_CC2D_vertices = new Vector2[3];

                float shiftedVertexAngle = Mathf.Atan2(Vertices[0].y, Vertices[0].x);

                C_CC2D_vertices[0] = Vertices[0];
                C_CC2D_vertices[1] = new Vector2(Mathf.Cos(shiftedVertexAngle - Mathf.PI * 0.5f), Mathf.Sin(shiftedVertexAngle - Mathf.PI * 0.5f)) * Radius;
                C_CC2D_vertices[2] = new Vector2(Mathf.Cos(shiftedVertexAngle + Mathf.PI * 0.5f), Mathf.Sin(shiftedVertexAngle + Mathf.PI * 0.5f)) * Radius;

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
        public float Radius;
        public Vector2 Shift;
        public int Sides;
    }
}