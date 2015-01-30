//
// GPActionLoopInspector.cs
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

using System.Reflection;
using Utils.Reflection;
using System.Linq;

namespace Utils.Event
{
	/*
	[GPActionInspector(typeof(GPActionIf))]
	public class GPActionIfInspector : GPActionDefaultInspector
	{
		#region Private Members

		/// <summary>
		/// The m_member names.
		/// </summary>
		private string[] m_targetMemberNames;
		private string[] m_compareMemberNames;

		/// <summary>
		/// The m_members.
		/// </summary>
		private ComponentNestedDataMemberWrapper[] m_targetMembers;
		private ComponentNestedDataMemberWrapper[] m_compareMembers;
	
		private SerializedProperty m_compareValueProperty;

		#endregion

		/// <summary>
		/// Raises the inspector GU event.
		/// </summary>
		protected override void OnInspectorGUI()
		{
			GPActionIf ifAction = (GPActionIf) TargetAction;

			bool useThisObj = EditorGUILayout.Toggle("Use This Object",ifAction._useThisObject);
			GameObject newObj = ifAction._targetObject;

			if(!useThisObj)
				newObj = (GameObject) EditorGUILayout.ObjectField("Object",ifAction._targetObject,typeof(GameObject),true);

			if(useThisObj != ifAction._useThisObject || 
			   newObj != ifAction._targetObject || 
			   m_targetMembers == null || m_targetMemberNames == null)
			{
				ifAction._useThisObject = useThisObj;
				ifAction._targetObject = newObj;

				CreateTargetMemberList();
			}

			DisplayTargetMemberPopups();

			ifAction._compareMethod = (GPActionIf.CompareMethod) EditorGUILayout.EnumPopup("Compare", ifAction._compareMethod);

			if(ifAction._compareMethod == GPActionIf.CompareMethod.OBJECT_MEMBER)
			{
				GameObject obj = (GameObject) EditorGUILayout.ObjectField(ifAction._compareObject,
				                                                          typeof(GameObject),true);

				if(obj != ifAction._compareObject)
				{
					ifAction._compareObject = obj;
					CreateCompareMemberList();
				}

				DisplayCompareMemberPopups();
			}
			else if(ifAction._compareMethod == GPActionIf.CompareMethod.CONSTANT_VALUE)
			{
				if(m_compareValueProperty == null)
					m_compareValueProperty = SerialObject.FindProperty("_compareValue");

				EditorGUILayout.PropertyField(m_compareValueProperty);
			}
		}

		/// <summary>
		/// Creates the member list.
		/// </summary>
		protected void CreateTargetMemberList()
		{
			GPActionIf ifAction = (GPActionIf) TargetAction;

			List<ComponentNestedDataMemberWrapper> members = new List<ComponentNestedDataMemberWrapper>();

			GameObject obj;

			if(ifAction._useThisObject)
				obj = ifAction.ParentGameObject;
			else
				obj = ifAction._targetObject;

			if(obj == null)
				return;

			foreach(Component comp in obj.GetComponents<Component>())
			{
                members.AddRange(ComponentNestedDataMemberWrapper.CreateComponentMemberList(comp));
			}

            m_targetMembers = ComponentNestedDataMemberWrapper.CreateGameObjectMemberList(ifAction._compareObject);
            m_targetMemberNames = new string[m_targetMembers.Length];

			for(int i=0 ; i<m_targetMembers.Length ; i++)
			{
				m_targetMemberNames[i] = m_targetMembers[i].EditorDisplayName();
			}
		}

		protected void CreateCompareMemberList()
		{
			GPActionIf ifAction = (GPActionIf) TargetAction;
			
			if(ifAction._compareObject == null)
				return;

            m_compareMembers = ComponentNestedDataMemberWrapper.CreateGameObjectMemberList(ifAction._compareObject);
            m_compareMemberNames = new string[m_compareMembers.Length];
			
			for(int i=0 ; i<m_compareMembers.Length ; i++)
			{
				m_compareMemberNames[i] = m_compareMembers[i].EditorDisplayName();
			}
		}

		protected void DisplayTargetMemberPopups()
		{
			GPActionIf ifAction = (GPActionIf) TargetAction;

			if(m_targetMemberNames == null || m_targetMembers == null)
				return;
			
			int idx = System.Array.IndexOf(m_targetMembers,ifAction._nestedDataMember);
			
			idx = Mathf.Max(0,idx);
			
			int newIdx = EditorGUILayout.Popup(idx,m_targetMemberNames);
			
			if(newIdx != idx)
			{
				ifAction._nestedDataMember = m_targetMembers[newIdx];

				EditorUtility.SetDirty(ifAction);
			}
		}

		protected void DisplayCompareMemberPopups()
		{
			GPActionIf ifAction = (GPActionIf) TargetAction;
			
			if(m_compareMemberNames == null || m_compareMembers == null)
				return;
			
			int idx = System.Array.IndexOf(m_compareMembers,ifAction._compareMember);
			
			idx = Mathf.Max(0,idx);
			
			int newIdx = EditorGUILayout.Popup(idx,m_compareMemberNames);
			
			if(newIdx != idx)
			{
				ifAction._compareMember = m_compareMembers[newIdx];

				EditorUtility.SetDirty(ifAction);
			}
		}
	}
	*/
}
