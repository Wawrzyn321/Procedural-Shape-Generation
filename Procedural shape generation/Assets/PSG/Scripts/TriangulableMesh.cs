using System.Collections.Generic;
using UnityEngine;

namespace PSG {

    public static class TriangulableMeshProxy
    {
        private static readonly System.Type triangulatedMeshType = typeof(TriangulatedMesh);

        public static TriangulatedMesh ConvertToTriangulatedMesh(TriangulableMesh sourceMeshScript)
        {
            //check if mesh is already a triangulated one
            if (sourceMeshScript.GetType() == triangulatedMeshType)
            {
                return sourceMeshScript.GetComponent<TriangulatedMesh>();
            }

            Object.DestroyImmediate(sourceMeshScript.GetComponent<Collider2D>());
            TriangulatedMesh triangulatedMesh = sourceMeshScript.gameObject.AddComponent<TriangulatedMesh>();
            Vector2[] points = MeshHelper.ConvertVec3ToVec2(sourceMeshScript.GetTriangulableVertices());
            List<Triangulation.IntTriple> connections = Triangulation.TriangulationToInt3(new List<Vector2>(points));
            triangulatedMesh.Build(points, connections, sourceMeshScript.C_MR.sharedMaterial);

            //delete base component
            Object.DestroyImmediate(sourceMeshScript);

            return triangulatedMesh;
        }

        public static TriangulatedMesh ToSmoothedMesh(TriangulableMesh sourceMeshScript, float smoothness)
        {
            sourceMeshScript = ConvertToTriangulatedMesh(sourceMeshScript);
            List<Vector2> subPoints = new List<Vector2>();

            for (int i = 0; i < sourceMeshScript.Vertices.Length; i++)
            {
                //start, middle and end of segment
                Vector2 a = sourceMeshScript.Vertices[i];
                Vector2 b = sourceMeshScript.Vertices[(i + 1) % sourceMeshScript.Vertices.Length];
                Vector2 c = sourceMeshScript.Vertices[(i + 2) % sourceMeshScript.Vertices.Length];

                //midpoints of segments
                Vector2 mid1 = Vector2.Lerp(a, b, 0.5f);
                Vector2 mid2 = Vector2.Lerp(b, c, 0.5f);
                //mid-point of mid-points
                Vector2 mid_mid = Vector2.Lerp(mid1, mid2, 0.5f);

                Vector2 diff = Vector2.LerpUnclamped(mid_mid, b, smoothness);

                subPoints.Add(mid1);
                subPoints.Add(diff);
            }
            List<Triangulation.IntTriple> connections = Triangulation.TriangulationToInt3(new List<Vector2>(subPoints));
            TriangulatedMesh triangulatedMesh = sourceMeshScript.gameObject.AddComponent<TriangulatedMesh>();
            triangulatedMesh.Build(subPoints.ToArray(), connections, sourceMeshScript.C_MR.sharedMaterial);
            triangulatedMesh.smoothingValue = smoothness;

            //delete base component
            Object.DestroyImmediate(sourceMeshScript);

            return triangulatedMesh;
        }
    }

    public abstract class TriangulableMesh : MeshBase
    {
        [Range(0f, 1f)]
        public float smoothingValue = 0.95f;

        public virtual Vector3[] GetTriangulableVertices()
        {
            return GetVerticesInGlobalSpace();
        }

        public static TriangulatedMesh ConvertToTriangulatedMesh(TriangulableMesh triangulableMesh)
        {
            return TriangulableMeshProxy.ConvertToTriangulatedMesh(triangulableMesh);
        }

        public static TriangulatedMesh ToSmoothedMesh(TriangulableMesh triangulableMesh, float smoothness)
        {
            return TriangulableMeshProxy.ToSmoothedMesh(triangulableMesh, smoothness);
        }
    }
}