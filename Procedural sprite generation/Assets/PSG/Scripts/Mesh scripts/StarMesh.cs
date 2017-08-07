using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Mesh formed in simple star shape.
    /// 
    /// Colliders:
    ///     - polygon
    /// </summary>
    /// 
    public class StarMesh : MeshBase
    {

        //mesh data
        private Vector3[] vertices;
        private int[] triangles;
        private Vector2[] uvs;

        //star data
        private float radiusA; //horizontal radius
        private float radiusB; //vertical radius
        private int sides;

        //collider
        private PolygonCollider2D C_PC2D;

        public static GameObject AddStartMesh(Vector3 position, float radiusA, float radiusB, int sides, Material meshMatt = null, bool attachRigidbody = true)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            GameObject star = new GameObject();
            star.transform.position = position;
            star.AddComponent<StarMesh>().Build(radiusA, radiusB, sides, meshMatt);
            if (attachRigidbody)
            {
                star.AddComponent<Rigidbody2D>();
            }
            return star;
        }

        //assign variables, get components and build mesh
        public void Build(float radiusA, float radiusB, int sides, Material meshMatt = null)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            name = "Star";

            this.sides = sides;
            this.radiusA = radiusA;
            this.radiusB = radiusB;
            mesh = new Mesh();
            GetOrAddComponents();

            C_MR.material = meshMatt;

            if (BuildStarMesh(radiusA, radiusB, sides))
            {
                UpdateMesh();
                UpdateCollider();
            }
        }

        //build a star
        private bool BuildStarMesh(float radiusA, float radiusB, int sides)
        {

            #region Validity Check

            if (sides < 2)
            {
                Debug.LogWarning("StarMesh::BuildStar: sides count can't be less than two!");
                return false;
            }
            if (radiusA == 0 || radiusB == 0)
            {
                Debug.LogWarning("StarMesh::BuildStar: any of radiuses can't be equal to zero!");
                return false;
            }
            if (radiusA < 0)
            {
                radiusA = -radiusA;
            }
            if (radiusB < 0)
            {
                radiusB = -radiusB;
            }

            #endregion

            vertices = new Vector3[1 + sides * 2];
            triangles = new int[2 * sides * 3];
            uvs = new Vector2[1 + sides * 2];

            vertices[0] = new Vector3(0, 0);
            float angleDelta = 360 / (float)sides / 2 * Mathf.Deg2Rad;
            float angleShift = -360f / (sides * 4) * Mathf.Deg2Rad;
            for (int i = 0; i < sides * 2; i++)
            {
                Vector3 vertVec = new Vector3(Mathf.Cos(i * angleDelta + angleShift), Mathf.Sin(i * angleDelta + angleShift));
                vertices[1 + i] = vertVec * (i % 2 == 0 ? radiusA : radiusB);
                triangles[(i * 3 + 2) % triangles.Length] = 0;
                triangles[(i * 3 + 1) % triangles.Length] = 1 + i % (sides * 2);
                triangles[i * 3] = 1 + (i + 1) % (sides * 2);
            }

            uvs = MeshHelper.UVUnwrap(vertices).ToArray();

            return true;
        }

        public StarStructure GetStructure()
        {
            return new StarStructure
            {
                radiusA = radiusA,
                radiusB = radiusB,
                sides = sides
            };
        }

        #region Abstract Implementation

        public override Vector3[] GetVertices()
        {
            return vertices;
        }

        public override void UpdateMesh()
        {
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.normals = MeshHelper.AddMeshNormals(vertices.Length);
            C_MF.mesh = mesh;
        }

        public override void UpdateCollider()
        {
            Vector2[] points = new Vector2[sides * 2];
            float angleDelta = 360 / (float)points.Length * Mathf.Deg2Rad;
            float angleShift = -360f / (sides * 4) * Mathf.Deg2Rad;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 vertPos = new Vector2(Mathf.Cos(i * angleDelta + angleShift), Mathf.Sin(i * angleDelta + angleShift));
                points[i] = vertPos * (i % 2 == 0 ? radiusA : radiusB);
            }
            C_PC2D.SetPath(0, points);
        }

        public override void GetOrAddComponents()
        {
            C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        #endregion
    }

    [System.Serializable]
    public struct StarStructure
    {
        public float radiusA;
        public float radiusB;
        public int sides;
    }
}