using UnityEngine;

/// <summary>
/// Basic mesh, consisting of three vertices and
/// one face.
/// 
/// Colliders:
///     - polygon
/// </summary>
public class TriangleMesh : MeshBase
{

    //mesh data
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    //collider
    private PolygonCollider2D C_PC2D;
    
    public static GameObject AddTriangle(Vector3 position, Vector2 p1, Vector2 p2, Vector2 p3, Material meshMatt, bool attachRigidbody = true)
    {
        GameObject triangle = new GameObject();
        triangle.transform.position = position;
        triangle.AddComponent<TriangleMesh>().Build(p1, p2, p3, meshMatt);
        if (attachRigidbody)
        {
            triangle.AddComponent<Rigidbody2D>();
        }
        return triangle;
    }

    public static GameObject AddTriangle(Vector3 position, Vector2[] vertices, Material meshMatt, bool attachRigidbody = true)
    {
        GameObject triangle = new GameObject();
        triangle.transform.position = position;
        triangle.AddComponent<TriangleMesh>().Build(vertices[0], vertices[1], vertices[2], meshMatt);
        if (attachRigidbody)
        {
            triangle.AddComponent<Rigidbody2D>();
        }
        return triangle;
    }

    //assign variables, get components and build mesh
    public void Build(Vector2 p1, Vector2 p2, Vector2 p3, Material meshMatt)
    {
        name = "Triangle";
        mesh = new Mesh();

        GetOrAddComponents();
        C_MR.material = meshMatt;

        if (SetPoints(p1, p2, p3))
        {
            UpdateMesh();
            UpdateCollider();
        }
    }

    //assign variables, get components and build mesh
    public void Build(Vector2[] vertices, Material meshMatt)
    {
        name = "Triangle";
        mesh = new Mesh();

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
        #region Validity Check

        if (p1 == p2 || p2 == p3 || p3 == p1)
        {
            Debug.LogWarning("TriangleMesh::SetPoints: some of the points are identity!");
            return false;
        }

        #endregion

        if (vertices == null)
        {
            vertices = new Vector3[3];
        }

        if (triangles == null)
        {
            triangles = new int[3];
            triangles[0] = 0;
            triangles[1] = 2;
            triangles[2] = 1;
        }

        vertices[0] = p1;

        float sign = GetSide(p2, p1, p3);
        if (sign == 0)
        {
            Debug.LogWarning("Triangle::SetPoints: Given points are colinear!");
            return false;
        }
        else if (sign == -1)
        {
            vertices[1] = p2;
            vertices[2] = p3;
        }
        else
        {
            vertices[1] = p3;
            vertices[2] = p2;
        }

        uvs = UVUnwrap(vertices).ToArray();

        return true;
    }

    // get triangle vertices
    public Vector3[] GetVertices()
    {
        return vertices;
    }

    #region Abstract Implementation

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
            var o_144_12_636376465613114936 = C_MF.mesh;
        }
    }
    public override void UpdateCollider()
    {
        C_PC2D.SetPath(0, ConvertVec3ToVec2(vertices));
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
