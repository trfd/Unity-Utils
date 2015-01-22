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
	[GPActionInspector(typeof(GPActionIf))]
	public class GPActionIfInspector : GPActionDefaultInspector
	{
		#region Private Members

		/// <summary>
		/// The m_member names.
		/// </summary>
		private string[] m_memberNames;

		/// <summary>
		/// The m_members.
		/// </summary>
		private ComponentNestedDataMemberWrapper[] m_members;
	

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
			   m_members == null || m_memberNames == null)
			{
				ifAction._useThisObject = useThisObj;
				ifAction._targetObject = newObj;

				CreateMemberList();
			}

			if(m_memberNames == null || m_members == null)
				return;

			int idx = System.Array.IndexOf(m_members,ifAction._nestedDataMember);

			if(idx == -1)
			{
				Debug.LogWarning("Member not found: "+ifAction._nestedDataMember.EditorDisplayName());
			}

			idx = Mathf.Max(0,idx);

			int newIdx = EditorGUILayout.Popup(idx,m_memberNames);

			if(newIdx != idx)
			{
				ifAction._nestedDataMember = m_members[newIdx];
	/*
				ComponentNestedDataMemberWrapper.SetSerializedPropertyValue(SerialObject.FindProperty("_nestedDataMember"),
				                                                            m_members[newIdx]);

	*/
				EditorUtility.SetDirty(ifAction);
			}
		}

		/// <summary>
		/// Creates the member list.
		/// </summary>
		protected void CreateMemberList()
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
				members.AddRange(CreateComponentMemberList(comp));
			}

			m_members = members.ToArray();
			m_memberNames = new string[members.Count];

			for(int i=0 ; i<m_members.Length ; i++)
			{
				m_memberNames[i] = m_members[i].EditorDisplayName();
			}
		}

		/// <summary>
		/// Creates the component member list.
		/// </summary>
		/// <returns>The component member list.</returns>
		/// <param name="comp">Comp.</param>
		protected ComponentNestedDataMemberWrapper[] CreateComponentMemberList(Component comp)
		{
			return CreateTypeMemberList(comp.GetType(), new ComponentNestedDataMemberWrapper(comp));
		}

		/// <summary>
		/// Creates the type member list.
		/// </summary>
		/// <returns>The type member list.</returns>
		/// <param name="type">Type.</param>
		/// <param name="parentMember">Parent member.</param>
		protected ComponentNestedDataMemberWrapper[] CreateTypeMemberList(System.Type type, 
		                                                                  ComponentNestedDataMemberWrapper parentMember)
		{
			List<ComponentNestedDataMemberWrapper> members = new List<ComponentNestedDataMemberWrapper>();

			FieldInfo[] fields = ReflectionUtils.GetAllFields(type);
			
			foreach(FieldInfo field in fields)
			{
				if(parentMember.HasIntermediateMemberOfType(field.FieldType))
				   continue;

				members.Add(parentMember.Append(field));
				//members.AddRange(CreateTypeMemberList(field.FieldType,members.Last()));
			}
			
			PropertyInfo[] properties = ReflectionUtils.GetAllProperties(type);

			foreach(PropertyInfo property in properties)
			{
				if(parentMember.HasIntermediateMemberOfType(property.PropertyType))
					   continue;

				members.Add(parentMember.Append(property));
				//members.AddRange(CreateTypeMemberList(property.PropertyType,members.Last()));
			}

			return members.ToArray();
		}
	}
}
