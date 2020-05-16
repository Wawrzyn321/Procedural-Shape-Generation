using System.Collections.Generic;
using UnityEngine;

namespace PSG
{
    public static class SplineSimplification
    {
        public enum Type
        {
            None,
            ByRelativeBoundingBoxArea,
            ByAbsoluteArea,
        }

        public static List<Vector2> Simplify(List<Vector2> points, float minArea, bool isClosed, bool useCopy = true)
        {
            return new List<Vector2>(MeshHelper.ConvertVec3ToVec2(Simplify(new List<Vector3>(MeshHelper.ConvertVec2ToVec3(points)), minArea, true, true)));

            const int minPointsCount = 10;
            var pts = useCopy ? new List<Vector2>(points) : points;
            Debug.Log("2 " + pts.Count + " "+ minArea);
            var bp = false;

            if (isClosed)
            {
                int prevPointCount;
                int index = 0;
                do
                {
                    prevPointCount = pts.Count;
                    while (index < pts.Count && pts.Count >= minPointsCount)
                    {
                        Vector2 p1 = pts[index];
                        Vector2 p2 = pts[(index + 1) % pts.Count];
                        Vector2 p3 = pts[(index + 2) % pts.Count];

                        var area = MeshHelper.GetTriangleArea(p1, p2, p3);
                        if (area < minArea)
                        {
                            if (!bp)
                            {
                                Debug.Log((index + 1) % pts.Count + " " + pts[(index + 1) % pts.Count]);
                                bp = true;
                            }
                            pts.RemoveAt((index + 1) % pts.Count);
                            index += 2;
                        }
                        else
                        {
                            index++;
                        }
                    }
                    index %= pts.Count;

                } while (prevPointCount != pts.Count);
            }
            else
            {
                int prevPointCount;
                do
                {
                    prevPointCount = pts.Count;
                    int index = 0;
                    while (index < pts.Count - 2 && pts.Count >= minPointsCount)
                    {
                        Vector2 p1 = pts[index];
                        Vector2 p2 = pts[index + 1];
                        Vector2 p3 = pts[index + 2];

                        var area = MeshHelper.GetTriangleArea(p1, p2, p3);
                        if (area < minArea)
                        {
                            pts.RemoveAt(index + 1);
                            index += 2;
                        }
                        else
                        {
                            index++;
                        }
                    }

                } while (prevPointCount != pts.Count);
            }
            Debug.Log("2 " + pts.Count);
            return pts;
        }

        public static List<Vector3> Simplify(List<Vector3> points, float minArea, bool isClosed, bool useCopy = true)
        {
            const int minPointsCount = 10;

            var pts = useCopy ? new List<Vector3>(points) : points;
            //Debug.Log("3 " + pts.Count + " " + minArea);
            var bp = false;
            if (isClosed)
            {
                int prevPointCount;
                int index = 0;
                do
                {
                    prevPointCount = pts.Count;
                    while (index < pts.Count && pts.Count >= minPointsCount)
                    {
                        Vector2 p1 = pts[index];
                        Vector2 p2 = pts[(index + 1) % pts.Count];
                        Vector2 p3 = pts[(index + 2) % pts.Count];

                        var area = MeshHelper.GetTriangleArea(p1, p2, p3);
                        if (area < minArea)
                        {
                            if (!bp)
                            {
                                //Debug.Log((index + 1) % pts.Count + " " + pts[(index + 1) % pts.Count]);
                                bp = true;
                            }
                            pts.RemoveAt((index + 1) % pts.Count);
                            index += 2;
                        }
                        else
                        {
                            index++;
                        }
                    }
                    index %= pts.Count;

                } while (prevPointCount != pts.Count);
            }
            else
            {
                int prevPointCount;
                do
                {
                    prevPointCount = pts.Count;
                    int index = 0;
                    while (index < pts.Count - 2 && pts.Count >= minPointsCount)
                    {
                        Vector2 p1 = pts[index];
                        Vector2 p2 = pts[index + 1];
                        Vector2 p3 = pts[index + 2];

                        var area = MeshHelper.GetTriangleArea(p1, p2, p3);
                        if (area < minArea)
                        {
                            pts.RemoveAt(index + 1);
                            index += 2;
                        }
                        else
                        {
                            index++;
                        }
                    }

                } while (prevPointCount != pts.Count);
            }
            //Debug.Log("3 " + pts.Count);
            return pts;
        }
    }
}