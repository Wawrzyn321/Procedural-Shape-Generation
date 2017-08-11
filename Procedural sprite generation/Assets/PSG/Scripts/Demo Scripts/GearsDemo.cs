using UnityEngine;
using PSG;

/// <summary>
/// Demo scene showing example of gears with motor.
/// </summary>
public class GearsDemo : MonoBehaviour {
        
    public Texture squareTexture;

    void Start ()
    {
        AddGearWithMotor(new Vector3(-4.8f, -0.5f, 0), new JointMotor2D
        {
            motorSpeed = 100,
            maxMotorTorque = 50000
        });
        AddGear(new Vector3(-2.15f, -1.5f, 0));
        AddGear(new Vector3(0.7f, -1.75f, 0));
        AddGear(new Vector3(3.3f, -0.5f, 0));
        AddBox(new Vector3(0,-2,0));
    }

    private void AddGearWithMotor(Vector3 pos, JointMotor2D C_JM2D)
    {
        //alternative way to add Mesh

        //add GameObject and move it to desired position
        GameObject gear = new GameObject();
        gear.transform.position = pos;
        
        //add GearMesh component, build the Gear and add a motor
        GearMesh gearMesh = gear.AddComponent<GearMesh>();
        gearMesh.Build(1, 1.2f, 1.5f, 12);
        gearMesh.AddHingeJoint(C_JM2D);
        
        //override default name
        gear.name = "Motor gear";

        //set mesh's color
        gearMesh.C_MR.material.color = Color.gray;
    }

    private void AddGear(Vector3 pos)
    {
        GameObject gear = new GameObject();
        gear.transform.position = pos;

        GearMesh gearMesh = gear.AddComponent<GearMesh>();
        gearMesh.Build(1,1.2f,1.5f, 12);
        gearMesh.AddHingeJoint();
        
        gearMesh.C_MR.material.color = Color.gray;
    }

    private void AddBox(Vector3 pos)
    {
        GameObject box = new GameObject();

        RectangleMesh rectangleMesh = box.AddComponent<RectangleMesh>();
        rectangleMesh.Build(new Vector2(1.2f,1.2f));
        rectangleMesh.SetTexture(squareTexture);
        rectangleMesh.SetPhysicsMaterialProperties(1,0);
       
        box.AddComponent<Rigidbody2D>();
        rectangleMesh.C_MR.material.color = new Color(0.8f,0.8f, 0.3f);
    }
}
