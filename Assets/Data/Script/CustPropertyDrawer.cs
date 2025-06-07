using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);
        Rect newpos = position;
        newpos.y += 144f;
        SerializedProperty data = property.FindPropertyRelative("rows");
        //data.row[0][]
        if (data.arraySize != 9 )
            data.arraySize = 9;
        for (int i = 0; i < 9 ; i++)
        {
            SerializedProperty row = data.GetArrayElementAtIndex(i).FindPropertyRelative("row");
            newpos.height = 18;
            if (row.arraySize != 7)
                row.arraySize = 7;
            newpos.width = position.width / 7;
            for (int j = 0; j < 7 ; j++)
            {
                EditorGUI.PropertyField(newpos, row.GetArrayElementAtIndex(j), GUIContent.none);
                newpos.x += newpos.width;

            }
            newpos.x = position.x;
            newpos.y -= 18f;
        }

        
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f * 10;
    }
}
