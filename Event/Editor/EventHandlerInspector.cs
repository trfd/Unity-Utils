using System;
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

        EventHandler.HandlerKind newKind = (EventHandler.HandlerKind) EditorGUILayout.EnumMaskField("Kind",handler.Kind);

		if(newKind != handler.Kind)
		{
			handler.Kind = newKind;
			EditorUtility.SetDirty(handler);
			AssetDatabase.SaveAssets();
		}

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
        DisplayActionDelete();
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
        else if (GUILayout.Button("Create Action Asset"))
            m_createNewAction = true;
    }

    private void CreateAction()
    {
        string path = EditorUtility.SaveFilePanel("Create Action", "Assets/", "New Action", "asset");

        if (path == null || path.Length == 0)
            return;

        string relativePath = path.Substring(path.IndexOf("Assets/"));

        EventHandler handler = (EventHandler)target;

        if (m_actionTypeSelectedIndex >= GPActionManager.s_gpactionTypes.Length)
            throw new Exception("Out of bound index");

        System.Type type = GPActionManager.s_gpactionTypes[m_actionTypeSelectedIndex];

        handler._action = (GPAction)ScriptableObject.CreateInstance(type);

        AssetDatabase.CreateAsset(handler._action, relativePath);
        AssetDatabase.SaveAssets();

		handler._action.EditionName = handler._action.name;
    }

    private void DisplayActionDelete()
    {
		EventHandler handler = (EventHandler)target;

		if(handler._action == null)
			return;

        if(GUILayout.Button("Delete Action Asset"))
        {
            if (EditorUtility.DisplayDialog("Delete Action Asset",
                "Are you sure you want to delete this action asset." +
                " If you need to change the action for this handler drag another " +
                "action asset in the field 'Action' above." +
                "THIS CAN'T BE UNDONE !!!!!!!!!!!!!!!!!!!!!!!!!!", "Confirm", "Cancel"))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(handler._action));
				handler._action = null;
            }
        }
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
