using UnityEngine;
using System.Collections.Generic;

namespace PSG
{
    /// <summary>
    /// Simple ellipse shape.
    /// If both radiuses are equal, we consider it as a circle.
    /// 
    /// Colliders:
    ///     - Polygon
    /// </summary>
    public class EllipseMesh : MeshBase
    {
        //ellipse data
        private float radiusHorizontal;
        private float radiusVertical;
        private int sides;

        //collider
        private PolygonCollider2D C_PC2D;

        #region Static Methods - building from values and from structure

        public static EllipseMesh AddEllipse(Vector3 position, float radiusHorizontal, float radiusVertical, int sides, Material meshMatt = null, bool attachRigidbody = true)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            GameObject ellipse = new GameObject();
            ellipse.transform.position = position;
            EllipseMesh ellipseComponent = ellipse.AddComponent<EllipseMesh>();
            ellipseComponent.Build(radiusHorizontal, radiusVertical, sides, meshMatt);
            if (attachRigidbody)
            {
                ellipse.AddComponent<Rigidbody2D>();
            }
            return ellipseComponent;
        }

        public static EllipseMesh AddEllipse(Vector3 position, EllipseStructure ellipseStructure, Material meshMatt = null, bool attachRigidbody = true)
        {
            return AddEllipse(position, ellipseStructure.radiusHorizontal, ellipseStructure.radiusVertical, ellipseStructure.sides, meshMatt, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(float radiusHorizontal, float radiusVertical, int sides, Material meshMatt = null)
        {
            MeshHelper.CheckMaterial(ref meshMatt);
            name = "Ellipse";
            this.radiusHorizontal = radiusHorizontal;
            this.radiusVertical = radiusVertical;
            this.sides = sides;

            _Mesh = new Mesh();
            GetOrAddComponents();

            C_MR.material = meshMatt;

            if (BuildEllipse(radiusHorizontal, radiusVertical, sides))
            {
                UpdateMesh();
                UpdateCollider();
            }
        }

        public void Build(EllipseStructure ellipseStructure, Material meshMatt = null)
        {
            Build(ellipseStructure.radiusHorizontal, ellipseStructure.radiusVertical, ellipseStructure.sides, meshMatt);
        }

        #endregion

        //build an ellipse
        private bool BuildEllipse(float radiusA, float radiusB, int sides)
        {

            #region Validity Check

            if (sides < 2)
            {
                Debug.LogWarning("EllipseMesh::BuildEllipse: sides count can't be less than two!");
                return false;
            }
            if (radiusA == 0 || radiusB == 0)
            {
                Debug.LogWarning("EllipseMesh::BuildEllipse: radiuses can't be equal to zero!");
                return false;
            }
            if (radiusA < 0)
            {
                radiusA = -radiusA;
            }
            if (radiusB < 0)
            {
                radiusB = -radiusB;
            }

            #endregion

            Vertices = new Vector3[sides + 1];
            Triangles = new int[3 * sides];
            UVs = new Vector2[sides + 1];

            Vertices[0] = Vector3.zero;
            UVs[0] = Vector3.one * 0.5f;
            float angleDelta = deg360 / sides;
            for (int i = 1; i < sides + 1; i++)
            {
                Vector3 vertPos = new Vector3(Mathf.Cos((i + 1) * angleDelta) * radiusA, Mathf.Sin((i + 1) * angleDelta) * radiusB);
                Vertices[i] = vertPos;
                UVs[i] = new Vector3(vertPos.x / 2 / radiusA, vertPos.y / 2 / radiusB) + new Vector3(0.5f, 0.5f, 0);
                Triangles[(i - 1) * 3 + 0] = 1 + i % sides;
                Triangles[(i - 1) * 3 + 1] = 1 + (i - 1) % sides;
                Triangles[(i - 1) * 3 + 2] = 0;
            }

            return true;
        }

        public EllipseStructure GetStructure()
        {
            return new EllipseStructure
            {
                radiusHorizontal = radiusHorizontal,
                radiusVertical = radiusVertical,
                sides = sides
            };
        }

        #region Abstract Implementation

        public override void UpdateCollider()
        {
            Vector2[] points = new Vector2[sides];
            float angleDelta = deg360 / sides;
            for (int i = 0; i < sides; i++)
            {
                points[i] = new Vector3(Mathf.Cos((i + 1) * angleDelta) * radiusHorizontal, Mathf.Sin((i + 1) * angleDelta) * radiusVertical);
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

    public struct EllipseStructure
    {
        public float radiusHorizontal;
        public float radiusVertical;
        public int sides;
    }
}