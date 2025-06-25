#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Levels))]
public class LevelsEditor : Editor
{
    private int lastWidth = -1;
    private int lastHeight = -1;

    public override void OnInspectorGUI()
    {
        Levels levels = (Levels)target;

        // Theo dõi sự thay đổi width/height
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
        {
            if (levels.width != lastWidth || levels.height != lastHeight)
            {
                ResizeGemBoardLayout(levels);
                ResizeTileMapLayout(levels);

                lastWidth = levels.width;
                lastHeight = levels.height;

                EditorUtility.SetDirty(levels);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }

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

        gemBoardRows.arraySize = levels.width;
        for (int x = 0; x < levels.width; x++)
        {
            SerializedProperty row = gemBoardRows.GetArrayElementAtIndex(x).FindPropertyRelative("row");
            row.arraySize = levels.height;
        }

        so.ApplyModifiedProperties();
    }

    private void ResizeTileMapLayout(Levels levels)
    {
        SerializedObject so = new SerializedObject(levels);
        SerializedProperty tileMapRows = so.FindProperty("tileMapLayout.rows");

        tileMapRows.arraySize = levels.width;
        for (int x = 0; x < levels.width; x++)
        {
            SerializedProperty row = tileMapRows.GetArrayElementAtIndex(x).FindPropertyRelative("row");
            row.arraySize = levels.height;
        }

        so.ApplyModifiedProperties();
    }
}
#endif
