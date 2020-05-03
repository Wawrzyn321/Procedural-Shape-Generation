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
        private PolygonCollider2D C_PC2D;

        //spline data
        private Vector2[] splinePoints;
        private float resolution;

        #region Static Building

        public static SplineShapeMesh AddSplineShape(Vector3 position, Vector2[] splinePoints, float resolution = 0.2f, Space space = Space.World, Material meshMatt = null, bool attachRigidbody = true)
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
            splineMeshComponent.Build(splinePoints, resolution, meshMatt);
            if (attachRigidbody)
            {
                splineShapeMesh.AddComponent<Rigidbody2D>();
            }
            return splineMeshComponent;
        }

        #endregion

        public void Build(Vector2[] splinePoints, float resolution, Material meshMatt)
        {
            name = "Spline mesh";
            this.splinePoints = splinePoints;
            this.resolution = resolution;

            BuildMesh(ref meshMatt);
        }

        public SplineShapeStructure GetStructure()
        {
            return new SplineShapeStructure
            {
                splinePoints = splinePoints,
                resolution = resolution,
                vertices = Vertices
            };
        }

        public RawSplineShapeStructure GetRawStructure()
        {
            return new RawSplineShapeStructure()
            {
                splinePoints = splinePoints,
                resolution = resolution
            };
        }

        #region Abstract Implementation

        protected override void BuildMeshComponents()
        {
            Vector2 center = new Vector2();
            for (int i = 0; i < splinePoints.Length; i++)
            {
                center += splinePoints[i];
            }
            center /= splinePoints.Length;
            for (int i = 0; i < splinePoints.Length; i++)
            {
                splinePoints[i] -= center;
            }

            Vertices = CatmullRomSpline.GetPoints(MeshHelper.ConvertVec2ToVec3(splinePoints), resolution).ToArray();

            var connections = Triangulation.TriangulationToInt3(new List<Vector2>(MeshHelper.ConvertVec3ToVec2(Vertices)));

            Triangles = new int[connections.Count * 3];
            for (int i = 0; i < connections.Count; i++)
            {
                Triangles[i * 3 + 0] = connections[i].a;
                Triangles[i * 3 + 1] = connections[i].b;
                Triangles[i * 3 + 2] = connections[i].c;
            }

            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        protected override bool ValidateMesh()
        {
            if (MeshHelper.HasDuplicates(splinePoints))
            {
                Debug.LogWarning("SplineShapeMesh::ValidateMesh: Duplicate points detected!");
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
    public struct RawSplineShapeStructure
    {
        public Vector2[] splinePoints;
        public float resolution;
    }

    [System.Serializable]
    public struct SplineShapeStructure
    {
        public Vector2[] splinePoints;
        public float resolution;
        public Vector3[] vertices;
    }
}
