using UnityEngine;
using System.Collections.Generic;

public class PointedCircleMesh : MeshBase
{
    //mesh data
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector2> uvs;

    //p-circle data
    private float radius;
    private Vector2 shift;
    //private int sides;

    //colliders
    private CircleCollider2D C_CC2D;
    private PolygonCollider2D C_PC2D;

    //constructor
    public void Build(float radius, int sides, Vector2 shift, Material meshMatt)
    {
        name = "PointedCircle";
        this.radius = radius;
        //this.sides = sides;
        this.shift = shift;

        mesh = new Mesh();
        GetOrAddComponents();

        C_MR.material = meshMatt;

        if (BuildPointedCircle(radius, sides, shift))
        {
            UpdateMesh();
            UpdateCollider();
        }
    }
    
    //build p-circle
    private bool BuildPointedCircle(float radius, int sides, Vector2 shift)
    {
        #region Validity Check

        if (sides < 2)
        {
            Debug.LogWarning("PointedCircleMesh::AddPointedCircle: radius can't be equal to zero!");
            return false;
        }
        if (radius == 0)
        {
            Debug.LogWarning("PointedCircleMesh::AddPointedCircle: radius can't be equal to zero!");
            return false;
        }
        if (radius < 0)
        {
            radius = -radius;
        }

        #endregion

        vertices = new List<Vector3>();
        triangles = new List<int>();

        float angleDelta = deg360 / sides;
        vertices.Add(shift);
        for (int i = 1; i < sides+1; i++)
        {
            Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * radius;
            vertices.Add(vertPos);
            triangles.Add(0);
            triangles.Add(1 + (i - 1) % sides);
            triangles.Add(1 + i % sides);
        }
        uvs = UVUnwrap(vertices.ToArray());

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

        if (radius < vertices[0].sqrMagnitude)
        {
            //not added in AddOrGetComponents
            C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();

            Vector2[] C_CC2D_vertices = new Vector2[3];

            float shiftedVertexAngle = Mathf.Atan2(vertices[0].y, vertices[0].x);

            C_CC2D_vertices[0] = vertices[0];
            C_CC2D_vertices[1] = new Vector2(Mathf.Cos(shiftedVertexAngle - Mathf.PI * 0.5f), Mathf.Sin(shiftedVertexAngle - Mathf.PI * 0.5f)) * radius;
            C_CC2D_vertices[2] = new Vector2(Mathf.Cos(shiftedVertexAngle + Mathf.PI * 0.5f), Mathf.Sin(shiftedVertexAngle + Mathf.PI * 0.5f)) * radius;

            C_PC2D.SetPath(0, C_CC2D_vertices);
        }
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