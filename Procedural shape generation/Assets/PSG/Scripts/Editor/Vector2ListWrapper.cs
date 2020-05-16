using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class Vector2ListWrapper
{
    public ReorderableList List { get; private set; }
    public string Title { get; private set; }
    private readonly SerializedObject serializedObject;

    public Vector2ListWrapper(SerializedObject serializedObject, string propertyName, string title)
    {
        this.serializedObject = serializedObject;
        Title = title;
        List = new ReorderableList(serializedObject,
            serializedObject.FindProperty(propertyName),
            draggable: true, displayHeader: true, displayAddButton: true, displayRemoveButton: true)
        {
            drawElementCallback = DrawElement,
            drawHeaderCallback = DrawHeader
        };
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = List.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("x"), GUIContent.none);
        EditorGUI.PropertyField(
            new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("y"), GUIContent.none);
    }

    private void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(new Rect(rect.width / 2 - 15, rect.y, rect.width, rect.height), Title);
    }

    public void Draw()
    {
        serializedObject.Update();
        List.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

}

