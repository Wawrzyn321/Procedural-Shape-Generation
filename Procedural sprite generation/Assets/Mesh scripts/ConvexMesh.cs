using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConvexMesh : MeshBase {

    //mesh data
    private List<Vector3> points;
    private List<Vector3> meshVertices;
    private List<int> triangles;
    private List<Vector2> uvs;

    //collider
    private PolygonCollider2D C_PC2D;

    public void Build(Vector3[] vertices, Material meshMatt)
    {
        name = "Convex Mesh";
        
        mesh = new Mesh();
        GetOrAddComponents();

        C_MR.material = meshMatt;

        if (BuildConvex(vertices))
        {
            UpdateMesh();
            UpdateCollider();
        }
    }

    private bool BuildConvex(Vector3[] verts)
    {
        #region Validity Check

        if (verts.Length < 2)
        {
            Debug.LogWarning("ConvexMesh::BuildConves: verts count must be greater than 2!");
            return false;
        }

        #endregion
        
        points = new List<Vector3>(verts);
        triangles = new List<int>();

        meshVertices = QuickHull(points);
        for (int i = 1; i < meshVertices.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
        uvs = UVUnwrap(meshVertices.ToArray());

        return true;
    }
    
    public List<Vector3> GetBasePoints()
    {
        return points;
    }

    
    #region Abstract Implementation

    public override void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = meshVertices.ToArray();
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
        C_PC2D.points = ConvertVec3ToVec2_collider(meshVertices.ToArray());
    }

    #endregion

    public override void GetOrAddComponents()
    {
        C_PC2D = GetComponent<PolygonCollider2D>();
        if (!C_PC2D)
        {
            C_PC2D = gameObject.AddComponent<PolygonCollider2D>();
        }
        C_MR = GetComponent<MeshRenderer>();
        if (!C_MR)
        {
            C_MR = gameObject.AddComponent<MeshRenderer>();
        }
        C_MF = GetComponent<MeshFilter>();
        if (!C_MF)
        {
            C_MF = gameObject.AddComponent<MeshFilter>();
        }
    }

    #region Convex Hull Algorithm

    private Vector3 getLineEquation(Vector3 a, Vector3 b)
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
            float cV = (bV * a.y - a.x);
            return new Vector3(1, -bV, cV);
        }
    }

    //get side of points {v} relative to vector {v1}{v2}
    private double getSide(Vector3 v, Vector3 v1, Vector3 v2)
    {
        return (v1.x - v2.x) * (v.y - v2.y) - (v1.y - v2.y) * (v.x - v2.x);
    }


    private Vector3? getLeftmostPoint(List<Vector3> v)
    {
        Vector3? p = null;
        float mX = float.MaxValue;
        for (int i = 0; i < v.Count; i++)
        {
            if (v[i].x < mX)
            {
                mX = v[i].x;
                p = v[i];
            }
        }
        return p;
    }

    //get pointer to points of highest x coordinate
    private Vector3? getRightmostPoint(List<Vector3> v)
    {
        Vector3? p = null;
        float mX = float.MinValue;
        for (int i = 0; i < v.Count; i++)
        {
            if (v[i].x > mX)
            {
                mX = v[i].x;
                p = v[i];
            }
        }
        return p;
    }

    //get vector of points on the right of two given points
    private List<Vector3> getPointsOnRight(List<Vector3> v, Vector3 vL, Vector3 vP)
    {
        List<Vector3> r = new List<Vector3>();
        for (int i = 0; i < v.Count; i++)
        {
            if (getSide(v[i], vL, vP) > 0)
            {
                r.Add(v[i]);
            }
        }
        return r;
    }

    //get points located the farthest from line of given equation
    private Vector3? getFarthestPoint(Vector3 lineEquation, List<Vector3> v)
    {
        double len = 0;
        Vector3? r = null;
        for (int i = 0; i < v.Count; i++)
        {
            double l =
                Mathf.Abs(lineEquation.x * v[i].x + lineEquation.y * v[i].y + lineEquation.z)
                / Mathf.Sqrt(lineEquation.x * lineEquation.x + lineEquation.y * lineEquation.y);
            if (l > len)
            {
                len = l;
                r = v[i];
            }
        }
        return r;
    }

    //recurrent subroutine to {QuickHull}
    private List<Vector3> QuickHullSub(Vector3? A, Vector3? B, List<Vector3> points)
    {
        if (A.HasValue == false || B.HasValue == false || points == null)
        {
            return null;
        }
        Vector3? C = getFarthestPoint(getLineEquation(A.Value, B.Value), points);
        if (!C.HasValue)
            return null;

        List<Vector3> s1 = getPointsOnRight(points, A.Value, C.Value);
        List<Vector3> s2 = getPointsOnRight(points, C.Value, B.Value);

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
    private List<Vector3> QuickHull(List<Vector3> points)
    {
        if (points.Count < 2)
        {
            return null;
        }
        Vector3? A = getLeftmostPoint(points);
        Vector3? B = getRightmostPoint(points);

        if (A == null || B == null)
        {
            return null;
        }
        //to get points on left of AB, just get points on right of BA!
        List<Vector3> s1 = getPointsOnRight(points, A.Value, B.Value);
        List<Vector3> s2 = getPointsOnRight(points, B.Value, A.Value);

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
