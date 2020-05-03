using UnityEngine;
using System.Collections.Generic;

namespace PSG
{
    /// <summary>
    /// Arbitrary mesh without intersecting edges, created from a set of
    /// points or list of triangles
    /// 
    /// Colliders:
    ///     - Polygon
    /// </summary>
    public class TriangulatedMesh : TriangulableMesh
    {

        //mesh data
        private Vector2[] points;
        private List<Triangulation.IntTriple> connections;

        //collider
        private PolygonCollider2D C_PC2D;

        #region Static Building

        public static TriangulatedMesh Add(Vector3 position, Vector2[] points, List<Triangulation.IntTriple> connections, Material meshMatt = null, bool attachRigidbody = true)
        {
            GameObject triangulatedMesh = new GameObject();
            Vector2 center = new Vector2();
            for (int i = 0; i < points.Length; i++)
            {
                center += points[i];
            }
            triangulatedMesh.transform.position = center / points.Length;

            TriangulatedMesh triComponent = triangulatedMesh.AddComponent<TriangulatedMesh>();
            triComponent.Build(points, connections, meshMatt);
            if (attachRigidbody)
            {
                triangulatedMesh.AddComponent<Rigidbody2D>();
            }
            return triComponent;
        }

        public static TriangulatedMesh Add(Vector3 position, Vector2[] points, Material meshMatt = null, bool attachRigidbody = true)
        {
            List<Triangulation.IntTriple> connections = Triangulation.TriangulationToInt3(new List<Vector2>(points));
            return Add(position, points, connections, meshMatt, attachRigidbody);
        }

        public static TriangulatedMesh Add(Vector3 position, TriangulatedMeshStructure structure, Material meshMatt = null, bool attachRigidbody = true)
        {
            return Add(position, structure.points, structure.connections, meshMatt, attachRigidbody);
        }

        #endregion

        public void Build(Vector2[] points, List<Triangulation.IntTriple> connections, Material meshMatt)
        {
            name = "Triangulated mesh";
            this.points = points;
            this.connections = connections;

            BuildMesh(ref meshMatt);
        }

        public static List<Vector2> Smooth(List<Vector2> sourcePoints, float smoothness)
        {
            List<Vector2> subPoints = new List<Vector2>();

            for (int i = 0; i < sourcePoints.Count; i++)
            {
                //start, middle and end of segment
                Vector2 a = sourcePoints[i];
                Vector2 b = sourcePoints[(i + 1) % sourcePoints.Count];
                Vector2 c = sourcePoints[(i + 2) % sourcePoints.Count];

                //midpoints of segments
                Vector2 mid1 = Vector2.Lerp(a, b, 0.5f);
                Vector2 mid2 = Vector2.Lerp(b, c, 0.5f);
                //mid-point of mid-points
                Vector2 mid_mid = Vector2.Lerp(mid1, mid2, 0.5f);

                Vector2 diff = Vector2.LerpUnclamped(mid_mid, b, smoothness);

                subPoints.Add(mid1);
                subPoints.Add(diff);

            }
            return subPoints;
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (MeshHelper.HasDuplicates(points))
            {
                Debug.LogWarning("TriangulatedMesh::ValidateMesh: Duplicate points detected!");
                return false;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {
            Vector2 center = new Vector2();
            for (int i = 0; i < points.Length; i++)
            {
                center += points[i];
            }
            center /= points.Length;
            Vertices = new Vector3[points.Length];
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i] = points[i] - center;
            }

            Triangles = new int[connections.Count * 3];
            for (int i = 0; i < connections.Count; i++)
            {
                Triangles[i * 3 + 0] = connections[i].a;
                Triangles[i * 3 + 1] = connections[i].b;
                Triangles[i * 3 + 2] = connections[i].c;
            }
            UVs = MeshHelper.UVUnwrap(Vertices);
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

    public struct TriangulatedMeshStructure
    {
        public Vector2[] points;
        public List<Triangulation.IntTriple> connections;
    }

}
