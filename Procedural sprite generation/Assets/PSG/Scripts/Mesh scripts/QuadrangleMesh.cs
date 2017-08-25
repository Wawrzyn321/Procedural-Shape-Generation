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

        //collider
        private PolygonCollider2D C_PC2D;

        public static QuadrangleMesh AddQuadrangle(Vector3 position, Vector2[] verts, Material meshMatt = null, bool attachRigidbody = true)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            GameObject quad = new GameObject();
            quad.transform.position = position;
            QuadrangleMesh quadComponent = quad.AddComponent<QuadrangleMesh>();
            quadComponent.Build(verts, meshMatt);
            if (attachRigidbody)
            {
                quad.AddComponent<Rigidbody2D>();
            }
            return quadComponent;
        }

        //assign variables, get components and build mesh
        public void Build(Vector2[] verts, Material meshMatt = null)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            name = "Quadrangle";

            _Mesh = new Mesh();
            GetOrAddComponents();

            C_MR.material = meshMatt;

            if (BuildQuadrangle(verts))
            {
                UpdateMesh();
                UpdateCollider();
            }
        }

        //build quad
        private bool BuildQuadrangle(Vector2[] verts)
        {
            Vertices = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                Vertices[i] = verts[i];
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

            UVs = MeshHelper.UVUnwrap(Vertices).ToArray();

            return true;
        }

        #region Abstract Implementation

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

        public static bool GetIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 d, ref Vector3 intersection)
        {
            Vector3 e = b - a;
            Vector3 f = d - c;
            Vector3 p = new Vector3(-e.y, e.x);

            double divider = Vector3.Dot(f, p);
            if (divider == 0)
            {
                return false;
            }
            double h = Vector3.Dot(a - c, p) / divider;
            if (h == 0 || h == 0)
            {
                return false;
            }
            if (h > 0 && h < 1)
            {
                intersection = c + f * (float)h;
                return true;
            }
            return false;
        }
    }

}