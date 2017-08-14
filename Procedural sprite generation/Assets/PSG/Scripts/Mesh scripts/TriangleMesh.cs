using System;
using UnityEditor;
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
        //collider
        private PolygonCollider2D C_PC2D;

        #region Static Building

        public static TriangleMesh AddTriangle(Vector3 position, Vector2 p1, Vector2 p2, Vector2 p3, Material meshMatt = null, bool attachRigidbody = true)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            GameObject triangle = new GameObject();
            triangle.transform.position = position;
            TriangleMesh triangleComponent = triangle.AddComponent<TriangleMesh>();
            triangleComponent.Build(p1, p2, p3, meshMatt);
            if (attachRigidbody)
            {
                triangle.AddComponent<Rigidbody2D>();
            }
            return triangleComponent;
        }

        #endregion

        //assign variables, get components and build mesh
        public void Build(Vector2 p1, Vector2 p2, Vector2 p3, Material meshMatt = null)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            name = "Triangle";
            _Mesh = new Mesh();

            GetOrAddComponents();
            C_MR.material = meshMatt;

            if (SetPoints(p1, p2, p3))
            {
                UpdateMesh();
                UpdateCollider();
            }
        }

        //assign variables, get components and build mesh
        public void Build(Vector2[] vertices, Material meshMatt = null)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            name = "Triangle";
            _Mesh = new Mesh();

            GetOrAddComponents();
            C_MR.material = meshMatt;

            if (SetPoints(vertices[0], vertices[1], vertices[2]))
            {
                UpdateMesh();
                UpdateCollider();
            }
        }

        //build triangle or set its points
        public bool SetPoints(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            p1 += (Vector2)transform.position;
            p2 += (Vector2)transform.position;
            p3 += (Vector2)transform.position;

            #region Validity Check

            if (p1 == p2 || p2 == p3 || p3 == p1)
            {
                Debug.LogWarning("TriangleMesh::SetPoints: some of the points are identity!");
                return false;
            }

            #endregion

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

            float sign = MeshHelper.GetSide(p2, p1, p3);
            if (sign == 0)
            {
                Debug.LogWarning("Triangle::SetPoints: Given points are colinear!");
                return false;
            }
            else if (sign == -1)
            {
                Vertices[1] = p2;
                Vertices[2] = p3;
            }
            else
            {
                Vertices[1] = p3;
                Vertices[2] = p2;
            }

            UVs = MeshHelper.UVUnwrap(Vertices).ToArray();

            return true;
        }

        #region Abstract Implementation

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