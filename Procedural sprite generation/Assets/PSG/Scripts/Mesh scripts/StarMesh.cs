using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Mesh formed in simple star shape.
    /// 
    /// Colliders:
    ///     - polygon
    /// </summary>
    /// 
    public class StarMesh : MeshBase
    {
        //star data
        private float radiusA; //horizontal radius
        private float radiusB; //vertical radius
        private int sides;

        //collider
        private PolygonCollider2D C_PC2D;

        #region Static Methods - building from values and from structure

        public static StarMesh AddStar(Vector3 position, float radiusA, float radiusB, int sides, Material meshMatt = null, bool attachRigidbody = true)
        {
            GameObject star = new GameObject();
            star.transform.position = position;

            StarMesh starComponent = star.AddComponent<StarMesh>();
            starComponent.Build(radiusA, radiusB, sides, meshMatt);
            if (attachRigidbody)
            {
                star.AddComponent<Rigidbody2D>();
            }
            return starComponent;
        }

        public static StarMesh AddStar(Vector3 position, StarStructure starStructure, Material meshMatt = null, bool attachRigidbody = true)
        {
            return AddStar(position, starStructure.radiusA, starStructure.radiusB, starStructure.sides, meshMatt, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(float radiusA, float radiusB, int sides, Material meshMatt = null)
        {
            name = "Star";

            this.sides = sides;
            this.radiusA = radiusA;
            this.radiusB = radiusB;
            _Mesh = new Mesh();
            
            GetOrAddComponents();

            C_MR.material = meshMatt;

            if (!Validate || ValidateMesh())
            {
                BuildMeshComponents();
                UpdateMeshFilter();
                UpdateCollider();
            }
        }

        public void Build(StarStructure starStructure, Material meshMatt = null)
        {
            Build(starStructure.radiusA, starStructure.radiusB, starStructure.sides, meshMatt);
        }

        #endregion

        public StarStructure GetStructure()
        {
            return new StarStructure
            {
                radiusA = radiusA,
                radiusB = radiusB,
                sides = sides
            };
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (sides < 2)
            {
                Debug.LogWarning("StarMesh::ValidateMesh: sides count can't be less than two!");
                return false;
            }
            if (radiusA == 0 || radiusB == 0)
            {
                Debug.LogWarning("StarMesh::ValidateMesh: any of radiuses can't be equal to zero!");
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
            return true;
        }

        protected override void BuildMeshComponents()
        {
            Vertices = new Vector3[1 + sides * 2];
            Triangles = new int[2 * sides * 3];
            UVs = new Vector2[1 + sides * 2];

            Vertices[0] = new Vector3(0, 0);
            float angleDelta = 360 / (float)sides / 2 * Mathf.Deg2Rad;
            float angleShift = -360f / (sides * 4) * Mathf.Deg2Rad;
            for (int i = 0; i < sides * 2; i++)
            {
                Vector3 vertVec = new Vector3(Mathf.Cos(i * angleDelta + angleShift), Mathf.Sin(i * angleDelta + angleShift));
                Vertices[1 + i] = vertVec * (i % 2 == 0 ? radiusA : radiusB);
                Triangles[(i * 3 + 2) % Triangles.Length] = 0;
                Triangles[(i * 3 + 1) % Triangles.Length] = 1 + i % (sides * 2);
                Triangles[i * 3] = 1 + (i + 1) % (sides * 2);
            }

            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        public override void UpdateCollider()
        {
            Vector2[] points = new Vector2[sides * 2];
            float angleDelta = 360 / (float)points.Length * Mathf.Deg2Rad;
            float angleShift = -360f / (sides * 4) * Mathf.Deg2Rad;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 vertPos = new Vector2(Mathf.Cos(i * angleDelta + angleShift), Mathf.Sin(i * angleDelta + angleShift));
                points[i] = vertPos * (i % 2 == 0 ? radiusA : radiusB);
            }
            C_PC2D.SetPath(0, points);
        }

        public override void GetOrAddComponents()
        {
            C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        #endregion
    }

    [System.Serializable]
    public struct StarStructure
    {
        public float radiusA;
        public float radiusB;
        public int sides;
    }
}