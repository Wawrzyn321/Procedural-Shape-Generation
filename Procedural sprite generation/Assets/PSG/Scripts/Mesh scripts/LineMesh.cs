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
        private bool useDoubleCollider;
        private Vector2[] lineVerts;
        private float lineWidth;

        //colliders
        private PolygonCollider2D C_PC2D;
        private EdgeCollider2D C_EC2D;

        //list of cached collider points
        private List<Vector2> cachedVertsLeft;
        private List<Vector2> cachedVertsRight;

        #region Static Methods - building from values and from structure

        public static LineMesh AddLine(Vector3 position, Vector2[] lineVerts, float lineWidth, bool useDoubleCollider, Space space, Material meshMatt = null, bool attachRigidbody = true)
        {
            GameObject line = new GameObject();

            if (space == Space.Self)
            {
                line.transform.position = position;
            }
            else
            {
                Vector2 center = new Vector2();
                for (int i = 0; i < lineVerts.Length; i++)
                {
                    center += lineVerts[i];
                }
                line.transform.position = position + (Vector3)center / lineVerts.Length;
            }

            LineMesh lineComponent = line.AddComponent<LineMesh>();
            lineComponent.Build(lineVerts, lineWidth, useDoubleCollider, meshMatt);
            if (attachRigidbody)
            {
                line.AddComponent<Rigidbody2D>();
            }
            return lineComponent;
        }

        public static LineMesh AddLine(Vector3 position, LineStructure structure, Material meshMatt = null, bool attachRigidbody = true)
        {
            return AddLine(position, structure.lineVerts, structure.lineWidth, structure.useDoubleCollider, Space.Self, meshMatt, attachRigidbody);
        }

        #endregion

        #region Public Build

        //assign variables, get components and build mesh
        public void Build(IList<Vector2> lineVerts, float lineWidth, bool useDoubleCollider, Material meshMatt = null)
        {
            name = "Line mesh";
            this.useDoubleCollider = useDoubleCollider;
            this.lineVerts = (Vector2[])lineVerts;
            this.lineWidth = lineWidth;

            BuildMesh(ref meshMatt);
        }

        public void Build(LineStructure structure, Material meshMatt = null)
        {
            Build(structure.lineVerts, structure.lineWidth, structure.useDoubleCollider, meshMatt);
        }

        #endregion

        public LineStructure GetStructure()
        {
            return new LineStructure
            {
                useDoubleCollider = useDoubleCollider,
                lineVerts = lineVerts,
                lineWidth = lineWidth
            };
        }

        #region Abstract Implementation

        protected override bool ValidateMesh()
        {
            if (lineWidth == 0)
            {
                Debug.LogWarning("LineMesh::ValidateMesh: Line width can't be equal to zero!");
                return false;
            }

            if (lineVerts.Length < 1)
            {
                Debug.LogWarning("LineMesh::ValidateMesh: Parameter size must be bigger than one!");
                return false;
            }

            if (MeshHelper.HasDuplicates(lineVerts))
            {
                Debug.LogWarning("LineMesh::ValidateMesh: Duplicate points detected!");
                return false;
            }

            if (lineWidth < 0)
            {
                lineWidth = -lineWidth;
            }
            return true;
        }

        protected override void BuildMeshComponents()
        {

            #region DoubleCollider
            if (useDoubleCollider)
            {
                cachedVertsLeft = new List<Vector2>();
                cachedVertsRight = new List<Vector2>();
            }
            #endregion

            List<Vector3> verticesList = new List<Vector3>();
            List<int> trianglesList = new List<int>();

            Vector2 center = new Vector2();
            for (int i = 0; i < lineVerts.Length; i++)
            {
                center += lineVerts[i];
            }
            center /= lineVerts.Length;
            for (int i = 0; i < lineVerts.Length; i++)
            {
                lineVerts[i] -= center;
            }

            int currentVertIndex = 0;
            int currentTriIndex = 0;
            //add first two vertices
            float angle = Mathf.Atan2(lineVerts[1].y - lineVerts[0].y, lineVerts[1].x - lineVerts[0].x);
            float oldAngle, angleDiff;
            Vector2 p1 = new Vector2(Mathf.Cos(angle + deg90), Mathf.Sin(angle + deg90)) * lineWidth;
            Vector2 p2 = new Vector2(Mathf.Cos(angle - deg90), Mathf.Sin(angle - deg90)) * lineWidth;
            if (p1 != p2)
            {
                verticesList.Add(lineVerts[currentVertIndex] + p1);
                verticesList.Add(lineVerts[currentVertIndex] + p2);
                #region DoubleCollider
                if (useDoubleCollider)
                {
                    cachedVertsLeft.Add(verticesList[verticesList.Count - 2]);
                    cachedVertsRight.Add(verticesList[verticesList.Count - 1]);
                }
                #endregion
            }
            else
            {
                verticesList.Add(lineVerts[currentVertIndex]);
                #region DoubleCollider
                if (useDoubleCollider)
                {
                    cachedVertsLeft.Add(verticesList[verticesList.Count - 1]);
                    cachedVertsRight.Add(verticesList[verticesList.Count - 1]);
                }
                #endregion
            }
            oldAngle = angle;
            currentVertIndex++;
            // add middle vertices
            for (int i = 0; i < lineVerts.Length - 2; i++, currentVertIndex++)
            {
                angle = Mathf.Atan2(lineVerts[currentVertIndex + 1].y - lineVerts[currentVertIndex].y, lineVerts[currentVertIndex + 1].x - lineVerts[currentVertIndex].x);
                angleDiff = oldAngle + MeshHelper.AngleDifference(oldAngle, angle) * 0.5f;
                p1 = new Vector2(Mathf.Cos(angleDiff + deg90), Mathf.Sin(angleDiff + deg90)) * lineWidth;
                p2 = new Vector2(Mathf.Cos(angleDiff - deg90), Mathf.Sin(angleDiff - deg90)) * lineWidth;
                if (p1 != p2)
                {
                    verticesList.Add(lineVerts[currentVertIndex] + p1);
                    verticesList.Add(lineVerts[currentVertIndex] + p2);
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
                    verticesList.Add(lineVerts[currentTriIndex] + p1);
                    if (verticesList[verticesList.Count - 1] != verticesList[verticesList.Count - 2])
                    {
                        trianglesList.Add(currentTriIndex + 0);
                        trianglesList.Add(currentTriIndex + 3);
                        trianglesList.Add(currentTriIndex + 1);
                        currentTriIndex++;
                    }
                }
                #region DoubleCollider
                if (useDoubleCollider)
                {
                    cachedVertsLeft.Add(verticesList[verticesList.Count - 2]);
                    cachedVertsRight.Add(verticesList[verticesList.Count - 1]);
                }
                #endregion
                oldAngle = angle;
            }

            //add last two vertices
            if (lineVerts[0] != lineVerts[currentVertIndex])
            {
                angle = Mathf.Atan2(lineVerts[currentVertIndex].y - lineVerts[currentVertIndex - 1].y, lineVerts[currentVertIndex].x - lineVerts[currentVertIndex - 1].x);
                p1 = new Vector2(Mathf.Cos(angle + deg90), Mathf.Sin(angle + deg90)) * lineWidth;
                p2 = new Vector2(Mathf.Cos(angle - deg90), Mathf.Sin(angle - deg90)) * lineWidth;
                if (p1 != p2)
                {
                    verticesList.Add(lineVerts[currentVertIndex] + p1);
                    verticesList.Add(lineVerts[currentVertIndex] + p2);
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
                        verticesList.Add(lineVerts[currentTriIndex] + p1);
                        trianglesList.Add(currentTriIndex + 0);
                        trianglesList.Add(currentTriIndex + 3);
                        trianglesList.Add(currentTriIndex + 1);
                    }
                }
                #region DoubleCollider
                if (useDoubleCollider)
                {
                    cachedVertsLeft.Add(verticesList[verticesList.Count - 2]);
                    cachedVertsRight.Add(verticesList[verticesList.Count - 1]);
                }
                #endregion
            }
            else
            {
                oldAngle = Mathf.Atan2(
                    lineVerts[0].y - lineVerts[currentVertIndex].y,
                    lineVerts[0].x - lineVerts[currentVertIndex].x);
                angle = Mathf.Atan2(
                    lineVerts[1].y - lineVerts[0].y,
                    lineVerts[1].x - lineVerts[0].x);
                angleDiff = oldAngle + MeshHelper.AngleDifference(oldAngle, angle) * 0.5f - deg90;
                p1 = new Vector2(Mathf.Cos(angleDiff - deg90), Mathf.Sin(angleDiff - deg90)) * lineWidth;
                p2 = new Vector2(Mathf.Cos(angleDiff + deg90), Mathf.Sin(angleDiff + deg90)) * lineWidth;
                verticesList[0] = lineVerts[currentVertIndex] + p1;
                verticesList[1] = lineVerts[currentVertIndex] + p2;
                #region DoubleCollider
                if (useDoubleCollider)
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
            if (useDoubleCollider)
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
                C_EC2D.points = lineVerts;
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

        #endregion

    }

    public struct LineStructure
    {
        public bool useDoubleCollider;
        public Vector2[] lineVerts;
        public float lineWidth;
    }
}