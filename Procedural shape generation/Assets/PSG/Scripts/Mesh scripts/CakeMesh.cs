using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Similar to circle, but only given fraction of shape is filled.
    /// If ratio is equal to one, we got a circle.
    /// 
    /// Colliders:
    ///     - Polygon
    /// 
    /// </summary>
    public class CakeMesh : MeshBase
    {
        //cake data
        public float Radius { get; protected set; }
        public int Sides { get; protected set; }
        public int SidesToFill { get; protected set; }

        //colliders
        public PolygonCollider2D C_PC2D { get; protected set; }

        #region Static Methods - building from values and from structure

        public static CakeMesh AddCakeMesh(Vector3 position, float radius, int sides, int sidesToFill, Material meshMat = null, bool attachRigidbody = true)
        {
            GameObject cake = new GameObject();
            cake.transform.position = position;
            CakeMesh cakeComponent = cake.AddComponent<CakeMesh>();
            cakeComponent.Build(radius, sides, sidesToFill, meshMat);
            if (attachRigidbody)
            {
                cake.AddComponent<Rigidbody2D>();
            }
            return cakeComponent;
        }

        public static CakeMesh AddCakeMesh(Vector3 position, CakeStructure cakeStructure, Material meshMat = null, bool attachRigidbody = true)
        {
            return AddCakeMesh(position, cakeStructure.Radius, cakeStructure.Sides, cakeStructure.Sides, meshMat, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(float radius, int sides, int sidesToFill, Material meshMat = null)
        {
            name = "Cake";
            Radius = radius;
            Sides = sides;
            SidesToFill = sidesToFill;

            BuildMesh(ref meshMat);
        }

        public void Build(CakeStructure cakeStructure, Material meshMat = null)
        {
            Build(cakeStructure.Radius, cakeStructure.Sides, cakeStructure.SidesToFill, meshMat);
        }

        #endregion

        public CakeStructure GetStructure()
        {
            return new CakeStructure
            {
                Radius = Radius,
                Sides = Sides,
                SidesToFill = SidesToFill
            };
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (Sides < 2)
            {
                Debug.LogWarning("CakeMesh::ValidateMesh: sides count can't be less than two!");
                return false;
            }
            if (Sides < SidesToFill)
            {
                Debug.LogWarning("CakeMesh::ValidateMesh: sidesToFill can't be biger than sides!");
                return false;
            }
            if (SidesToFill < 1)
            {
                Debug.LogWarning("CakeMesh::ValidateMesh: sidesToFill can't be less than one!");
                return false;
            }
            if (Radius == 0)
            {
                Debug.LogWarning("CakeMesh::ValidateMesh: radius can't be equal to zero!");
                return false;
            }
            if (Radius < 0)
            {
                Radius = -Radius;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {
            Vertices = new Vector3[SidesToFill + 4];
            Triangles = new int[SidesToFill * 3];
            UVs = new Vector2[SidesToFill + 4];

            Vertices[0] = Vector3.zero;
            UVs[0] = Vector3.one * 0.5f;
            float angleDelta = deg360 / Sides;
            for (int i = 0; i < SidesToFill + 2; i++)
            {
                Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * Radius;
                Vertices[i + 1] = vertPos;
                UVs[i + 1] = vertPos / 2 / Radius + new Vector3(0.5f, 0.5f, 0);
            }
            for (int i = 0; i < SidesToFill; i++)
            {
                Triangles[i * 3 + 0] = 1 + i + 1;
                Triangles[i * 3 + 1] = 1 + i;
                Triangles[i * 3 + 2] = 0;
            }
        }

        public override void UpdateCollider()
        {
            Vector2[] points = new Vector2[SidesToFill + 2];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = Vertices[i];
            }
            C_PC2D.points = points;
        }

        public override void GetOrAddComponents()
        {
            C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        #endregion

    }

    public struct CakeStructure
    {
        public float Radius;
        public int Sides;
        public int SidesToFill;
        public Vector2 CenterShift;
    }
}