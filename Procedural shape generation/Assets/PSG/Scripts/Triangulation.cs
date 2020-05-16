using System.Collections.Generic;
using UnityEngine;

namespace PSG
{
    public static class Triangulation
    {
        public struct Vector2Triple
        {
            public Vector2 A, B, C;
            public Vector2Triple(Vector2 a, Vector2 b, Vector2 c)
            {
                A = a;
                B = b;
                C = c;
            }
        }
        public struct IntTriple
        {
            public int A, B, C;
            public IntTriple(int a, int b, int c)
            {
                A = a;
                B = b;
                C = c;
            }
        }
        
        private struct VecIndexPair
        {
            public Vector2 v;
            public int index;
            public VecIndexPair(Vector2 v, int index)
            {
                this.v = v;
                this.index = index;
            }
            public static List<VecIndexPair> Get(List<Vector2> l)
            {
                List<VecIndexPair> li = new List<VecIndexPair>(l.Count);
                for (int i = 0; i < l.Count; i++)
                {
                    li.Add(new VecIndexPair(l[i], i));
                }
                return li;
            }
        }

        public static List<Vector2Triple> GetTriangles(List<Vector2> sourcePoints)
        {
            List<Vector2Triple> triangles = new List<Vector2Triple>();
            int MAX = sourcePoints.Count * sourcePoints.Count;

            //temporary List of points
            List<Vector2> verts = sourcePoints;

            //was shape drawn clockwise?
            bool isCW = IsShapeClockWise(verts);
            int start = 0;

            int repeats = 0;

            while (verts.Count > 2 && repeats < MAX)
            {
                bool earNotFound = true;
                while (earNotFound && repeats < MAX)
                {
                    repeats++;
                    start %= verts.Count;
                    int middle = (start + 1) % verts.Count;
                    int end = (start + 2) % verts.Count;

                    triangles.Add(new Vector2Triple(verts[start], verts[middle], verts[end]));
                    //is current point convex?
                    bool isConvex =
                        IsPointConvex(verts[start], verts[middle], verts[middle], verts[end], !isCW)
                        && IsPointConvex(verts[middle], verts[end], verts[end], verts[start], !isCW)
                        && IsPointConvex(verts[end], verts[start], verts[start], verts[middle], !isCW);

                    if (!isConvex)
                    {
                        //reject the triangle
                        start++;
                        triangles.RemoveAt(triangles.Count - 1);
                        continue;
                    }

                    bool noPointsIn = true;

                    for (int i = 0; i < verts.Count; i++)
                        if (i != start && i != middle && i != end)
                            if (MeshHelper.IsPointInTriangle(verts[i], verts[start], verts[middle], verts[end]) && noPointsIn)
                            {
                                //there's a point in triangle
                                noPointsIn = false;
                                //reject the triangle
                                start++;
                                triangles.RemoveAt(triangles.Count - 1);
                                break;
                            }

                    if (noPointsIn)
                    {
                        earNotFound = false;
                        //add triangle to set
                        verts.RemoveAt(middle);
                    }
                }
            }
            return triangles;
        }

