using UnityEngine;
using System.Collections.Generic;
using System;

public class CircleMesh : MeshBase
{
    //mesh data
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector2> uvs;

    //circle data
    private float radius;
    //private int sides;

    //collider
    private CircleCollider2D C_CC2D;

    public void Build(float radius, int sides, Material meshMatt)
    {
        name = "Circle";
        this.radius = radius;
        //this.sides = sides;

        mesh = new Mesh();
        GetOrAddComponents();

        C_MR.material = meshMatt;

        if (BuildCircle(radius, sides))
        {
            UpdateMesh();
            UpdateCollider();
        }
    }
    
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
        uvs.Add(new Vector2(0.5f, 0.5f));
        float angleDelta = deg360 / sides;
        for (int i = 1; i < sides+1; i++)
        {
            Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * radius;
            vertices.Add(vertPos);
            uvs.Add(vertPos / 2 / radius + new Vector3(0.5f, 0.5f, 0));
            triangles.Add(0);
            triangles.Add(1 + (i - 1) % sides);
            triangles.Add(1 + i % sides);
        }

        return true;
    }

    #region Abstract Implementation

    public override void GetOrAddComponents()
    {
        C_CC2D = gameObject.GetOrAddComponent<CircleCollider2D>();
        C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
        C_MF = gameObject.GetOrAddComponent<MeshFilter>();
    }

    public override void UpdateCollider()
    {
        C_CC2D.radius = radius;
    }

    public override void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        C_MF.mesh = mesh;
        if (OptimizeMesh)
        {
            C_MF.mesh.Optimize();
        }
    }

    #endregion

}
