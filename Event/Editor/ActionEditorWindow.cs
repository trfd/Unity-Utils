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
using System.Linq;

namespace Utils.Event
{
	public class ActionEditorWindow : EditorWindow 
	{
		#region Static Interface
		
		[MenuItem ("Window/Utils/Action Tool")]
		public static void ShowWindow() 
		{
			EditorWindow.GetWindow(typeof(ActionEditorWindow));
		}
	
		#endregion

		#region Private Members

		private float m_inspectorWidth = 250;

		private GPAction[] m_actions;
		private GPActionInspector[] m_actionInspectors;

		private EventHandler m_handler;

		private Vector2 m_blueprintScrollPosition;
		private Vector2 m_sidebarScrollPosition;

		private int m_selectedBoxID = -1;

		private ActionEditorNode m_selectedNode;

		private bool m_createNewAction = false;

		private int m_actionTypeSelectedIndex;

		private GUIStyle m_windowStyle;

		#endregion

		#region Const

		private Color m_backColor          = new Color(0.18f,0.18f,0.18f);
		private Color m_borderLineColor    = new Color(0.13f,0.13f,0.13f);
		private Color m_inspectorBackColor = new Color(0.22f, 0.22f, 0.22f); 

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
				GUILayout.Label("No Event Handler Selected ");
				return false;
			}
			
