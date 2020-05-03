using UnityEngine;
using System.Collections.Generic;

namespace PSG
{

    //Interpolation between points with a Catmull-Rom spline
    public class CatmullRomSpline : MonoBehaviour
    {
        public const float MIN_RESOLUTION = 0.02f;

        public Transform[] controlPointsList;
        public bool isLooping = true;
        [Range(0.001f, 0.5f)]
        public float resolution = 0.2f;
        
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

        public float GetLength(float resoultion = 0.2f)
        {
            float length = 0;
            for (int i = 0; i < controlPointsList.Length; i++)
            {
                if ((i == 0 || i == controlPointsList.Length - 2 || i == controlPointsList.Length - 1) && !isLooping)
                {
                    continue;
                }

                Vector3 p0 = controlPointsList[ClampListPos(i - 1, controlPointsList.Length)].position;
                Vector3 p1 = controlPointsList[i].position;
                Vector3 p2 = controlPointsList[ClampListPos(i + 1, controlPointsList.Length)].position;
                Vector3 p3 = controlPointsList[ClampListPos(i + 2, controlPointsList.Length)].position;

                Vector3 lastPos = p1;

                for (int j = 1; j <= 1 / resolution; j++)
                {
                    Vector3 newPos = GetCatmullRomPosition(j * resolution, p0, p1, p2, p3);
                    length += Vector3.Distance(newPos, lastPos);
                    lastPos = newPos;
                }
            }
            return length;
        }

        public static Vector3[] DecimatePoints(Vector3[] vertices, float maxDistanceSqr = 0.01f, float maxAngle = 1f)
        {
            int a = vertices.Length;
            List<Vector3> newVertices = new List<Vector3>();

            bool isFirst = true;
            bool isStreak = false;
            float distanceSqr = 0;
            Vector3 firstVert = new Vector3();

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 current = vertices[i];

                //Debug.Log(current.x.ToString("0.00000000")+" "+ current.x.ToString("0.00000000"));

                if (isFirst)
                {
                    isFirst = false;
                    isStreak = true;
                    newVertices.Add(current);
                    firstVert = current;
                }
                else
                {
                    if (!isStreak)
                    {
                        distanceSqr = (firstVert - current).sqrMagnitude;
                        isStreak = true;
                        isFirst = false;
                    }
                    else
                    {
                        float newDistanceSqr = (firstVert - current).sqrMagnitude;
                        if (Mathf.Abs(newDistanceSqr - distanceSqr) > maxDistanceSqr)
                        {
                            if (current != firstVert)
                            {
                                newVertices.Add(current);
                            }
                            isFirst = true;
                            isStreak = false;
                        }
                    }
                }
            }



            //bool isFirst = true;
            //bool isStreak = false;
            //float angle = 0;
            //Vector3 firstVert = new Vector3();

            //for (int i = 0; i < vertices.Length; i++)
            //{
            //    Vector3 current = vertices[i];

            //    if (i != 0)
            //    {
            //        float eee = Mathf.Atan2(firstVert.y - vertices[i - 1].y, firstVert.x - vertices[i - 1].x);
            //        print(Mathf.Abs(MeshHelper.AngleDifference(angle, eee)) * Mathf.Rad2Deg);
            //        print(firstVert.x.ToString("0.000000000") + " " + firstVert.y+"    "+ vertices[i - 1].x + " " + vertices[i - 1].y);
            //    }

            //    if (isFirst)
            //    {
            //        print(i+": isfirst: iF: false, iSs: true");
            //        isFirst = false;
            //        isStreak = true;
            //        newVertices.Add(current);
            //        GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = current;
            //        firstVert = current;
            //    }
            //    else
            //    {
            //        if (!isStreak)
            //        {
            //            print(i + ": nie streak: isstreak T, isfirst f");
            //            angle = Mathf.Atan2(firstVert.y - vertices[i - 1].y, firstVert.x - vertices[i - 1].x);
            //            isStreak = true;
            //            isFirst = false;
            //        }
            //        else
            //        {
            //            float newAngle = Mathf.Atan2(firstVert.y - vertices[i - 1].y, firstVert.x - vertices[i - 1].x);
            //            //print(Mathf.Abs(angle - newAngle) * Mathf.Rad2Deg +" "+ 
            //              //  Mathf.Abs(MeshHelper.AngleDifference(angle,  newAngle)) * Mathf.Rad2Deg);
            //            if (Mathf.Abs(MeshHelper.AngleDifference(angle, newAngle)) * Mathf.Rad2Deg > maxAngle)
            //            {
            //                if (current != firstVert)
            //                {
            //                    newVertices.Add(current);
            //                    GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = current;
            //                    print(i + ": add");
            //                }
            //                isFirst = true;
            //                isStreak = false;
            //                print(i + "fin isf: t, iss: t");
            //            }
            //        }
            //    }
            //}

            //print(newVertices.Count + " " + vertices.Length);

            return newVertices.ToArray();
        }

        public float GetApproxLength()
        {
            float length = 0;
            for (int i = 0; i < controlPointsList.Length - 1; i++)
            {
                length += Vector2.Distance(controlPointsList[i].position,
                    controlPointsList[(i + 1) % controlPointsList.Length].position);
            }
            return length;
        }

        public List<Vector3> GetPoints(float precision = 0.2f, bool isClosed = true)
        {
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < controlPointsList.Length; i++)
            {
                if ((i == 0 || i == controlPointsList.Length - 2 || i == controlPointsList.Length - 1) && !isClosed)
                {
                    continue;
                }

                Vector3 p0 = controlPointsList[ClampListPos(i - 1, controlPointsList.Length)].position;
                Vector3 p1 = controlPointsList[i].position;
                Vector3 p2 = controlPointsList[ClampListPos(i + 1, controlPointsList.Length)].position;
                Vector3 p3 = controlPointsList[ClampListPos(i + 2, controlPointsList.Length)].position;

                for (int j = 1; j <= 1 / resolution; j++)
                {
                    points.Add(GetCatmullRomPosition(j * resolution, p0, p1, p2, p3));
                }
            }
            return points;
        }

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
                //tirnaulated mesh spawnuje się nie tam gdzie trzeba
                for (int j = 1; j <= 1 / precision; j++)
                {
                    points.Add(GetCatmullRomPosition(j * precision, p0, p1, p2, p3));
                }
            }
            return points;
        }
    }
}