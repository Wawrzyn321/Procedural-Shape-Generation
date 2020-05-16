using UnityEditor;
using UnityEngine;
using PSG;
using System.Collections.Generic;

[CustomEditor(typeof(MeshCreator), true)]
public class MeshCreatorEditor : Editor
{
    private Vector2ListWrapper linePointsList;
    private Vector2ListWrapper convexPointList;
    private Vector2ListWrapper triangulatedPointList;
    private Vector2ListWrapper splineShapePointList;
    private Vector2ListWrapper splineCurvePointList;
    private Vector2ListWrapper convexSplinePointList;

    private void OnEnable()
    {
        linePointsList = new Vector2ListWrapper(serializedObject, "linePoints", "Line Points");
        convexPointList = new Vector2ListWrapper(serializedObject, "convexPoints", "Convex Shape Points");
        triangulatedPointList = new Vector2ListWrapper(serializedObject, "triangulatedPoints", "Triangulated Mesh Points");
        splineShapePointList = new Vector2ListWrapper(serializedObject, "splinePoints", "Spline Shape Points");
        splineCurvePointList = new Vector2ListWrapper(serializedObject, "splineCurvePoints", "Spline Curve Points");
        convexSplinePointList = new Vector2ListWrapper(serializedObject, "convexSplinePoints", "Convex Spline Points");
    }

    //standard override
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshCreator meshCreatorScript = (MeshCreator) target;

        DrawMeshTypeInspector(meshCreatorScript);

