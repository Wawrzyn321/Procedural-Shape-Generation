using System;
using UnityEngine;
public class RectangleMesh : MeshBase
{

    //mesh parameter
    private Vector2 size;

    //mesh data
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    //colliders
    private BoxCollider2D C_BC2D;

    public static GameObject AddRectangleMesh(Vector3 position, Vector2 size, Material meshMatt, bool attachRigidbody = true)
    {
        GameObject rectangleMesh = new GameObject();
        rectangleMesh.transform.position = position;
        rectangleMesh.AddComponent<RectangleMesh>().Build(size, meshMatt);
        if (attachRigidbody)
        {
            rectangleMesh.AddComponent<Rigidbody2D>();
        }
        return rectangleMesh;
    }

    //assign variables, get components and build mesh
    public void Build(Vector2 size, Material meshMatt)
    {
        name = "Rectangle";
        this.size = size;

        mesh = new Mesh();
        GetOrAddComponents();

        C_MR.material = meshMatt;

        if (BuildRectangle(size))
        {
            UpdateMesh();
            UpdateCollider();
        }
    }

    //build a box
    private bool BuildRectangle(Vector2 size)
    {
        #region  Validity Check

        if (size.x == 0 || size.y == 0)
        {
            Debug.LogWarning("RectangleMesh::BuildRectangle: Size of box can't be zero!");
            return false;
        }

        #endregion

        vertices = new Vector3[]
        {
            new Vector3(-size.x*0.5f, -size.y*0.5f, 0), //topleft
            new Vector3(size.x*0.5f, -size.y*0.5f, 0), //topright
            new Vector3(size.x*0.5f, size.y*0.5f, 0), //downleft
            new Vector3(-size.x*0.5f, size.y*0.5f, 0), //downright
        };

        triangles = new int[] {1, 0, 2, 2, 0, 3};

        uvs = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };
        return true;
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
        C_BC2D.size = size;
    }
    public override void GetOrAddComponents()
    {
        C_BC2D = gameObject.GetOrAddComponent<BoxCollider2D>();
        C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
        C_MF = gameObject.GetOrAddComponent<MeshFilter>();
    }

    #endregion

}
