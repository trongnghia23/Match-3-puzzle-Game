#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Levels))]
public class LevelsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Levels levels = (Levels)target;

        // Vẽ Inspector mặc định
        base.OnInspectorGUI();

        GUILayout.Space(10);
        if (GUILayout.Button("Resize Layouts (Thủ công)"))
        {
            ResizeGemBoardLayout(levels);
            ResizeTileMapLayout(levels);

            EditorUtility.SetDirty(levels);
            AssetDatabase.SaveAssets();
        }
    }

    private void ResizeGemBoardLayout(Levels levels)
    {
        SerializedObject so = new SerializedObject(levels);
        SerializedProperty gemBoardRows = so.FindProperty("gemBoardLayout.rows");

        gemBoardRows.arraySize = levels.height; // rows = height
        for (int y = 0; y < levels.height; y++)
        {
            SerializedProperty row = gemBoardRows.GetArrayElementAtIndex(y).FindPropertyRelative("row");
            row.arraySize = levels.width; // columns = width
        }

        so.ApplyModifiedProperties();
    }

    private void ResizeTileMapLayout(Levels levels)
    {
        SerializedObject so = new SerializedObject(levels);
        SerializedProperty tileMapRows = so.FindProperty("tileMapLayout.rows");

        tileMapRows.arraySize = levels.height; // rows = height
        for (int y = 0; y < levels.height; y++)
        {
            SerializedProperty row = tileMapRows.GetArrayElementAtIndex(y).FindPropertyRelative("row");
            row.arraySize = levels.width; // columns = width
        }

        so.ApplyModifiedProperties();
    }

}
#endif
