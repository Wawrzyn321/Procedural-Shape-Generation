using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Gear for PSG.
    /// First radius ({innerRadius}) can be zero.
    /// 
    /// Colliders:
    ///     - Polygon
    /// </summary>
    public class GearMesh : MeshBase
    {

        //gear data
        private int sides;
        private float outerRadius;
        private float rootRadius;
        private float innerRadius;

        //colliders
        private PolygonCollider2D C_PC2D;

        #region Static Methods - building from values and from structure

        public static GearMesh AddGear(Vector3 position, float innerRadius, float rootRadius, float outerRadius, int sides, Material meshMatt = null, bool attachRigidbody = true)
        {
            MeshHelper.SetupMaterial(ref meshMatt);
            GameObject gear = new GameObject();
            gear.transform.position = position;

            GearMesh gearComponent = gear.AddComponent<GearMesh>();
            gearComponent.Build(innerRadius, rootRadius, outerRadius, sides, meshMatt);
            if (attachRigidbody)
            {
                gear.AddComponent<Rigidbody2D>();
            }
            return gearComponent;
        }

        public static GearMesh AddGear(Vector3 position, GearStructure gearStructure, Material meshMatt = null, bool attachRigidbody = true)
        {
            return AddGear(position, gearStructure.innerRadius, gearStructure.rootRadius, gearStructure.outerRadius, gearStructure.sides, meshMatt, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(float innerRadius, float rootRadius, float outerRadius, int sides, Material meshMatt = null)
        {
            MeshHelper.SetupMaterial(ref meshMatt);
            name = "Gear";
            this.innerRadius = innerRadius;
            this.rootRadius = rootRadius;
            this.outerRadius = outerRadius;
            this.sides = sides;

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

        public void Build(GearStructure gearStructure, Material meshMatt = null)
        {
            Build(gearStructure.innerRadius, gearStructure.rootRadius, gearStructure.outerRadius, gearStructure.sides, meshMatt);
        }

        #endregion

        public GearStructure GetStructure()
        {
            return new GearStructure
            {
                innerRadius = innerRadius,
                rootRadius = rootRadius,
                outerRadius = outerRadius,
                sides = sides
            };
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (sides < 2)
            {
                Debug.LogWarning("GearMesh::ValidateMesh: sides count can't be less than two!");
                return false;
            }
            if (rootRadius == 0)
            {
                Debug.LogWarning("GearMesh::ValidateMesh: rootRadius can't be equal to zero!");
                return false;
            }
            if (outerRadius == 0)
            {
                Debug.LogWarning("GearMesh::ValidateMesh: outerRadius can't be equal to zero!");
                return false;
            }
            if (innerRadius < 0)
            {
                innerRadius = -innerRadius;
            }
            if (rootRadius < 0)
            {
                rootRadius = -rootRadius;
            }
            if (innerRadius < 0)
            {
                outerRadius = -outerRadius;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {
            int doubleSides = 2 * sides;

            Vertices = new Vector3[6 * sides];
            Triangles = new int[6 * 3 * sides];

            float angleDelta = deg360 / doubleSides;
            float angleShift = angleDelta * 0.5f;
            float outerAngleShift = angleDelta * 0.2f;

            int triangleIndex = 0;

            for (int i = 0; i < doubleSides; i++)
            {
                Vector3 innerVertPos =
                    new Vector3(Mathf.Cos(i * angleDelta + angleShift), Mathf.Sin(i * angleDelta + angleShift)) * innerRadius;
                Vector3 rootVertPos =
                    new Vector3(Mathf.Cos(i * angleDelta + angleShift), Mathf.Sin(i * angleDelta + angleShift)) * rootRadius;
                Vector3 outerVertPos;
                if (i % 2 == 0)
                {
                    outerVertPos =
                        new Vector3(Mathf.Cos(i * angleDelta + angleShift + outerAngleShift), Mathf.Sin(i * angleDelta + angleShift + outerAngleShift)) * outerRadius;
                }
                else
                {
                    outerVertPos =
                        new Vector3(Mathf.Cos(i * angleDelta + angleShift - outerAngleShift), Mathf.Sin(i * angleDelta + angleShift - outerAngleShift)) * outerRadius;
                }
                Vertices[i * 3 + 0] = innerVertPos;
                Vertices[i * 3 + 1] = rootVertPos;
                Vertices[i * 3 + 2] = outerVertPos;

                int a = 3 * i;
                int b = 3 * i + 1;
                int c = (3 * (i + 1)) % (3 * doubleSides);
                int d = (3 * (i + 1) + 1) % (3 * doubleSides);
                Triangles[triangleIndex++] = d;
                Triangles[triangleIndex++] = b;
                Triangles[triangleIndex++] = c;
                Triangles[triangleIndex++] = b;
                Triangles[triangleIndex++] = a;
                Triangles[triangleIndex++] = c;

                //add tooth
                if (i % 2 == 0)
                {
                    a = 3 * i + 1;
                    b = 3 * i + 2;
                    c = (3 * (i + 1) + 1) % (3 * doubleSides);
                    d = (3 * (i + 1) + 2) % (3 * doubleSides);
                    Triangles[triangleIndex++] = d;
                    Triangles[triangleIndex++] = b;
                    Triangles[triangleIndex++] = c;
                    Triangles[triangleIndex++] = b;
                    Triangles[triangleIndex++] = a;
                    Triangles[triangleIndex++] = c;
                }
            }
            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        public override void UpdateCollider()
        {
            bool isHollow = innerRadius > 0;
            int numberOfPoints = isHollow ? sides * 6+2 : sides * 4;
            Vector2[] colliderPoints = new Vector2[numberOfPoints];
            for (int i = 0; i < sides; i++)
            {
                colliderPoints[4 * i + 0] = Vertices[i * 6 + 1];
                colliderPoints[4 * i + 1] = Vertices[i * 6 + 2];
                colliderPoints[4 * i + 2] = Vertices[i * 6 + 5];
                colliderPoints[4 * i + 3] = Vertices[i * 6 + 4];
            }
            if (isHollow)
            {
                colliderPoints[4 * sides] = colliderPoints[0];
                for (int i = 0; i < sides * 2; i++)
                {
                    colliderPoints[sides * 4 + i+1] = Vertices[i * 3];
                }
                colliderPoints[numberOfPoints-1] = Vertices[0];
            }
            C_PC2D.points = colliderPoints;
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
    public struct GearStructure
    {
        public float innerRadius;
        public float rootRadius;
        public float outerRadius;
        public int sides;
    }
}