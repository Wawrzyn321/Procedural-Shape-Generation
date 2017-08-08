using UnityEngine;
using PSG;
using System.Collections.Generic;

//DISABLE COLLIDERS

public class ConvexMeshDemo : MonoBehaviour {

    private MeshBase[] meshes;
    private ConvexMesh convexMesh;
    private JointMotor2D C_JM2D;

    private Vector3[] allVertices;

	void Start () {

        C_JM2D = new JointMotor2D()
        {
            motorSpeed = 100f,
            maxMotorTorque = 10f
        };

        meshes = new MeshBase[5];

        meshes[0] = AddGear();
        meshes[1] = AddStar();
        meshes[2] = AddBox();
        meshes[3] = AddPointedCircle();
        meshes[4] = AddTriangleMesh();

        int v = 0;
        for(int i = 0; i < meshes.Length; i++)
        {
            v += meshes[i].GetVertices().Length;
        }
        allVertices = new Vector3[v];

        convexMesh = ConvexMesh.AddConvexMesh(Vector3.zero, UpdateVertices(), null, false);
        convexMesh.C_MR.sortingOrder = -1;
        convexMesh.C_MR.material.color = Color.white * 0.5f;
    }

    void FixedUpdate()
    {
        convexMesh.Build(UpdateVertices());
    }

    private Vector3[] UpdateVertices()
    {
        int index = 0;
        for(int i = 0; i < meshes.Length; i++)
        {
            Vector3[] vertices = meshes[i].GetVertices();
            for(int j =0;j<vertices.Length;j++, index++)
            {
                allVertices[index] = meshes[i].transform.TransformPoint(vertices[j]);
            }
        }
        return allVertices;
    }

    private MeshBase AddGear()
    {
        Vector3 pos = new Vector3(-7, 3, 0);
        GearMesh gearMesh = GearMesh.AddGearMesh(pos, 0, 0.9f, 1.2f, 5, null, false);
        gearMesh.SetColor(Color.red);
        gearMesh.AddHingeJoint(C_JM2D);
        gearMesh.GetComponent<Collider2D>().enabled = false;
        return gearMesh;
    }

    private MeshBase AddStar()
    {
        Vector3 pos = new Vector3(1.5f, 3, 0);
        StarMesh starMesh = StarMesh.AddStartMesh(pos, 0.7f, 1.4f, 12, null, false);
        starMesh.SetColor(Color.yellow);
        starMesh.AddHingeJoint(C_JM2D);
        starMesh.GetComponent<Collider2D>().enabled = false;
        return starMesh;
    }

    private MeshBase AddBox()
    {
        Vector3 pos = new Vector3(0, -4, 0);
        RectangleMesh rectangleMesh = RectangleMesh.AddRectangleMesh(pos, new Vector2(0.5f, 2.5f), null, false);
        rectangleMesh.SetColor(Color.blue);
        rectangleMesh.AddHingeJoint(C_JM2D);
        rectangleMesh.GetComponent<Collider2D>().enabled = false;
        return rectangleMesh;
    }

    private MeshBase AddPointedCircle()
    {
        Vector3 pos = new Vector3(6.7f, -1.25f, 0);
        PointedCircleMesh pointedCircleMesh = PointedCircleMesh.AddPointedCircleMesh(pos, 0.8f, 6, new Vector2(2, 2.5f), null, false);
        pointedCircleMesh.SetColor(Color.gray);
        pointedCircleMesh.AddHingeJoint(C_JM2D);
        Collider2D[] ce = pointedCircleMesh.GetComponents<Collider2D>();
        foreach (Collider2D c in ce)
        {
            c.enabled = false;
        }
        return pointedCircleMesh;
    }

    private MeshBase AddTriangleMesh()
    {
        Vector3 pos = new Vector3(-5, 0.7f, 0);
        TriangleMesh triangleMesh = TriangleMesh.AddTriangle(pos, new Vector2(-1, -1), new Vector2(2, 1), new Vector2(-1, 2), null, false);
        triangleMesh.SetColor(Color.green);
        triangleMesh.AddHingeJoint(C_JM2D);
        triangleMesh.GetComponent<Collider2D>().enabled = false;
        return triangleMesh;
    }
}
