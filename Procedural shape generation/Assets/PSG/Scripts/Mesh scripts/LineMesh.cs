using UnityEngine;
using System.Collections.Generic;

namespace PSG
{
    /// <summary>
    /// Line, constructed from given set of points.
    /// Includes automatic joining, when first and last vertices are equal.
    /// 
    /// If {useDoubleCollider} is enabled, its collider includes thickness of shape.
    /// In other case, it passes through its center.
    /// 
    /// Colliders:
    ///     - Polygon (if {useDoubleCollider}
    ///     - Edge (in other case)
    /// </summary>
    public class LineMesh : MeshBase
    {

        //line data
        public bool UseDoubleCollider { get; protected set; }
        public Vector2[] LineVerts { get; protected set; }
        public float LineWidth { get; protected set; }

        //colliders
        public PolygonCollider2D C_PC2D { get; protected set; }
        public EdgeCollider2D C_EC2D { get; protected set; }
        public Collider2D Collider
        {
            get
            {
                if (C_PC2D)
                {
                    return C_PC2D;
                }
                return C_EC2D;
            }
        }

        //list of cached collider points
        private List<Vector2> cachedVertsLeft;
        private List<Vector2> cachedVertsRight;

        #region Static Methods - building from values and from structure

        public static LineMesh AddLine(Vector3 position, Vector2[] lineVerts, float lineWidth, bool useDoubleCollider, Space space, Material meshMat = null, bool attachRigidbody = true)
        {
            GameObject line = new GameObject();

            if (space == Space.Self)
            {
                line.transform.position = position;
            }
            else
            {
                line.transform.position = position + (Vector3)MeshHelper.GetCenter(lineVerts);
            }

            LineMesh lineComponent = line.AddComponent<LineMesh>();
            lineComponent.Build(lineVerts, lineWidth, useDoubleCollider, meshMat);
            if (attachRigidbody)
            {
                line.AddComponent<Rigidbody2D>();
            }
            return lineComponent;
        }

