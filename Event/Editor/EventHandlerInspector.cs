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

        EditorGUILayout.BeginHorizontal();

        m_actionTypeSelectedIndex = EditorGUILayout.Popup("Action", m_actionTypeSelectedIndex, GPActionManager.s_gpactionTypeNames);

        if (GUILayout.Button("Set"))
        {
            if (EditorUtility.DisplayDialog("Confirm change","Setting the action will erase previous action. This can not be undone", "Confirm", "Cancel"))
            {
                ChangeSelectedAction();
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        string actionTypeName;
        if (handler.Action == null)
            actionTypeName = "NULL";
        else
            actionTypeName = handler.Action.GetType().Name;

        EditorGUILayout.LabelField("Action: "+actionTypeName);

        if (m_actionInspector == null && handler.Action != null)
        {
            m_actionInspector = new GPActionDefaultInspector(handler.Action);
            m_actionInspector._targetAction = handler.Action;
        }
        else if (m_actionInspector != null && handler.Action == null)
            m_actionInspector = null;

        if(handler.Action != null)
            m_actionInspector.DrawInspector();
    }

    private void ChangeSelectedAction()
    {
        EventHandler handler = (EventHandler)target;

        if (m_actionTypeSelectedIndex >= GPActionManager.s_gpactionTypes.Length)
            throw new Exception("Out of bound index");

        System.Type type = GPActionManager.s_gpactionTypes[m_actionTypeSelectedIndex];

        handler.Action = (GPAction) Activator.CreateInstance(type);

        EditorUtility.SetDirty(handler);
    }

    #endregion
}
