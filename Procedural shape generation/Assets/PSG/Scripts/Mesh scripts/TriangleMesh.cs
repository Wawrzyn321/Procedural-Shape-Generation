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
        public Vector3 P1 { get; protected set; }
        [SerializeField, HideInInspector]
        public Vector3 P2 { get; protected set; }
        [SerializeField, HideInInspector]
        public Vector3 P3 { get; protected set; }

        //collider
        public PolygonCollider2D C_PC2D { get; protected set; }

        #region Static Methods

        public static TriangleMesh AddTriangle(Vector3 position, Vector2[] points, Space space = Space.World, Material meshMat = null, bool attachRigidbody = true)
        {
            Debug.Assert(points.Length == 3, "TriangleMesh::AddTriangle: supplied triangle array length must be equal to 3!");
            return AddTriangle(position, points[0], points[1], points[2], space, meshMat, attachRigidbody);
        }

        public static TriangleMesh AddTriangle(Vector3 position, Vector2 p1, Vector2 p2, Vector2 p3, Space space = Space.World, Material meshMat = null, bool attachRigidbody = true)
        {
            GameObject triangle = new GameObject();
            triangle.transform.position = position + (space == Space.World ? (Vector3)(p1 + p2 + p3) / 3f : Vector3.zero);
            TriangleMesh triangleComponent = triangle.AddComponent<TriangleMesh>();
            triangleComponent.Build(p1, p2, p3, space, meshMat);
            if (attachRigidbody)
            {
                triangle.AddComponent<Rigidbody2D>();
            }
            return triangleComponent;
        }

        #endregion

        //assign variables, get components and build mesh
        public void Build(Vector2 p1, Vector2 p2, Vector2 p3, Space space = Space.World, Material meshMat = null)
        {
            name = "Triangle";
            Vector2 center = (p1 + p2 + p3) / 3f;
            P1 = p1 - center;
            P2 = p2 - center;
            P3 = p3 - center;

            BuildMesh(ref meshMat);
        }

        //assign variables, get components and build mesh
        public void Build(IList<Vector2> vertices, Material meshMat = null)
        {
            Vector2 center = (vertices[0] + vertices[1] + vertices[2]) / 3f;
            P1 = vertices[0] - center;
            P2 = vertices[1] - center;
            P3 = vertices[2] - center;

            BuildMeshComponents();
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (P1 == P2 || P2 == P3 || P3 == P1)
            {
                Debug.LogWarning("TriangleMesh::ValidateMesh: some of the points are identity!");
                return false;
            }

            int sign = MeshHelper.GetSide(P2, P1, P3);
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

            Vertices[0] = P1;

            int sign = MeshHelper.GetSide(P2, P1, P3);
            if (sign == -1)
            {
                Vertices[1] = P2;
                Vertices[2] = P3;
            }
            else
            {
                Vertices[1] = P3;
                Vertices[2] = P2;
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