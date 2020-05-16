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
        //spline data
        public Vector2[] BasePoints { get; protected set; }
        public Vector2[] SplinePoints { get; protected set; }
        public float Resolution { get; protected set; }
        // used for simplification
        public float? MinArea { get; protected set; }

        //collider
        public PolygonCollider2D C_PC2D { get; protected set; }

        public Vector3 CenterShift { get; protected set; }

        #region Static Building

        public static ConvexSplineMesh AddConvexSpline(Vector3 position, Vector2[] basePoints, float resolution = 0.2f, float? minArea = null, Space space = Space.World, Material meshMat = null, bool attachRigidbody = true)
        {
            GameObject splineShapeMesh = new GameObject();

            ConvexSplineMesh convexSplineMeshComponent = splineShapeMesh.AddComponent<ConvexSplineMesh>();
            convexSplineMeshComponent.Build(basePoints, resolution, minArea, meshMat);
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

        public static ConvexSplineMesh AddConvexSpline(Vector3 position, Vector2[] basePoints, float resolution = 0.2f, Space space = Space.World, Material meshMat = null, bool attachRigidbody = true)
        {
            return AddConvexSpline(position, basePoints, resolution, null, space, meshMat, attachRigidbody);
        }

        #endregion

        public void Build(Vector2[] basePoints, float resolution, float? minArea, Material meshMat)
        {
            name = "Convex spline mesh";
            BasePoints = basePoints;
            Resolution = resolution;
            MinArea = minArea;
            SplinePoints = ConvexHull.QuickHull(basePoints).ToArray();

            BuildMesh(ref meshMat);
        }

        public void Build(Vector2[] basePoints, float resolution, Material meshMat)
        {
            Build(basePoints, resolution, null, meshMat);
        }

        public ConvexSplineStructure GetStructure()
        {
            return new ConvexSplineStructure
            {
                SplinePoints = SplinePoints,
                Resolution = Resolution,
                Vertices = Vertices
            };
        }

        public RawConvexSplineStructure GetRawStructure()
        {
            return new RawConvexSplineStructure()
            {
                SplinePoints = SplinePoints,
                Resolution = Resolution
            };
        }

        #region Abstract Implementation

        protected override void BuildMeshComponents()
        {
            var points = CatmullRomSpline.GetPoints(MeshHelper.ConvertVec2ToVec3(SplinePoints), Resolution);
            if (MinArea.HasValue)
            {
                points = SplineSimplification.Simplify(points, MinArea.Value, true, false);
            }
            Vertices = points.ToArray();

            CenterShift = MeshHelper.GetCenter(Vertices);
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
            if (MeshHelper.HasDuplicates(BasePoints))
            {
                Debug.LogWarning("SplineConvexShapeMesh::ValidateMesh: Duplicate points detected!");
                return false;
            }
            if (Resolution < CatmullRomSpline.MIN_RESOLUTION)
            {
                Resolution = CatmullRomSpline.MIN_RESOLUTION;
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
        public Vector2[] SplinePoints;
        public float Resolution;
        public float? MinArea;
    }

    [System.Serializable]
    public struct ConvexSplineStructure
    {
        public Vector2[] SplinePoints;
        public float Resolution;
        public float? MinArea;
        public Vector3[] Vertices;
    }
}
