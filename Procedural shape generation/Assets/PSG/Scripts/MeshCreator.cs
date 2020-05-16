using PSG;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In-editor helper for visual creation of meshes.
/// 
/// Most of fields here are public-but-hidden to expose
/// them to MeshCreatorInspector
/// todo use properties with C# 6 initial values?
/// script.
/// </summary>
public class MeshCreator : MonoBehaviour
{

    public enum MeshType
    {
        Triangle,
        Rectangle,
        Circle,
        Quadrangle,
        Ellipse,
        PointedCircle,
        Cake,
        Convex,
        Star,
        Gear,
        Line,
        TriangulatedMesh,
        SplineShape,
        SplineCurve,
        SplineConvexShape,
    }
    [HideInInspector]
    public MeshType meshType;

    #region Shapes Properties

    [HideInInspector]
    public Vector2 triangleVertex1 = new Vector2(-1.5f, -0.5f);
    [HideInInspector]
    public Vector2 triangleVertex2 = new Vector2(1, 1);
    [HideInInspector]
    public Vector2 triangleVertex3 = new Vector2(2, -0.5f);

    [HideInInspector]
    public Vector2 boxSize = new Vector2(4, 2);
    [HideInInspector]
    public Vector2 boxControlPoint = new Vector2(2, 1);

    [HideInInspector]
    public float circleRadius = 1;
    [HideInInspector]
    public Vector2 circleControlPoint = new Vector2(0.72f, 0.72f);
    [HideInInspector]
    public int circleSides = 24;
    [HideInInspector]
    public bool circleUseCircleCollider = false;

    [HideInInspector]
    public Vector2 quadrangleVertex1 = new Vector2(-2, 2);
    [HideInInspector]
    public Vector2 quadrangleVertex2 = new Vector2(3, 1);
    [HideInInspector]
    public Vector2 quadrangleVertex3 = new Vector2(4, 0);
    [HideInInspector]
    public Vector2 quadrangleVertex4 = new Vector2(-1, -2);

    [HideInInspector]
    public float ellipseHorizontalRadius = 1.2f;
    [HideInInspector]
    public float ellipseVerticalRadius = 1;
    [HideInInspector]
    public Vector2 ellipseControlPoint = new Vector2(1.2f, 1);
    [HideInInspector]
    public int ellipseSides = 32;

    [HideInInspector]
    public float pointedCircleRadius = 1.2f;
    [HideInInspector]
    public Vector2 pointedCircleControlPoint = new Vector2(0.8f, 0.8f);
    [HideInInspector]
    public int pointedCircleSides = 24;
    [HideInInspector]
    public Vector2 pointedCircleShift = new Vector2(2, 2);

    [HideInInspector]
    public float cakeRadius = 1.1f;
    [HideInInspector]
    public Vector2 cakeControlPoint = new Vector2(0.8f, 0.8f);
    [HideInInspector]
    public int cakeSides = 16;
    [HideInInspector]
    public int cakeSidesToFill = 9;

    [HideInInspector]
    public List<Vector2> convexPoints;

    [HideInInspector]
    public float starRadiusA = 0.5f;
    [HideInInspector]
    public Vector2 starControlPointA = new Vector2(0.1f, 0.1f);
    [HideInInspector]
    public float starRadiusB = 1.41f;
    [HideInInspector]
    public Vector2 starControlPointB = new Vector2(1, 1);
    [HideInInspector]
    public int starSides = 5;

    [HideInInspector]
    public float gearInnerRadius = 0.1f;
    [HideInInspector]
    public Vector2 gearInnerControlPoint = new Vector2(0.1f, 0.1f);
    [HideInInspector]
    public float gearRootRadius = 0.9f;
    [HideInInspector]
    public Vector2 gearRootControlPoint = new Vector2(0.7f, 0.7f);
    [HideInInspector]
    public float gearOuterRadius = 1.1f;
    [HideInInspector]
    public Vector2 gearOuterControlPoint = new Vector2(0.8f, 0.8f);
    [HideInInspector]
    public int gearSides = 12;

    [HideInInspector]
    public List<Vector2> linePoints = new List<Vector2> {new Vector2(-2,-2), new Vector2(0, -3),
        new Vector2(2, -1), new Vector2(3, 1), new Vector2(2, 2)};
    [HideInInspector]
    public float lineWidth = 0.3f;
    [HideInInspector]
    public bool lineUseDoubleCollider;
    
