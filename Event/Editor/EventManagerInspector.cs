using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Utils.Event;

[CustomEditor(typeof(Utils.Event.EventManager))]
public class EventManagerInspector : Editor 
{
	public override void OnInspectorGUI()
	{
		Utils.Event.EventManager manager = (Utils.Event.EventManager) target;

		if(!manager.ReservedEventsAdded)
			manager.AddReservedEvents();

		EditorGUI.BeginChangeCheck();

		for(int i = 0 ; i <  manager.EventIDs.Length ; i++)
		{
			GPEventID id = manager.EventIDs[i];

			if(id.ID < 0)
				continue;

			EditorGUILayout.BeginHorizontal();

			string newName = EditorGUILayout.TextField(id.ID.ToString(), id.Name);

            id.Name = newName;
		    
            //manager.CheckNames(id);

			if(GUILayout.Button("Remove"))
			{
				manager.RemoveEventName(id);
				i--;
			}

			EditorGUILayout.EndHorizontal();
		}

		if(GUILayout.Button("Add Event"))
		{
			manager.AddEventName();
		}

		/*
        if (GUILayout.Button("Add Default Events"))
        {
			manager.AddReservedEvents();
        }
        */

		if(GUILayout.Button("Refresh"))
		{
			manager.RefreshIDList();
		}

		EditorGUI.EndChangeCheck();
	}
}
