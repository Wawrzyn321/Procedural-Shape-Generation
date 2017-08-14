using UnityEngine;
using System.Collections.Generic;

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
        private float radius;
        private int sides;
        private int sidesToFill;
        private Vector2 centerShift;

        //colliders
        private PolygonCollider2D C_PC2D;


        #region Static Methods - building from values and from structure

        public static CakeMesh AddCakeMesh(Vector3 position, float radius, int sides, int sidesToFill, Material meshMatt = null, bool attachRigidbody = true)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            GameObject cake = new GameObject();
            cake.transform.position = position;
            CakeMesh cakeComponent = cake.AddComponent<CakeMesh>();
            cakeComponent.Build(radius, sides, sidesToFill, meshMatt);
            if (attachRigidbody)
            {
                cake.AddComponent<Rigidbody2D>();
            }
            return cakeComponent;
        }

        public static CakeMesh AddCakeMesh(Vector3 position, CakeStructure cakeStructure, Material meshMatt = null, bool attachRigidbody = true)
        {
            return AddCakeMesh(position, cakeStructure.radius, cakeStructure.sides, cakeStructure.sides, meshMatt, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(float radius, int sides, int sidesToFill, Material meshMatt = null)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            name = "Cake";

            this.radius = radius;
            this.sides = sides;
            this.sidesToFill = sidesToFill;

            _Mesh = new Mesh();
            GetOrAddComponents();

            C_MR.material = meshMatt;

            if (BuildCake(radius, sides, sidesToFill))
            {
                UpdateMesh();
                UpdateCollider();
            }
        }

        public void Build(CakeStructure cakeStructure, Material meshMatt = null)
        {
            Build(cakeStructure.radius, cakeStructure.sides, cakeStructure.sidesToFill, meshMatt);
        }

        #endregion

        //build a cake
        private bool BuildCake(float radius, int sides, int sidesToFill)
        {
            #region Validity Check

            if (sides < 2)
            {
                Debug.LogWarning("CakeMesh::AddCake: sides count can't be less than two!");
                return false;
            }
            if (sides < sidesToFill)
            {
                Debug.LogWarning("CakeMesh::AddCake: sidesToFill can't be biger than sides!");
                return false;
            }
            if (sidesToFill < 1)
            {
                Debug.LogWarning("CakeMesh::AddCake: sidesToFill can't be less than one!");
                return false;
            }
            if (radius == 0)
            {
                Debug.LogWarning("CakeMesh::AddCake: radius can't be equal to zero!");
                return false;
            }
            if (radius < 0)
            {
                radius = -radius;
            }

            #endregion

            Vertices = new Vector3[sidesToFill+4];
            Triangles = new int[sidesToFill * 3];
            UVs = new Vector2[sidesToFill + 4];

            Vertices[0] = Vector3.zero;
            UVs[0] = Vector3.one * 0.5f;
            float angleDelta = deg360 / sides;
            for (int i = 0; i < sidesToFill+2; i++)
            {
                Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * radius;
                Vertices[i+1] = vertPos;
                UVs[i+1] = vertPos / 2 / radius + new Vector3(0.5f, 0.5f, 0);
            }
            for (int i = 0; i < sidesToFill; i++)
            {
                Triangles[i * 3 + 0] = 1 + i + 1;
                Triangles[i * 3 + 1] = 1 + i;
                Triangles[i * 3 + 2] = 0;
            }

            centerShift = new Vector2(0, 0);
            for (int i = 0; i < Vertices.Length; i++)
            {
                centerShift += (Vector2)Vertices[i];
            }
            centerShift /= Vertices.Length;

            return true;
        }

        public CakeStructure GetStructure()
        {
            return new CakeStructure
            {
                radius = radius,
                sides = sides,
                sidesToFill = sidesToFill,
                centerShift = centerShift
            };
        }

        #region Abstract Implementation

        public override void UpdateCollider()
        {
            Vector2[] points = new Vector2[sidesToFill + 2];
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
        public float radius;
        public int sides;
        public int sidesToFill;
        public Vector2 centerShift;
    }
}