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
    
    //constructor
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

        uvs = UVUnwrap(vertices).ToArray();

        return true;
    }

    // get triangle vertices
    public Vector3[] GetVertices()
    {
        return vertices;
    }

    // checks the side vector {v} lays on, relative to segment {v1,v2}
    private int GetSide(Vector3 v1, Vector3 v2, Vector3 v)
    {
        //using {Math} instead of {Mathf}, because Mathf.Sign returns {1} for {0}!
        return Math.Sign((v1.x - v.x) * (v2.y - v.y) - (v2.x - v.x) * (v1.y - v.y));
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
