using System.Collections.Generic;
using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Similar to TriangulatedMesh, created upon the spline points.
    /// 
    /// If {useDoubleCollider} is enabled, its collider includes thickness of shape.
    /// In other case, it passes through its center.
    /// 
    /// Colliders:
    ///     - Polygon (if {useDoubleCollider}
    ///     - Edge (in other case)
    /// </summary>
    public class SplineCurveMesh : TriangulableMesh
    {
        //spline data
        private Vector2[] splinePoints;
        private float resolution;
        private float width;
        private bool useDoubleCollider;

        //colliders
        private PolygonCollider2D C_PC2D;
        private EdgeCollider2D C_EC2D;

        #region Static Methods - building from values and from structure

        public static SplineCurveMesh AddSplineCurve(Vector3 position, Vector2[] splinePoints, float resolution, float width, bool useDoubleCollider, Space space, Material meshMatt = null, bool attachRigidbody = true)
        {
            GameObject curve = new GameObject();

            if (space == Space.Self)
            {
                curve.transform.position = position;
            }
            else
            {
                Vector2 center = new Vector2();
                for (int i = 0; i < splinePoints.Length; i++)
                {
                    center += splinePoints[i];
                }
                curve.transform.position = position + (Vector3)center / splinePoints.Length;
            }

            curve.transform.position = position;
            SplineCurveMesh curveComponent = curve.AddComponent<SplineCurveMesh>();
            curveComponent.Build(splinePoints, resolution, width, useDoubleCollider, meshMatt);
            if (attachRigidbody)
            {
                curve.AddComponent<Rigidbody2D>();
            }
            return curveComponent;
        }

        public static SplineCurveMesh AddSplineCurve(Vector3 position, RawSplineCurveStructure structure, Material meshMatt = null, bool attachRigidbody = true)
        {
            return AddSplineCurve(position, structure.splinePoints, structure.resolution, structure.width, structure.useDoubleCollider, Space.Self, meshMatt, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(Vector2[] splinePoints, float resolution, float width, bool useDoubleCollider, Material meshMatt = null)
        {
            name = "Spline curve mesh";
            this.splinePoints = splinePoints;
            this.resolution = resolution;
            this.width = width;
            this.useDoubleCollider = useDoubleCollider;

            BuildMesh(ref meshMatt);
        }

        public void Build(RawSplineCurveStructure structure, Material meshMatt = null)
        {
            Build(structure.splinePoints, structure.resolution, structure.width, structure.useDoubleCollider, meshMatt);
        }

        #endregion

        public SplineCurveStructure GetStructure()
        {
            return new SplineCurveStructure
            {
                useDoubleCollider = useDoubleCollider,
                resolution = resolution,
                width = width,
                splinePoints = splinePoints,
                vertices = Vertices
            };
        }

        public RawSplineCurveStructure GetRawStructure()
        {
            return new RawSplineCurveStructure
            {
                useDoubleCollider = useDoubleCollider,
                resolution = resolution,
                width = width,
                splinePoints = splinePoints
            };
        }

        protected override bool ValidateMesh()
        {
            if (MeshHelper.HasDuplicates(splinePoints))
            {
                Debug.LogWarning("SplineCurveMesh::ValidateMesh: Duplicate points detected!");
                return false;
            }
            if (resolution < CatmullRomSpline.MIN_RESOLUTION)
            {
                resolution = CatmullRomSpline.MIN_RESOLUTION;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {
            List<Vector2> curvePoints = CatmullRomSpline.GetPoints(splinePoints, resolution, false);
            int len = curvePoints.Count;
            if (len <= 1) return;

            List<Vector2> leftCurvePoints = new List<Vector2>();
            List<Vector2> rightCurvePoints = new List<Vector2>();
            // first vertex
            {
                Vector2 dir = (curvePoints[1] - curvePoints[0]).normalized;
                dir = new Vector2(-dir.y, dir.x) * width;
                leftCurvePoints.Add(curvePoints[0] - dir);
                rightCurvePoints.Add(curvePoints[0] + dir);
            }
            // second to last - 1 vertices
            for (int i = 1; i < len - 1; i++)
            {
                float leftAngle = Mathf.Atan2(curvePoints[i].y - curvePoints[i - 1].y, curvePoints[i].x - curvePoints[i - 1].x) * Mathf.Rad2Deg + 90;
                float rightAngle = Mathf.Atan2(curvePoints[i + 1].y - curvePoints[i].y, curvePoints[i + 1].x - curvePoints[i].x) * Mathf.Rad2Deg + 90;
                float middleAngle = leftAngle + Mathf.DeltaAngle(leftAngle, rightAngle) * 0.5f;
                Vector2 dir = new Vector2(Mathf.Cos(middleAngle * Mathf.Deg2Rad), Mathf.Sin(middleAngle * Mathf.Deg2Rad)) * width;
                leftCurvePoints.Add(curvePoints[i] - dir);
                rightCurvePoints.Add(curvePoints[i] + dir);
            }
            // last vertex
            {
                Vector2 dir = (curvePoints[len - 2] - curvePoints[len - 1]).normalized;
                dir = new Vector2(-dir.y, dir.x) * width;
                leftCurvePoints.Add(curvePoints[len - 1] + dir);
                rightCurvePoints.Add(curvePoints[len - 1] - dir);
            }

            Vertices = new Vector3[leftCurvePoints.Count * 2];
            for (int i = 0; i < leftCurvePoints.Count; i++)
            {
                Vertices[i * 2] = leftCurvePoints[i];
                Vertices[i * 2 + 1] = rightCurvePoints[i];
            }

            Triangles = new int[(leftCurvePoints.Count - 1) * 6]; // number of quads * 6
            int triangleIndex = 0;
            for (int i = 0; i < leftCurvePoints.Count - 1; i++)
            {
                int ii = i * 2;
                Triangles[triangleIndex++] = ii;
                Triangles[triangleIndex++] = ii + 3;
                Triangles[triangleIndex++] = ii + 1;
                Triangles[triangleIndex++] = ii;
                Triangles[triangleIndex++] = ii + 3;
                Triangles[triangleIndex++] = ii + 2;
            }

        }

        public override void UpdateCollider()
        {
            if (useDoubleCollider)
            {
                C_PC2D.points = MeshHelper.ConvertVec3ToVec2(Vertices);
            }
            else
            {
                C_EC2D.points = CatmullRomSpline.GetPoints(splinePoints, resolution, false).ToArray();
            }
        }

        public override void GetOrAddComponents()
        {
            if (useDoubleCollider)
            {
                C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
            }
            else
            {
                C_EC2D = gameObject.GetOrAddComponent<EdgeCollider2D>();
            }
            C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
            C_MF = gameObject.GetOrAddComponent<MeshFilter>();
        }

    }

    [System.Serializable]
    public struct RawSplineCurveStructure
    {
        public Vector2[] splinePoints;
        public float resolution;
        public float width;
        public bool useDoubleCollider;
    }

    [System.Serializable]
    public struct SplineCurveStructure
    {
        public Vector2[] splinePoints;
        public float resolution;
        public float width;
        public Vector3[] vertices;
        public bool useDoubleCollider;
    }
}