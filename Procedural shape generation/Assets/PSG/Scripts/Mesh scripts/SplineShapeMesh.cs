using System.Collections.Generic;
using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Similar to TriangulatedMesh, created upon the spline points.
    /// 
    /// Colliders:
    ///     - Polygon
    /// </summary>
    public class SplineShapeMesh : TriangulableMesh
    {

        //collider
        public PolygonCollider2D C_PC2D { get; protected set; }

        //spline data
        public Vector2[] SplinePoints { get; protected set; }
        public float Resolution { get; protected set; }
        // used for simplification
        public float? MinArea { get; protected set; }

        #region Static Building

        public static SplineShapeMesh AddSplineShape(Vector3 position, Vector2[] splinePoints, float resolution = 0.2f, float? minArea = null, Space space = Space.World, Material meshMat = null, bool attachRigidbody = true)
        {
            GameObject splineShapeMesh = new GameObject();

            if (space == Space.Self)
            {
                splineShapeMesh.transform.position = position;
            }
            else
            {
                splineShapeMesh.transform.position = position + (Vector3)MeshHelper.GetCenter(splinePoints);
            }


            SplineShapeMesh splineMeshComponent = splineShapeMesh.AddComponent<SplineShapeMesh>();
            splineMeshComponent.Build(splinePoints, resolution, minArea, meshMat);
            if (attachRigidbody)
            {
                splineShapeMesh.AddComponent<Rigidbody2D>();
            }
            return splineMeshComponent;
        }

        public static SplineShapeMesh AddSplineShape(Vector3 position, Vector2[] splinePoints, float resolution = 0.2f, Space space = Space.World, Material meshMat = null, bool attachRigidbody = true)
        {
            return AddSplineShape(position, splinePoints, resolution, null, space, meshMat, attachRigidbody);
        }

        #endregion

        public void Build(Vector2[] splinePoints, float resolution, float? minArea, Material meshMat)
        {
            name = "Spline mesh";
            SplinePoints = splinePoints;
            Resolution = resolution;
            MinArea = minArea;

            BuildMesh(ref meshMat);
        }

        public void Build(Vector2[] splinePoints, float resolution, Material meshMat)
        {
            Build(splinePoints, resolution, null, meshMat);
        }

        public SplineShapeStructure GetStructure()
        {
            return new SplineShapeStructure
            {
                SplinePoints = SplinePoints,
                Resolution = Resolution,
                MinArea = MinArea,
                Vertices = Vertices
            };
        }

        public RawSplineShapeStructure GetRawStructure()
        {
            return new RawSplineShapeStructure()
            {
                SplinePoints = SplinePoints,
                Resolution = Resolution,
                MinArea = MinArea,
            };
        }

        #region Abstract Implementation

        protected override void BuildMeshComponents()
        {
            Vector2 center = MeshHelper.GetCenter(SplinePoints);
            for (int i = 0; i < SplinePoints.Length; i++)
            {
                SplinePoints[i] -= center;
            }

            var points = CatmullRomSpline.GetPoints(MeshHelper.ConvertVec2ToVec3(SplinePoints), Resolution);
            if (MinArea.HasValue)
            {
                points = SplineSimplification.Simplify(points, MinArea.Value, true, false);
            }
            Vertices = points.ToArray();

            var connections = Triangulation.TriangulationToInt3(new List<Vector2>(MeshHelper.ConvertVec3ToVec2(Vertices)));

            Triangles = new int[connections.Count * 3];
            for (int i = 0; i < connections.Count; i++)
            {
                Triangles[i * 3 + 0] = connections[i].A;
                Triangles[i * 3 + 1] = connections[i].B;
                Triangles[i * 3 + 2] = connections[i].C;
            }

            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        protected override bool ValidateMesh()
        {
            if (MeshHelper.HasDuplicates(SplinePoints))
            {
                Debug.LogWarning("SplineShapeMesh::ValidateMesh: Duplicate points detected!");
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
    public struct RawSplineShapeStructure
    {
        public Vector2[] SplinePoints;
        public float Resolution;
        public float? MinArea;
    }

    [System.Serializable]
    public struct SplineShapeStructure
    {
        public Vector2[] SplinePoints;
        public float Resolution;
        public float? MinArea;
        public Vector3[] Vertices;
    }
}
