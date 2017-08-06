using UnityEngine;
using System.Collections.Generic;

namespace PSG
{
    /// <summary>
    /// Gear for PSG.
    /// First radius ({innerRadius}) can be zero.
    /// 
    /// Colliders:
    ///     - Polygon
    /// </summary>
    public class GearMesh : MeshBase
    {

        //mesh data
        private List<Vector3> vertices;
        private List<int> triangles;
        private List<Vector2> uvs;

        //gear data
        private int sides;
        private float outerRadius;
        private float rootRadius;
        private float innerRadius;

        //colliders
        private PolygonCollider2D C_EC2D;

        public static GameObject AddGearMesh(Vector3 position, float innerRadius, float rootRadius, float outerRadius, int sides, Material meshMatt = null, bool attachRigidbody = true)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            GameObject gear = new GameObject();
            gear.transform.position = position;
            gear.AddComponent<GearMesh>().Build(innerRadius, rootRadius, outerRadius, sides, meshMatt);
            if (attachRigidbody)
            {
                gear.AddComponent<Rigidbody2D>();
            }
            return gear;
        }

        //assign variables, get components and build mesh
        public void Build(float innerRadius, float rootRadius, float outerRadius, int sides, Material meshMatt = null)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            name = "Gear";
            this.innerRadius = innerRadius;
            this.rootRadius = rootRadius;
            this.outerRadius = outerRadius;
            this.sides = sides;

            mesh = new Mesh();
            GetOrAddComponents();

            C_MR.material = meshMatt;

            if (BuildGear(innerRadius, rootRadius, outerRadius, sides))
            {
                UpdateMesh();
                UpdateCollider();
            }
        }

        //build a gear
        private bool BuildGear(float innerRadius, float rootRadius, float outerRadius, int sides)
        {

            #region Validity Check

            if (sides < 2)
            {
                Debug.LogWarning("GearMesh::BuildGear: sides count can't be less than two!");
                return false;
            }
            if (rootRadius == 0)
            {
                Debug.LogWarning("GearMesh::BuildGear: rootRadius can't be equal to zero!");
                return false;
            }
            if (outerRadius == 0)
            {
                Debug.LogWarning("GearMesh::BuildGear: outerRadius can't be equal to zero!");
                return false;
            }
            if (innerRadius < 0)
            {
                innerRadius = -innerRadius;
            }
            if (rootRadius < 0)
            {
                rootRadius = -rootRadius;
            }
            if (innerRadius < 0)
            {
                outerRadius = -outerRadius;
            }

            #endregion

            int doubleSides = 2 * sides;

            vertices = new List<Vector3>();
            triangles = new List<int>();

            float angleDelta = deg360 / doubleSides;
            float angleShift = angleDelta * 0.5f;
            float outerAngleShift = angleDelta * 0.2f;

            for (int i = 0; i < doubleSides; i++)
            {
                Vector3 innerVertPos =
                    new Vector3(Mathf.Cos(i * angleDelta + angleShift), Mathf.Sin(i * angleDelta + angleShift)) * innerRadius;
                Vector3 rootVertPos =
                    new Vector3(Mathf.Cos(i * angleDelta + angleShift), Mathf.Sin(i * angleDelta + angleShift)) * rootRadius;
                Vector3 outerVertPos;
                if (i % 2 == 0)
                {
                    outerVertPos =
                        new Vector3(Mathf.Cos(i * angleDelta + angleShift + outerAngleShift), Mathf.Sin(i * angleDelta + angleShift + outerAngleShift)) * outerRadius;
                }
                else
                {
                    outerVertPos =
                        new Vector3(Mathf.Cos(i * angleDelta + angleShift - outerAngleShift), Mathf.Sin(i * angleDelta + angleShift - outerAngleShift)) * outerRadius;
                }
                vertices.Add(innerVertPos);
                vertices.Add(rootVertPos);
                vertices.Add(outerVertPos);

                int a = 3 * i;
                int b = 3 * i + 1;
                int c = (3 * (i + 1)) % (3 * doubleSides);
                int d = (3 * (i + 1) + 1) % (3 * doubleSides);
                triangles.Add(d);
                triangles.Add(b);
                triangles.Add(c);
                triangles.Add(b);
                triangles.Add(a);
                triangles.Add(c);

                if (i % 2 == 0)
                {
                    a = 3 * i + 1;
                    b = 3 * i + 2;
                    c = (3 * (i + 1) + 1) % (3 * doubleSides);
                    d = (3 * (i + 1) + 2) % (3 * doubleSides);
                    triangles.Add(d);
                    triangles.Add(b);
                    triangles.Add(c);
                    triangles.Add(b);
                    triangles.Add(a);
                    triangles.Add(c);
                }
            }
            uvs = MeshHelper.UVUnwrap(vertices.ToArray());

            return true;
        }

        #region Abstract Implementation

        public override void GetOrAddComponents()
        {
            C_EC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        public override void UpdateCollider()
        {
            bool isHollow = innerRadius > 0;
            int numberOfPoints = isHollow ? sides * 6+2 : sides * 4;
            Vector2[] colliderPoints = new Vector2[numberOfPoints];
            for (int i = 0; i < sides; i++)
            {
                colliderPoints[4 * i + 0] = vertices[i * 6 + 1];
                colliderPoints[4 * i + 1] = vertices[i * 6 + 2];
                colliderPoints[4 * i + 2] = vertices[i * 6 + 5];
                colliderPoints[4 * i + 3] = vertices[i * 6 + 4];
            }
            if (isHollow)
            {
                colliderPoints[4 * sides] = colliderPoints[0];
                for (int i = 0; i < sides * 2; i++)
                {
                    colliderPoints[sides * 4 + i+1] = vertices[i * 3];
                }
                colliderPoints[numberOfPoints-1] = vertices[0];
            }
            C_EC2D.points = colliderPoints;
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