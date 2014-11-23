//
// GameObjectRefDrawer.cs
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


[CustomPropertyDrawer(typeof(Utils.GameObjectRef))]
public class GameObjectRefDrawer : PropertyDrawer 
{
	#region Private Member

	protected bool m_foldout;
	protected int m_lastTimeID;
	protected int m_lastUniqueID;
	protected GameObject m_lastObject;

	#endregion

	public override float GetPropertyHeight (SerializedProperty prop,
	                                         GUIContent label) 
	{
		if(m_foldout)
			return base.GetPropertyHeight(prop, label) +2f*EditorGUIUtility.singleLineHeight + 2f*EditorGUIUtility.standardVerticalSpacing;

		return base.GetPropertyHeight(prop, label);
	}
	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
	{
		Rect totalPropertySize = position;
		
		totalPropertySize.height = GetPropertyHeight(property,label);

		position.height = EditorGUIUtility.singleLineHeight;

		if((m_foldout = EditorGUI.Foldout(position,m_foldout, label)))
		{
			EditorGUI.indentLevel++;

			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			DisplayGameObjectGUI(position,property,label);

			EditorGUI.indentLevel--;
		}
	}

	protected virtual void DisplayGameObjectGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position,label,property);
		
		SerializedProperty accessProperty = property.FindPropertyRelative("m_access");
		
		accessProperty.enumValueIndex = EditorGUI.Popup(position,"Access Method",accessProperty.enumValueIndex,accessProperty.enumNames);
		
		position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		
		switch((Utils.GameObjectRef.AccessMethod)accessProperty.enumValueIndex)
		{
		case Utils.GameObjectRef.AccessMethod.USING_NAME:
			DisplayGUIForNameAccess(position,property);
			break;
		case Utils.GameObjectRef.AccessMethod.USING_TAG:
			DisplayGUIForTagAccess(position,property);
			break;
		case Utils.GameObjectRef.AccessMethod.USING_REF:
			DisplayGUIForRefAccess(position,property);
			break;
		}
		
		EditorGUI.EndProperty();
	}
	
	protected virtual void DisplayGUIForNameAccess(Rect position, SerializedProperty property)
	{
		SerializedProperty nameProperty =  property.FindPropertyRelative("m_name");

		nameProperty.stringValue = EditorGUI.TextField(position,"Name",nameProperty.stringValue);

		position.y += EditorGUIUtility.singleLineHeight+EditorGUIUtility.standardVerticalSpacing;
	}

	protected virtual void DisplayGUIForTagAccess(Rect position, SerializedProperty property)
	{
		SerializedProperty tagProperty =  property.FindPropertyRelative("m_tag");
		
		tagProperty.stringValue = EditorGUI.TagField(position,"Tag",tagProperty.stringValue);

		position.y += EditorGUIUtility.singleLineHeight+EditorGUIUtility.standardVerticalSpacing;
	}

	protected virtual void DisplayGUIForRefAccess(Rect position, SerializedProperty property)
	{
		SerializedProperty idProperty =  property.FindPropertyRelative("m_instanceID");
		
		SerializedProperty timeStampProperty = idProperty.FindPropertyRelative("m_timeStamp");
		SerializedProperty uniqueStampProperty = idProperty.FindPropertyRelative("m_uniqueStamp");
		
		Utils.UID uid = new Utils.UID(timeStampProperty.intValue,uniqueStampProperty.intValue);
		
		if(m_lastTimeID != timeStampProperty.intValue || 
		   m_lastUniqueID != uniqueStampProperty.intValue)
		{
			m_lastObject =  Utils.GameObjectManager.Instance.InstanceIDToObject(uid);
		}
		
		m_lastObject = (GameObject) EditorGUI.ObjectField(position,"",m_lastObject,ObjectFieldConstrainType(),true);

		position.y += EditorGUIUtility.singleLineHeight+EditorGUIUtility.standardVerticalSpacing;

		if(m_lastObject != null)
		{
			Utils.ObjectID objID = m_lastObject.GetComponent<Utils.ObjectID>();
			
			if(objID == null)
				objID = m_lastObject.AddComponent<Utils.ObjectID>();
			
			if(!objID.IsRegistered)
				objID.Register();
			else
				objID.CheckRegistration();
			
			timeStampProperty.intValue   = (int) objID.ID.TimeStamp;
			uniqueStampProperty.intValue = (int) objID.ID.UniqueStamp;
			m_lastTimeID   = timeStampProperty.intValue;
			m_lastUniqueID = uniqueStampProperty.intValue;
		}
	}

	protected virtual System.Type ObjectFieldConstrainType()
	{
		return typeof(GameObject);
	}
}
