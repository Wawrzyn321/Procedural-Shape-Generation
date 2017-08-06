using UnityEngine;
using System.Collections.Generic;

namespace PSG
{
    /// <summary>
    /// Simple circle for PSG.
    /// 
    /// Colliders:
    ///     - Circle or Polygon
    /// 
    /// Use CircleCollider to improve performance:
    /// PolygonCollider is advised when sides are
    /// less than 8.
    /// 
    /// </summary>

    public class CircleMesh : MeshBase
    {
        //mesh data
        private List<Vector3> vertices;
        private List<int> triangles;
        private List<Vector2> uvs;

        //circle data
        private float radius;
        private int sides;

        //collider
        private bool useCircleCollider;
        private Collider2D C_C2D;

        public static GameObject AddCircle(Vector3 position, float radius, int sides, bool useCircleCollider, bool attachRigidbody = true, Material meshMatt = null)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            GameObject circle = new GameObject();
            circle.transform.position = position;
            circle.AddComponent<CircleMesh>().Build(radius, sides, useCircleCollider, meshMatt);
            if (attachRigidbody)
            {
                circle.AddComponent<Rigidbody2D>();
            }
            return circle;
        }

        //assign variables, get components and build mesh
        public void Build(float radius, int sides, bool useCircleCollider, Material meshMatt = null)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            name = "Circle";
            this.radius = radius;
            this.sides = sides;
            this.useCircleCollider = useCircleCollider;

            mesh = new Mesh();
            GetOrAddComponents();

            C_MR.material = meshMatt;

            if (BuildCircle(radius, sides))
            {
                UpdateMesh();
                UpdateCollider();
            }
        }

        //build a circle
        private bool BuildCircle(float radius, int sides)
        {

            #region Validity Check

            if (sides < 2)
            {
                Debug.LogWarning("CircleMesh::AddCircle: sides count can't be less than two!");
                return false;
            }
            if (radius == 0)
            {
                Debug.LogWarning("CircleMesh::AddCircle: radius can't be equal to zero!");
                return false;
            }
            if (radius < 0)
            {
                radius = -radius;
            }

            #endregion

            vertices = new List<Vector3>();
            triangles = new List<int>();
            uvs = new List<Vector2>();

            vertices.Add(new Vector3(0, 0));
            //uvs are manually unwrapped here
            uvs.Add(new Vector2(0.5f, 0.5f));
            float angleDelta = deg360 / sides;
            for (int i = 1; i < sides + 1; i++)
            {
                Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * radius;
                vertices.Add(vertPos);
                uvs.Add(vertPos / 2 / radius + new Vector3(0.5f, 0.5f, 0));
                triangles.Add(1 + i % sides);
                triangles.Add(1 + (i - 1) % sides);
                triangles.Add(0);
            }

            return true;
        }

        #region Abstract Implementation

        public override void GetOrAddComponents()
        {
            if (useCircleCollider)
            {
                C_C2D = gameObject.GetOrAddComponent<CircleCollider2D>();
            }
            else
            {
                C_C2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            }
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        public override void UpdateCollider()
        {
            if (useCircleCollider)
            {
                ((CircleCollider2D)C_C2D).radius = radius;
            }

            else
            {
                ((PolygonCollider2D)C_C2D).SetPath(0, MeshHelper.ConvertVec3ToVec2(vertices.ToArray()));
            }
        }

        public override void UpdateMesh()
        {
            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.normals = MeshHelper.AddMeshNormals(vertices.Count);
            C_MF.mesh = mesh;
        }

        #endregion

    }

}