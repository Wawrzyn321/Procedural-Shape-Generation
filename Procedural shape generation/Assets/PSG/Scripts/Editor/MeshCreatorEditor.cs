using UnityEditor;
using UnityEngine;
using PSG;
using UnityEditorInternal;

[CustomEditor(typeof(MeshCreator), true)]
public class MeshCreatorEditor : Editor
{
    private ReorderableList lineReorderableList;
    private ReorderableList convexReorderableList;
    private ReorderableList triangulatedReorderableList;

    private void OnEnable()
    {
        lineReorderableList = CreateVector2List("linePoints", "Line Points");
        convexReorderableList = CreateVector2List("convexPoints", "Convex Shape Points");
        triangulatedReorderableList = CreateVector2List("triangulatedPoints", "Triangulated Mesh Points");
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
        MeshCreator mc = (MeshCreator) target;
        DrawHandles(mc);
    }

    private void DrawHandles(MeshCreator mc)
    {
        //cache casted position
        Vector2 p2 = mc.transform.position;
        //and this one too, for brevity
        Vector3 p3 = mc.transform.position;

        switch (mc.meshType)
        {
            case MeshCreator.MeshType.Triangle:
                mc.triangleVertex1 = Handles.DoPositionHandle(p2 + mc.triangleVertex1, Quaternion.identity) - p3;
                mc.triangleVertex2 = Handles.DoPositionHandle(p2 + mc.triangleVertex2, Quaternion.identity) - p3;
                mc.triangleVertex3 = Handles.DoPositionHandle(p2 + mc.triangleVertex3, Quaternion.identity) - p3;
                break;
            case MeshCreator.MeshType.Rectangle:
                mc.boxControlPoint = mc.boxSize / 2;
                mc.boxControlPoint = Handles.DoPositionHandle(p2 + mc.boxControlPoint, Quaternion.identity) - p3;
                mc.boxSize = mc.boxControlPoint * 2;
                break;
            case MeshCreator.MeshType.Circle:
                mc.circleControlPoint = SetVectorLength(mc.circleControlPoint, mc.circleRadius);
                mc.circleControlPoint = Handles.DoPositionHandle(p2 + mc.circleControlPoint, Quaternion.identity) - p3;
                mc.circleRadius = mc.circleControlPoint.magnitude;
                break;
            case MeshCreator.MeshType.Quadrangle:
                mc.quadrangleVertex1 = Handles.DoPositionHandle(p3 + mc.quadrangleVertex1, Quaternion.identity) - p3;
                mc.quadrangleVertex2 = Handles.DoPositionHandle(p3 + mc.quadrangleVertex2, Quaternion.identity) - p3;
                mc.quadrangleVertex3 = Handles.DoPositionHandle(p3 + mc.quadrangleVertex3, Quaternion.identity) - p3;
                mc.quadrangleVertex4 = Handles.DoPositionHandle(p3 + mc.quadrangleVertex4, Quaternion.identity) - p3;
                break;
            case MeshCreator.MeshType.Ellipse:
                mc.ellipseControlPoint = new Vector2(mc.ellipseHorizontalRadius, mc.ellipseVerticalRadius);
                mc.ellipseControlPoint = Handles.DoPositionHandle(p2 + mc.ellipseControlPoint, Quaternion.identity) - p3;
                mc.ellipseHorizontalRadius = mc.ellipseControlPoint.x;
                mc.ellipseVerticalRadius = mc.ellipseControlPoint.y;
                break;
            case MeshCreator.MeshType.PointedCircle:
                mc.pointedCircleControlPoint = SetVectorLength(mc.pointedCircleControlPoint, mc.pointedCircleRadius);
                mc.pointedCircleControlPoint = Handles.DoPositionHandle(p2 + mc.pointedCircleControlPoint, Quaternion.identity) - p3;
                mc.pointedCircleRadius = mc.pointedCircleControlPoint.magnitude;
                mc.pointedCircleShift = Handles.DoPositionHandle(p2 + mc.pointedCircleShift, Quaternion.identity) - p3;
                break;
            case MeshCreator.MeshType.Cake:
                mc.cakeControlPoint = SetVectorLength(mc.cakeControlPoint, mc.cakeRadius);
                mc.cakeControlPoint = Handles.DoPositionHandle(p2 + mc.cakeControlPoint, Quaternion.identity) - p3;
                mc.cakeRadius = mc.cakeControlPoint.magnitude;
                break;
            case MeshCreator.MeshType.Convex:
                for (int i = 0; i < mc.convexPoints.Count; i++)
                {
                    mc.convexPoints[i] = Handles.DoPositionHandle(p2 + mc.convexPoints[i], Quaternion.identity) - p3;
                }
                break;
            case MeshCreator.MeshType.Star:
                mc.starControlPointA = SetVectorLength(mc.starControlPointA, mc.starRadiusA);
                mc.starControlPointB = SetVectorLength(mc.starControlPointB, mc.starRadiusB);
                mc.starControlPointA = Handles.DoPositionHandle(p2 + mc.starControlPointA, Quaternion.identity) - p3;
                mc.starControlPointB = Handles.DoPositionHandle(p2 + mc.starControlPointB, Quaternion.identity) - p3;
                mc.starRadiusA = mc.starControlPointA.magnitude;
                mc.starRadiusB = mc.starControlPointB.magnitude;
                break;
            case MeshCreator.MeshType.Gear:
                mc.gearInnerControlPoint = SetVectorLength(mc.gearInnerControlPoint, mc.gearInnerRadius);
                mc.gearRootControlPoint = SetVectorLength(mc.gearRootControlPoint, mc.gearRootRadius);
                mc.gearOuterControlPoint = SetVectorLength(mc.gearOuterControlPoint, mc.gearOuterRadius);
                mc.gearInnerControlPoint = Handles.DoPositionHandle(p2 + mc.gearInnerControlPoint, Quaternion.identity) - p3;
                mc.gearRootControlPoint = Handles.DoPositionHandle(p2 + mc.gearRootControlPoint, Quaternion.identity) - p3;
                mc.gearOuterControlPoint = Handles.DoPositionHandle(p2 + mc.gearOuterControlPoint, Quaternion.identity) - p3;
                mc.gearInnerRadius = mc.gearInnerControlPoint.magnitude;
                mc.gearRootRadius = mc.gearRootControlPoint.magnitude;
                mc.gearOuterRadius = mc.gearOuterControlPoint.magnitude;
                break;
            case MeshCreator.MeshType.Line:
                for (int i = 0; i < mc.linePoints.Count; i++)
                {
                    mc.linePoints[i] = Handles.DoPositionHandle(p2 + mc.linePoints[i], Quaternion.identity) - p3;
                }
                break;
            case MeshCreator.MeshType.TriangulatedMesh:
                for (int i = 0; i < mc.triangulatedPoints.Count; i++)
                {
                    mc.triangulatedPoints[i] = Handles.DoPositionHandle(p2 + mc.triangulatedPoints[i], Quaternion.identity) - p3;
                }
                break;
        }
    }

    private Vector2 SetVectorLength(Vector2 vector, float length)
    {
        return vector.normalized * length;
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
                DrawReorderableList(convexReorderableList);
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
                DrawReorderableList(lineReorderableList);
                break;
            case MeshCreator.MeshType.TriangulatedMesh:
                DrawReorderableList(triangulatedReorderableList);
                break;
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
        }
        return null;
    }

    #endregion

    #region Reorderable List Methods

    private void DrawReorderableList(ReorderableList reorderableList)
    {
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    private ReorderableList CreateVector2List(string propertyName, string title)
    {
        ReorderableList reorderableList = new ReorderableList(serializedObject,
            serializedObject.FindProperty(propertyName), true, true, true, true);

        reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("x"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("y"), GUIContent.none);
        };
        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(new Rect(rect.width / 2 - 15, rect.y, rect.width, rect.height), title);
        };
        return reorderableList;
    }

    #endregion
}