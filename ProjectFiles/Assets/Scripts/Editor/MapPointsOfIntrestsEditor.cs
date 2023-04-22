using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapPointsOfIntrests))]
[CanEditMultipleObjects]
public class MapPointsOfIntrestsEditor : Editor
{
    SerializedObject pointsOfIntrest;
    SerializedProperty Paths;

    /*

private void OnEnable()
{
    pointsOfIntrest = serializedObject;
    Paths = pointsOfIntrest.FindProperty("Paths");

}


public override void OnInspectorGUI()
{
    //base.OnInspectorGUI();
    pointsOfIntrest.Update();

    EditorGUILayout.PropertyField(Paths);

    serializedObject.ApplyModifiedProperties();

   if(Paths.isArray)
    {
        int length = Paths.arraySize;

        for(int i = 0; i < length; i++)
        {
            SerializedProperty PathClass = Paths.FindPropertyRelative("Points");
            int PathLenght = PathClass.arraySize;

            if (PathLenght < 1)
                return;

            for (int y = 0; y < PathLenght - 1; y++)
            {
                Vector3 point1 = PathClass.GetArrayElementAtIndex(y).vector3Value;
                Vector3 point2 = PathClass.GetArrayElementAtIndex(y + 1).vector3Value;

                Gizmos.color = Color.white;
                Gizmos.DrawLine(point1, point2);
            }

        }

    }


}

*/
}
