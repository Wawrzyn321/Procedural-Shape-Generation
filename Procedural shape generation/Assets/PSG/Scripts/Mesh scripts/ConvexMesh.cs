using UnityEngine;
using System.Collections.Generic;

namespace PSG
{
    /// <summary>
    /// Convex shape, constructed upon given set of points
    /// using QuickHull algorithm (https://en.wikipedia.org/wiki/Quickhull).
    /// 
    /// Colliders:
    ///     - Polygon
    /// 
    /// </summary>
    public class ConvexMesh : MeshBase
    {
        //mesh data
        private Vector3[] baseVertices;
        private Vector3[] vertices;

        //collider
        private PolygonCollider2D C_PC2D;

        #region Static Methods

        public static ConvexMesh AddConvexMesh(Vector3 position, Vector3[] vertices, Material meshMatt = null, bool attachRigidbody = true)
        {
            GameObject convex = new GameObject();

            convex.transform.position = position;

            ConvexMesh convexComponent = convex.AddComponent<ConvexMesh>();
            convexComponent.Build(vertices, meshMatt);
            if (attachRigidbody)
            {
                convex.AddComponent<Rigidbody2D>();
            }
            return convexComponent;
        }

        public static ConvexMesh AddConvexMesh(Vector3 position, List<Vector3> vertices, Material meshMatt = null, bool attachRigidbody = true)
        {
            return AddConvexMesh(position, vertices.ToArray(), meshMatt, attachRigidbody);
        }

        #endregion

        //assign variables, get components and build mesh
        public void Build(Vector3[] vertices, Material meshMatt = null)
        {
            name = "Convex Mesh";
            this.vertices = vertices;

            BuildMesh(ref meshMatt);
        }

        //get points set in constructor
        public Vector3[] GetBasePoints()
        {
            return baseVertices;
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (vertices.Length < 2)
            {
                Debug.LogWarning("ConvexMesh::ValidateMesh: verts count must be greater than 2!");
                return false;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {
            baseVertices = vertices;

            Vertices = MeshHelper.ConvertVec2ToVec3(ConvexHull.QuickHull(MeshHelper.ConvertVec3ToVec2(vertices)).ToArray()); // oh no

            Triangles = new int[Vertices.Length * 3];

            for (int i = 1; i < Vertices.Length - 1; i++)
            {
                Triangles[i * 3 + 0] = 0;
                Triangles[i * 3 + 1] = i;
                Triangles[i * 3 + 2] = i + 1;
            }
            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        public override void UpdateCollider()
        {
            C_PC2D.points = MeshHelper.ConvertVec3ToVec2(Vertices);
        }

        public override void GetOrAddComponents()
        {
            C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        #endregion
    }
}