    [HideInInspector]
    public List<Vector2> triangulatedPoints = new List<Vector2> {
        new Vector2(-2,-2), new Vector2(0, -3),
        new Vector2(2, -1), new Vector2(3, 1),
        new Vector2(2, 2)
    };
    #endregion

    #region Spline Shape
    [HideInInspector]
    public List<Vector2> splinePoints = new List<Vector2> {
        new Vector2(-3,-3), new Vector2(1,-3),
        new Vector2(0,0), new Vector2(-2,2)
    };
    #endregion

    #region Spline Curve
    [HideInInspector]
    public List<Vector2> splineCurvePoints = new List<Vector2> {
        new Vector2(-2,-3), new Vector2(0, -3),
        new Vector2(1, -1), new Vector2(3, 1),
        new Vector2(2, 2)
    };
    [HideInInspector]
    public float splineCurveWidth = 0.2f;
    [HideInInspector]
    public bool splineCurveUseDoubleCollider = true;
    #endregion

    #region Convex Spline
    [HideInInspector]
    public List<Vector2> convexSplinePoints = new List<Vector2> {
        new Vector2(-2,-3), new Vector2(0, -3),
        new Vector2(1, -1), new Vector2(3, 1),
        new Vector2(2, 2)
    };
    #endregion

    #region Spline Common
    [HideInInspector]
    public float splineResolution = 0.2f;
    [HideInInspector]
    public SplineSimplification.Type splineSimplification = SplineSimplification.Type.None;
    [HideInInspector]
    public float minRelativeSplineArea = 0;
    [HideInInspector]
    public float minAbsoluteSplineArea = 0;
    #endregion

    //common properties
    public Material material;
    public bool attachRigidbody;
    public bool setRandomColor;
    [Range(0f, 10f)]
    public float friction = 0.4f;
    [Range(0f, 1f)]
    public float bounciness = 0.1f;
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        //cache position
        Vector2 p2 = transform.position;

        Vector2 lastPos;
        float angleDelta;
        float angleShift;

