using System.Collections.Generic;
using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Basic mesh, consisting of three vertices and
    /// one face.
    /// 
    /// Colliders:
    ///     - polygon
    /// </summary>
    public class TriangleMesh : MeshBase
    {
        //mesh data
        [SerializeField, HideInInspector]
        private Vector3 p1;
        [SerializeField, HideInInspector]
        private Vector3 p2;
        [SerializeField, HideInInspector]
        private Vector3 p3;

        //collider
        private PolygonCollider2D C_PC2D;

        #region Static Methods

        public static TriangleMesh AddTriangle(Vector3 position, Vector2[] points, Space space = Space.World, Material meshMatt = null, bool attachRigidbody = true)
        {
            Debug.Assert(points.Length == 3, "TriangleMesh::AddTriangle: supplied triangle array length must be equal to 3!");
            return AddTriangle(position, points[0], points[1], points[2], space, meshMatt, attachRigidbody);
        }

        public static TriangleMesh AddTriangle(Vector3 position, Vector2 p1, Vector2 p2, Vector2 p3, Space space = Space.World, Material meshMatt = null, bool attachRigidbody = true)
        {
            GameObject triangle = new GameObject();
            triangle.transform.position = position + (space == Space.World ? (Vector3)(p1 + p2 + p3) / 3f : Vector3.zero);
            TriangleMesh triangleComponent = triangle.AddComponent<TriangleMesh>();
            triangleComponent.Build(p1, p2, p3, space, meshMatt);
            if (attachRigidbody)
            {
                triangle.AddComponent<Rigidbody2D>();
            }
            return triangleComponent;
        }

        #endregion

        //assign variables, get components and build mesh
        public void Build(Vector2 p1, Vector2 p2, Vector2 p3, Space space = Space.World, Material meshMatt = null)
        {
            name = "Triangle";
            Vector2 center = (p1 + p2 + p3) / 3f;
            this.p1 = p1 - center;
            this.p2 = p2 - center;
            this.p3 = p3 - center;

            BuildMesh(ref meshMatt);
        }

        //assign variables, get components and build mesh
        public void Build(IList<Vector2> vertices, Material meshMatt = null)
        {
            Vector2 center = (vertices[0] + vertices[1] + vertices[2]) / 3f;
            p1 = vertices[0] - center;
            p2 = vertices[1] - center;
            p3 = vertices[2] - center;

            BuildMeshComponents();
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (p1 == p2 || p2 == p3 || p3 == p1)
            {
                Debug.LogWarning("TriangleMesh::ValidateMesh: some of the points are identity!");
                return false;
            }

            int sign = MeshHelper.GetSide(p2, p1, p3);
            if (sign == 0)
            {
                Debug.LogWarning("TriangleMesh::ValidateMesh: Given points are colinear!");
                return false;
            }

            return true;
        }

        protected override void BuildMeshComponents()
        {
            if (Vertices == null)
            {
                Vertices = new Vector3[3];
            }

            if (Triangles == null)
            {
                Triangles = new int[3];
                Triangles[0] = 0;
                Triangles[1] = 2;
                Triangles[2] = 1;
            }

            Vertices[0] = p1;

            int sign = MeshHelper.GetSide(p2, p1, p3);
            if (sign == -1)
            {
                Vertices[1] = p2;
                Vertices[2] = p3;
            }
            else
            {
                Vertices[1] = p3;
                Vertices[2] = p2;
            }

            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        public override void UpdateCollider()
        {
            C_PC2D.SetPath(0, MeshHelper.ConvertVec3ToVec2(Vertices));
        }

        public override void GetOrAddComponents()
        {
            C_PC2D = GetComponent<PolygonCollider2D>();
            if (!C_PC2D)
            {
                C_PC2D = gameObject.AddComponent<PolygonCollider2D>();
            }
            C_MR = GetComponent<MeshRenderer>();
            if (!C_MR)
            {
                C_MR = gameObject.AddComponent<MeshRenderer>();
            }
            C_MF = GetComponent<MeshFilter>();
            if (!C_MF)
            {
                C_MF = gameObject.AddComponent<MeshFilter>();
            }
        }


        #endregion
    }

}