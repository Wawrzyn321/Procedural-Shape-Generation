using UnityEngine;
using PSG;

public class ConvexMeshDemo : MonoBehaviour {

    private MeshBase[] meshes;
    private ConvexMesh convexMesh;
    private JointMotor2D C_JM2D;

    private Vector3[] allVertices;

    private Material convexMaterial;

    void Awake()
    {
        MeshHelper.CheckMaterial(ref convexMaterial);
        convexMaterial.color = new Color(0.8f, 0.8f, 0.9f, 1f);

        C_JM2D = new JointMotor2D()
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

        int v = 0;
        for(int i = 0; i < meshes.Length; i++)
        {
            v += meshes[i].Vertices.Length;

            meshes[i].AddHingeJoint(C_JM2D);
            meshes[i].SetCollidersEnabled(false);
        }
        allVertices = new Vector3[v];

        convexMesh = ConvexMesh.AddConvexMesh(Vector3.zero, UpdateVertices(), Space.Self, convexMaterial, false);
        convexMesh.C_MR.sortingOrder = -1;
    }

    void FixedUpdate()
    {
        convexMesh.Build(UpdateVertices(), convexMaterial);
    }

    private Vector3[] UpdateVertices()
    {
        int index = 0;
        for(int i = 0; i < meshes.Length; i++)
        {
            Vector3[] vertices = meshes[i].Vertices;
            for (int j = 0; j < vertices.Length; j++, index++)
            {
                allVertices[index] = meshes[i].transform.TransformPoint(vertices[j]);
            }
        }
        return allVertices;
    }

    private MeshBase AddGear()
    {
        Vector3 pos = new Vector3(-7, 3, 0);
        GearMesh gearMesh = 
            GearMesh.AddGear(pos, 0, 0.9f, 1.2f, 5, null, false);
        gearMesh.SetColor(Color.red);
        return gearMesh;
    }

    private MeshBase AddStar()
    {
        Vector3 pos = new Vector3(1.5f, 3, 0);
        StarMesh starMesh = 
            StarMesh.AddStar(pos, 0.7f, 1.4f, 12, null, false);
        starMesh.SetColor(Color.yellow);
        return starMesh;
    }

    private MeshBase AddBox()
    {
        Vector3 pos = new Vector3(0, -4, 0);
        RectangleMesh rectangleMesh = 
            RectangleMesh.AddRectangle(pos, new Vector2(0.5f, 2.5f), null, false);
        rectangleMesh.SetColor(Color.blue);
        return rectangleMesh;
    }

    private MeshBase AddPointedCircle()
    {
        Vector3 pos = new Vector3(6.7f, -1.25f, 0);
        PointedCircleMesh pointedCircleMesh = 
            PointedCircleMesh.AddPointedCircle(pos, 0.8f, 6, new Vector2(2, 2.5f), null, false);
        pointedCircleMesh.SetColor(Color.gray);
        return pointedCircleMesh;
    }

    private MeshBase AddTriangleMesh()
    {
        Vector3 pos = new Vector3(-5, 0.7f, 0);
        TriangleMesh triangleMesh =
            TriangleMesh.AddTriangle(pos, new Vector2(-3, -3), new Vector2(0, -1), new Vector2(-3, 0), Space.World, null, false);
        triangleMesh.SetColor(Color.green);
        return triangleMesh;
    }
}
