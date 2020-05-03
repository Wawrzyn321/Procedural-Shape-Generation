using System.Collections.Generic;
using UnityEngine;

namespace PSG
{
    public static class ConvexHull
    {
        //recurrent subroutine to {QuickHull}
        private static List<Vector2> QuickHullSub(Vector2? A, Vector2? B, List<Vector2> setOfPoints)
        {
            if (A.HasValue == false || B.HasValue == false || setOfPoints == null)
            {
                return null;
            }
            Vector3? C = GetFarthestPoint(GetLineEquation(A.Value, B.Value), setOfPoints);
            if (!C.HasValue)
            {
                return null;
            }

            List<Vector2> s1 = GetPointsOnRight(setOfPoints, A.Value, C.Value);
            List<Vector2> s2 = GetPointsOnRight(setOfPoints, C.Value, B.Value);

            List<Vector2> arg1 = QuickHullSub(A, C, s1);
            List<Vector2> arg3 = QuickHullSub(C, B, s2);

            //QHS(A,C,s1) + C + QHS(C,B,s2)
            List<Vector2> vec = new List<Vector2>();
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

        //get general line equation from two given points
        private static Vector3 GetLineEquation(Vector2 a, Vector2 b)
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
                float cV = bV * a.y - a.x;
                return new Vector3(1, -bV, cV);
            }
        }

        private static Vector2? GetLeftmostPoint(List<Vector2> setOfPoints)
        {
            Vector2? p = null;
            float mX = float.MaxValue;
            for (int i = 0; i < setOfPoints.Count; i++)
            {
                if (setOfPoints[i].x < mX)
                {
                    mX = setOfPoints[i].x;
                    p = setOfPoints[i];
                }
            }
            return p;
        }

        //get pointer to points of highest x coordinate
        private static Vector2? GetRightmostPoint(List<Vector2> setOfPoints)
        {
            Vector2? p = null;
            float mX = float.MinValue;
            for (int i = 0; i < setOfPoints.Count; i++)
            {
                if (setOfPoints[i].x > mX)
                {
                    mX = setOfPoints[i].x;
                    p = setOfPoints[i];
                }
            }
            return p;
        }

        //get vector of points on the right of two given setOfPoints
        private static List<Vector2> GetPointsOnRight(List<Vector2> setOfPoints, Vector2 vL, Vector2 vP)
        {
            List<Vector2> r = new List<Vector2>();
            for (int i = 0; i < setOfPoints.Count; i++)
            {
                if (MeshHelper.GetSide(setOfPoints[i], vL, vP) > 0)
                {
                    r.Add(setOfPoints[i]);
                }
            }
            return r;
        }

        //get points located the farthest from line of given equation
        private static Vector2? GetFarthestPoint(Vector3 lineEquation, List<Vector2> setOfPoints)
        {
            double len = 0;
            Vector2? r = null;
            for (int i = 0; i < setOfPoints.Count; i++)
            {
                double l =
                    Mathf.Abs(lineEquation.x * setOfPoints[i].x + lineEquation.y * setOfPoints[i].y + lineEquation.z)
                    / Mathf.Sqrt(lineEquation.x * lineEquation.x + lineEquation.y * lineEquation.y);
                if (l > len)
                {
                    len = l;
                    r = setOfPoints[i];
                }
            }
            return r;
        }

        //Quick Hull main
        public static List<Vector2> QuickHull(List<Vector2> setOfPoints)
        {
            if (setOfPoints.Count < 2)
            {
                return null;
            }
            Vector2? A = GetLeftmostPoint(setOfPoints);
            Vector2? B = GetRightmostPoint(setOfPoints);

            if (A == null || B == null)
            {
                return null;
            }
            //to get points on left of AB, just get setOfPoints on right of BA!
            List<Vector2> s1 = GetPointsOnRight(setOfPoints, A.Value, B.Value);
            List<Vector2> s2 = GetPointsOnRight(setOfPoints, B.Value, A.Value);

            List<Vector2> arg2 = QuickHullSub(A, B, s1);
            List<Vector2> arg4 = QuickHullSub(B, A, s2);

            //A + QHS(A,B,s1) + B + QHS(B,A,s2)
            List<Vector2> vec = new List<Vector2>();

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

        public static List<Vector2> QuickHull(Vector2[] setOfPoints)
        {
            return QuickHull(new List<Vector2>(setOfPoints));
        }

    }
}
