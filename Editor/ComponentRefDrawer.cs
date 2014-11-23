//
// ComponentRefDrawer.cs
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

using Utils;

[CustomPropertyDrawer(typeof(Utils.ComponentRefAttribute))]
public class ComponentRefDrawer : PropertyDrawer 
{
	#region Private Member

	private int m_lastComponentID;
	private int m_lastGameObjectID;
	private Component m_lastComponent;
	private GameObject m_lastObject;

	#endregion

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
	{
		ComponentRefAttribute attr = (ComponentRefAttribute) attribute;

		SerializedProperty compIDProperty =  property.FindPropertyRelative("m_componentInstanceID");
		SerializedProperty goIDProperty =  property.FindPropertyRelative("m_gameObjectInstanceID");

		if(m_lastComponentID != compIDProperty.intValue)
		{
			m_lastComponent = (Component) EditorUtility.InstanceIDToObject(compIDProperty.intValue);
			m_lastObject    = (GameObject) EditorUtility.InstanceIDToObject(goIDProperty.intValue);
		}
		
		m_lastComponent = (Component) EditorGUILayout.ObjectField(label,m_lastComponent,attr._componentType,true);

		if(m_lastComponent != null)
		{
			compIDProperty.intValue = m_lastComponent.GetInstanceID();
			m_lastComponentID = compIDProperty.intValue;
		}
	}
}
