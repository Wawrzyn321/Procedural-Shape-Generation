using UnityEngine;
using System.Collections.Generic;
using System;

namespace PSG
{
    //Interpolation between points with a Catmull-Rom spline
    public class CatmullRomSpline : MonoBehaviour
    {
        public const float MIN_RESOLUTION = 0.01f;

        //public Transform[] controlPointsList;
        //public bool isLooping = true;
        //[Range(0.001f, 0.5f)]
        //public float resolution = 0.2f;

        private static int ClampListPos(int pos, int controlPointsLenght)
        {
            if (pos < 0)
            {
                pos = controlPointsLenght - 1;
            }

            if (pos > controlPointsLenght)
            {
                pos = 1;
            }
            else if (pos > controlPointsLenght - 1)
            {
                pos = 0;
            }

            return pos;
        }

        //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
        //http://www.iquilezles.org/www/articles/minispline/minispline.htm
        public static Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos;
        }

        //public float GetLength()
        //{
        //    float length = 0;
        //    for (int i = 0; i < controlPointsList.Length; i++)
        //    {
        //        if ((i == 0 || i == controlPointsList.Length - 2 || i == controlPointsList.Length - 1) && !isLooping)
        //        {
        //            continue;
        //        }

        //        Vector3 p0 = controlPointsList[ClampListPos(i - 1, controlPointsList.Length)].position;
        //        Vector3 p1 = controlPointsList[i].position;
        //        Vector3 p2 = controlPointsList[ClampListPos(i + 1, controlPointsList.Length)].position;
        //        Vector3 p3 = controlPointsList[ClampListPos(i + 2, controlPointsList.Length)].position;

        //        Vector3 lastPos = p1;

        //        for (int j = 1; j <= 1 / resolution; j++)
        //        {
        //            Vector3 newPos = GetCatmullRomPosition(j * resolution, p0, p1, p2, p3);
        //            length += Vector3.Distance(newPos, lastPos);
        //            lastPos = newPos;
        //        }
        //    }
        //    return length;
        //}

        //public float GetApproxLength()
        //{
        //    float length = 0;
        //    for (int i = 0; i < controlPointsList.Length - 1; i++)
        //    {
        //        length += Vector2.Distance(controlPointsList[i].position,
        //            controlPointsList[(i + 1) % controlPointsList.Length].position);
        //    }
        //    return length;
        //}

        public static List<Vector2> GetPoints(IList<Vector2> splinePoints, float precision = 0.2f, bool looping = true)
        {
            List<Vector2> points = new List<Vector2>();
            int count = looping ? splinePoints.Count : splinePoints.Count - 1;
            for (int i = 0; i < count; i++)
            {
                Vector3 p0 = splinePoints[ClampListPos(i - 1, splinePoints.Count)];
                Vector3 p1 = splinePoints[i];
                Vector3 p2 = splinePoints[ClampListPos(i + 1, splinePoints.Count)];
                Vector3 p3 = splinePoints[ClampListPos(i + 2, splinePoints.Count)];

                for (int j = 1; j <= 1 / precision; j++)
                {
                    points.Add(GetCatmullRomPosition(j * precision, p0, p1, p2, p3));
                }
            }
            return points;
        }

        public static List<Vector3> GetPoints(IList<Vector3> splinePoints, float precision = 0.2f, bool looping = true)
        {
            List<Vector3> points = new List<Vector3>();
            int count = looping ? splinePoints.Count : splinePoints.Count - 1;
            for (int i = 0; i < count; i++)
            {
                Vector3 p0 = splinePoints[ClampListPos(i - 1, splinePoints.Count)];
                Vector3 p1 = splinePoints[i];
                Vector3 p2 = splinePoints[ClampListPos(i + 1, splinePoints.Count)];
                Vector3 p3 = splinePoints[ClampListPos(i + 2, splinePoints.Count)];
                for (int j = 1; j <= 1 / precision; j++)
                {
                    points.Add(GetCatmullRomPosition(j * precision, p0, p1, p2, p3));
                }
            }
            return points;
        }

        public static float BoundingBoxArea(IList<Vector2> splinePoints)
        {
            var bounds = MeshHelper.GetBounds(splinePoints);
            return (bounds.z - bounds.x) * (bounds.w - bounds.y);
        }
    }
}