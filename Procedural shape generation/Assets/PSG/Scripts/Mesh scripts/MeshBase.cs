using UnityEngine;
using Random = UnityEngine.Random;

namespace PSG
{

    /// <summary>
    /// Base class for all Meshes.
    /// </summary>
    public abstract class MeshBase : MonoBehaviour
    {
        public static bool Validate = true;

        //mesh data
        [SerializeField, HideInInspector]
        private Vector3[] vertices;
        public Vector3[] Vertices { get { return vertices; } protected set { vertices = value; } }

        [SerializeField, HideInInspector]
        private int[] triangles;
        public int[] Triangles { get { return triangles; } protected set { triangles = value; } }

        [SerializeField, HideInInspector]
        private Vector2[] uvs;
        public Vector2[] UVs { get { return uvs; } protected set { uvs = value; } }


        //common mesh components

        [SerializeField, HideInInspector]
        private Mesh mesh;
        public Mesh _Mesh { get { return mesh; } protected set { mesh = value; } }

        [SerializeField, HideInInspector]
        private MeshFilter c_mf;
        public MeshFilter C_MF { get { return c_mf; } protected set { c_mf = value; } }

        [SerializeField, HideInInspector]
        private MeshRenderer c_mr;
        public MeshRenderer C_MR { get { return c_mr; } protected set { c_mr = value; } }

        //math constants
        protected const float deg90 = Mathf.Deg2Rad * 90f;
        protected const float deg360 = 2 * Mathf.PI;

        public Vector3[] GetVerticesInGlobalSpace()
        {
            Vector3[] verts = new Vector3[Vertices.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = transform.TransformPoint(Vertices[i]);
            }
            return verts;
        }


        public void BuildMesh()
        {
           Material m = MeshHelper.CachedDefaultMaterial;
           BuildMesh(ref m);
        }

        public void BuildMesh(ref Material meshMatt)
        {
            if (!Validate || ValidateMesh())
            {
                MeshHelper.SetupMaterial(ref meshMatt);

                _Mesh = new Mesh();
                GetOrAddComponents();
                C_MR.material = meshMatt;

                BuildMeshComponents();
                UpdateMeshFilter();
                UpdateCollider();
            }
            else
            {
                Debug.LogError("MeshBase::BuildMesh: "+name+" generation failed");
            }
        }


        #region Abstract and Virtual

        ///check if mesh parameters are valid
        protected abstract bool ValidateMesh();

        //get vertices, triangles and UVs
        protected abstract void BuildMeshComponents();

        //update mesh in MeshFilter component
        public virtual void UpdateMeshFilter()
        {
            _Mesh.Clear();
            _Mesh.vertices = Vertices;
            _Mesh.triangles = Triangles;
            _Mesh.uv = UVs;
            _Mesh.normals = MeshHelper.AddMeshNormals(Vertices.Length);
            C_MF.mesh = _Mesh;
        }

        //update attached colliders
        public abstract void UpdateCollider();

        //find necessary components
        public abstract void GetOrAddComponents();

        //most of meshes have only one collider
        public virtual void SetCollidersEnabled(bool enable)
        {
            GetComponent<Collider2D>().enabled = enable;
        }

        #endregion
            
        #region Joints Physics

        //add HingeJoint2D at the center of the object and attach it to background
        public HingeJoint2D AddHingeJoint()
        {
            return AddHingeJoint(transform.position);
        }

        //add HingeJoint2D to the object and attach it to background
        public HingeJoint2D AddHingeJoint(Vector2 position)
        {
            HingeJoint2D C_HJ2D = gameObject.AddComponent<HingeJoint2D>();
            C_HJ2D.anchor = transform.InverseTransformPoint(position);
            return C_HJ2D;
        }

        //specify motor - by default on center
        public HingeJoint2D AddHingeJoint(JointMotor2D C_JM2D)
        {
            return AddHingeJoint(C_JM2D, transform.position);
        }

        //specify motor
        public HingeJoint2D AddHingeJoint(JointMotor2D C_JM2D, Vector2 position)
        {
            HingeJoint2D C_HJ2D = gameObject.AddComponent<HingeJoint2D>();
            C_HJ2D.anchor = transform.InverseTransformPoint(position);
            C_HJ2D.motor = C_JM2D;
            C_HJ2D.useMotor = true;
            return C_HJ2D;
        }

        //specify motor and connected body (on center, by default)
        public HingeJoint2D AddHingeJoint(JointMotor2D C_JM2D, Rigidbody2D connectedBody)
        {
            return AddHingeJoint(C_JM2D, connectedBody, transform.position);
        }

        //specify motor and connected body
        public HingeJoint2D AddHingeJoint(JointMotor2D C_JM2D, Rigidbody2D connectedBody, Vector2 position)
        {
            HingeJoint2D C_HJ2D = gameObject.AddComponent<HingeJoint2D>();
            C_HJ2D.anchor = transform.InverseTransformPoint(position);
            C_HJ2D.motor = C_JM2D;
            C_HJ2D.useMotor = true;
            C_HJ2D.connectedBody = connectedBody;
            return C_HJ2D;
        }

        //fix object to background
        public FixedJoint2D AddFixedJoint()
        {
            FixedJoint2D C_HJ2D = gameObject.AddComponent<FixedJoint2D>();
            C_HJ2D.anchor = transform.InverseTransformPoint(transform.position);
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
            C_FJ2D.anchor = meshA.transform.InverseTransformPoint(meshA.transform.position);
            C_FJ2D.anchor = meshB.transform.InverseTransformPoint(meshB.transform.position);
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
            C_FJ2D.anchor = transform.InverseTransformPoint(transform.position);
            C_FJ2D.anchor = otherMesh.transform.InverseTransformPoint(otherMesh.transform.position);
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
                sharedMaterial = new PhysicsMaterial2D(name+"_PhysicsMaterial2d");
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

        //set color
        public void SetColor(Color color)
        {
            C_MR.material.color = color;
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

        #endregion

    }
    
}