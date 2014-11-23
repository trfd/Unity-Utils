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

	private int m_lastID;
	private GameObject m_lastObject;

	#endregion

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
	{
		SerializedProperty idProperty =  property.FindPropertyRelative("m_instanceID");

		if(m_lastID != idProperty.intValue)
		{
			m_lastObject =  (GameObject) EditorUtility.InstanceIDToObject(idProperty.intValue);
		}
		
		m_lastObject = (GameObject) EditorGUILayout.ObjectField(label,m_lastObject,typeof(GameObject),true);

		if(m_lastObject != null)
		{
			idProperty.intValue = m_lastObject.GetInstanceID();
			m_lastID = idProperty.intValue;
		}
	}
}
