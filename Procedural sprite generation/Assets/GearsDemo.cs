using UnityEngine;
using System.Collections;

public class GearsDemo : MonoBehaviour {

    public Material material;
    public Texture squareTexture;

    void Start ()
    {
        AddGearWithMotor(new Vector3(-4.8f, -0.5f, 0), new JointMotor2D { motorSpeed = 100, maxMotorTorque = float.PositiveInfinity });
        AddGear(new Vector3(-2.15f, -1.5f, 0));
        AddGear(new Vector3(0.7f, -1.75f, 0));
        AddGear(new Vector3(3.3f, -0.5f, 0));
        AddBox(new Vector3(0,-2,0));
    }

    private void AddGearWithMotor(Vector3 pos, JointMotor2D C_JM2D)
    {
        GameObject gear = new GameObject();
        gear.transform.position = pos;
        
        GearMesh gearMesh = gear.AddComponent<GearMesh>();
        gearMesh.Build(1, 1.2f, 1.5f, 12, material);
        gearMesh.AddHingeJoint(C_JM2D);

        gear.name = "First gear";
        gear.GetComponent<MeshRenderer>().material.color = Color.gray;
    }

    private void AddGear(Vector3 pos)
    {
        GameObject gear = new GameObject();
        gear.transform.position = pos;

        GearMesh gearMesh = gear.AddComponent<GearMesh>();
        gearMesh.Build(1,1.2f,1.5f, 12, material);
        gearMesh.AddHingeJoint();
        
        gear.GetComponent<MeshRenderer>().material.color = Color.gray;
    }

    private void AddBox(Vector3 pos)
    {
        GameObject box = new GameObject();

        RectangleMesh rectangleMesh = box.AddComponent<RectangleMesh>();
        rectangleMesh.Build(new Vector2(1.2f,1.2f), material);
        rectangleMesh.SetTexture(squareTexture);
       
        box.AddComponent<Rigidbody2D>();
        box.GetComponent<MeshRenderer>().material.color = new Color(0.8f,0.8f, 0.3f);
        box.GetComponent<BoxCollider2D>().sharedMaterial = new PhysicsMaterial2D{bounciness = 1f};
    }
}