        public static LineMesh AddLine(Vector3 position, LineStructure structure, Material meshMat = null, bool attachRigidbody = true)
        {
            return AddLine(position, structure.LineVerts, structure.LineWidth, structure.UseDoubleCollider, Space.Self, meshMat, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(IList<Vector2> lineVerts, float lineWidth, bool useDoubleCollider, Material meshMat = null)
        {
            name = "Line mesh";
            UseDoubleCollider = useDoubleCollider;
            LineVerts = (Vector2[])lineVerts;
            LineWidth = lineWidth;

            BuildMesh(ref meshMat);
        }

        public void Build(LineStructure structure, Material meshMat = null)
        {
            Build(structure.LineVerts, structure.LineWidth, structure.UseDoubleCollider, meshMat);
        }

        #endregion

        public LineStructure GetStructure()
        {
            return new LineStructure
            {
                UseDoubleCollider = UseDoubleCollider,
                LineVerts = LineVerts,
                LineWidth = LineWidth
            };
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (LineWidth == 0)
            {
                Debug.LogWarning("LineMesh::ValidateMesh: Line width can't be equal to zero!");
                return false;
            }

            if (LineVerts.Length < 1)
            {
                Debug.LogWarning("LineMesh::ValidateMesh: Parameter size must be bigger than one!");
                return false;
            }

            if (MeshHelper.HasDuplicates(LineVerts))
            {
                Debug.LogWarning("LineMesh::ValidateMesh: Duplicate points detected!");
                return false;
            }

            if (LineWidth < 0)
            {
                LineWidth = -LineWidth;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {

            #region DoubleCollider
            if (UseDoubleCollider)
            {
                cachedVertsLeft = new List<Vector2>();
                cachedVertsRight = new List<Vector2>();
            }
            #endregion

            List<Vector3> verticesList = new List<Vector3>();
            List<int> trianglesList = new List<int>();

            Vector2 center = MeshHelper.GetCenter(LineVerts);
            for (int i = 0; i < LineVerts.Length; i++)
            {
                LineVerts[i] -= center;
            }

            int currentVertIndex = 0;
            int currentTriIndex = 0;
            //add first two vertices
            float angle = Mathf.Atan2(LineVerts[1].y - LineVerts[0].y, LineVerts[1].x - LineVerts[0].x);
            float oldAngle, angleDiff;
            Vector2 p1 = new Vector2(Mathf.Cos(angle + deg90), Mathf.Sin(angle + deg90)) * LineWidth;
            Vector2 p2 = new Vector2(Mathf.Cos(angle - deg90), Mathf.Sin(angle - deg90)) * LineWidth;
            if (p1 != p2)
            {
                verticesList.Add(LineVerts[currentVertIndex] + p1);
                verticesList.Add(LineVerts[currentVertIndex] + p2);
                #region DoubleCollider
                if (UseDoubleCollider)
                {
                    cachedVertsLeft.Add(verticesList[verticesList.Count - 2]);
                    cachedVertsRight.Add(verticesList[verticesList.Count - 1]);
                }
                #endregion
            }
            else
            {
                verticesList.Add(LineVerts[currentVertIndex]);
                #region DoubleCollider
                if (UseDoubleCollider)
                {
                    cachedVertsLeft.Add(verticesList[verticesList.Count - 1]);
                    cachedVertsRight.Add(verticesList[verticesList.Count - 1]);
                }
                #endregion
            }
            oldAngle = angle;
            currentVertIndex++;
            // add middle vertices
            for (int i = 0; i < LineVerts.Length - 2; i++, currentVertIndex++)
            {
                angle = Mathf.Atan2(LineVerts[currentVertIndex + 1].y - LineVerts[currentVertIndex].y, LineVerts[currentVertIndex + 1].x - LineVerts[currentVertIndex].x);
                angleDiff = oldAngle + MeshHelper.AngleDifference(oldAngle, angle) * 0.5f;
                p1 = new Vector2(Mathf.Cos(angleDiff + deg90), Mathf.Sin(angleDiff + deg90)) * LineWidth;
                p2 = new Vector2(Mathf.Cos(angleDiff - deg90), Mathf.Sin(angleDiff - deg90)) * LineWidth;
                if (p1 != p2)
                {
                    verticesList.Add(LineVerts[currentVertIndex] + p1);
                    verticesList.Add(LineVerts[currentVertIndex] + p2);
                    trianglesList.Add(currentTriIndex + 0);
                    trianglesList.Add(currentTriIndex + 3);
                    trianglesList.Add(currentTriIndex + 1);
                    trianglesList.Add(currentTriIndex + 3);
                    trianglesList.Add(currentTriIndex + 0);
                    trianglesList.Add(currentTriIndex + 2);
                    currentTriIndex += 2;
                }
                else
                {
                    verticesList.Add(LineVerts[currentTriIndex] + p1);
                    if (verticesList[verticesList.Count - 1] != verticesList[verticesList.Count - 2])
                    {
                        trianglesList.Add(currentTriIndex + 0);
                        trianglesList.Add(currentTriIndex + 3);
                        trianglesList.Add(currentTriIndex + 1);
                        currentTriIndex++;
                    }
                }
                #region DoubleCollider
                if (UseDoubleCollider)
                {
                    cachedVertsLeft.Add(verticesList[verticesList.Count - 2]);
                    cachedVertsRight.Add(verticesList[verticesList.Count - 1]);
                }
                #endregion
                oldAngle = angle;
            }

            //add last two vertices
            if (LineVerts[0] != LineVerts[currentVertIndex])
            {
                angle = Mathf.Atan2(LineVerts[currentVertIndex].y - LineVerts[currentVertIndex - 1].y, LineVerts[currentVertIndex].x - LineVerts[currentVertIndex - 1].x);
                p1 = new Vector2(Mathf.Cos(angle + deg90), Mathf.Sin(angle + deg90)) * LineWidth;
                p2 = new Vector2(Mathf.Cos(angle - deg90), Mathf.Sin(angle - deg90)) * LineWidth;
                if (p1 != p2)
                {
                    verticesList.Add(LineVerts[currentVertIndex] + p1);
                    verticesList.Add(LineVerts[currentVertIndex] + p2);
                    trianglesList.Add(currentTriIndex + 0);
                    trianglesList.Add(currentTriIndex + 3);
                    trianglesList.Add(currentTriIndex + 1);
                    trianglesList.Add(currentTriIndex + 3);
                    trianglesList.Add(currentTriIndex + 0);
                    trianglesList.Add(currentTriIndex + 2);
                }
                else
                {
                    //make LineMesh loop
                    if (verticesList[verticesList.Count - 1] != verticesList[verticesList.Count - 2])
                    {
                        verticesList.Add(LineVerts[currentTriIndex] + p1);
                        trianglesList.Add(currentTriIndex + 0);
                        trianglesList.Add(currentTriIndex + 3);
                        trianglesList.Add(currentTriIndex + 1);
                    }
                }
                #region DoubleCollider
                if (UseDoubleCollider)
                {
                    cachedVertsLeft.Add(verticesList[verticesList.Count - 2]);
                    cachedVertsRight.Add(verticesList[verticesList.Count - 1]);
                }
                #endregion
            }
            else
            {
                oldAngle = Mathf.Atan2(
                    LineVerts[0].y - LineVerts[currentVertIndex].y,
                    LineVerts[0].x - LineVerts[currentVertIndex].x);
                angle = Mathf.Atan2(
                    LineVerts[1].y - LineVerts[0].y,
                    LineVerts[1].x - LineVerts[0].x);
                angleDiff = oldAngle + MeshHelper.AngleDifference(oldAngle, angle) * 0.5f - deg90;
                p1 = new Vector2(Mathf.Cos(angleDiff - deg90), Mathf.Sin(angleDiff - deg90)) * LineWidth;
                p2 = new Vector2(Mathf.Cos(angleDiff + deg90), Mathf.Sin(angleDiff + deg90)) * LineWidth;
                verticesList[0] = LineVerts[currentVertIndex] + p1;
                verticesList[1] = LineVerts[currentVertIndex] + p2;
                #region DoubleCollider
                if (UseDoubleCollider)
                {
                    cachedVertsLeft[0] = verticesList[0];
                    cachedVertsRight[0] = verticesList[1];
                    cachedVertsLeft.Add(verticesList[verticesList.Count - 2]);
                    cachedVertsRight.Add(verticesList[verticesList.Count - 1]);
                }
                #endregion

                trianglesList.Add(0);
                trianglesList.Add(verticesList.Count - 1);
                trianglesList.Add(1);
                trianglesList.Add(verticesList.Count - 1);
                trianglesList.Add(0);
                trianglesList.Add(verticesList.Count - 2);
            }
            Vertices = verticesList.ToArray();
            Triangles = trianglesList.ToArray();

            UVs = MeshHelper.UVUnwrap(Vertices);
        }

        public override void UpdateCollider()
        {
            if (UseDoubleCollider)
            {
                Vector2[] points = new Vector2[cachedVertsRight.Count + cachedVertsLeft.Count];
                for (int i = 0; i < cachedVertsLeft.Count; i++)
                {
                    points[i] = cachedVertsLeft[i];
                }
                for (int i = 0; i < cachedVertsRight.Count; i++)
                {
                    //reverse order
                    points[i + cachedVertsLeft.Count] = cachedVertsRight[cachedVertsRight.Count - 1 - i];
                }
                C_PC2D.points = points;
            }
            else
            {
                C_EC2D.points = LineVerts;
            }
        }
        public override void GetOrAddComponents()
        {
            if (UseDoubleCollider)
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

        #endregion

    }

    public struct LineStructure
    {
        public bool UseDoubleCollider;
        public Vector2[] LineVerts;
        public float LineWidth;
    }
}