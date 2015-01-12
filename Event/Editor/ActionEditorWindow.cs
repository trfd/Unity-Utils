//
// ActionEditorWindow.cs
//
// Author(s):
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


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace Utils.Event
{
	public class ActionEditorWindow : EditorWindow 
	{
		#region Static Interface
		
		[MenuItem ("Window/Visual Script/Open")]
		public static void ShowWindow() 
		{
			EditorWindow.GetWindow(typeof(ActionEditorWindow));
		}
	
		#endregion

		#region Private Members

		private GPAction[] m_actions;
		private GPActionInspector[] m_actionInspectors;

		private EventHandler m_handler;

		#endregion

		#region EventHandler Management

		/// <summary>
		/// Fetches the Event Handler that should be displayed
		/// </summary>
		protected virtual bool FetchEventHandler()
		{
			EventHandler handler;
			
			if(Selection.activeObject == null && Selection.activeGameObject == null)
			{
				GUILayout.Label("No VisualScript Selected ");
				return false;
			}
			
			if(Selection.activeGameObject != null)
			{
				if((handler = Selection.activeGameObject.GetComponent<EventHandler>()) == null)
				{
					GUILayout.Label("No EventHandler Selected ");
					return false;
				}
				
				if(m_handler == null || m_handler != handler)
					ChangeEventHandler(handler);
			}
			
			return (m_handler != null);
		}

		/// <summary>
		/// Changes the handler currently displayed
		/// </summary>
		/// <param name="handler">Handler.</param>
		protected virtual void ChangeEventHandler(EventHandler handler)
		{
			Reset();
			
			m_handler = handler;
		}

		#endregion

		#region Action Management

		protected virtual void FetchActions()
		{
			GPAction[] actions = m_handler.GetGPActionObjectMapperOrCreate().GetAllActions(m_handler);

			// Check if arrays have diff length
			// if true recompute all inspectors

			if(m_actionInspectors == null || actions.Length != m_actions.Length)
			{
				m_actions = actions;
				CreateAllInspectors();

				return;
			}

			// else
			// recompute only inspectors for new actions

			for(int i = 0 ; i < actions.Length ; i++)
			{
				if(m_actions[i] == actions[i])
					continue;

				m_actions[i] = actions[i];
				CreateInspector(i);
			}
		}

		#endregion

		#region Inspector Management

		protected virtual void CreateAllInspectors()
		{
			m_actionInspectors = new GPActionInspector[m_actions.Length];

			for(int i=0 ; i<m_actionInspectors.Length ; i++)
				CreateInspector(i);

		}

		protected virtual void CreateInspector(int idx)
		{
			if(idx < 0 || idx >= m_actionInspectors.Length)
				throw new System.IndexOutOfRangeException();

			System.Type inspectorType = GPActionInspectorManager.InspectorTypeForAction(m_actions[idx]);
			
			if(inspectorType == null)
				return;
			
			m_actionInspectors[idx] = (GPActionInspector) System.Activator.CreateInstance(inspectorType);
			m_actionInspectors[idx].TargetAction = m_actions[idx];
		}

		#endregion
		
		public virtual void Reset()
		{
		
		}
		
		/// <summary>
		/// Raises the GUI event.
		/// </summary>
		private void OnGUI() 
		{
			if(!FetchEventHandler())
				return;

			FetchActions();

			Handles.BeginGUI();
			
			BeginWindows();
		
			for(int i=0 ; i<m_actions.Length ; i++)
			{
				GPAction box = m_actions[i];

				/*
				if(m_selectedModule == box)
					GUI.backgroundColor = Color.white;
				else
					GUI.backgroundColor = new Color(0.8f,0.8f,0.8f);
				*/

				box._windowRect = GUI.Window(i, box._windowRect, DisplayAction, box.GetType().Name);
			}
			
			EndWindows();
			
			Handles.EndGUI();
		}

		protected virtual void DisplayAction(int id)
		{
			if(m_actionInspectors[id] == null)
				CreateInspector(id);

			m_actionInspectors[id].DrawInspector();
			m_actionInspectors[id].IsFoldedOut
			EditorUtility.SetDirty(m_actions[id]);

			GUI.DragWindow(new Rect(0,0,100,30));
		}

	}
}