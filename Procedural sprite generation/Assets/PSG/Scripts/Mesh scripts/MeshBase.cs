using UnityEngine;
using Random = UnityEngine.Random;

namespace PSG
{

    /// <summary>
    /// Base class for all Meshes.
    /// </summary>
    public abstract class MeshBase : MonoBehaviour
    {

        //common mesh components
        protected Mesh mesh;
        protected MeshFilter C_MF;
        protected MeshRenderer C_MR;

        //math constants
        protected const float deg90 = Mathf.Deg2Rad * 90f;
        protected const float deg360 = 2 * Mathf.PI;

        #region Abstract and Virtual

        //some meshes have List<>, some arrays
        public abstract Vector3[] GetVertices();
        //update mesh in MeshFilter component
        public abstract void UpdateMesh();
        //update attached colliders
        public abstract void UpdateCollider();
        //find necessary components
        public abstract void GetOrAddComponents();

        //return center of object (this function may be overrided, if origin should be changed)
        public virtual Vector2 GetCenter()
        {
            return transform.position;
        }

        #endregion

        #region Joints Physics

        //add HingeJoint2D at the center of the object and attach it to background
        public HingeJoint2D AddHingeJoint()
        {
            HingeJoint2D C_HJ2D = gameObject.AddComponent<HingeJoint2D>();
            C_HJ2D.anchor = transform.InverseTransformPoint(GetCenter());
            return C_HJ2D;
        }

        //specify motor
        public HingeJoint2D AddHingeJoint(JointMotor2D C_JM2D)
        {
            HingeJoint2D C_HJ2D = gameObject.AddComponent<HingeJoint2D>();
            C_HJ2D.anchor = transform.InverseTransformPoint(GetCenter());
            C_HJ2D.motor = C_JM2D;
            C_HJ2D.useMotor = true;
            return C_HJ2D;
        }

        //specify motor and connected body
        public HingeJoint2D AddHingeJoint(JointMotor2D C_JM2D, Rigidbody2D connectedBody)
        {
            HingeJoint2D C_HJ2D = gameObject.AddComponent<HingeJoint2D>();
            C_HJ2D.anchor = transform.InverseTransformPoint(GetCenter());
            C_HJ2D.motor = C_JM2D;
            C_HJ2D.useMotor = true;
            C_HJ2D.connectedBody = connectedBody;
            return C_HJ2D;
        }
        //fix object to background
        public FixedJoint2D AddFixedJoint()
        {
            FixedJoint2D C_HJ2D = gameObject.AddComponent<FixedJoint2D>();
            C_HJ2D.anchor = transform.InverseTransformPoint(GetCenter());
            return C_HJ2D;
        }

        //joins two shapes by FixedJoint2D
        public static bool Join(MeshBase meshA, MeshBase meshB)
        {
            FixedJoint2D C_FJ2D = meshA.gameObject.AddComponent<FixedJoint2D>();
            C_FJ2D.connectedBody = meshB.GetComponent<Rigidbody2D>();
            if (C_FJ2D.connectedBody)
            {
                return false;
            }
            C_FJ2D.anchor = meshA.transform.InverseTransformPoint(meshA.GetCenter());
            C_FJ2D.anchor = meshB.transform.InverseTransformPoint(meshB.GetCenter());
            return true;
        }

        //join with other shape by FixedJoint2D
        public bool JoinTo(MeshBase otherMesh)
        {
            FixedJoint2D C_FJ2D = gameObject.AddComponent<FixedJoint2D>();
            C_FJ2D.connectedBody = otherMesh.GetComponent<Rigidbody2D>();
            if (C_FJ2D.connectedBody == null)
            {
                return false;
            }
            C_FJ2D.anchor = transform.InverseTransformPoint(GetCenter());
            C_FJ2D.anchor = otherMesh.transform.InverseTransformPoint(otherMesh.GetCenter());
            return true;
        }

        #endregion

        #region Mesh material

        //set physics material properties
        public void SetPhysicsMaterialProperties(float bounciness, float friction)
        {
            PhysicsMaterial2D sharedMaterial = gameObject.GetComponent<Collider2D>().sharedMaterial;
            if (sharedMaterial == null)
            {
                sharedMaterial = new PhysicsMaterial2D();
                gameObject.GetComponent<Collider2D>().sharedMaterial = sharedMaterial;
            }
            sharedMaterial.bounciness = bounciness;
            sharedMaterial.friction = friction;
        }

        //set physics material properties
        public void SetPhysicsMaterial(PhysicsMaterial2D C_PS2D)
        {
            gameObject.GetComponent<Collider2D>().sharedMaterial = C_PS2D;
        }

        //set material to random color
        public void SetRandomColor()
        {
            C_MR.material.color = Random.ColorHSV();
        }

        //set material
        public void SetMaterial(Material material)
        {
            C_MR.material = material;
        }

        //set material texture
        public void SetTexture(Texture texture)
        {
            C_MR.material.mainTexture = texture;
        }

        //set material and texture
        public void SetMaterial(Material material, Texture texture)
        {
            C_MR.material = material;
            C_MR.material.mainTexture = texture;
        }

        //get {MeshRenderer} reference
        public MeshRenderer GetMeshRenderer()
        {
            return C_MR;
        }

        #endregion

    }
    
}