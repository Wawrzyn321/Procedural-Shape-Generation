using UnityEngine;

/// <summary>
/// Quadrangle of arbitrary vertices.
/// 
/// Colliders:
///     - Polygon
/// </summary>
public class QuadrangleMesh : MeshBase {
    
    //mesh data
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    //helper collider array
    private Vector2[] points;

    //collider
    private PolygonCollider2D C_PC2D;

    public static GameObject AddRectangleMesh(Vector3 position, Vector2[] verts, Material meshMatt, bool attachRigidbody = true)
    {
        GameObject quad = new GameObject();
        quad.transform.position = position;
        quad.AddComponent<QuadrangleMesh>().Build(verts, meshMatt);
        if (attachRigidbody)
        {
            quad.AddComponent<Rigidbody2D>();
        }
        return quad;
    }

    //assign variables, get components and build mesh
    public void Build(Vector2[] verts, Material meshMatt)
    {
        name = "Quadrangle";

        mesh = new Mesh();
        GetOrAddComponents();

        C_MR.material = meshMatt;

        if (BuildQuadrangleMesh(verts))
        {
            UpdateMesh();
            UpdateCollider();
        }
    }

    //build quad
    private bool BuildQuadrangleMesh(Vector2[] verts)
    {
        vertices = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            vertices[i] = verts[i];
        }
        triangles = new int[6];
        uvs = new Vector2[4];
        points = new Vector2[4];
        
        if (!isPointInTriangle(verts[3], verts[0], verts[1], verts[2]))
        {

            if (GetSide(verts[3], verts[0], verts[1]) * GetSide(verts[2], verts[0], verts[1]) <= 0)
            {
                triangles = new int[] {0,1,2,3,1,0};
                points[0] = verts[0];
                points[1] = verts[3];
                points[2] = verts[1];
                points[3] = verts[2];
            }
            else if (GetSide(verts[3], verts[1], verts[2]) * GetSide(verts[0], verts[1], verts[2]) <= 0)
            {
                triangles = new int[]{ 0,1,2,3,2,1};
                points[0] = verts[0];
                points[1] = verts[1];
                points[2] = verts[3];
                points[3] = verts[2];
            }
            else
            {
                triangles = new int[] { 0, 1, 2, 0,2,3 };
                points = verts;
            }
        }
        else
        {
            if (GetSide(verts[0], verts[3], verts[1]) <= 0 && GetSide(verts[2], verts[3], verts[1]) >= 0)
            {
                triangles = new int[] { 3, 2, 1, 3, 1, 0 };
                points = verts;
            }
            else if (GetSide(verts[0], verts[1], verts[2]) <= 0 && GetSide(verts[3], verts[1], verts[2]) >= 0)
            {
                triangles = new int[] {1,2,3,0,1,2};
                points[0] = verts[0];
                points[1] = verts[1];
                points[2] = verts[3];
                points[3] = verts[2];
            }
            else
            {
                triangles = new int[] {3,2,1,0,2,3};
                points[0] = verts[0];
                points[1] = verts[3];
                points[2] = verts[1];
                points[3] = verts[2];
            }
        }

        uvs = UVUnwrap(vertices).ToArray();

        return true;
    }

    //checks if point v is within triangle {v1,v2,v3}
    private bool isPointInTriangle(Vector2 v, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        double a1 = GetSide(v, v1, v2);
        double a2 = GetSide(v, v2, v3);
        double a3 = GetSide(v, v3, v1);
        return (a1 >= 0 && a2 >= 0 && a3 >= 0) || (a1 <= 0 && a2 <= 0 && a3 <= 0);
    }

    #region Abstract Implementation

    public override void GetOrAddComponents()
    {
        C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
        C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
        C_MF = gameObject.GetOrAddComponent<MeshFilter>();
    }

    public override void UpdateCollider()
    {
        C_PC2D.points = points;
    }

    public override void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = AddMeshNormals(vertices.Length);
        C_MF.mesh = mesh;
        if (OptimizeMesh)
        {
            C_MF.mesh.Optimize();
        }
    }

    #endregion

}