        switch (meshType)
        {
            case MeshType.Triangle:
                Gizmos.DrawLine(p2 + triangleVertex1, p2 + triangleVertex2);
                Gizmos.DrawLine(p2 + triangleVertex2, p2 + triangleVertex3);
                Gizmos.DrawLine(p2 + triangleVertex3, p2 + triangleVertex1);
                break;
            case MeshType.Rectangle:
                Gizmos.DrawLine(p2 + new Vector2(-boxSize.x / 2, -boxSize.y / 2), p2 + new Vector2(boxSize.x / 2, -boxSize.y / 2));
                Gizmos.DrawLine(p2 + new Vector2(boxSize.x / 2, -boxSize.y / 2), p2 + new Vector2(boxSize.x / 2, boxSize.y / 2));
                Gizmos.DrawLine(p2 + new Vector2(boxSize.x / 2, boxSize.y / 2), p2 + new Vector2(-boxSize.x / 2, boxSize.y / 2));
                Gizmos.DrawLine(p2 + new Vector2(-boxSize.x / 2, boxSize.y / 2), p2 + new Vector2(-boxSize.x / 2, -boxSize.y / 2));
                break;
            case MeshType.Circle:
                lastPos = Vector2.right * circleRadius;
                angleDelta = 360 * Mathf.Deg2Rad / circleSides;
                for (int i = 1; i < circleSides + 1; i++)
                {
                    var vertPos = new Vector2(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * circleRadius;
                    Gizmos.DrawLine(p2 + lastPos, p2 + vertPos);
                    lastPos = vertPos;
                }
                break;
            case MeshType.Quadrangle:
                Gizmos.DrawLine(p2 + quadrangleVertex1, p2 + quadrangleVertex2);
                Gizmos.DrawLine(p2 + quadrangleVertex2, p2 + quadrangleVertex3);
                Gizmos.DrawLine(p2 + quadrangleVertex3, p2 + quadrangleVertex4);
                Gizmos.DrawLine(p2 + quadrangleVertex4, p2 + quadrangleVertex1);
                break;
            case MeshType.Ellipse:
                lastPos = Vector2.right * ellipseHorizontalRadius;
                angleDelta = 360 * Mathf.Deg2Rad / ellipseSides;
                for (int i = 1; i < ellipseSides + 1; i++)
                {
                    var vertPos = new Vector2(Mathf.Cos(i * angleDelta) * ellipseHorizontalRadius, Mathf.Sin(i * angleDelta) * ellipseVerticalRadius);
                    Gizmos.DrawLine(p2 + lastPos, p2 + vertPos);
                    lastPos = vertPos;
                }
                break;
            case MeshType.PointedCircle:
                lastPos = Vector2.right * pointedCircleRadius;
                angleDelta = 360 * Mathf.Deg2Rad / pointedCircleSides;
                for (int i = 1; i < pointedCircleSides + 1; i++)
                {
                    var vertPos = new Vector2(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * pointedCircleRadius;
                    Gizmos.DrawLine(p2 + lastPos, p2 + vertPos);
                    lastPos = vertPos;
                }

                float angle = Mathf.Atan2(pointedCircleShift.y, pointedCircleShift.x);

                Vector2 a = new Vector2(Mathf.Cos(angle + Mathf.Deg2Rad * 90), Mathf.Sin(angle + Mathf.Deg2Rad * 90)) * pointedCircleRadius;
                Vector2 b = new Vector2(Mathf.Cos(angle - Mathf.Deg2Rad * 90), Mathf.Sin(angle - Mathf.Deg2Rad * 90)) * pointedCircleRadius;

                Gizmos.DrawLine(p2 + a, p2 + pointedCircleShift);
                Gizmos.DrawLine(p2 + pointedCircleShift, p2 + b);
                Gizmos.DrawLine(p2 + b, p2 + a);
                break;
            case MeshType.Cake:
                lastPos = Vector2.right * cakeRadius;
                angleDelta = 360 * Mathf.Deg2Rad / cakeSides;
                for (int i = 1; i < cakeSidesToFill + 1; i++)
                {
                    var vertPos = new Vector2(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * cakeRadius;
                    Gizmos.DrawLine(p2 + lastPos, p2 + vertPos);
                    lastPos = vertPos;
                }
                if (cakeSidesToFill != cakeSides)
                {
                    Gizmos.DrawLine(p2, p2 + lastPos);
                    Gizmos.DrawLine(p2, p2 + Vector2.right * cakeRadius);
                }

                break;
            case MeshType.Convex:
                if (convexPoints != null && convexPoints.Count < 2) return;
                var convexOutline = ConvexHull.QuickHull(convexPoints);
                for (int i = 0; i < convexOutline.Count; i++)
                {
                    Gizmos.DrawLine(p2 + convexOutline[i], p2 + convexOutline[(i + 1) % convexOutline.Count]);
                }
                break;
            case MeshType.Star:
                angleDelta = 360 / (float)starSides / 2 * Mathf.Deg2Rad;
                angleShift = -360f / (starSides * 4) * Mathf.Deg2Rad;
                lastPos = new Vector3(Mathf.Cos(angleShift), Mathf.Sin(angleShift)) * starRadiusA;
                for (int i = 1; i < starSides * 2 + 1; i++)
                {
                    var vertPos = new Vector2(Mathf.Cos(i * angleDelta + angleShift),
                        Mathf.Sin(i * angleDelta + angleShift));
                    vertPos *= (i % 2 == 0 ? starRadiusA : starRadiusB);
                    Gizmos.DrawLine(p2 + lastPos, p2 + vertPos);
                    lastPos = vertPos;
                }
                break;
            case MeshType.Gear:
                angleDelta = 360 * Mathf.Deg2Rad / gearSides / 2;
                angleShift = angleDelta * 0.5f;
                float outerAngleShift = angleDelta * 0.2f;
                Vector2 lastInnerPos = new Vector2(Mathf.Cos(angleShift), Mathf.Sin(angleShift)) * gearInnerRadius;
                Vector2 lastRootPos = new Vector2(Mathf.Cos(angleShift), Mathf.Sin(angleShift)) * gearRootRadius;
                Vector2 lastOuterPos = new Vector2(Mathf.Cos(angleShift + outerAngleShift), Mathf.Sin(angleShift + outerAngleShift)) * gearOuterRadius;
                for (int i = 1; i < gearSides * 2 + 1; i++)
                {
                    var vertPos = new Vector2(Mathf.Cos(i * angleDelta + angleShift), Mathf.Sin(i * angleDelta + angleShift)) * gearInnerRadius;
                    Gizmos.DrawLine(p2 + lastInnerPos, p2 + vertPos);
                    lastInnerPos = vertPos;

                    vertPos = new Vector3(Mathf.Cos(i * angleDelta + angleShift), Mathf.Sin(i * angleDelta + angleShift)) * gearRootRadius;
                    Gizmos.DrawLine(p2 + lastRootPos, p2 + vertPos);
                    lastRootPos = vertPos;

                    int sign = (i % 2) * 2 - 1;
                    vertPos = new Vector3(Mathf.Cos(i * angleDelta + angleShift - outerAngleShift * sign), Mathf.Sin(i * angleDelta + angleShift - outerAngleShift * sign)) * gearOuterRadius;
                    Gizmos.DrawLine(p2 + lastRootPos, p2 + vertPos);
                    if (i % 2 == 1)
                    {
                        Gizmos.DrawLine(p2 + lastOuterPos, p2 + vertPos);
                    }
                    lastOuterPos = vertPos;
                }
                break;
            case MeshType.Line:
                for (int i = 0; i < linePoints.Count - 1; i++)
                {
                    Gizmos.DrawLine(p2 + linePoints[i], p2 + linePoints[i + 1]);
                }

                if (linePoints.Count < 2 || lineWidth <= 0.0001f) return;

                Vector2[] points = GetLinePoints();
                for (int i = 0; i < points.Length - 1; i++)
                {
                    Gizmos.DrawLine(p2 + points[i], p2 + points[i + 1]);
                }
                break;
            case MeshType.TriangulatedMesh:
                for (int i = 0; i < triangulatedPoints.Count; i++)
                {
                    Gizmos.DrawLine(p2 + triangulatedPoints[i],
                        p2 + triangulatedPoints[(i + 1) % triangulatedPoints.Count]);
                }
                break;
            case MeshType.SplineShape:
                List<Vector2> shapePoints = CatmullRomSpline.GetPoints(splinePoints.ToArray(), splineResolution);
                shapePoints = SimplifySplinePoints(shapePoints, true);
                for (int i = 0; i < shapePoints.Count; i++)
                {
                    Gizmos.DrawLine(p2 + shapePoints[i], p2 + shapePoints[(i + 1) % shapePoints.Count]);
                }
                break;
            case MeshType.SplineCurve:
                List<Vector2> curvePoints = CatmullRomSpline.GetPoints(splineCurvePoints, splineResolution, false);
                curvePoints = SimplifySplinePoints(curvePoints, false);
                int len = curvePoints.Count;
                if (len <= 1) return;

                for (int i = 0; i < len - 1; i++)
                {
                    Gizmos.DrawLine(p2 + curvePoints[i], p2 + curvePoints[i + 1]);
                }

                List<Vector2> leftCurvePoints = new List<Vector2>();
                List<Vector2> rightCurvePoints = new List<Vector2>();
                // first vertex
                {
                    Vector2 dir = (curvePoints[1] - curvePoints[0]).normalized;
                    dir = new Vector2(-dir.y, dir.x) * splineCurveWidth;
                    Gizmos.DrawLine(p2 + curvePoints[0] - dir, p2 + curvePoints[0] + dir);
                    leftCurvePoints.Add(p2 + curvePoints[0] - dir);
                    rightCurvePoints.Add(p2 + curvePoints[0] + dir);
                }
                // second to last - 1 vertices
                for (int i = 1; i < len - 1; i++)
                {
                    float leftAngle = Mathf.Atan2(curvePoints[i].y - curvePoints[i - 1].y, curvePoints[i].x - curvePoints[i - 1].x) * Mathf.Rad2Deg + 90;
                    float rightAngle = Mathf.Atan2(curvePoints[i + 1].y - curvePoints[i].y, curvePoints[i + 1].x - curvePoints[i].x) * Mathf.Rad2Deg + 90;
                    float middleAngle = leftAngle + Mathf.DeltaAngle(leftAngle, rightAngle) * 0.5f;
                    Vector2 dir = new Vector2(Mathf.Cos(middleAngle * Mathf.Deg2Rad), Mathf.Sin(middleAngle * Mathf.Deg2Rad)) * splineCurveWidth;
                    //Gizmos.DrawLine(p2 + curvePoints[i] - dir, p2 + curvePoints[i] + dir);
                    leftCurvePoints.Add(p2 + curvePoints[i] - dir);
                    rightCurvePoints.Add(p2 + curvePoints[i] + dir);
                }
                // last vertex
                {
                    Vector2 dir = (curvePoints[len - 2] - curvePoints[len - 1]).normalized;
                    dir = new Vector2(-dir.y, dir.x) * splineCurveWidth;
                    Gizmos.DrawLine(p2 + curvePoints[len - 1] - dir, p2 + curvePoints[len - 1] + dir);
                    leftCurvePoints.Add(p2 + curvePoints[len - 1] + dir);
                    rightCurvePoints.Add(p2 + curvePoints[len - 1] - dir);
                }

                for (int i = 0; i < leftCurvePoints.Count - 1; i++)
                {
                    Gizmos.DrawLine(leftCurvePoints[i], leftCurvePoints[i + 1]);
                }
                for (int i = 0; i < rightCurvePoints.Count - 1; i++)
                {
                    Gizmos.DrawLine(rightCurvePoints[i], rightCurvePoints[i + 1]);
                }
                break;
            case MeshType.SplineConvexShape:
                List<Vector2> splineConvexPoints = ConvexHull.QuickHull(convexSplinePoints);
                splineConvexPoints = CatmullRomSpline.GetPoints(splineConvexPoints, splineResolution);
                splineConvexPoints = SimplifySplinePoints(splineConvexPoints, true);
                for (int i = 0; i < splineConvexPoints.Count; i++)
                {
                    Gizmos.DrawLine(p2 + splineConvexPoints[i], p2 + splineConvexPoints[(i + 1) % splineConvexPoints.Count]);
                }
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }

    private List<Vector2> SimplifySplinePoints(List<Vector2> points, bool isClosed)
    {
        if (splineSimplification == SplineSimplification.Type.None)
        {
            return points;
        }
        else
        {
            return SplineSimplification.Simplify(points, minAbsoluteSplineArea, isClosed);
        }
    }

    private Vector2[] GetLinePoints()
    {
        List<Vector2> cachedVertsLeft = new List<Vector2>();
        List<Vector2> cachedVertsRight = new List<Vector2>();

        float deg90 = Mathf.Deg2Rad * 90;

        //add first two vertices
        int currentVertIndex = 0;
        float angle = Mathf.Atan2(linePoints[1].y - linePoints[0].y, linePoints[1].x - linePoints[0].x);
        float oldAngle, angleDiff;
        Vector2 p1 = new Vector2(Mathf.Cos(angle + deg90), Mathf.Sin(angle + deg90)) * lineWidth;
        Vector2 p2 = new Vector2(Mathf.Cos(angle - deg90), Mathf.Sin(angle - deg90)) * lineWidth;
        cachedVertsLeft.Add(linePoints[currentVertIndex] + p1);
        cachedVertsRight.Add(linePoints[currentVertIndex] + p2);
        oldAngle = angle;
        currentVertIndex++;
        // add middle vertices
        for (int i = 0; i < linePoints.Count - 2; i++, currentVertIndex++)
        {
            angle = Mathf.Atan2(linePoints[currentVertIndex + 1].y - linePoints[currentVertIndex].y, linePoints[currentVertIndex + 1].x - linePoints[currentVertIndex].x);
            angleDiff = oldAngle + MeshHelper.AngleDifference(oldAngle, angle) * 0.5f;
            p1 = new Vector2(Mathf.Cos(angleDiff + deg90), Mathf.Sin(angleDiff + deg90)) * lineWidth;
            p2 = new Vector2(Mathf.Cos(angleDiff - deg90), Mathf.Sin(angleDiff - deg90)) * lineWidth;
            cachedVertsLeft.Add(linePoints[currentVertIndex] + p1);
            cachedVertsRight.Add(linePoints[currentVertIndex] + p2);
            oldAngle = angle;
        }

        angle = Mathf.Atan2(linePoints[currentVertIndex].y - linePoints[currentVertIndex - 1].y, linePoints[currentVertIndex].x - linePoints[currentVertIndex - 1].x);
        p1 = new Vector2(Mathf.Cos(angle + deg90), Mathf.Sin(angle + deg90)) * lineWidth;
        p2 = new Vector2(Mathf.Cos(angle - deg90), Mathf.Sin(angle - deg90)) * lineWidth;
        cachedVertsLeft.Add(linePoints[currentVertIndex] + p1);
        cachedVertsRight.Add(linePoints[currentVertIndex] + p2);

        Vector2[] points = new Vector2[cachedVertsRight.Count + cachedVertsLeft.Count + 1];
        for (int i = 0; i < cachedVertsLeft.Count; i++)
        {
            points[i] = cachedVertsLeft[i];
        }
        for (int i = 0; i < cachedVertsRight.Count; i++)
        {
            points[i + cachedVertsLeft.Count] = cachedVertsRight[cachedVertsRight.Count - 1 - i];
        }
        if (linePoints[0] != linePoints[linePoints.Count - 1])
        {
            points[points.Length - 1] = cachedVertsLeft[0];
        }

        return points;
    }

}
