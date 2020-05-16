using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Simple ellipse shape.
    /// If both radiuses are equal, we consider it as a circle.
    /// 
    /// Colliders:
    ///     - Polygon
    /// </summary>
    public class EllipseMesh : MeshBase
    {
        //ellipse data
        public float RadiusHorizontal { get; protected set; }
        public float RadiusVertical { get; protected set; }
        public int Sides { get; protected set; }

        //collider
        public PolygonCollider2D C_PC2D { get; protected set; }

        #region Static Methods - building from values and from structure
        
        public static EllipseMesh AddEllipse(Vector3 position, float radiusHorizontal, float radiusVertical, int sides, Material meshMat = null, bool attachRigidbody = true)
        {
            GameObject ellipse = new GameObject();
            ellipse.transform.position = position;
            EllipseMesh ellipseComponent = ellipse.AddComponent<EllipseMesh>();
            ellipseComponent.Build(radiusHorizontal, radiusVertical, sides, meshMat);
            if (attachRigidbody)
            {
                ellipse.AddComponent<Rigidbody2D>();
            }
            return ellipseComponent;
        }

        public static EllipseMesh AddEllipse(Vector3 position, EllipseStructure ellipseStructure, Material meshMat = null, bool attachRigidbody = true)
        {
            return AddEllipse(position, ellipseStructure.RadiusHorizontal, ellipseStructure.RadiusVertical, ellipseStructure.Sides, meshMat, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(float radiusHorizontal, float radiusVertical, int sides, Material meshMat = null)
        {
            name = "Ellipse";
            RadiusHorizontal = radiusHorizontal;
            RadiusVertical = radiusVertical;
            Sides = sides;

            BuildMesh(ref meshMat);
        }

        public void Build(EllipseStructure ellipseStructure, Material meshMat = null)
        {
            Build(ellipseStructure.RadiusHorizontal, ellipseStructure.RadiusVertical, ellipseStructure.Sides, meshMat);
        }

        #endregion

        public EllipseStructure GetStructure()
        {
            return new EllipseStructure
            {
                RadiusHorizontal = RadiusHorizontal,
                RadiusVertical = RadiusVertical,
                Sides = Sides
            };
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (Sides < 2)
            {
                Debug.LogWarning("EllipseMesh::ValidateMesh: sides count can't be less than two!");
                return false;
            }
            if (RadiusHorizontal == 0 || RadiusVertical == 0)
            {
                Debug.LogWarning("EllipseMesh::ValidateMesh: radiuses can't be equal to zero!");
                return false;
            }
            if (RadiusHorizontal < 0)
            {
                RadiusHorizontal = -RadiusHorizontal;
            }
            if (RadiusVertical < 0)
            {
                RadiusVertical = -RadiusVertical;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {
            Vertices = new Vector3[Sides + 1];
            Triangles = new int[3 * Sides];
            UVs = new Vector2[Sides + 1];

            Vertices[0] = Vector3.zero;
            UVs[0] = Vector3.one * 0.5f;
            float angleDelta = deg360 / Sides;
            for (int i = 1; i < Sides + 1; i++)
            {
                Vector3 vertPos = new Vector3(Mathf.Cos((i + 1) * angleDelta) * RadiusHorizontal, Mathf.Sin((i + 1) * angleDelta) * RadiusVertical);
                Vertices[i] = vertPos;
                UVs[i] = new Vector3(vertPos.x / 2 / RadiusHorizontal, vertPos.y / 2 / RadiusVertical) + new Vector3(0.5f, 0.5f, 0);
                Triangles[(i - 1) * 3 + 0] = 1 + i % Sides;
                Triangles[(i - 1) * 3 + 1] = 1 + (i - 1) % Sides;
                Triangles[(i - 1) * 3 + 2] = 0;
            }
        }

        public override void UpdateCollider()
        {
            Vector2[] points = new Vector2[Sides];
            float angleDelta = deg360 / Sides;
            for (int i = 0; i < Sides; i++)
            {
                points[i] = new Vector3(Mathf.Cos((i + 1) * angleDelta) * RadiusHorizontal, Mathf.Sin((i + 1) * angleDelta) * RadiusVertical);
            }
            C_PC2D.points = points;
        }

        public override void GetOrAddComponents()
        {
            C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        #endregion
    }

    public struct EllipseStructure
    {
        public float RadiusHorizontal;
        public float RadiusVertical;
        public int Sides;
    }
}