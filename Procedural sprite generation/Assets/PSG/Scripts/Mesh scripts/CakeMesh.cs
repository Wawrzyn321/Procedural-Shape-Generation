using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Similar to circle, but only given fraction of shape is filled.
/// If ratio is equal to one, we got a circle.
/// 
/// Colliders:
///     - Polygon (if {sidesToFill} is less than {sides})
///     - Circle (in other case)
/// 
/// </summary>
public class CakeMesh : MeshBase
{

    //mesh data
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector2> uvs;

    //cake data
    private float radius;
    private int sides;
    private int sidesToFill;
    private Vector2 centerShift;

    //colliders
    private PolygonCollider2D C_PC2D;
    private CircleCollider2D C_CC2D;

    public static GameObject AddCakeMesh(Vector3 position, float radius, int sides, int sidesToFill, Material meshMatt, bool attachRigidbody = true)
    {
        GameObject cake = new GameObject();
        cake.transform.position = position;
        cake.AddComponent<CakeMesh>().Build(radius,sides, sidesToFill, meshMatt);
        if (attachRigidbody)
        {
            cake.AddComponent<Rigidbody2D>();
        }
        return cake;
    }

    //assign variables, get components and build mesh
    public void Build(float radius, int sides, int sidesToFill, Material meshMatt)
    {
        name = "Cake";

        this.radius = radius;
        this.sides = sides;
        this.sidesToFill = sidesToFill;

        mesh = new Mesh();
        GetOrAddComponents();

        C_MR.material = meshMatt;

        if (BakeCake(radius, sides, sidesToFill))
        {
            UpdateMesh();
            UpdateCollider();
        }
    }
    
    //build a cake
    private bool BakeCake(float radius, int sides, int sidesToFill)
    {
        #region Validity Check

        if (sides < 2)
        {
            Debug.LogWarning("CakeMesh::AddCake: sides count can't be less than two!");
            return false;
        }
        if (sides<sidesToFill)
        {
            Debug.LogWarning("CakeMesh::AddCake: sidesToFill can't be biger than sides!");
            return false;
        }
        if (radius == 0)
        {
            Debug.LogWarning("CakeMesh::AddCake: radius can't be equal to zero!");
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
        uvs.Add(new Vector2(0.5f, 0.5f));
        float angleDelta = deg360 / sides;
        for (int i = 0; i < sidesToFill + 2; i++)
        {
            Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * radius;
            vertices.Add(vertPos);
            uvs.Add(vertPos / 2 / radius + new Vector3(0.5f, 0.5f, 0));
        }
        for(int i = 0; i < sidesToFill;i++)
        {
            triangles.Add(1 + i + 1);
            triangles.Add(1 + i);
            triangles.Add(0);
        }

        centerShift = new Vector2(0,0);
        for (int i = 0; i < vertices.Count; i++)
        {
            centerShift += (Vector2)vertices[i];
        }
        centerShift /= vertices.Count;

        return true;
    }

    #region Abstract Implementation

    public override void GetOrAddComponents()
    {
        if (sidesToFill == sides)
        {
            C_CC2D = gameObject.GetOrAddComponent<CircleCollider2D>();
        }
        else
        {
            C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
        }

        C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
        C_MF = gameObject.GetOrAddComponent<MeshFilter>();
    }

    public override void UpdateCollider()
    {
        if(sidesToFill == sides)
        {
            C_CC2D.radius = radius;
        }
        else
        {
            Vector2[] points = new Vector2[sidesToFill + 3];
            for (int i = 0; i < vertices.Count-1; i++)
            {
                points[i] = vertices[i];
            }
            points[points.Length - 1] = points[0];
            C_PC2D.points = points;
        }
    }

    public override void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.normals = AddMeshNormals(vertices.Count);
        C_MF.mesh = mesh;
        if (OptimizeMesh)
        {
            C_MF.mesh.Optimize();
        }
    }

    #endregion

}
