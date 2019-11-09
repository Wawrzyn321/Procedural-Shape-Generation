using System;
using UnityEngine;
using PSG;

/// <summary>
/// Demo scene for wonders of QuickHull.
///
/// Warning: dragging the bodies inside the hull
/// may create strange results - to prevent this,
/// create new ConvexMesh instead of using Build().
/// </summary>
public class ConvexMeshDemo : MonoBehaviour {

    private MeshBase[] meshes;
    private ConvexMesh convexMesh;
    private JointMotor2D C_JM2D;

    private Vector3[] allVertices;
    private Material convexMaterial;

    void Awake()
    {
        MeshHelper.SetupMaterial(ref convexMaterial);
        convexMaterial.color = new Color(0.8f, 0.8f, 0.9f, 1f);

        C_JM2D = new JointMotor2D
        {
            motorSpeed = 100f,
            maxMotorTorque = 10f
        };
    }

    void Start () {

        meshes = new MeshBase[5];

        meshes[0] = AddGear();
        meshes[1] = AddStar();
        meshes[2] = AddBox();
        meshes[3] = AddPointedCircle();
        meshes[4] = AddTriangleMesh();

        int verticesCount = 0;
        foreach (MeshBase mesh in meshes)
        {
            verticesCount += mesh.Vertices.Length;

            mesh.AddHingeJoint(C_JM2D);
            mesh.SetCollidersEnabled(false);
        }
        allVertices = new Vector3[verticesCount];

        convexMesh = ConvexMesh.AddConvexMesh(Vector3.zero, GetUpdatedVertices(), convexMaterial, false);
        //move body back a bit
        convexMesh.transform.Translate(Vector3.forward * 0.1f);
    }

    void FixedUpdate()
    {
        convexMesh.Build(GetUpdatedVertices(), convexMaterial);
    }

    private Vector3[] GetUpdatedVertices()
    {
        int index = 0;
        for (int i = 0; i < meshes.Length; i++)
        {
            //get transformed vertices
            Vector3[] vertices = meshes[i].GetVerticesInGlobalSpace();

            //assign them to source vertices
            Array.Copy(vertices, 0, allVertices, index, vertices.Length);
            index += vertices.Length;
        }
        return allVertices;
    }

    private GearMesh AddGear()
    {
        Vector3 pos = new Vector3(-7, 3, -0.1f);
        GearMesh gearMesh = GearMesh.AddGear(pos, 0, 0.9f, 1.2f, 5, null, false);
        gearMesh.SetColor(Color.red);
        return gearMesh;
    }

    private StarMesh AddStar()
    {
        Vector3 pos = new Vector3(1.5f, 3, -0.1f);
        StarMesh starMesh = StarMesh.AddStar(pos, 0.7f, 1.4f, 12, null, false);
        starMesh.SetColor(Color.yellow);
        return starMesh;
    }

    private RectangleMesh AddBox()
    {
        Vector3 pos = new Vector3(0, -4, -0.1f);
        RectangleMesh rectangleMesh = RectangleMesh.AddRectangle(pos, new Vector2(0.5f, 2.5f), null, false);
        rectangleMesh.SetColor(Color.blue);
        return rectangleMesh;
    }

    private PointedCircleMesh AddPointedCircle()
    {
        Vector3 pos = new Vector3(6.7f, -1.25f, -0.1f);
        Vector2 shift = new Vector2(2, 2.5f);
        PointedCircleMesh pointedCircleMesh = 
            PointedCircleMesh.AddPointedCircle(pos, 0.8f, 6, shift, null, false);
        pointedCircleMesh.SetColor(Color.gray);
        return pointedCircleMesh;
    }

    private TriangleMesh AddTriangleMesh()
    {
        Vector3 pos = new Vector3(-5, 0.7f, -0.1f);
        Vector2[] vertices =
        {
            new Vector2(-3, -3),
            new Vector2(0, -1),
            new Vector2(-3, 0)
        };
        TriangleMesh triangleMesh = TriangleMesh.AddTriangle(pos, vertices, Space.World, null, false);
        triangleMesh.SetColor(Color.green);
        return triangleMesh;
    }
}
