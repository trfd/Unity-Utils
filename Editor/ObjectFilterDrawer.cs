using UnityEngine;
using UnityEditor;
using System.Collections;

using Utils;

public class ObjectFilterDrawer : PropertyDrawer
{
    
    
    public static void Display(string label, ObjectFilter filter)
    {
        EditorGUILayout.LabelField(label);

        EditorGUI.indentLevel++;

        EditorGUILayout.LabelField(label);
        
        filter._filterMethod = (ObjectFilter.Method)EditorGUILayout.EnumPopup("Method", filter._filterMethod);

       
        switch(filter._filterMethod)
        {
            case ObjectFilter.Method.OBJECTS: DisplayFilterObjects(filter);
                break;
            case ObjectFilter.Method.TAGS: DisplayFilterTags(filter);
                break;
        }

        EditorGUI.indentLevel--;
    }

    private static void DisplayFilterObjects(ObjectFilter filter)
    {
        for(int i= 0 ; i<filter._objects.Count ; i++)
        {
            EditorGUILayout.BeginHorizontal();

            filter._objects[i] = (GameObject) EditorGUILayout.ObjectField(filter._objects[i], typeof(GameObject), true);

            if(GUILayout.Button("Remove"))
            {
                filter._objects.RemoveAt(i);
                --i;
            }

            EditorGUILayout.EndHorizontal();
        }

        if(GUILayout.Button("Add Object"))
            filter._objects.Add(null);
    }

    private static void DisplayFilterTags(ObjectFilter filter)
    {
        for(int i=0 ; i<filter._tags.Count ; i++)
        {
            EditorGUILayout.BeginHorizontal();

            filter._tags[i] = EditorGUILayout.TagField(filter._tags[i]);

            if (GUILayout.Button("Remove"))
            {
                filter._tags.RemoveAt(i);
                --i;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Tag"))
            filter._tags.Add(null);
    }
}
