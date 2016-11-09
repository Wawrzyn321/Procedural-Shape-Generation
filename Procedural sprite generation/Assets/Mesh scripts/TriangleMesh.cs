using UnityEngine;
using System;

public class TriangleMesh : MeshBase
{

    //mesh data
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    //collider
    private PolygonCollider2D C_PC2D;
    
    public void Build(Vector2 p1, Vector2 p2, Vector2 p3, Material meshMatt)
    {
        name = "Triangle";
        mesh = new Mesh();

        GetOrAddComponents();
        C_MR.material = meshMatt;
        
        transform.Translate(-(p1 + p2 + p3) / 3);

        if (SetPoints(p1, p2, p3))
        {
            UpdateMesh();
            UpdateCollider();
        }
    }
    public bool SetPoints(Vector2 p1, Vector2 p2, Vector2 p3)
    {

        #region Validity Check

        if (p1 == p2 || p2 == p3 || p3 == p1)
        {
            Debug.LogWarning("TriangleMesh::SetPoints: some of the points are identity!");
            return false;
        }

        #endregion

        vertices = new Vector3[3];
        triangles = new int[3];

        vertices[0] = p1;

        float sign = GetSide(p2, p1, p3);
        if (sign == 0)
        {
            Debug.LogWarning("Triangle::SetPoints: Given points are colinear!");
        }
        else if (sign == 1)
        {
            vertices[1] = p2;
            vertices[2] = p3;
        }
        else
        {
            vertices[1] = p3;
            vertices[2] = p2;
        }
        
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;

        uvs = UVUnwrap(vertices).ToArray();

        return true;
    }

    public Vector3[] GetVertices()
    {
        return vertices;
    }

    private int GetSide(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //using {Math} instead of {Mathf}, because Mathf.Sign returns {1} for {0}!
        return Math.Sign((p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y));
    }

    #region Abstract Implementation

    public override void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        C_MF.mesh = mesh;
        if (OptimizeMesh)
        {
            C_MF.mesh.Optimize();
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
