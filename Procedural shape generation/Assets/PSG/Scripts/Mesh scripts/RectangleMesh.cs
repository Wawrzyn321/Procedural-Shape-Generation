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
        public Vector2 Size { get; protected set; }

        //colliders
        public BoxCollider2D C_BC2D { get; protected set; }

        #region Static Methods
        
        public static RectangleMesh AddRectangle(Vector3 position, Vector2 size, Material meshMatt = null, bool attachRigidbody = true)
        {
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
            name = "Rectangle";
            Size = size;

            BuildMesh(ref meshMatt);
        }

        //convert to quad
        public QuadrangleMesh ToQuad(bool attachRigidbody = true)
        {
            return QuadrangleMesh.AddQuadrangle(transform.position, MeshHelper.ConvertVec3ToVec2(Vertices), Space.World, C_MR.material, attachRigidbody);
        }

        //get dimensions of box - equivalent to GetStructure
        public Vector2 GetSize()
        {
            return Size;
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (Size.x == 0 || Size.y == 0)
            {
                Debug.LogWarning("RectangleMesh::ValidateMesh: Size of box can't be zero!");
                return false;
            }
            if (Size.x < 0)
            {
                Size = new Vector2(-Size.x, Size.y);
            }
            if (Size.y < 0)
            {
                Size = new Vector2(Size.x, -Size.y);
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {

            Vertices = new Vector3[]
            {
            new Vector3(-Size.x*0.5f, -Size.y*0.5f, 0), //topleft
            new Vector3(Size.x*0.5f, -Size.y*0.5f, 0), //topright
            new Vector3(Size.x*0.5f, Size.y*0.5f, 0), //downleft
            new Vector3(-Size.x*0.5f, Size.y*0.5f, 0), //downright
            };

            Triangles = new int[] { 1, 0, 2, 2, 0, 3 };

            UVs = new Vector2[]
            {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
            };
        }

        public override void UpdateCollider()
        {
            C_BC2D.size = Size;
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