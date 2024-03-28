using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustomArrayLayout : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);
        Rect newposition = position;
        newposition.y += 18f;
        SerializedProperty data = property.FindPropertyRelative("Rows");
        if (data.arraySize != 7)
            data.arraySize = 7;
        for (int j = 6; j >= 0; j--)
        {
            SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("Row");
            newposition.height = 18f;
            if (row.arraySize != 3)
                row.arraySize = 3;
            newposition.width = position.width / 10;
            for (int i = 0; i < 3; i++)
            {
                EditorGUI.PropertyField(newposition, row.GetArrayElementAtIndex(i), GUIContent.none);
                newposition.x += newposition.width;
            }

            newposition.x = position.x;
            newposition.y += 18f;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f * 8;
    }
}
