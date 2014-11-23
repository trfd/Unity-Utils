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
public class ComponentRefDrawer : GameObjectRefDrawer 
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
	{
		position.height = EditorGUIUtility.singleLineHeight;

		ComponentRefAttribute attr = (ComponentRefAttribute) attribute;
	
		label.text += " ("+attr._componentType.Name+")";

		if((m_foldout = EditorGUI.Foldout(position,m_foldout,label)))
		{
			EditorGUI.indentLevel++;

			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			DisplayGameObjectGUI(position,property,label);

			SerializedProperty typeProperty = property.FindPropertyRelative("m_componentType");
			
			SerializedProperty typeNameProperty = typeProperty.FindPropertyRelative("m_name");
			
			typeNameProperty.stringValue = attr._componentType.FullName;

			EditorGUI.indentLevel--;
		}
	}

	protected override void DisplayGameObjectGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		SerializedProperty objRefProperty = property.FindPropertyRelative("m_gameObjectRef");
		
		base.DisplayGameObjectGUI(position,objRefProperty,new GUIContent("GameObject"));
	}
}
