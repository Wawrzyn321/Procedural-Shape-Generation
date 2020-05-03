using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Smooth, convex shape created upon convex hull 
    /// of set of points
    /// 
    /// Colliders:
    ///     - Polygon
    /// </summary>
    public class ConvexSplineMesh : TriangulableMesh
    {
        //collider
        private PolygonCollider2D C_PC2D;

        //spline data
        private Vector2[] basePoints;
        private Vector2[] splinePoints;
        private float resolution;

        public Vector3 CenterShift { get; set; }

        #region Static Building

        public static ConvexSplineMesh AddConvexSpline(Vector3 position, Vector2[] basePoints, float resolution = 0.2f, Space space = Space.World, Material meshMatt = null, bool attachRigidbody = true)
        {
            GameObject splineShapeMesh = new GameObject();

            ConvexSplineMesh convexSplineMeshComponent = splineShapeMesh.AddComponent<ConvexSplineMesh>();
            convexSplineMeshComponent.Build(basePoints, resolution, meshMatt);
            if (attachRigidbody)
            {
                splineShapeMesh.AddComponent<Rigidbody2D>();
            }

            if (space == Space.Self)
            {
                splineShapeMesh.transform.position = position;
            }
            else
            {
                splineShapeMesh.transform.position = position + convexSplineMeshComponent.CenterShift;
            }

            return convexSplineMeshComponent;
        }

        #endregion

        public void Build(Vector2[] basePoints, float resolution, Material meshMatt)
        {
            name = "Convex spline mesh";
            this.basePoints = basePoints;
            this.resolution = resolution;
            splinePoints = ConvexHull.QuickHull(basePoints).ToArray(); // oh no

            BuildMesh(ref meshMatt);
        }

        public ConvexSplineStructure GetStructure()
        {
            return new ConvexSplineStructure
            {
                splinePoints = splinePoints,
                resolution = resolution,
                vertices = Vertices
            };
        }

        public RawConvexSplineStructure GetRawStructure()
        {
            return new RawConvexSplineStructure()
            {
                splinePoints = splinePoints,
                resolution = resolution
            };
        }

        #region Abstract Implementation

        protected override void BuildMeshComponents()
        {
            Vertices = MeshHelper.ConvertVec2ToVec3(CatmullRomSpline.GetPoints(splinePoints, resolution).ToArray());

            Vertices = CatmullRomSpline.DecimatePoints(Vertices);

            CenterShift = new Vector3();
            for (int i = 0; i < Vertices.Length; i++)
            {
                CenterShift += Vertices[i];
            }
            CenterShift /= Vertices.Length;
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i] -= CenterShift;
            }

            Triangles = new int[Vertices.Length * 3];
            for (int i = 0; i < Vertices.Length; i++)
            {
                Triangles[i * 3 + 0] = 0;
                Triangles[i * 3 + 1] = i;
                Triangles[i * 3 + 2] = (i + 1) % Vertices.Length;
            }

            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        protected override bool ValidateMesh()
        {
            if (MeshHelper.HasDuplicates(basePoints))
            {
                Debug.LogWarning("SplineConvexShapeMesh::ValidateMesh: Duplicate points detected!");
                return false;
            }
            if (resolution < CatmullRomSpline.MIN_RESOLUTION)
            {
                resolution = CatmullRomSpline.MIN_RESOLUTION;
            }
            return true;
        }

        public override void GetOrAddComponents()
        {
            C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        public override void UpdateCollider()
        {
            C_PC2D.points = MeshHelper.ConvertVec3ToVec2(Vertices);
        }

        #endregion
    }

    [System.Serializable]
    public struct RawConvexSplineStructure
    {
        public Vector2[] splinePoints;
        public float resolution;
    }

    [System.Serializable]
    public struct ConvexSplineStructure
    {
        public Vector2[] splinePoints;
        public float resolution;
        public Vector3[] vertices;
    }
}
