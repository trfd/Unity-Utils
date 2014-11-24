﻿using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

using Utils.Event;
using Utils.Reflection;
using EventHandler = Utils.Event.EventHandler;


[CustomEditor(typeof(EventHandler))]
public class EventHandlerInspector : Editor
{
    #region Private Members

    /// <summary>
    /// The Index in GPAction popup currently selected
    /// </summary>
    private int m_actionTypeSelectedIndex = 0;

    /// <summary>
    /// Holds whether the button "Create Action" button has been pushed
    /// </summary>
    private bool m_createNewAction = false;

	private GPActionInspector m_actionInspector;

    #endregion

    #region Inspector 

    public override void OnInspectorGUI()
    {
        EventHandler handler = (EventHandler)target;

        // Display Default MonoBehaviour editor

        base.OnInspectorGUI();

        // Display Handler Kind popup

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("State: "+handler.State.ToString());

        EditorGUILayout.Space();

        // Display Change Action 

        EditorGUILayout.Space();

        if (m_actionInspector == null && handler._action != null)
        {
			CreateActionInspector(handler);
        }
        else if (m_actionInspector != null && handler._action == null)
            m_actionInspector = null;

        if(handler._action != null)
            m_actionInspector.DrawInspector();

		EditorGUILayout.Space();

		DisplayActionCreationField();

		EditorGUILayout.Space();
		
		if(GUILayout.Button("Export Action"))
		{
			ExportActionPrefab();
		}
    }

    private void DisplayActionCreationField()
    {
        if(m_createNewAction)
        {
            EditorGUILayout.BeginHorizontal();

            m_actionTypeSelectedIndex = EditorGUILayout.Popup("Action", m_actionTypeSelectedIndex, 
			                                                  GPActionManager.s_gpactionTypeNames);

            if (GUILayout.Button("Create"))
            {
               CreateAction();
                m_createNewAction = false;
            }
            
            if (GUILayout.Button("Cancel"))
                m_createNewAction = false;
            
            EditorGUILayout.EndHorizontal();

           
        }
        else if (GUILayout.Button("Create Action"))
            m_createNewAction = true;
    }

	private void CreateAction()
	{
		if (m_actionTypeSelectedIndex >= GPActionManager.s_gpactionTypes.Length)
			throw new Exception("Out of bound index");

		EventHandler handler = (EventHandler)target;

		handler._action = (GPAction) ScriptableObject.CreateInstance(
				GPActionManager.s_gpactionTypes[m_actionTypeSelectedIndex]);

		handler._action.SetParentHandler(handler);
	}

    private void ExportActionPrefab()
    {
		EditorUtility.DisplayDialog("Not yet ready","This functionnality is not yet implemented.","Ok");

		return;

		if(EditorApplication.isPlaying)
		{
			Debug.LogError("Can not export in play mode");
			return;
		}

		EventHandler handler = (EventHandler)target;

		string path = EditorUtility.SaveFilePanel("Export Action", "Assets/", "New Action Prefab", "prefab");

		if (path == null || path.Length == 0)
			return;
		
		string relativePath = path.Substring(path.IndexOf("Assets/"));

		GameObject go = new GameObject();

		System.Type componentType = handler.GetType();

		EventHandler copyhandler = (EventHandler) go.AddComponent(componentType);

		System.Reflection.FieldInfo[] fields = 
			componentType.GetFields(System.Reflection.BindingFlags.Instance  | 
	 								System.Reflection.BindingFlags.Public 	 |
			                        System.Reflection.BindingFlags.NonPublic ); 

		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copyhandler, field.GetValue(handler));
		}

		PrefabUtility.CreatePrefab(relativePath,go);

		DestroyImmediate(go);
    }
	
	private void CreateActionInspector(EventHandler handler)
	{
		System.Type inspectorType = GPActionInspectorManager.InspectorTypeForAction(handler._action);

		if(inspectorType == null)
			return;

		m_actionInspector = (GPActionInspector) Activator.CreateInstance(inspectorType);
		m_actionInspector.TargetAction = handler._action;
	}

    #endregion
}
