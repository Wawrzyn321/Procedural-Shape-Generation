using UnityEngine;
using System.Collections.Generic;

public class LineMesh : MeshBase
{

    //mesh data
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector2> uvs;

    //line data
    private bool useDoubleCollider;
    private Vector2[] lineVerts;
    //private float lineWidth;

    //collider
    private PolygonCollider2D C_PC2D;

    //list of 
    private List<Vector2> cachedVertsLeft;
    private List<Vector2> cachedVertsRight;

    //constructor
    public void Build(Vector2[] lineVerts, float lineWidth, bool useDoubleCollider, Material meshMatt)
    {
        name = "Line mesh";
        this.useDoubleCollider = useDoubleCollider;
        this.lineVerts = lineVerts;
        //this.lineWidth = lineWidth;

        mesh = new Mesh();
        GetOrAddComponents();

        C_MR.material = meshMatt;

        if (BuildLine(lineVerts, lineWidth, useDoubleCollider))
        {
            UpdateMesh();
            UpdateCollider();
        }
    }

    //build line
    private bool BuildLine(Vector2[] lineVerts, float lineWidth, bool useDoubleCollider)
    {
        #region Validity Check
        
        if (lineWidth==0)
        {
            Debug.LogWarning("LineMesh::BuildLine: Line width can't be equal to zero!");
            return false;
        }

        if (lineVerts.Length < 1)
        {
            Debug.LogWarning("LineMesh::BuildLine: Parameter size must be bigger than one!");
            return false;
        }

        if (lineWidth < 0)
        {
            lineWidth = -lineWidth;
        }

        #endregion

        #region DoubleCollider
        if (useDoubleCollider)
        {
            cachedVertsLeft = new List<Vector2>();
            cachedVertsRight = new List<Vector2>();
        }
        #endregion

        vertices = new List<Vector3>();
        triangles = new List<int>();

        int currentVertIndex = 0;
        int currentTriIndex = 0;
        //add first two vertices
        float angle = Mathf.Atan2(lineVerts[1].y - lineVerts[0].y, lineVerts[1].x - lineVerts[0].x);
        float oldAngle, angleDiff;
        Vector2 p1 = new Vector2(Mathf.Cos(angle + deg90), Mathf.Sin(angle + deg90)) * lineWidth;
        Vector2 p2 = new Vector2(Mathf.Cos(angle - deg90), Mathf.Sin(angle - deg90)) * lineWidth;
        if (p1 != p2)
        {
            vertices.Add(lineVerts[currentVertIndex] + p1);
            vertices.Add(lineVerts[currentVertIndex] + p2);
            #region DoubleCollider
            if (useDoubleCollider)
            {
                cachedVertsLeft.Add(vertices[vertices.Count - 2]);
                cachedVertsRight.Add(vertices[vertices.Count - 1]);
            }
            #endregion
        }
        else
        {
            vertices.Add(lineVerts[currentVertIndex]);
            #region DoubleCollider
            if (useDoubleCollider)
            {
                cachedVertsLeft.Add(vertices[vertices.Count - 1]);
                cachedVertsRight.Add(vertices[vertices.Count - 1]);
            }
            #endregion
        }
        oldAngle = angle;
        currentVertIndex++;
        // add middle vertices
        for (int i = 0; i < lineVerts.Length - 2; i++, currentVertIndex++)
        {
            angle = Mathf.Atan2(lineVerts[currentVertIndex + 1].y - lineVerts[currentVertIndex].y, lineVerts[currentVertIndex + 1].x - lineVerts[currentVertIndex].x);
            angleDiff = oldAngle + AngleDifference(oldAngle, angle) * 0.5f;
            p1 = new Vector2(Mathf.Cos(angleDiff + deg90), Mathf.Sin(angleDiff + deg90)) * lineWidth;
            p2 = new Vector2(Mathf.Cos(angleDiff - deg90), Mathf.Sin(angleDiff - deg90)) * lineWidth;
            if (p1 != p2)
            {
                vertices.Add(lineVerts[currentVertIndex] + p1);
                vertices.Add(lineVerts[currentVertIndex] + p2);
                triangles.Add(currentTriIndex + 0);
                triangles.Add(currentTriIndex + 3);
                triangles.Add(currentTriIndex + 1);
                triangles.Add(currentTriIndex + 3);
                triangles.Add(currentTriIndex + 0);
                triangles.Add(currentTriIndex + 2);
                currentTriIndex += 2;
            }
            else
            {
                vertices.Add(lineVerts[currentTriIndex] + p1);
                if (vertices[vertices.Count - 1] != vertices[vertices.Count - 2])
                {
                    triangles.Add(currentTriIndex + 0);
                    triangles.Add(currentTriIndex + 3);
                    triangles.Add(currentTriIndex + 1);
                    currentTriIndex++;
                }
            }
            #region DoubleCollider
            if (useDoubleCollider)
            {
                cachedVertsLeft.Add(vertices[vertices.Count - 2]);
                cachedVertsRight.Add(vertices[vertices.Count - 1]);
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
                vertices.Add(lineVerts[currentVertIndex] + p1);
                vertices.Add(lineVerts[currentVertIndex] + p2);
                triangles.Add(currentTriIndex + 0);
                triangles.Add(currentTriIndex + 3);
                triangles.Add(currentTriIndex + 1);
                triangles.Add(currentTriIndex + 3);
                triangles.Add(currentTriIndex + 0);
                triangles.Add(currentTriIndex + 2);
            }
            else
            {
                //make LineMesh loop
                if (vertices[vertices.Count - 1] != vertices[vertices.Count - 2])
                {
                    vertices.Add(lineVerts[currentTriIndex] + p1);
                    triangles.Add(currentTriIndex + 0);
                    triangles.Add(currentTriIndex + 3);
                    triangles.Add(currentTriIndex + 1);
                }
            }
            #region DoubleCollider
            if (useDoubleCollider)
            {
                cachedVertsLeft.Add(vertices[vertices.Count - 2]);
                cachedVertsRight.Add(vertices[vertices.Count - 1]);
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
            angleDiff = oldAngle + AngleDifference(oldAngle, angle) * 0.5f - deg90;
            p1 = new Vector2(Mathf.Cos(angleDiff - deg90), Mathf.Sin(angleDiff - deg90)) * lineWidth;
            p2 = new Vector2(Mathf.Cos(angleDiff + deg90), Mathf.Sin(angleDiff + deg90)) * lineWidth;
            vertices[0] = lineVerts[currentVertIndex] + p1;
            vertices[1] = lineVerts[currentVertIndex] + p2;
            #region DoubleCollider
            if (useDoubleCollider)
            {
                cachedVertsLeft[0] = vertices[0];
                cachedVertsRight[0] = vertices[1];
                cachedVertsLeft.Add(vertices[vertices.Count - 2]);
                cachedVertsRight.Add(vertices[vertices.Count - 1]);
            }
            #endregion
            
            triangles.Add(0);
            triangles.Add(vertices.Count-1);
            triangles.Add(1);
            triangles.Add(vertices.Count - 1);
            triangles.Add(0);
            triangles.Add(vertices.Count - 2);
        }
        uvs = UVUnwrap(vertices.ToArray());

        return true;
    }

    //difference between angles in radians
    private float AngleDifference(float a, float b)
    {
        float diff = b - a;
        if (diff > deg90 * 2)
        {
            diff -= deg90 * 4;
        }
        if (diff < -deg90 * 2)
        {
            diff += deg90 * 4;
        }
        return diff;
    }

    #region Abstract Implementation

    public override void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        C_MF.mesh = mesh;
        if (OptimizeMesh)
        {
            C_MF.mesh.Optimize();
        }
    }
    public override void UpdateCollider()
    {
        if (useDoubleCollider)
        {
            Vector2[] points = new Vector2[cachedVertsRight.Count + cachedVertsLeft.Count + 1];
            for (int i = 0; i < cachedVertsLeft.Count; i++)
            {
                points[i] = cachedVertsLeft[i];
            }
            for (int i = 0; i < cachedVertsRight.Count; i++)
            {
                //reverse order
                points[i + cachedVertsLeft.Count] = cachedVertsRight[cachedVertsRight.Count - 1 - i];
            }
            //when shape isn't closed
            if (vertices[0] != vertices[vertices.Count - 1])
            {
                points[points.Length - 1] = cachedVertsLeft[0];
            }
            C_PC2D.points = points;
        }
        else
        {
            C_PC2D.points = lineVerts;
        }
    }
    public override void GetOrAddComponents()
    {
        C_PC2D = gameObject.GetOrAddComponent<PolygonCollider2D>();
        C_MR = gameObject.GetOrAddComponent<MeshRenderer>();
        C_MF = gameObject.GetOrAddComponent<MeshFilter>();
    }

    #endregion

}
