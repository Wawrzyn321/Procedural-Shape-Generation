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

        #region Static Building

        public static SplineShapeMesh AddSplineShape(Vector3 position, Vector2[] splinePoints, float resolution = 0.2f, Space space = Space.World, Material meshMat = null, bool attachRigidbody = true)
        {
            GameObject splineShapeMesh = new GameObject();

            if (space == Space.Self)
            {
                splineShapeMesh.transform.position = position;
            }
            else
            {
                Vector2 center = new Vector2();
                for (int i = 0; i < splinePoints.Length; i++)
                {
                    center += splinePoints[i];
                }
                splineShapeMesh.transform.position = position + (Vector3)center / splinePoints.Length;
            }


            SplineShapeMesh splineMeshComponent = splineShapeMesh.AddComponent<SplineShapeMesh>();
            splineMeshComponent.Build(splinePoints, resolution, meshMat);
            if (attachRigidbody)
            {
                splineShapeMesh.AddComponent<Rigidbody2D>();
            }
            return splineMeshComponent;
        }

        #endregion

        public void Build(Vector2[] splinePoints, float resolution, Material meshMat)
        {
            name = "Spline mesh";
            SplinePoints = splinePoints;
            Resolution = resolution;

            BuildMesh(ref meshMat);
        }

        public SplineShapeStructure GetStructure()
        {
            return new SplineShapeStructure
            {
                SplinePoints = SplinePoints,
                Resolution = Resolution,
                Vertices = Vertices
            };
        }

        public RawSplineShapeStructure GetRawStructure()
        {
            return new RawSplineShapeStructure()
            {
                SplinePoints = SplinePoints,
                Resolution = Resolution
            };
        }

        #region Abstract Implementation

        protected override void BuildMeshComponents()
        {
            Vector2 center = new Vector2();
            for (int i = 0; i < SplinePoints.Length; i++)
            {
                center += SplinePoints[i];
            }
            center /= SplinePoints.Length;
            for (int i = 0; i < SplinePoints.Length; i++)
            {
                SplinePoints[i] -= center;
            }

            Vertices = CatmullRomSpline.GetPoints(MeshHelper.ConvertVec2ToVec3(SplinePoints), Resolution).ToArray();

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
    }

    [System.Serializable]
    public struct SplineShapeStructure
    {
        public Vector2[] SplinePoints;
        public float Resolution;
        public Vector3[] Vertices;
    }
}
