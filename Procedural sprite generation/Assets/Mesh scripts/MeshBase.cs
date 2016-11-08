using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public abstract class MeshBase : MonoBehaviour {

    protected Mesh mesh;
    protected MeshFilter C_MF;
    protected MeshRenderer C_MR;

    protected Material matt;

    public void SetRandomColor()
    {
        C_MR.material.color = Random.ColorHSV();
    }

    public void SetMaterial(Material material)
    {
        matt = material;
    }

    public void SetTexture(Texture texture)
    {
        C_MR.material.mainTexture = texture;
    }

    public void SetMaterial(Material material, Texture texture)
    {
        matt = material;
        C_MR.material.mainTexture = texture;
    }

    public Vector2 GetCenter()
    {
        return transform.position;
    }

    public void AddHingeJoint()
    {
        HingeJoint2D C_HJ2D = gameObject.AddComponent<HingeJoint2D>();
        C_HJ2D.anchor = transform.InverseTransformPoint(GetCenter());
    }

    protected static int Side(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //using {Math} instead of {Mathf}, because Mathf.Sign returns {1} for {0}!
        return Math.Sign((p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y));
    }

    public abstract void UpdateMesh();
    public abstract void UpdateCollider();
    public abstract void GetOrAddComponents();

    protected static Vector2[] ConvertVec3ToVec2(Vector3[] verts3D)
    {
        Vector2[] verts2D = new Vector2[verts3D.Length];
        for (int i = 0; i < verts3D.Length; i++)
        {
            verts2D[i] = verts3D[i];
        }
        return verts2D;
    }
    protected static Vector2[] ConvertVec3ToVec2_collider(Vector3[] verts3D)
    {
        Vector2[] verts2D = new Vector2[verts3D.Length+1];
        for (int i = 0; i < verts3D.Length; i++)
        {
            verts2D[i] = verts3D[i];
        }
        verts2D[verts2D.Length - 1] = verts3D[0];
        return verts2D;
    }

    #region UV Unwrapping

    protected static Vector4 GetMinXCoordinate(Vector3[] vec)
    {
        /* x - minX
         * y - minY
         * z - maxX
         * w - maxY
         */
        float x = float.MaxValue;
        float y = float.MaxValue;
        float z = float.MinValue;
        float w = float.MinValue;
        for (int i = 0; i < vec.Length; i++)
        {
            if (vec[i].x < x)
            {
                x = vec[i].x;
            }
            if (vec[i].y < y)
            {
                y = vec[i].y;
            }
            if (vec[i].x > z)
            {
                z = vec[i].x;
            }
            if (vec[i].y > w)
            {
                w = vec[i].y;
            }
        }
        return new Vector4(x, y, z, w);
    }
    protected static Vector4 GetMinXCoordinate(Vector2[] vec)
    {
        /* x - minX
         * y - minY
         * z - maxX
         * w - maxY
         */
        float x = float.MaxValue;
        float y = float.MaxValue;
        float z = float.MinValue;
        float w = float.MinValue;
        for (int i = 0; i < vec.Length; i++)
        {
            if (vec[i].x < x)
            {
                x = vec[i].x;
            }
            if (vec[i].y < y)
            {
                y = vec[i].y;
            }
            if (vec[i].x > z)
            {
                z = vec[i].x;
            }
            if (vec[i].y > w)
            {
                w = vec[i].y;
            }
        }
        return new Vector4(x, y, z, w);
    }
    protected static List<Vector2> UVUnwrap(Vector3[] vertices)
    {
        List<Vector2> uv = new List<Vector2>();
        Vector4 boundingBox = GetMinXCoordinate(vertices);
        float length = boundingBox.z - boundingBox.x;
        float width = boundingBox.w - boundingBox.y;
        for (int i = 0; i < vertices.Length; i++)
        {
            float ux = (vertices[i].x - boundingBox.x) / length;
            float uy = (vertices[i].y - boundingBox.y) / width;
            uv.Add(new Vector2(ux, uy));
        }
        return uv;
    }
    protected static List<Vector2> UVUnwrap(Vector2[] vertices)
    {
        List<Vector2> uv = new List<Vector2>();
        Vector4 boundingBox = GetMinXCoordinate(vertices);
        float length = boundingBox.z - boundingBox.x;
        float width = boundingBox.w - boundingBox.y;
        for (int i = 0; i < vertices.Length; i++)
        {
            float ux = (vertices[i].x - boundingBox.x) / length;
            float uy = (vertices[i].y - boundingBox.y) / width;
            uv.Add(new Vector2(ux, uy));
        }
        return uv;
    }

    #endregion

    protected static readonly float deg90 = Mathf.Deg2Rad * 90f;
    protected static readonly float deg360 = 2*Mathf.PI;
    protected static bool OptimizeMesh = true;
}