			if(Selection.activeGameObject != null)
			{
				if((handler = Selection.activeGameObject.GetComponent<EventHandler>()) == null)
				{
					GUILayout.Label("No Event Handler Selected ");
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

		#region Dot Managenement

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

		#region Selection Management
		
		private void CheckSelectedActionBox()
		{
			if(!UnityEngine.Event.current.isMouse || UnityEngine.Event.current.type != EventType.MouseDown)
				return;

			// If click in inspector skip change
			if(UnityEngine.Event.current.mousePosition.x >= position.width-m_inspectorWidth)
				return;
			
			int currSelectedIndex = -1;
			
			for(int i=0 ; i<m_actions.Length ; i++)
			{

				if(IsMouseIn(m_actions[i]._windowRect))
					currSelectedIndex = i;
			}

			ChangeSelectedModule(currSelectedIndex);
		}
		
		private void ChangeSelectedModule(int currSelectedIndex)
		{
			m_selectedBoxID = currSelectedIndex;
		}

		private void CheckSelectedNode()
		{
			if(!UnityEngine.Event.current.isMouse || UnityEngine.Event.current.type != EventType.MouseDown)
				return;

			ActionEditorNode newNode = null;

			foreach(GPAction action in m_actions)
			{
				if(IsMouseOverNode(action._leftNode))
				{
					newNode = action._leftNode;
				}

				foreach(ActionEditorNode node in action._rightNodes)
				{
					if(IsMouseOverNode(node))
					{
						newNode = node;
					}
				}
			}

			ChangeSelectedNode(newNode);
		}

		private void ChangeSelectedNode(ActionEditorNode node)
		{
			if(m_selectedNode != null)
			{
				if(node != null)
				{
					CreateConnection(node,m_selectedNode);
				}

				m_selectedNode._selected = false;
				m_selectedNode = null;

				return;
			}
			// else

			m_selectedNode = node;

			if(m_selectedNode != null)
				m_selectedNode._selected = true;
		}

		#endregion

		#region Connection Management

		private void CreateConnection(ActionEditorNode node1, ActionEditorNode node2)
		{
			if(node1._action == node2._action || node1._action == null || node2._action == null)
				return;

			bool node1IsLeft = (node1._action._leftNode == node1);
			bool node2IsLeft = (node2._action._leftNode == node2);

			if((node1IsLeft && node2IsLeft) || (!node1IsLeft && !node2IsLeft))
				return;

			GPAction parent;
			GPAction child;

			if(node2IsLeft)
			{
				child  = node2._action;
				parent = node1._action;
			}
			else
			{
				child  = node1._action; 
				parent = node2._action;
			}

			if(!(parent is IActionOwner))
				return;

			if(child._leftNode._connection != null)
			{
				((IActionOwner)child._leftNode._connection._nodeParent._action).Disconnect(child);
			}

			((IActionOwner) parent).Connect(child);
		}

		protected virtual void DisplayAllConnections()
		{
			foreach(GPAction action in m_actions)
			{
				if(action._leftNode._connection == null)
					continue;

				DisplayConnection(action._leftNode._connection);
			}
		}

		protected virtual void DisplayConnection(ActionEditorConnection connection)
		{
			Texture2D texture = new Texture2D(1, 1);
			texture.SetPixel(0,0,Color.white);
			texture.Apply();

			Vector2 inPos = connection._nodeParent._center + connection._nodeParent._action._windowRect.position;
			Vector2 outPos = connection._nodeChild._center + connection._nodeChild._action._windowRect.position;
			
			Handles.DrawBezier(inPos, outPos,
			                   inPos  + 30 * Vector2.right,
			                   outPos - 30 * Vector2.right,
			                   Color.white,texture,1f);
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

			EditorGUIUtility.labelWidth = 80;

			FetchActions();

			CheckSelectedActionBox();

			CheckSelectedNode();

			DrawBackground();

			DisplayAllConnections();

			DisplaySidebar();

			DisplayBlueprint();

			
			EditorGUIUtility.labelWidth = 0;
		}

		protected virtual void DisplayAction(int id)
		{
			if(m_actionInspectors[id] == null)
				CreateInspector(id);

			m_actions[id]._leftNode.Draw();

			for(int i =0 ; i< m_actions[id]._rightNodes.Count ; i++)
			{
				m_actions[id]._rightNodes[i].Draw();
			}

			EditorUtility.SetDirty(m_actions[id]);

			GUI.DragWindow(new Rect(0,0,10000,20));
		}

		#region Background

		protected virtual void DrawBackground()
		{
			float xInspector = position.width-m_inspectorWidth;
			
			DrawQuad(new Rect(0           , 0, xInspector      , position.height),m_backColor);
			DrawQuad(new Rect(xInspector-1, 0, 10              , position.height),m_borderLineColor);
			DrawQuad(new Rect(xInspector  , 0, m_inspectorWidth, position.height),m_inspectorBackColor);

		}

		#endregion

		#region Sidebar

		protected virtual void DisplaySidebar()
		{	
			float xInspector = position.width-m_inspectorWidth;
		
			GUILayout.BeginArea(new Rect(position.width-m_inspectorWidth+5,0,
			                             m_inspectorWidth-10,position.height));

			m_sidebarScrollPosition = GUILayout.BeginScrollView(m_sidebarScrollPosition,
			                                                    GUILayout.Width(m_inspectorWidth-10),
			                                                    GUILayout.Height(position.height));

			DisplaySidebarHeader();

			Rect rect = EditorGUILayout.GetControlRect();
			rect.height = 1f;
			EditorGUI.DrawRect(rect,new Color(0.282f,0.282f,0.282f));
			
			DisplaySidebarInspector();

			GUILayout.EndScrollView();

			GUILayout.EndArea();
		}

		protected virtual void DisplaySidebarHeader()
		{
			EditorGUILayout.Space();

			DisplayActionCreationField();
		}

		protected virtual void DisplaySidebarInspector()
		{	
			if(m_selectedBoxID == -1)
				return;

			try
			{
				GUILayout.Label(m_actions[m_selectedBoxID].GetType().Name);
			
				m_actionInspectors[m_selectedBoxID].DrawInspector();
			}
			catch(System.Exception)
			{}
		}

		private void DisplayActionCreationField()
		{
			if(m_createNewAction)
			{
				m_actionTypeSelectedIndex = EditorGUILayout.Popup("Action", m_actionTypeSelectedIndex, 
				                                                  GPActionManager.s_gpactionTypeNames);

				EditorGUILayout.BeginHorizontal();
				
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
				throw new System.Exception("Out of bound index");
			
			System.Type actionType = GPActionManager.s_gpactionTypes[m_actionTypeSelectedIndex];
			
			m_handler.AddAction(actionType);
		}

		#endregion

		#region BluePrint

		protected virtual void DisplayBlueprint()
		{
			m_windowStyle = GUI.skin.window;

			float xInspector = position.width-m_inspectorWidth;

			Handles.BeginGUI();

			GUILayout.BeginArea(new Rect(0,0,xInspector,position.height));

			m_blueprintScrollPosition = GUILayout.BeginScrollView(m_blueprintScrollPosition,
			                                                      GUILayout.Width(xInspector),
			                                                      GUILayout.Height(position.height));

			BeginWindows();

			for(int i=0 ; i<m_actions.Length ; i++)
			{
				GPAction box = m_actions[i];

				if(m_selectedBoxID == i)
					GUI.backgroundColor = Color.white;
				else
					GUI.backgroundColor = new Color(0.8f,0.8f,0.8f);

				box._windowRect = GUI.Window(i, box._windowRect, DisplayAction, box.GetType().Name);
			}

			EndWindows();

			GUILayout.EndScrollView();
			GUILayout.EndArea();

			Handles.EndGUI();
		}

		#endregion

		#region Utils

		/// <summary>
		/// Draws a rectangle of specified size and color in the GUI.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		private void DrawQuad(Rect position, Color color)
		{
			Texture2D texture = new Texture2D(1, 1);
			texture.SetPixel(0,0,color);
			texture.Apply();
			GUI.skin.box.normal.background = texture;
			GUI.Box(position, GUIContent.none);
		}
		
		private bool IsMouseIn(Rect rect)
		{		
			Vector2 screenPos = UnityEngine.Event.current.mousePosition;
			
			return rect.Contains(screenPos);
		}

		private bool IsMouseOverNode(ActionEditorNode node)
		{

			return IsMouseIn(new Rect(node._action._windowRect.x + node._center.x - 3, 
			                          node._action._windowRect.y + node._center.y - 3, 6,6));
		}

		#endregion
	}


}