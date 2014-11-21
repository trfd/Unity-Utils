//
// GPActionDefaultInspector.cs
//
// Author:
//       Baptiste Dupy <baptiste.dupy@gmail.com>
//
// Copyright (c) 2014 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils.Event;


public class GPActionInspector
{
	#region Private Members

	private GPAction m_targetAction;

	protected SerializedObject m_serialObject;
	
	private bool m_inspectorFoldout;
	
	#endregion

	#region Property

	public GPAction TargetAction
	{
		get{ return m_targetAction; }
		set
		{
			SetAction(value);
		}
	}

	public SerializedObject SerialObject
	{
		get{ return m_serialObject; }
	}

	#endregion
	
	#region Public Members

	#endregion

	#region Accessors

	public virtual void SetAction(GPAction action)
	{
		m_targetAction = action;
		m_serialObject = new SerializedObject(m_targetAction);
	}

	#endregion
	
	public void DrawInspector()
	{
		if((m_inspectorFoldout = EditorGUILayout.Foldout(m_inspectorFoldout, TargetAction.GetType().Name)))
		{
			EditorGUI.indentLevel++;

			OnInspectorGUI();

			EditorGUI.indentLevel--;
		}
	}

	protected virtual void OnInspectorGUI(){}
}

public class GPActionDefaultInspector : GPActionInspector
{
    protected override void OnInspectorGUI()
    {
        SerializedProperty property = m_serialObject.GetIterator();

        bool hasChanged = false;

        while (property.NextVisible(true))
        {
			EditorGUILayout.PropertyField(property);
        }

       	m_serialObject.ApplyModifiedProperties();
    }
}
