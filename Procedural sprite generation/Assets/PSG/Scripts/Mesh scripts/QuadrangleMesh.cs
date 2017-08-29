using System.Collections.Generic;
using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Quadrangle of arbitrary vertices.
    /// 
    /// Colliders:
    ///     - Polygon
    /// </summary>
    public class QuadrangleMesh : MeshBase
    {

        //mesh data
        private Vector2[] verts;

        //collider
        private PolygonCollider2D C_PC2D;

        #region Static Build

        public static QuadrangleMesh AddQuadrangle(Vector3 position, IList<Vector2> verts, Space space = Space.World, Material meshMatt = null, bool attachRigidbody = true)
        {
            GameObject quad = new GameObject();
            quad.transform.position = position + (space == Space.World ? (Vector3)(verts[0] + verts[1] + verts[2] + verts[3]) * 0.25f : Vector3.zero);
            QuadrangleMesh quadComponent = quad.AddComponent<QuadrangleMesh>();
            quadComponent.Build(verts, meshMatt);
            if (attachRigidbody)
            {
                quad.AddComponent<Rigidbody2D>();
            }
            return quadComponent;
        }

        #endregion

        //assign variables, get components and build mesh
        public void Build(IList<Vector2> verts, Material meshMatt = null)
        {
            name = "Quadrangle";
            this.verts = (Vector2[])verts;

            BuildMesh(ref meshMatt);
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (MeshHelper.HasDuplicates(verts))
            {
                Debug.LogWarning("QuadrangleMesh::ValidateMesh: Duplicate points detected!");
                return false;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {
            Vertices = new Vector3[4];
            Vector2 center = (verts[0] + verts[1] + verts[2] + verts[3]) * 0.25f;
            for (int i = 0; i < 4; i++)
            {
                Vertices[i] = verts[i] - center;
            }

            if (MeshHelper.IsPointInTriangle(verts[0], verts[1], verts[2], verts[3]))
            {
                Triangles = new int[] { 0, 2, 3, 0, 1, 2 };
            }
            else if (!MeshHelper.IsPointInTriangle(verts[3], verts[0], verts[1], verts[2]))
            {
                if (MeshHelper.IsPointInTriangle(verts[2], verts[0], verts[1], verts[3]))
                {
                    Triangles = new int[] { 0, 2, 3, 1, 0, 2 };
                }
                else
                {
                    Triangles = new int[] { 0, 1, 3, 1, 2, 3 };
                }
            }
            else
            {
                Triangles = new int[] { 0, 1, 3, 2, 3, 1 };
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