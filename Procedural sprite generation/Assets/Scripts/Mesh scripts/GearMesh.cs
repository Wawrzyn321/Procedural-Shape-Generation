using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
    private PolygonCollider2D C_EC2D_inner;
    private PolygonCollider2D C_EC2D_outer;

    //construct
    public void Build(float innerRadius, float rootRadius, float outerRadius, int sides, Material meshMatt)
    {
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

        int doubleSides = 2*sides;

        vertices = new List<Vector3>();
        triangles = new List<int>();

        float angleDelta = deg360/doubleSides;
        float angleShift = angleDelta*0.5f;
        float outerAngleShift = angleDelta*0.2f;
        
        for (int i = 0; i < doubleSides; i++)
        {
            Vector3 innerVertPos =
                new Vector3(Mathf.Cos(i*angleDelta + angleShift), Mathf.Sin(i*angleDelta + angleShift))*innerRadius;
            Vector3 rootVertPos =
                new Vector3(Mathf.Cos(i*angleDelta + angleShift), Mathf.Sin(i*angleDelta + angleShift))*rootRadius;
            Vector3 outerVertPos;
            if (i%2 == 0)
            {
                outerVertPos =
                    new Vector3(Mathf.Cos(i*angleDelta + angleShift + outerAngleShift), Mathf.Sin(i*angleDelta + angleShift+ outerAngleShift))*outerRadius;
            }
            else
            {
                outerVertPos =
                    new Vector3(Mathf.Cos(i*angleDelta + angleShift - outerAngleShift), Mathf.Sin(i*angleDelta + angleShift - outerAngleShift))*outerRadius;
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
        uvs = UVUnwrap(vertices.ToArray());

        return true;
    }  

    #region Abstract Implementation

    public override void GetOrAddComponents()
    {
        if (innerRadius == 0)
        {
            C_EC2D_outer = gameObject.GetOrAddComponent<PolygonCollider2D>();
        }
        else
        {
            PolygonCollider2D[] C_EC2Ds = GetComponents<PolygonCollider2D>();
            switch (C_EC2Ds.Length)
            {
                case 0:
                    C_EC2D_inner = gameObject.AddComponent<PolygonCollider2D>();
                    C_EC2D_outer = gameObject.AddComponent<PolygonCollider2D>();
                    break;
                case 1:
                    C_EC2D_inner = C_EC2Ds[0];
                    C_EC2D_outer = gameObject.AddComponent<PolygonCollider2D>();
                    break;
                default:
                    C_EC2D_inner = C_EC2Ds[0];
                    C_EC2D_outer = C_EC2Ds[1];
                    break;
            }
        }
        C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
        C_MF = gameObject.GetOrAddComponent<MeshFilter>();
    }

    public override void UpdateCollider()
    {
        if (innerRadius != 0)
        {
            Vector2[] points_inner = new Vector2[sides*2];
            for (int i = 0; i < points_inner.Length; i++)
            {
                points_inner[i] = vertices[i*3];
            }
            C_EC2D_inner.points = points_inner;
        }
        if (outerRadius != rootRadius)
        {
            Vector2[] points_outer = new Vector2[4*sides];
            for (int i = 0; i < sides; i++)
            {
                points_outer[4*i + 0] = vertices[i*6 + 1];
                points_outer[4*i + 1] = vertices[i*6 + 2];
                points_outer[4*i + 2] = vertices[i*6 + 5];
                points_outer[4*i + 3] = vertices[i*6 + 4];
            }
            C_EC2D_outer.points = points_outer;
        }
        else
        {
            Vector2[] points_outer = new Vector2[sides*2];
            float angleDelta = deg360 / sides/2;
            float angleShift = angleDelta/2;
            for (int i = 0; i < sides*2; i++)
            {
                float x = Mathf.Cos(i*angleDelta + angleShift);
                float y = Mathf.Sin(i*angleDelta + angleShift);
                points_outer[i] = new Vector2(x, y) * outerRadius;
            }
            C_EC2D_outer.points = points_outer;
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
