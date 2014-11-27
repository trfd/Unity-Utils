using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Utils.Event.EventManager))]
public class EventManagerInspector : Editor 
{
	protected override void OnInspectorGUI()
	{
		Utils.Event.EventManager manager = (Utils.Event.EventManager) target;

		for(int i = 0 ; i<  manager.EventIDs.Length ; i++)
		{
			int id = manager.EventIDs[i];
			string name = manager.EventNames[i];

			EditorGUILayout.BeginHorizontal();

			manager.EventMap[id] = EditorGUILayout.TextField(id.ToString(), name);

			if(GUILayout.Button("Remove"))
			{
				manager.EventMap.Remove(id);
				i--;
			}

			EditorGUILayout.EndHorizontal();
		}

		if(GUILayout.Button("Add Event"))
		{
			manager.AddEventName();
		}
	}
}