        if (GUILayout.Button("Build"))
        {
            BuildSelectedMesh(meshCreatorScript);
        }
    }

    private void OnSceneGUI()
    {
        MeshCreator meshCreator = (MeshCreator) target;
        DrawHandles(meshCreator);
    }

    private void DrawHandles(MeshCreator meshCreator)
    {
        //cache casted position
        Vector2 p2 = meshCreator.transform.position;
        //and this one too, for brevity
        Vector3 p3 = meshCreator.transform.position;

        switch (meshCreator.meshType)
        {
            case MeshCreator.MeshType.Triangle:
                meshCreator.triangleVertex1 = Handles.DoPositionHandle(p2 + meshCreator.triangleVertex1, Quaternion.identity) - p3;
                meshCreator.triangleVertex2 = Handles.DoPositionHandle(p2 + meshCreator.triangleVertex2, Quaternion.identity) - p3;
                meshCreator.triangleVertex3 = Handles.DoPositionHandle(p2 + meshCreator.triangleVertex3, Quaternion.identity) - p3;
                break;
            case MeshCreator.MeshType.Rectangle:
                meshCreator.boxControlPoint = meshCreator.boxSize / 2;
                meshCreator.boxControlPoint = Handles.DoPositionHandle(p2 + meshCreator.boxControlPoint, Quaternion.identity) - p3;
                meshCreator.boxSize = meshCreator.boxControlPoint * 2;
                break;
            case MeshCreator.MeshType.Circle:
                meshCreator.circleControlPoint = SetVectorLength(meshCreator.circleControlPoint, meshCreator.circleRadius);
                meshCreator.circleControlPoint = Handles.DoPositionHandle(p2 + meshCreator.circleControlPoint, Quaternion.identity) - p3;
                meshCreator.circleRadius = meshCreator.circleControlPoint.magnitude;
                break;
            case MeshCreator.MeshType.Quadrangle:
                meshCreator.quadrangleVertex1 = Handles.DoPositionHandle(p2 + meshCreator.quadrangleVertex1, Quaternion.identity) - p3;
                meshCreator.quadrangleVertex2 = Handles.DoPositionHandle(p2 + meshCreator.quadrangleVertex2, Quaternion.identity) - p3;
                meshCreator.quadrangleVertex3 = Handles.DoPositionHandle(p2 + meshCreator.quadrangleVertex3, Quaternion.identity) - p3;
                meshCreator.quadrangleVertex4 = Handles.DoPositionHandle(p2 + meshCreator.quadrangleVertex4, Quaternion.identity) - p3;
                break;
            case MeshCreator.MeshType.Ellipse:
                meshCreator.ellipseControlPoint = new Vector2(meshCreator.ellipseHorizontalRadius, meshCreator.ellipseVerticalRadius);
                meshCreator.ellipseControlPoint = Handles.DoPositionHandle(p2 + meshCreator.ellipseControlPoint, Quaternion.identity) - p3;
                meshCreator.ellipseHorizontalRadius = meshCreator.ellipseControlPoint.x;
                meshCreator.ellipseVerticalRadius = meshCreator.ellipseControlPoint.y;
                break;
            case MeshCreator.MeshType.PointedCircle:
                meshCreator.pointedCircleControlPoint = SetVectorLength(meshCreator.pointedCircleControlPoint, meshCreator.pointedCircleRadius);
                meshCreator.pointedCircleControlPoint = Handles.DoPositionHandle(p2 + meshCreator.pointedCircleControlPoint, Quaternion.identity) - p3;
                meshCreator.pointedCircleRadius = meshCreator.pointedCircleControlPoint.magnitude;
                meshCreator.pointedCircleShift = Handles.DoPositionHandle(p2 + meshCreator.pointedCircleShift, Quaternion.identity) - p3;
                break;
            case MeshCreator.MeshType.Cake:
                meshCreator.cakeControlPoint = SetVectorLength(meshCreator.cakeControlPoint, meshCreator.cakeRadius);
                meshCreator.cakeControlPoint = Handles.DoPositionHandle(p2 + meshCreator.cakeControlPoint, Quaternion.identity) - p3;
                meshCreator.cakeRadius = meshCreator.cakeControlPoint.magnitude;
                break;
            case MeshCreator.MeshType.Convex:
                for (int i = 0; i < meshCreator.convexPoints.Count; i++)
                {
                    meshCreator.convexPoints[i] = Handles.DoPositionHandle(p2 + meshCreator.convexPoints[i], Quaternion.identity) - p3;
                }
                break;
            case MeshCreator.MeshType.Star:
                meshCreator.starControlPointA = SetVectorLength(meshCreator.starControlPointA, meshCreator.starRadiusA);
                meshCreator.starControlPointB = SetVectorLength(meshCreator.starControlPointB, meshCreator.starRadiusB);
                meshCreator.starControlPointA = Handles.DoPositionHandle(p2 + meshCreator.starControlPointA, Quaternion.identity) - p3;
                meshCreator.starControlPointB = Handles.DoPositionHandle(p2 + meshCreator.starControlPointB, Quaternion.identity) - p3;
                meshCreator.starRadiusA = meshCreator.starControlPointA.magnitude;
                meshCreator.starRadiusB = meshCreator.starControlPointB.magnitude;
                break;
            case MeshCreator.MeshType.Gear:
                meshCreator.gearInnerControlPoint = SetVectorLength(meshCreator.gearInnerControlPoint, meshCreator.gearInnerRadius);
                meshCreator.gearRootControlPoint = SetVectorLength(meshCreator.gearRootControlPoint, meshCreator.gearRootRadius);
                meshCreator.gearOuterControlPoint = SetVectorLength(meshCreator.gearOuterControlPoint, meshCreator.gearOuterRadius);
                meshCreator.gearInnerControlPoint = Handles.DoPositionHandle(p2 + meshCreator.gearInnerControlPoint, Quaternion.identity) - p3;
                meshCreator.gearRootControlPoint = Handles.DoPositionHandle(p2 + meshCreator.gearRootControlPoint, Quaternion.identity) - p3;
                meshCreator.gearOuterControlPoint = Handles.DoPositionHandle(p2 + meshCreator.gearOuterControlPoint, Quaternion.identity) - p3;
                meshCreator.gearInnerRadius = meshCreator.gearInnerControlPoint.magnitude;
                meshCreator.gearRootRadius = meshCreator.gearRootControlPoint.magnitude;
                meshCreator.gearOuterRadius = meshCreator.gearOuterControlPoint.magnitude;
                break;
            case MeshCreator.MeshType.Line:
                for (int i = 0; i < meshCreator.linePoints.Count; i++)
                {
                    meshCreator.linePoints[i] = Handles.DoPositionHandle(p2 + meshCreator.linePoints[i], Quaternion.identity) - p3;
                }
                break;
            case MeshCreator.MeshType.TriangulatedMesh:
                for (int i = 0; i < meshCreator.triangulatedPoints.Count; i++)
                {
                    meshCreator.triangulatedPoints[i] = Handles.DoPositionHandle(p2 + meshCreator.triangulatedPoints[i], Quaternion.identity) - p3;
                }
                break;
            case MeshCreator.MeshType.SplineShape:
                for (int i = 0; i < meshCreator.splinePoints.Count; i++)
                {
                    meshCreator.splinePoints[i] = Handles.DoPositionHandle(p2 + meshCreator.splinePoints[i], Quaternion.identity) - p3;
                }
                break;
            case MeshCreator.MeshType.SplineCurve:
                for (int i = 0; i < meshCreator.splineCurvePoints.Count; i++)
                {
                    meshCreator.splineCurvePoints[i] = Handles.DoPositionHandle(p2 + meshCreator.splineCurvePoints[i], Quaternion.identity) - p3;
                }
                //Handles.Button(Vector3.zero, Quaternion.identity, 10, 6, (id, position, rotation, size, type) => { });
                break;
            case MeshCreator.MeshType.SplineConvexShape:
                for (int i = 0; i < meshCreator.convexSplinePoints.Count; i++)
                {
                    meshCreator.convexSplinePoints[i] = Handles.DoPositionHandle(p2 + meshCreator.convexSplinePoints[i], Quaternion.identity) - p3;
                }
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }

    private Vector2 SetVectorLength(Vector2 vector, float length)
    {
        return vector.normalized * length;
    }

    private void DrawSplineInspector(MeshCreator meshCreator, List<Vector2> points, bool isClosed)
    {
        meshCreator.splineResolution = EditorGUILayout.Slider("Resolution", meshCreator.splineResolution, CatmullRomSpline.MIN_RESOLUTION, 0.25f);
        meshCreator.splineSimplification = (SplineSimplification.Type)EditorGUILayout.EnumPopup("Simplify spline", meshCreator.splineSimplification);
        if (meshCreator.splineSimplification == SplineSimplification.Type.None) return;

        float area = CatmullRomSpline.BoundingBoxArea(points);
        if (meshCreator.splineSimplification == SplineSimplification.Type.ByRelativeBoundingBoxArea)
        {
            meshCreator.minRelativeSplineArea = EditorGUILayout.Slider("Minimal shape area percentage", meshCreator.minRelativeSplineArea, 0, 1, null);
            meshCreator.minAbsoluteSplineArea = meshCreator.minRelativeSplineArea * area;
            EditorGUI.BeginDisabledGroup(true);
            meshCreator.minAbsoluteSplineArea = EditorGUILayout.Slider("Minimal absolute area", meshCreator.minAbsoluteSplineArea, 0, area, null);
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            meshCreator.minAbsoluteSplineArea = EditorGUILayout.Slider("Minimal absolute area", meshCreator.minAbsoluteSplineArea, 0, area, null);
            meshCreator.minRelativeSplineArea = Mathf.Clamp01(meshCreator.minAbsoluteSplineArea / area);
            EditorGUI.BeginDisabledGroup(true);
            meshCreator.minRelativeSplineArea = EditorGUILayout.Slider("Minimal shape area percentage", meshCreator.minRelativeSplineArea, 0, 1, null);
            EditorGUI.EndDisabledGroup();
        }
        var simplifiedPoints = GetSimplifiedSplinePoints(meshCreator, points, isClosed);
        if (points.Count != simplifiedPoints.Count)
        {
            EditorGUILayout.HelpBox("Point count: " + simplifiedPoints.Count + "/" + points.Count, MessageType.Info);
        }
    }

    private List<Vector2> GetSimplifiedSplinePoints(MeshCreator meshCreator, List<Vector2> points, bool isClosed)
    {
        if (meshCreator.splineSimplification == SplineSimplification.Type.None)
        {
            return points;
        }
        else
        {
            return SplineSimplification.Simplify(points, meshCreator.minAbsoluteSplineArea, isClosed);
        }
    }
    private void DrawMeshTypeInspector(MeshCreator meshCreator)
    {
        meshCreator.meshType = (MeshCreator.MeshType)EditorGUILayout.EnumPopup("Type", meshCreator.meshType);
        GUILayoutOption[] emptyLayout = null;

        switch (meshCreator.meshType)
        {
            case MeshCreator.MeshType.Triangle:
                meshCreator.triangleVertex1 = EditorGUILayout.Vector2Field("First vertex", meshCreator.triangleVertex1);
                meshCreator.triangleVertex2 = EditorGUILayout.Vector2Field("Second vertex", meshCreator.triangleVertex2);
                meshCreator.triangleVertex3 = EditorGUILayout.Vector2Field("Third vertex", meshCreator.triangleVertex3);
                break;
            case MeshCreator.MeshType.Rectangle:
                meshCreator.boxSize = EditorGUILayout.Vector2Field("Size", meshCreator.boxSize);
                break;
            case MeshCreator.MeshType.Circle:
                meshCreator.circleRadius = EditorGUILayout.Slider("Radius", meshCreator.circleRadius, 0.0001f, 16, null);
                meshCreator.circleSides = EditorGUILayout.IntSlider("Sides", meshCreator.circleSides, 3, 128);
                meshCreator.circleUseCircleCollider = EditorGUILayout.Toggle("Use CircleCollider", meshCreator.circleUseCircleCollider);
                break;
            case MeshCreator.MeshType.Quadrangle:
                meshCreator.quadrangleVertex1 = EditorGUILayout.Vector3Field("First vertex", meshCreator.quadrangleVertex1);
                meshCreator.quadrangleVertex2 = EditorGUILayout.Vector3Field("Second vertex", meshCreator.quadrangleVertex2);
                meshCreator.quadrangleVertex3 = EditorGUILayout.Vector3Field("Third vertex", meshCreator.quadrangleVertex3);
                meshCreator.quadrangleVertex4 = EditorGUILayout.Vector3Field("Fourth vertex", meshCreator.quadrangleVertex4);
                break;
            case MeshCreator.MeshType.Ellipse:
                meshCreator.ellipseHorizontalRadius = EditorGUILayout.Slider("Horizontal Radius", meshCreator.ellipseHorizontalRadius, 0.0001f, 16, null);
                meshCreator.ellipseVerticalRadius = EditorGUILayout.Slider("Vertical Radius", meshCreator.ellipseVerticalRadius, 0.0001f, 16, null);
                meshCreator.ellipseSides = EditorGUILayout.IntSlider("Sides", meshCreator.ellipseSides, 3, 128);
                break;
            case MeshCreator.MeshType.PointedCircle:
                meshCreator.pointedCircleRadius = EditorGUILayout.Slider("Radius", meshCreator.pointedCircleRadius, 0.0001f, 16, null);
                meshCreator.pointedCircleSides = EditorGUILayout.IntSlider("Sides", meshCreator.pointedCircleSides, 3, 128);
                meshCreator.pointedCircleShift = EditorGUILayout.Vector2Field("Shift", meshCreator.pointedCircleShift);
                break;
            case MeshCreator.MeshType.Cake:
                meshCreator.cakeRadius = EditorGUILayout.Slider("Radius", meshCreator.cakeRadius, 0.0001f, 16, null);
                meshCreator.cakeSides = EditorGUILayout.IntSlider("Sides", meshCreator.cakeSides, 3, 128);
                meshCreator.cakeSidesToFill = EditorGUILayout.IntSlider("Sides to fill", meshCreator.cakeSidesToFill, 1, meshCreator.cakeSides);
                break;
            case MeshCreator.MeshType.Convex:
                convexPointList.Draw();
                break;
            case MeshCreator.MeshType.Star:
                meshCreator.starRadiusA = EditorGUILayout.Slider("Radius A", meshCreator.starRadiusA, 0.0001f, 16, null);
                meshCreator.starRadiusB = EditorGUILayout.Slider("Radius B", meshCreator.starRadiusB, 0.0001f, 16, null);
                meshCreator.starSides = EditorGUILayout.IntSlider("Sides", meshCreator.starSides, 3, 128);
                break;
            case MeshCreator.MeshType.Gear:
                meshCreator.gearInnerRadius = EditorGUILayout.Slider("Inner radius", meshCreator.gearInnerRadius, 0.0001f, 16, null);
                meshCreator.gearRootRadius = EditorGUILayout.Slider("Root radius", meshCreator.gearRootRadius, meshCreator.gearInnerRadius, meshCreator.gearOuterRadius, null);
                meshCreator.gearOuterRadius = EditorGUILayout.Slider("Outer radius", meshCreator.gearOuterRadius, meshCreator.gearRootRadius, 16, null);
                meshCreator.gearSides = EditorGUILayout.IntSlider("Sides", meshCreator.gearSides, 3, 128);
                break;
            case MeshCreator.MeshType.Line:
                meshCreator.lineWidth = EditorGUILayout.Slider("Line width", meshCreator.lineWidth, 0.0001f, 1f, null);
                meshCreator.lineUseDoubleCollider = EditorGUILayout.Toggle("Use double collider", meshCreator.lineUseDoubleCollider, emptyLayout);
                linePointsList.Draw();
                break;
            case MeshCreator.MeshType.TriangulatedMesh:
                triangulatedPointList.Draw();
                break;
            case MeshCreator.MeshType.SplineShape:
                splineShapePointList.Draw();
                var points = CatmullRomSpline.GetPoints(meshCreator.splinePoints.ToArray(), meshCreator.splineResolution);
                DrawSplineInspector(meshCreator, points, true);
                break;
            case MeshCreator.MeshType.SplineCurve:
                splineCurvePointList.Draw();
                meshCreator.splineCurveWidth = EditorGUILayout.Slider("Width", meshCreator.splineCurveWidth, 0.0001f, 5f);
                meshCreator.splineCurveUseDoubleCollider = EditorGUILayout.Toggle("Use double collider",
                    meshCreator.splineCurveUseDoubleCollider);
                DrawSplineInspector(meshCreator, CatmullRomSpline.GetPoints(meshCreator.splineCurvePoints, meshCreator.splineResolution, false), false);
                break;
            case MeshCreator.MeshType.SplineConvexShape:
                convexSplinePointList.Draw();
                List<Vector2> splineConvexPoints = ConvexHull.QuickHull(meshCreator.convexSplinePoints);
                splineConvexPoints = CatmullRomSpline.GetPoints(splineConvexPoints, meshCreator.splineResolution, true);
                DrawSplineInspector(meshCreator, splineConvexPoints, true);
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }

        SceneView.RepaintAll();
    }

    #region Building The Mesh

    private void BuildSelectedMesh(MeshCreator meshCreator)
    {
        MeshBase createdMesh = BuildMesh(meshCreator);
        if (createdMesh != null)
        {
            if (meshCreator.setRandomColor)
            {
                createdMesh.SetRandomColor();
            }
            createdMesh.SetPhysicsMaterialProperties(meshCreator.bounciness, meshCreator.friction);
            Undo.RegisterCreatedObjectUndo(createdMesh, "creating " + createdMesh.name);
        }
    }

    private MeshBase BuildMesh(MeshCreator meshCreator)
    {
        var mesh = GetChoosenMesh(meshCreator);
        Undo.RegisterCreatedObjectUndo(mesh.gameObject, mesh.name);
        return mesh;
    }

    private MeshBase GetChoosenMesh(MeshCreator meshCreator)
    {
        float? minArea = meshCreator.splineSimplification != SplineSimplification.Type.None
            ? meshCreator.minAbsoluteSplineArea
            : (float?)null;

        switch (meshCreator.meshType)
        {
            case MeshCreator.MeshType.Triangle:
                return TriangleMesh.AddTriangle(meshCreator.transform.position, meshCreator.triangleVertex1,
                    meshCreator.triangleVertex2, meshCreator.triangleVertex3, Space.World, meshCreator.material,
                    meshCreator.attachRigidbody);
            case MeshCreator.MeshType.Rectangle:
                return RectangleMesh.AddRectangle(meshCreator.transform.position, meshCreator.boxSize,
                    meshCreator.material, meshCreator.attachRigidbody);
            case MeshCreator.MeshType.Circle:
                return CircleMesh.AddCircle(meshCreator.transform.position, meshCreator.circleRadius,
                    meshCreator.circleSides, meshCreator.circleUseCircleCollider, meshCreator.material,
                    meshCreator.attachRigidbody);
            case MeshCreator.MeshType.Quadrangle:
                Vector2[] verts = new Vector2[4] {meshCreator.quadrangleVertex1, meshCreator.quadrangleVertex2,
                    meshCreator.quadrangleVertex3, meshCreator.quadrangleVertex4 };
                return QuadrangleMesh.AddQuadrangle(meshCreator.transform.position, verts, Space.World,
                    meshCreator.material, meshCreator.attachRigidbody);
            case MeshCreator.MeshType.Ellipse:
                return EllipseMesh.AddEllipse(meshCreator.transform.position, meshCreator.ellipseHorizontalRadius,
                    meshCreator.ellipseVerticalRadius, meshCreator.ellipseSides, meshCreator.material,
                    meshCreator.attachRigidbody);
            case MeshCreator.MeshType.PointedCircle:
                return PointedCircleMesh.AddPointedCircle(meshCreator.transform.position, meshCreator.pointedCircleRadius,
                    meshCreator.pointedCircleSides, meshCreator.pointedCircleShift, meshCreator.material,
                    meshCreator.attachRigidbody);
            case MeshCreator.MeshType.Cake:
                return CakeMesh.AddCakeMesh(meshCreator.transform.position, meshCreator.cakeRadius, meshCreator.cakeSides,
                    meshCreator.cakeSidesToFill, meshCreator.material, meshCreator.attachRigidbody);
            case MeshCreator.MeshType.Convex:
                return ConvexMesh.AddConvexMesh(meshCreator.transform.position,
                    MeshHelper.ConvertVec2ToVec3(meshCreator.convexPoints),
                    meshCreator.material, meshCreator.attachRigidbody);
            case MeshCreator.MeshType.Star:
                return StarMesh.AddStar(meshCreator.transform.position, meshCreator.starRadiusA, meshCreator.starRadiusB,
                    meshCreator.starSides, meshCreator.material, meshCreator.attachRigidbody);
            case MeshCreator.MeshType.Gear:
                return GearMesh.AddGear(meshCreator.transform.position, meshCreator.gearInnerRadius, meshCreator.gearRootRadius,
                    meshCreator.gearOuterRadius, meshCreator.gearSides, meshCreator.material, meshCreator.attachRigidbody);
            case MeshCreator.MeshType.Line:
                return LineMesh.AddLine(meshCreator.transform.position, meshCreator.linePoints.ToArray(), meshCreator.lineWidth,
                    meshCreator.lineUseDoubleCollider, Space.World, meshCreator.material, meshCreator.attachRigidbody);
            case MeshCreator.MeshType.TriangulatedMesh:
                return TriangulatedMesh.Add(meshCreator.transform.position, meshCreator.triangulatedPoints.ToArray(),
                    meshCreator.material, meshCreator.attachRigidbody);
            case MeshCreator.MeshType.SplineShape:
                return SplineShapeMesh.AddSplineShape(meshCreator.transform.position, meshCreator.splinePoints.ToArray(), meshCreator.splineResolution,
                   minArea, Space.World, meshCreator.material, meshCreator.attachRigidbody);
            case MeshCreator.MeshType.SplineCurve:
                return SplineCurveMesh.AddSplineCurve(meshCreator.transform.position, meshCreator.splineCurvePoints.ToArray(),
                    meshCreator.splineResolution, meshCreator.splineCurveWidth, meshCreator.splineCurveUseDoubleCollider, minArea,
                    Space.World, meshCreator.material, meshCreator.attachRigidbody);
            case MeshCreator.MeshType.SplineConvexShape:
                return ConvexSplineMesh.AddConvexSpline(meshCreator.transform.position, meshCreator.convexSplinePoints.ToArray(),
                    meshCreator.splineResolution, minArea, Space.World, meshCreator.material, meshCreator.attachRigidbody);
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }

    #endregion
}