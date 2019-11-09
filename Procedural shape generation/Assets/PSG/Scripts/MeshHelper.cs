using System.Collections.Generic;
using UnityEngine;

namespace PSG
{
    /// <summary>
    /// Static helper class for mesh creation and other
    /// utility functions.
    /// </summary>
    public static class MeshHelper
    {
        #region Default Material

        private static Material cachedDefaultMaterial;
        public static Material CachedDefaultMaterial
        {
            get
            {
                if (cachedDefaultMaterial == null)
                {
                    cachedDefaultMaterial = new Material(Shader.Find("Sprites/Default"));
                }
                return cachedDefaultMaterial;
            }
            set { cachedDefaultMaterial = value; }
        }

        //if material is null, replace it with default
        public static void SetupMaterial(ref Material meshMatt)
        {
            if (meshMatt == null)
            {
                meshMatt = CachedDefaultMaterial;
            }
        }

        #endregion

        #region Math

        public static double AngleBetweenPoints(Vector2 first, Vector2 center, Vector2 last)
        {
            double centerFirstDistance = Vector2.Distance(center, first);
            double centerLastDistance = Vector2.Distance(center, last);
            double deltaPosition = (first.x - center.x) * (last.x - center.x) + (first.y - center.y) * (last.y - center.y);
            double v = deltaPosition / centerFirstDistance / centerLastDistance;
            return System.Math.Acos(v) * 180 / System.Math.PI;
        }

        //checks if point v is within triangle {v1,v2,v3}
        public static bool IsPointInTriangle(Vector2 v, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            double a1 = GetSide(v, v1, v2);
            double a2 = GetSide(v, v2, v3);
            double a3 = GetSide(v, v3, v1);
            return (a1 >= 0 && a2 >= 0 && a3 >= 0) || (a1 <= 0 && a2 <= 0 && a3 <= 0);
        }

        //difference between angles in radians
        public static float AngleDifference(float a, float b)
        {
            float diff = b - a;
            if (diff > Mathf.Deg2Rad * 180f)
            {
                diff -= Mathf.Deg2Rad * 360f;
            }
            if (diff < -Mathf.Deg2Rad * 180f)
            {
                diff += Mathf.Deg2Rad * 360f;
            }
            return diff;
        }

        // checks the side point {v} it lays on, relative to segment {v1,v2}
        public static int GetSide(Vector3 v1, Vector3 v2, Vector3 v)
        {
            //using {Math} instead of {Mathf}, because Mathf.Sign returns {1} for {0}!
            return System.Math.Sign((v1.x - v.x) * (v2.y - v.y) - (v2.x - v.x) * (v1.y - v.y));
        }

        #endregion

        #region Building helper functions

        //check for duplicates (expensive!)
        public static bool HasDuplicates(IList<Vector2> verts)
        {
            for (int i = 0; i < verts.Count; i++)
            {
                for (int j = i + 1; j < verts.Count; j++)
                {
                    if (verts[i].Equals(verts[j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //in case of sprites, all normals can be just {Vector3.up}
        public static Vector3[] AddMeshNormals(int verticesLength)
        {
            Vector3[] normals = new Vector3[verticesLength];
            for (int i = 0; i < verticesLength; i++)
            {
                normals[i] = Vector3.up;
            }
            return normals;
        }

        #endregion

        #region Vector Conversion

        //convert Vector3 array to Vector3 one
        public static Vector2[] ConvertVec3ToVec2(IList<Vector3> verts3D)
        {
            Vector2[] verts2D = new Vector2[verts3D.Count];
            for (int i = 0; i < verts3D.Count; i++)
            {
                verts2D[i] = verts3D[i];
            }
            return verts2D;
        }

        //inverse ditto
        public static Vector3[] ConvertVec2ToVec3(IList<Vector2> vertsD)
        {
            Vector3[] verts2D = new Vector3[vertsD.Count];
            for (int i = 0; i < vertsD.Count; i++)
            {
                verts2D[i] = vertsD[i];
            }
            return verts2D;
        }

        #endregion

        #region UV Unwrapping

        //get bounding box of supplied points
        private static Vector4 GetBounds(IList<Vector3> vec)
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
            for (int i = 0; i < vec.Count; i++)
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

        //return array of Vector2 UVs
        public static Vector2[] UVUnwrap(IList<Vector3> vertices)
        {
            Vector2[] uv = new Vector2[vertices.Count];
            Vector4 boundingBox = GetBounds(vertices);
            float length = boundingBox.z - boundingBox.x;
            float width = boundingBox.w - boundingBox.y;
            for (int i = 0; i < vertices.Count; i++)
            {
                float ux = (vertices[i].x - boundingBox.x) / length;
                float uy = (vertices[i].y - boundingBox.y) / width;
                uv[i] = new Vector2(ux, uy);
            }
            return uv;
        }

        #endregion

    }

}