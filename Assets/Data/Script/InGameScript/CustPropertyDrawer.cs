using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, 18), label.text);

        SerializedProperty rows = property.FindPropertyRelative("rows");
        int rowCount = 9;
        int colCount = 7;

        if (rows.arraySize != rowCount)
            rows.arraySize = rowCount;

        float cellSize = position.width / colCount;
        float yOffset = position.y + 18f;

        for (int i = rowCount - 1; i >= 0; i--)
        {
            SerializedProperty row = rows.GetArrayElementAtIndex(i).FindPropertyRelative("row");

            if (row.arraySize != colCount)
                row.arraySize = colCount;

            float yPos = yOffset + (rowCount - 1 - i) * 18f;

            for (int j = 0; j < colCount; j++)
            {
                Rect cellRect = new Rect(position.x + j * cellSize, yPos, cellSize, 18f);
                EditorGUI.PropertyField(cellRect, row.GetArrayElementAtIndex(j), GUIContent.none);
            }
        }

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f * 10; // 1 line for label, 9 for rows
    }
}