        public static List<IntTriple> TriangulationToInt3(List<Vector2> sourcePoints)
        {
            List<IntTriple> triangles = new List<IntTriple>();
            int MAX = sourcePoints.Count * sourcePoints.Count;

            //temporary List of points
            List<VecIndexPair> verts = VecIndexPair.Get(sourcePoints);

            //was shape drew clockwise?
            bool isCW = IsShapeClockWise(sourcePoints);
            int start = 0;

            int repeats = 0;

            while (verts.Count > 2 && repeats < MAX)
            {
                bool earNotFound = true;
                while (earNotFound && repeats < MAX)
                {
                    repeats++;
                    start %= verts.Count;
                    int middle = (start + 1) % verts.Count;
                    int end = (start + 2) % verts.Count;

                    //change the order to fit triangle mesh facing
                    triangles.Add(new IntTriple(verts[start].index, verts[end].index, verts[middle].index));

                    //is current point convex?
                    bool isConvex =
                        IsPointConvex(verts[start].v, verts[middle].v, verts[middle].v, verts[end].v, !isCW)
                        && IsPointConvex(verts[middle].v, verts[end].v, verts[end].v, verts[start].v, !isCW)
                        && IsPointConvex(verts[end].v, verts[start].v, verts[start].v, verts[middle].v, !isCW);

                    if (!isConvex)
                    {
                        //reject the triangle
                        start++;
                        triangles.RemoveAt(triangles.Count - 1);
                        continue;
                    }

                    bool noPointsIn = true;

                    for (int i = 0; i < verts.Count; i++)
                    {
                        if (i != start && i != middle && i != end)
                            if (MeshHelper.IsPointInTriangle(verts[i].v, verts[start].v, verts[middle].v, verts[end].v) && noPointsIn)
                            {
                                //there's a point in triangle
                                noPointsIn = false;
                                //reject the triangle
                                start++;
                                triangles.RemoveAt(triangles.Count - 1);
                                break;
                            }
                    }

                    if (noPointsIn)
                    {
                        earNotFound = false;
                        //add triangle to set
                        verts.RemoveAt(middle);
                    }
                }
            }
            return triangles;
        }

        public static bool IsShapeClockWise(List<Vector2> sourcePoints)
        {
            Debug.Assert(sourcePoints.Count>=3, "Triangulation::IsShapeClockwise: points count must be greater than two!");
            double sum = 0;
            for (int i = 0; i < sourcePoints.Count; i++)
            {

                int k1 = i + 1;
                if (k1 >= sourcePoints.Count) k1 -= sourcePoints.Count;

                int k2 = i + 2;
                if (k2 >= sourcePoints.Count) k2 -= sourcePoints.Count;

                if (AreVecsConvex(sourcePoints[i], sourcePoints[k1], sourcePoints[k2]))
                {
                    sum += MeshHelper.AngleBetweenPoints(sourcePoints[i], sourcePoints[k1], sourcePoints[k2]);
                }
                else
                {
                    sum += 360 - MeshHelper.AngleBetweenPoints(sourcePoints[i], sourcePoints[k1], sourcePoints[k2]);
                }
            }
            return System.Math.Round(sum) == 180 * (sourcePoints.Count - 2);
        }

        public static bool CheckIntersectionHelper(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return (v2.y - v1.y) * (v3.x - v1.x) <= (v3.y - v1.y) * (v2.x - v1.x);
        }

        public static bool IsIntersection(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
        {
            if (CheckIntersectionHelper(v1, v2, v3) == CheckIntersectionHelper(v1, v2, v4))
                return false;
            if (CheckIntersectionHelper(v3, v4, v1) == CheckIntersectionHelper(v3, v4, v2))
                return false;
            return true;
        }

        //public static bool IsPointInTriangle(Vector2 v, Vector2 v1, Vector2 v2, Vector2 v3)
        //{
        //    double totalArea = GetTriangleArea(v1, v2, v3);
        //    double AArea = GetTriangleArea(v1, v, v3);
        //    double BArea = GetTriangleArea(v, v2, v3);
        //    double CArea = GetTriangleArea(v1, v2, v);
        //    return totalArea == AArea + BArea + CArea;
        //}

        public static bool IsPointConvex(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4, bool isCW)
        {
            double k;
            k = Mathf.Atan2((v2.x - v1.x) * (v4.y - v3.y) - (v4.x - v3.x) * (v2.y - v1.y), (v2.x - v1.x) * (v4.x - v3.x) + (v2.y - v1.y) * (v4.y - v3.y));
            if (isCW)
                return k < 0;
            else
                return k > 0;
        }

        public static bool AreVecsConvex(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return IsPointConvex(v2, v1, v2, v3, true);
        }

    }
}
