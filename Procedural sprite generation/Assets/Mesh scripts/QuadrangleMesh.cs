using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class QuadrangleMesh : MeshBase {
    
    //mesh data
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    //helper collider array
    private Vector2[] points;

    //collider
    private PolygonCollider2D C_PC2D;

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
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            if (getSide(verts[3], verts[0], verts[1]) * getSide(verts[2], verts[0], verts[1]) <= 0)
            {
                triangles[3] = 3;
                triangles[4] = 1;
                triangles[5] = 0;
                points[0] = verts[0];
                points[1] = verts[3];
                points[2] = verts[1];
                points[3] = verts[2];
            }
            else if (getSide(verts[3], verts[1], verts[2]) * getSide(verts[0], verts[1], verts[2]) <= 0)
            {
                triangles[3] = 1;
                triangles[4] = 2;
                triangles[5] = 3;
                points[0] = verts[0];
                points[1] = verts[1];
                points[2] = verts[3];
                points[3] = verts[2];
            }
            else
            {
                triangles[3] = 0;
                triangles[4] = 2;
                triangles[5] = 3;
                points = verts;
            }
        }
        else
        {
            triangles[0] = 1;
            triangles[1] = 2;
            triangles[2] = 3;
            if (getSide(verts[0], verts[3], verts[1]) <= 0 && getSide(verts[2], verts[3], verts[1]) >= 0)
            {
                triangles[3] = 0;
                triangles[4] = 1;
                triangles[5] = 3;
                points = verts;
            }
            else if (getSide(verts[0], verts[1], verts[2]) <= 0 && getSide(verts[3], verts[1], verts[2]) >= 0)
            {
                triangles[3] = 0;
                triangles[4] = 1;
                triangles[5] = 2;
                points[0] = verts[0];
                points[1] = verts[1];
                points[2] = verts[3];
                points[3] = verts[2];
            }
            else
            {
                triangles[3] = 0;
                triangles[4] = 2;
                triangles[5] = 3;
                points[0] = verts[0];
                points[1] = verts[3];
                points[2] = verts[1];
                points[3] = verts[2];
            }
        }

        uvs = UVUnwrap(vertices).ToArray();

        return true;
    }

    private bool isPointInTriangle(Vector2 v, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        double a1 = getSide(v, v1, v2);
        double a2 = getSide(v, v2, v3);
        double a3 = getSide(v, v3, v1);
        return (a1 >= 0 && a2 >= 0 && a3 >= 0) || (a1 <= 0 && a2 <= 0 && a3 <= 0);
    }

    private double getSide(Vector2 v, Vector2 v1, Vector2 v2)
    {
        return (v1.x - v2.x) * (v.y - v2.y) - (v1.y - v2.y) * (v.x - v2.x);
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
        
        //points[points.Length - 1] = points[0];
        C_PC2D.points = points;
    }

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

    #endregion

}
