using UnityEngine;
using System.Collections.Generic;

namespace PSG
{
    /// <summary>
    /// A ring. If both radiuses are equal, it
    /// degenerates to circle.
    /// 
    /// Colliders:
    ///     - Polygon
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
        private PolygonCollider2D C_PC2D;

        public static GameObject AddRingMesh(Vector3 position, float innerRadius, float outerRadius, int sides, Material meshMatt = null, bool attachRigidbody = true)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
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
        public void Build(float innerRadius, float outerRadius, int sides, Material meshMatt = null)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
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

        public RingStructure GetStructure()
        {
            return new RingStructure
            {
                innerRadius = innerRadius,
                outerRadius = outerRadius,
                sides = sides
            };
        }

        #region Abstract Implementation

        public override Vector3[] GetVertices()
        {
            return vertices.ToArray();
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
        public override void GetOrAddComponents()
        {
            C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }
        public override void UpdateCollider()
        {
            bool isHollow = innerRadius > 0;
            int numberOfPoints = isHollow ? (sides+1) * 2 : sides;
            Vector2[] colliderPoints = new Vector2[numberOfPoints];

            float angleDelta = deg360 / sides;
            colliderPoints[0] = new Vector2(Mathf.Cos(angleDelta), Mathf.Sin(angleDelta)) * outerRadius;
            for (int i = 0; i < sides; i++)
            {
                colliderPoints[i] = new Vector2(Mathf.Cos((i + 1) * angleDelta), Mathf.Sin((i + 1) * angleDelta)) * outerRadius;
            }

            if (isHollow)
            {
                colliderPoints[sides] = colliderPoints[0];
                for (int i = 0; i < sides; i++)
                {
                    colliderPoints[i + sides+1] = new Vector2(Mathf.Cos((i + 1) * angleDelta), Mathf.Sin((i + 1) * angleDelta)) * innerRadius;
                }
                colliderPoints[numberOfPoints - 1] = colliderPoints[sides+1];
            }

            C_PC2D.points = colliderPoints;
        }

        #endregion

    }

    [System.Serializable]
    public struct RingStructure
    {
        public float innerRadius;
        public float outerRadius;
        public int sides;
    }
}