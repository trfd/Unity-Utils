//
// GenericObjectDrawer.cs
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

namespace Utils
{
	[CustomPropertyDrawer(typeof(GenericObject))]
	public class GenericObjectDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
		{
			SerializedProperty prop = null;
	
			switch((GenericObject.Type) property.FindPropertyRelative("m_type").enumValueIndex)
			{
			case GenericObject.Type.UNDEFINED : return;
			case GenericObject.Type.BOOL:       prop = property.FindPropertyRelative("_boolValue"); break;
			case GenericObject.Type.INT:        prop = property.FindPropertyRelative("_intValue"); break;
			case GenericObject.Type.FLOAT:      prop = property.FindPropertyRelative("_floatValue"); break;
			case GenericObject.Type.ENUM:       prop = property.FindPropertyRelative("_enumIndex"); break;
			case GenericObject.Type.VECTOR2:    prop = property.FindPropertyRelative("_vector2Value"); break;
			case GenericObject.Type.VECTOR3:    prop = property.FindPropertyRelative("_vector3Value"); break;
			case GenericObject.Type.VECTOR4:    prop = property.FindPropertyRelative("_vector4Value"); break;
			case GenericObject.Type.QUATERNION: prop = property.FindPropertyRelative("_quaternionValue"); break;
			case GenericObject.Type.COLOR:      prop = property.FindPropertyRelative("_colorValue"); break;
			case GenericObject.Type.RECT:       prop = property.FindPropertyRelative("_rectValue"); break;
			case GenericObject.Type.STRING:     prop = property.FindPropertyRelative("_stringValue"); break;
			case GenericObject.Type.OBJECT:     prop = property.FindPropertyRelative("_objectValue"); break;
				
			default:
				Debug.LogWarning("Unsupported type "+(GenericObject.Type) property.FindPropertyRelative("m_type").enumValueIndex);
				break;
			}

			if(prop != null)
				EditorGUILayout.PropertyField(prop, label);
		}
	}
}

