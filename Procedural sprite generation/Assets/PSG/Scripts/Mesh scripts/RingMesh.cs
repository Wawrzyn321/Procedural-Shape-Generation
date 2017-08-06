using UnityEngine;
using System.Collections.Generic;

namespace PSG
{
    /// <summary>
    /// A ring. If both radiuses are equal, it
    /// degenerates to circle.
    /// 
    /// Colliders:
    ///     - 2x Polygon(if ring radiuses are different)
    ///     - Circle (if ring radiuses are equal)
    /// </summary>
    public class RingMesh : MeshBase
    {

        //mesh data
        private List<Vector3> vertices;
        private List<int> triangles;
        private List<Vector2> uvs;

        //ring data
        private float innerRadius;
        private float outerRadius;
        private int sides;

        //colliders
        private PolygonCollider2D C_PC2D_outer;
        private PolygonCollider2D C_PC2D_inner;
        private CircleCollider2D C_CC2D;

        public static GameObject AddRingMesh(Vector3 position, float innerRadius, float outerRadius, int sides, Material meshMatt, bool attachRigidbody = true)
        {
            GameObject ring = new GameObject();
            ring.transform.position = position;
            ring.AddComponent<RingMesh>().Build(innerRadius, outerRadius, sides, meshMatt);
            if (attachRigidbody)
            {
                ring.AddComponent<Rigidbody2D>();
            }
            return ring;
        }

        //assign variables, get components and build mesh
        public void Build(float innerRadius, float outerRadius, int sides, Material meshMatt)
        {
            name = "Ring";
            this.innerRadius = innerRadius;
            this.outerRadius = outerRadius;
            this.sides = sides;

            mesh = new Mesh();
            GetOrAddComponents();

            C_MR.material = meshMatt;

            if (BuildRing(innerRadius, outerRadius, sides))
            {
                UpdateMesh();
                UpdateCollider();
            }
        }

        //build a ring
        private bool BuildRing(float innerRadius, float outerRadius, int sides)
        {

            #region Validity Check

            if (sides < 2)
            {
                Debug.LogWarning("RingMesh::BuildRing: sides count can't be less than two!");
                return false;
            }
            if (innerRadius == 0 && outerRadius == 0)
            {
                Debug.LogWarning("RingMesh::BuildRing: radius can't be equal to zero!");
                return false;
            }
            if (innerRadius < 0)
            {
                innerRadius = -innerRadius;
            }
            if (outerRadius < 0)
            {
                outerRadius = -outerRadius;
            }

            #endregion

            vertices = new List<Vector3>();
            triangles = new List<int>();

            //swap radiuses if inner one is greater than outer
            if (innerRadius > outerRadius)
            {
                float tempRadius = innerRadius;
                innerRadius = outerRadius;
                outerRadius = tempRadius;
            }

            //build ordinary circle
            if (innerRadius == outerRadius)
            {
                vertices.Add(new Vector3(0, 0));
                float angleDelta = deg360 / sides;
                for (int i = 1; i < sides + 1; i++)
                {
                    Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * innerRadius;
                    vertices.Add(vertPos);
                    triangles.Add(0);
                    triangles.Add(1 + (i - 1) % sides);
                    triangles.Add(1 + i % sides);
                }
            }
            else
            {
                float angleDelta = deg360 / sides;
                for (int i = 0; i < sides; i++)
                {
                    Vector3 vertPosInner = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * innerRadius;
                    Vector3 vertPosOuter = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * outerRadius;
                    vertices.Add(vertPosInner);
                    vertices.Add(vertPosOuter);
                    triangles.Add(i * 2 + 0);
                    triangles.Add((i * 2 + 2) % (sides * 2));
                    triangles.Add((i * 2 + 1) % (sides * 2));
                    triangles.Add((i * 2 + 3) % (sides * 2));
                    triangles.Add((i * 2 + 1) % (sides * 2));
                    triangles.Add((i * 2 + 2) % (sides * 2));
                }
            }
            uvs = MeshHelper.UVUnwrap(vertices.ToArray());

            return true;
        }

        #region Abstract Implementation

        public override void UpdateMesh()
        {
            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.normals = MeshHelper.AddMeshNormals(vertices.Count);
            C_MF.mesh = mesh;
        }
        public override void GetOrAddComponents()
        {
            if (innerRadius == outerRadius)
            {
                C_CC2D = gameObject.GetOrAddComponent<CircleCollider2D>();
            }
            else
            {
                PolygonCollider2D[] C_EC2Ds = GetComponents<PolygonCollider2D>();
                switch (C_EC2Ds.Length)
                {
                    case 0:
                        C_PC2D_inner = gameObject.AddComponent<PolygonCollider2D>();
                        C_PC2D_outer = gameObject.AddComponent<PolygonCollider2D>();
                        break;
                    case 1:
                        C_PC2D_inner = C_EC2Ds[0];
                        C_PC2D_outer = gameObject.AddComponent<PolygonCollider2D>();
                        break;
                    default:
                        C_PC2D_inner = C_EC2Ds[0];
                        C_PC2D_outer = C_EC2Ds[1];
                        break;
                }
            }
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }
        public override void UpdateCollider()
        {
            if (innerRadius == outerRadius)
            {
                C_CC2D.radius = innerRadius;
            }
            else
            {
                Vector2[] points_inner = new Vector2[sides];
                Vector2[] points_outer = new Vector2[sides];

                float angleDelta = deg360 / sides;
                points_inner[0] = new Vector2(Mathf.Cos(angleDelta), Mathf.Sin(angleDelta)) * innerRadius;
                points_outer[0] = new Vector2(Mathf.Cos(angleDelta), Mathf.Sin(angleDelta)) * outerRadius;
                for (int i = 1; i < sides; i++)
                {
                    points_inner[i] = new Vector2(Mathf.Cos((i + 1) * angleDelta), Mathf.Sin((i + 1) * angleDelta)) * innerRadius;
                    points_outer[i] = new Vector2(Mathf.Cos((i + 1) * angleDelta), Mathf.Sin((i + 1) * angleDelta)) * outerRadius;
                }
                C_PC2D_inner.points = points_inner;
                C_PC2D_outer.points = points_outer;
            }
        }

        #endregion

    }

}