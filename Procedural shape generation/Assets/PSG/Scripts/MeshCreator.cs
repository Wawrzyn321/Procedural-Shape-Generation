using PSG;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In-editor helper for visual creation of meshes.
/// 
/// Most of fields here are public-but-hidden to easily
/// expose them both to Inspector and MeshCreatorInspector
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
    public Vector3 quadrangleVertex1 = new Vector3(-2, 2);
    [HideInInspector]
    public Vector3 quadrangleVertex2 = new Vector3(3, 1);
    [HideInInspector]
    public Vector3 quadrangleVertex3 = new Vector3(4, 0);
    [HideInInspector]
    public Vector3 quadrangleVertex4 = new Vector3(-1, -2);

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

        //cache position - 2 and 3 dimensional vectors to prevent type conversion
        Vector2 p2 = transform.position;
        Vector3 p3 = transform.position;

        Vector3 lastPos;
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
                lastPos = Vector3.right * circleRadius;
                angleDelta = 360 * Mathf.Deg2Rad / circleSides;
                for (int i = 1; i < circleSides + 1; i++)
                {
                    Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * circleRadius;
                    Gizmos.DrawLine(p3 + lastPos, p3 + vertPos);
                    lastPos = vertPos;
                }
                break;
            case MeshType.Quadrangle:
                Gizmos.DrawLine(p3 + quadrangleVertex1, p3 + quadrangleVertex2);
                Gizmos.DrawLine(p3 + quadrangleVertex2, p3 + quadrangleVertex3);
                Gizmos.DrawLine(p3 + quadrangleVertex3, p3 + quadrangleVertex4);
                Gizmos.DrawLine(p3 + quadrangleVertex4, p3 + quadrangleVertex1);
                break;
            case MeshType.Ellipse:
                lastPos = Vector3.right * ellipseHorizontalRadius;
                angleDelta = 360 * Mathf.Deg2Rad / ellipseSides;
                for (int i = 1; i < ellipseSides + 1; i++)
                {
                    Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta) * ellipseHorizontalRadius, Mathf.Sin(i * angleDelta) * ellipseVerticalRadius);
                    Gizmos.DrawLine(p3 + lastPos, p3 + vertPos);
                    lastPos = vertPos;
                }
                break;
            case MeshType.PointedCircle:
                lastPos = Vector3.right * pointedCircleRadius;
                angleDelta = 360 * Mathf.Deg2Rad / pointedCircleSides;
                for (int i = 1; i < pointedCircleSides + 1; i++)
                {
                    Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * pointedCircleRadius;
                    Gizmos.DrawLine(p3 + lastPos, p3 + vertPos);
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
                lastPos = Vector3.right * cakeRadius;
                angleDelta = 360 * Mathf.Deg2Rad / cakeSides;
                for (int i = 1; i < cakeSidesToFill + 1; i++)
                {
                    Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta), Mathf.Sin(i * angleDelta)) * cakeRadius;
                    Gizmos.DrawLine(p3 + lastPos, p3 + vertPos);
                    lastPos = vertPos;
                }
                if (cakeSidesToFill != cakeSides)
                {
                    Gizmos.DrawLine(p3, p3 + lastPos);
                    Gizmos.DrawLine(p3, p3 + Vector3.right * cakeRadius);
                }

                break;
            case MeshType.Convex:
                if (convexPoints != null && convexPoints.Count < 2) return;
                List<Vector3> convexOutline = GetConvexPoints();
                for (int i = 0; i < convexOutline.Count; i++)
                {
                    Gizmos.DrawLine(p3 + convexOutline[i], p3 + convexOutline[(i + 1) % convexOutline.Count]);
                }
                break;
            case MeshType.Star:
                angleDelta = 360 / (float)starSides / 2 * Mathf.Deg2Rad;
                angleShift = -360f / (starSides * 4) * Mathf.Deg2Rad;
                lastPos = new Vector3(Mathf.Cos(angleShift), Mathf.Sin(angleShift)) * starRadiusA;
                for (int i = 1; i < starSides * 2 + 1; i++)
                {
                    Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta + angleShift),
                        Mathf.Sin(i * angleDelta + angleShift));
                    vertPos *= (i % 2 == 0 ? starRadiusA : starRadiusB);
                    Gizmos.DrawLine(p3 + lastPos, p3 + vertPos);
                    lastPos = vertPos;
                }
                break;
            case MeshType.Gear:
                angleDelta = 360 * Mathf.Deg2Rad / gearSides / 2;
                angleShift = angleDelta * 0.5f;
                float outerAngleShift = angleDelta * 0.2f;
                Vector3 lastInnerPos = new Vector3(Mathf.Cos(angleShift), Mathf.Sin(angleShift)) * gearInnerRadius;
                Vector3 lastRootPos = new Vector3(Mathf.Cos(angleShift), Mathf.Sin(angleShift)) * gearRootRadius;
                Vector3 lastOuterPos = new Vector3(Mathf.Cos(angleShift + outerAngleShift), Mathf.Sin(angleShift + outerAngleShift)) * gearOuterRadius;
                for (int i = 1; i < gearSides * 2 + 1; i++)
                {
                    Vector3 vertPos = new Vector3(Mathf.Cos(i * angleDelta + angleShift), Mathf.Sin(i * angleDelta + angleShift)) * gearInnerRadius;
                    Gizmos.DrawLine(p3 + lastInnerPos, p3 + vertPos);
                    lastInnerPos = vertPos;

                    vertPos = new Vector3(Mathf.Cos(i * angleDelta + angleShift), Mathf.Sin(i * angleDelta + angleShift)) * gearRootRadius;
                    Gizmos.DrawLine(p3 + lastRootPos, p3 + vertPos);
                    lastRootPos = vertPos;

                    int sign = (i % 2) * 2 - 1;
                    vertPos = new Vector3(Mathf.Cos(i * angleDelta + angleShift - outerAngleShift * sign), Mathf.Sin(i * angleDelta + angleShift - outerAngleShift * sign)) * gearOuterRadius;
                    Gizmos.DrawLine(p3 + lastRootPos, p3 + vertPos);
                    if (i % 2 == 1)
                    {
                        Gizmos.DrawLine(p3 + lastOuterPos, p3 + vertPos);
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
        }
    }

    private List<Vector3> GetConvexPoints()
    {
        return ConvexMesh.QuickHull(new List<Vector3>(MeshHelper.ConvertVec2ToVec3(convexPoints)));
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
