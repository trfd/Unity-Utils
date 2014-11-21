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

    private GPActionDefaultInspector m_actionInspector;

    #endregion

    #region Inspector 

    public override void OnInspectorGUI()
    {
        EventHandler handler = (EventHandler)target;

        // Display Default MonoBehaviour editor

        base.OnInspectorGUI();

        // Display Handler Kind popup

        handler.Kind = (EventHandler.HandlerKind) EditorGUILayout.EnumMaskField("Kind",handler.Kind);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("State: "+handler.State.ToString());

        EditorGUILayout.Space();

        // Display Change Action 

        DisplayActionManagement();


        EditorGUILayout.Space();

        EditorGUILayout.ObjectField("Action", handler.Action, typeof(GPAction));

        if (m_actionInspector == null && handler.Action != null)
        {
            m_actionInspector = new GPActionDefaultInspector(handler.Action);
            m_actionInspector._targetAction = handler.Action;
        }
        else if (m_actionInspector != null && handler.Action == null)
            m_actionInspector = null;

        if(handler.Action != null)
            m_actionInspector.DrawInspector();

        DisplayActionDelete();
    }

    private void DisplayActionManagement()
    {
        if(m_createNewAction)
        {
            EditorGUILayout.BeginHorizontal();

            m_actionTypeSelectedIndex = EditorGUILayout.Popup("Action", m_actionTypeSelectedIndex, GPActionManager.s_gpactionTypeNames);

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

        if (path == null && path.Length != 0)
            return;

        string relativePath = path.Substring(path.IndexOf("Assets/"));

        EventHandler handler = (EventHandler)target;

        if (m_actionTypeSelectedIndex >= GPActionManager.s_gpactionTypes.Length)
            throw new Exception("Out of bound index");

        System.Type type = GPActionManager.s_gpactionTypes[m_actionTypeSelectedIndex];

        handler.Action = (GPAction)ScriptableObject.CreateInstance(type);

        AssetDatabase.CreateAsset(handler.Action, relativePath);
        AssetDatabase.SaveAssets();
    }

    private void DisplayActionDelete()
    {
        if(GUILayout.Button("Delete Action Asset"))
        {
             EventHandler handler = (EventHandler)target;

            if (EditorUtility.DisplayDialog("Delete Action Asset",
                "Are you sure you want to delete this action asset." +
                " If you need to change the action for this handler drag another " +
                "action asset in the field 'Action' above." +
                "THIS CAN'T BE UNDONE !!!!!!!!!!!!!!!!!!!!!!!!!!", "Confirm", "Cancel"))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(handler.Action));
            }
        }
    }

    #endregion
}
