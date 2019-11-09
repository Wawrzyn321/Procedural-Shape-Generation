using UnityEngine;
using System.Collections.Generic;

namespace PSG
{
    /// <summary>
    /// Convex shape, constructed upon given set of points
    /// using QuickHull algorithm (https://en.wikipedia.org/wiki/Quickhull).
    /// 
    /// Colliders:
    ///     - Polygon
    /// 
    /// </summary>
    public class ConvexMesh : MeshBase
    {
        //mesh data
        private Vector3[] baseVertices;
        private Vector3[] vertices;

        //collider
        private PolygonCollider2D C_PC2D;

        #region Static Methods

        public static ConvexMesh AddConvexMesh(Vector3 position, Vector3[] vertices, Material meshMatt = null, bool attachRigidbody = true)
        {
            GameObject convex = new GameObject();

            convex.transform.position = position;

            ConvexMesh convexComponent = convex.AddComponent<ConvexMesh>();
            convexComponent.Build(vertices, meshMatt);
            if (attachRigidbody)
            {
                convex.AddComponent<Rigidbody2D>();
            }
            return convexComponent;
        }

        public static ConvexMesh AddConvexMesh(Vector3 position, List<Vector3> vertices, Material meshMatt = null, bool attachRigidbody = true)
        {
            return AddConvexMesh(position, vertices.ToArray(), meshMatt, attachRigidbody);
        }

        #endregion

        //assign variables, get components and build mesh
        public void Build(Vector3[] vertices, Material meshMatt = null)
        {
            name = "Convex Mesh";
            this.vertices = vertices;

            BuildMesh(ref meshMatt);
        }

        //get points set in constructor
        public Vector3[] GetBasePoints()
        {
            return baseVertices;
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (vertices.Length < 2)
            {
                Debug.LogWarning("ConvexMesh::ValidateMesh: verts count must be greater than 2!");
                return false;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {
            baseVertices = vertices;

            Vertices = QuickHull(new List<Vector3>(vertices)).ToArray();

            Triangles = new int[Vertices.Length * 3];

            for (int i = 1; i < Vertices.Length - 1; i++)
            {
                Triangles[i * 3 + 0] = 0;
                Triangles[i * 3 + 1] = i;
                Triangles[i * 3 + 2] = i + 1;
            }
            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        public override void UpdateCollider()
        {
            C_PC2D.points = MeshHelper.ConvertVec3ToVec2(Vertices);
        }

        public override void GetOrAddComponents()
        {
            C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

        #endregion

        #region Convex Hull Algorithm

        //get general line equation from two given points
        private static Vector3 GetLineEquation(Vector3 a, Vector3 b)
        {
            if (a.x == b.x)
            {
                return new Vector3(1, 0, -a.x);
            }
            else if (a.y == b.y)
            {
                return new Vector3(0, 1, -a.y);
            }
            else
            {
                float bV = (a.x - b.x) / (a.y - b.y);
                float cV = bV * a.y - a.x;
                return new Vector3(1, -bV, cV);
            }
        }

        private static Vector3? GetLeftmostPoint(List<Vector3> setOfPoints)
        {
            Vector3? p = null;
            float mX = float.MaxValue;
            for (int i = 0; i < setOfPoints.Count; i++)
            {
                if (setOfPoints[i].x < mX)
                {
                    mX = setOfPoints[i].x;
                    p = setOfPoints[i];
                }
            }
            return p;
        }

        //get pointer to points of highest x coordinate
        private static Vector3? GetRightmostPoint(List<Vector3> setOfPoints)
        {
            Vector3? p = null;
            float mX = float.MinValue;
            for (int i = 0; i < setOfPoints.Count; i++)
            {
                if (setOfPoints[i].x > mX)
                {
                    mX = setOfPoints[i].x;
                    p = setOfPoints[i];
                }
            }
            return p;
        }

        //get vector of points on the right of two given setOfPoints
        private static List<Vector3> GetPointsOnRight(List<Vector3> setOfPoints, Vector3 vL, Vector3 vP)
        {
            List<Vector3> r = new List<Vector3>();
            for (int i = 0; i < setOfPoints.Count; i++)
            {
                if (MeshHelper.GetSide(setOfPoints[i], vL, vP) > 0)
                {
                    r.Add(setOfPoints[i]);
                }
            }
            return r;
        }

        //get points located the farthest from line of given equation
        private static Vector3? GetFarthestPoint(Vector3 lineEquation, List<Vector3> setOfPoints)
        {
            double len = 0;
            Vector3? r = null;
            for (int i = 0; i < setOfPoints.Count; i++)
            {
                double l =
                    Mathf.Abs(lineEquation.x * setOfPoints[i].x + lineEquation.y * setOfPoints[i].y + lineEquation.z)
                    / Mathf.Sqrt(lineEquation.x * lineEquation.x + lineEquation.y * lineEquation.y);
                if (l > len)
                {
                    len = l;
                    r = setOfPoints[i];
                }
            }
            return r;
        }

        //recurrent subroutine to {QuickHull}
        private static List<Vector3> QuickHullSub(Vector3? A, Vector3? B, List<Vector3> setOfPoints)
        {
            if (A.HasValue == false || B.HasValue == false || setOfPoints == null)
            {
                return null;
            }
            Vector3? C = GetFarthestPoint(GetLineEquation(A.Value, B.Value), setOfPoints);
            if (!C.HasValue)
            {
                return null;
            }

            List<Vector3> s1 = GetPointsOnRight(setOfPoints, A.Value, C.Value);
            List<Vector3> s2 = GetPointsOnRight(setOfPoints, C.Value, B.Value);

            List<Vector3> arg1 = QuickHullSub(A, C, s1);
            List<Vector3> arg3 = QuickHullSub(C, B, s2);

            //QHS(A,C,s1) + C + QHS(C,B,s2)
            List<Vector3> vec = new List<Vector3>();
            if (arg1 != null)
            {
                vec.AddRange(arg1);
            }
            vec.Add(C.Value);
            if (arg3 != null)
            {
                vec.AddRange(arg3);
            }
            return vec;
        }

        //Quick Hull main
        public static List<Vector3> QuickHull(List<Vector3> setOfPoints)
        {
            if (setOfPoints.Count < 2)
            {
                return null;
            }
            Vector3? A = GetLeftmostPoint(setOfPoints);
            Vector3? B = GetRightmostPoint(setOfPoints);

            if (A == null || B == null)
            {
                return null;
            }
            //to get points on left of AB, just get setOfPoints on right of BA!
            List<Vector3> s1 = GetPointsOnRight(setOfPoints, A.Value, B.Value);
            List<Vector3> s2 = GetPointsOnRight(setOfPoints, B.Value, A.Value);

            List<Vector3> arg2 = QuickHullSub(A, B, s1);
            List<Vector3> arg4 = QuickHullSub(B, A, s2);

            //A + QHS(A,B,s1) + B + QHS(B,A,s2)
            List<Vector3> vec = new List<Vector3>();

            vec.Add(A.Value);
            if (arg2 != null)
            {
                vec.AddRange(arg2);
            }
            vec.Add(B.Value);
            if (arg4 != null)
            {
                vec.AddRange(arg4);
            }
            return vec;
        }

        #endregion


    }

}