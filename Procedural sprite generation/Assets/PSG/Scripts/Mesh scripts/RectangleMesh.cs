using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Rectangle shape fo PSG.
    /// 
    /// Colliders:
    ///     - Box
    /// </summary>
    public class RectangleMesh : MeshBase
    {

        //mesh parameter
        private Vector2 size;

        //colliders
        private BoxCollider2D C_BC2D;

        #region Static Methods
        
        public static RectangleMesh AddRectangle(Vector3 position, Vector2 size, Material meshMatt = null, bool attachRigidbody = true)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            GameObject rectangleMesh = new GameObject();
            rectangleMesh.transform.position = position;
            RectangleMesh rectangleComponent = rectangleMesh.AddComponent<RectangleMesh>();
            rectangleComponent.Build(size, meshMatt);
            if (attachRigidbody)
            {
                rectangleMesh.AddComponent<Rigidbody2D>();
            }
            return rectangleComponent;
        }

        // fill area {from}, {to} by rectangle
        public static RectangleMesh FillRectangle(Vector3 from, Vector3 to, Material meshMatt = null, bool attachRigidbody = true)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            GameObject rectangleMesh = new GameObject();
            rectangleMesh.transform.position = (from + to) / 2;
            RectangleMesh rectangleComponent = rectangleMesh.AddComponent<RectangleMesh>();
            rectangleComponent.Build(to - from, meshMatt);
            if (attachRigidbody)
            {
                rectangleMesh.AddComponent<Rigidbody2D>();
            }
            return rectangleComponent;
        }

        // build rectangle from Rect
        public static RectangleMesh FillRectangle(Rect rect, Material meshMatt = null, bool attachRigidbody = true)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            GameObject rectangleMesh = new GameObject();
            rectangleMesh.transform.position = rect.center;
            RectangleMesh rectangleComponent = rectangleMesh.AddComponent<RectangleMesh>();
            rectangleComponent.Build(rect.size, meshMatt);
            if (attachRigidbody)
            {
                rectangleMesh.AddComponent<Rigidbody2D>();
            }
            return rectangleComponent;
        }

        #endregion

        //assign variables, get components and build mesh
        public void Build(Vector2 size, Material meshMatt = null)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            name = "Rectangle";
            this.size = size;

            _Mesh = new Mesh();
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

            Vertices = new Vector3[]
            {
            new Vector3(-size.x*0.5f, -size.y*0.5f, 0), //topleft
            new Vector3(size.x*0.5f, -size.y*0.5f, 0), //topright
            new Vector3(size.x*0.5f, size.y*0.5f, 0), //downleft
            new Vector3(-size.x*0.5f, size.y*0.5f, 0), //downright
            };

            Triangles = new int[] { 1, 0, 2, 2, 0, 3 };

            UVs = new Vector2[]
            {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
            };
            return true;
        }

        //convert to quad
        public QuadrangleMesh ToQuad(bool attachRigidbody = true)
        {
            return QuadrangleMesh.AddQuadrangle(transform.position, MeshHelper.ConvertVec3ToVec2(Vertices), C_MR.material, attachRigidbody);
        }

        //get dimensions of box - equivalent to GetStructure
        public Vector2 GetSize()
        {
            return size;
        }

        #region Abstract Implementation

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

}