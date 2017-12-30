using UnityEngine;
using PSG;

public class CarDemo : MonoBehaviour {

    public Material material;
    public GameObject[] carElements;

    void Start ()
    {
        ResetCar();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCar();
        }
    }

    private void ResetCar()
    {
        RemoveCar();
        
        carElements = new GameObject[3];

        carElements[0] = new GameObject();
        AddBody(carElements[0]);

        Vector3[] bodyVerts = carElements[0].GetComponent<MeshFilter>().mesh.vertices;

        AddRearWheel(carElements[0], bodyVerts[0]);
        AddFrontWheel(carElements[0], bodyVerts[1]);

        for (int i = 0; i < carElements.Length; i++)
        {
            carElements[i].transform.Translate(new Vector3(-6.5f, -3, 0));
        }
    }
    private void RemoveCar()
    {
        if (carElements != null)
        {
            for (int i = 0; i < carElements.Length; i++)
            {
                Destroy(carElements[i]);
            }
        }
    }
    private void AddBody(GameObject body)
    {
        //add main part
        QuadrangleMesh quadScript = body.AddComponent<QuadrangleMesh>();
        Vector2[] verts = new Vector2[]
        {
            new Vector2(-1.5f,-1) + new Vector2(Random.Range(-0.2f, 0.2f),Random.Range(-0.2f, 0.2f)),
            new Vector2(1.5f,-1) + new Vector2(Random.Range(-0.2f, 0.2f),Random.Range(-0.2f, 0.2f)),
            new Vector2(0.3f,0.5f) + new Vector2(Random.Range(-0.2f, 0.2f),Random.Range(-0.2f, 0.2f)),
            new Vector2(-0.8f,0.5f) + new Vector2(Random.Range(-0.2f, 0.2f),Random.Range(-0.2f, 0.2f))  
        };
        quadScript.Build(verts, material);

        body.AddComponent<Rigidbody2D>();

        quadScript.C_MR.material.color = Color.green;

    }
    private void AddRearWheel(GameObject body, Vector3 position)
    {
        carElements[1] = new GameObject();
        //build rear wheel
        CircleMesh rearWheelScript = carElements[1].AddComponent<CircleMesh>();
        float radius = Random.Range(0.2f, 1f);
        int sides = Random.Range(15, 30);
        rearWheelScript.Build(radius, sides, true, material);
        carElements[1].transform.position = position;
        //add pasive hinge
        rearWheelScript.AddHingeJoint(new JointMotor2D
        {
            motorSpeed = 0,
            maxMotorTorque = 0
        }, body.GetComponent<Rigidbody2D>());

        rearWheelScript.C_MR.material.color = Color.black;
        rearWheelScript.SetPhysicsMaterialProperties(0, 10f);
    }
    private void AddFrontWheel(GameObject body, Vector3 position)
    {
        carElements[2] = new GameObject();
        //build front wheel
        GearMesh frontWheelScript = carElements[2].AddComponent<GearMesh>();
        float radius = Random.Range(0.4f, 0.6f);
        int sides = Random.Range(8, 12);
        frontWheelScript.Build(0.1f, radius, radius+0.2f, sides, material);
        carElements[2].transform.position = position;
        //add motor
        frontWheelScript.AddHingeJoint(new JointMotor2D
        {
            motorSpeed = 300,
            maxMotorTorque = 1000
        }, body.GetComponent<Rigidbody2D>());

        frontWheelScript.C_MR.material.color = Color.black;
        frontWheelScript.SetPhysicsMaterialProperties(0, 10f);
    }
}
