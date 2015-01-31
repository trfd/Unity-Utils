//
// EditorUtils.cs
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

#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Reflection;

public class EditorUtils
{
	public const float defaultTextHeight = 16f;

    public static System.Object DrawField(GUIContent label, System.Object value, System.Type fieldType )
    {
             if (fieldType == typeof(int))     return EditorGUILayout.IntField(label,          (int)value);
        else if (fieldType == typeof(float))   return EditorGUILayout.FloatField(label,        (float)value);
        else if (fieldType == typeof(bool))    return EditorGUILayout.Toggle(label,            (bool)value);
        else if (fieldType == typeof(string))  return EditorGUILayout.TextField(label,         (string)value);
        else if (fieldType == typeof(Vector2)) return EditorGUILayout.Vector2Field(label,      (Vector2)value);
        else if (fieldType == typeof(Vector3)) return EditorGUILayout.Vector3Field(label,      (Vector3)value);
        else if (fieldType == typeof(Vector4)) return EditorGUILayout.Vector4Field(label.text, (Vector4)value);
        else if (fieldType == typeof(AnimationCurve)) return EditorGUILayout.CurveField(label, (AnimationCurve)value);
		else if (typeof(UnityEngine.Component).IsAssignableFrom(fieldType))  return EditorGUILayout.ObjectField(label, (Component)value, typeof(Component));
		else if (typeof(UnityEngine.GameObject).IsAssignableFrom(fieldType)) return EditorGUILayout.ObjectField(label, (GameObject)value, typeof(GameObject));
		else if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))     return EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, typeof(UnityEngine.Object));

        return null;
    }

	/// <summary>
	/// Draw in the inspector a button for every method
	/// in the target object that has an attribute GUIButton.
	/// This method should be called into OnInspectorGUI()
	/// </summary>
	/// <param name="target">Inspector's target.</param>
	public static void DrawMethodGUIButton(UnityEngine.Object target)
	{
		Type targetType = target.GetType();
		
		MethodInfo[] mInfos = targetType.GetMethods(BindingFlags.Instance  | 
		                                            BindingFlags.NonPublic | 
		                                            BindingFlags.Public);

		// Go through all methods 
		// to find attribute GUIButton
		for(int i=0 ; i<mInfos.Length ; i++)
		{
			if(Attribute.IsDefined(mInfos[i] , typeof(InspectorButton)))
			{
				InspectorButton button = 
					(InspectorButton) Attribute.GetCustomAttribute(mInfos[i], typeof(InspectorButton));
				
				if(GUILayout.Button(button.name))
				{
					mInfos[i].Invoke(target,button.args);
				}
			}
		}
	}

	public static void DrawMemberValue(UnityEngine.Object target)
	{
		DrawFieldValue(target);
		DrawPropertyValue(target);
	}

	private static void DrawFieldValue(UnityEngine.Object target)
	{
		Type targetType = target.GetType();
		
		FieldInfo[] fieldInfos = targetType.GetFields(BindingFlags.Instance  | 
		                                              BindingFlags.NonPublic | 
		                                              BindingFlags.Public);
		
		// Go through all methods 
		// to find attribute GUIButton
		for(int i=0 ; i<fieldInfos.Length ; i++)
		{
			if(Attribute.IsDefined(fieldInfos[i] , typeof(InspectorLabel)))
			{
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField(fieldInfos[i].Name);
				EditorGUILayout.LabelField(fieldInfos[i].GetValue(target).ToString());

				EditorGUILayout.EndHorizontal();
			}
		}
	}

	private static void DrawPropertyValue(UnityEngine.Object target)
	{
		Type targetType = target.GetType();
		
		PropertyInfo[] propInfos = targetType.GetProperties(BindingFlags.Instance  | 
		                                                    BindingFlags.NonPublic | 
		                                                    BindingFlags.Public);
		
		// Go through all methods 
		// to find attribute GUIButton
		for(int i=0 ; i<propInfos.Length ; i++)
		{
			if(Attribute.IsDefined(propInfos[i] , typeof(InspectorLabel)))
			{
				EditorGUILayout.BeginHorizontal();
				
				EditorGUILayout.LabelField(propInfos[i].Name);
				EditorGUILayout.LabelField(propInfos[i].GetValue(target,null).ToString());
				
				EditorGUILayout.EndHorizontal();
			}
		}
	}

	/// 
	/// 
	/// 
	public class ReflectionProperty
	{
		#region Public Members

		/// <summary>
		/// Object Represented by property
		/// </summary>
		public System.Object _object;

		/// <summary>
		/// Owner of property
		/// </summary>
		public System.Object _parentObject;

		/// <summary>
		/// Field in parent object
		/// </summary>
		public FieldInfo _field;

		#endregion
	}

	public static ReflectionProperty ObjectFromProperty(SerializedProperty property)
	{
		BindingFlags flags = BindingFlags.Public    | 
						     BindingFlags.NonPublic | 
				             BindingFlags.Instance  | 
				             BindingFlags.FlattenHierarchy;

		ReflectionProperty refProp = new ReflectionProperty();

		System.Object currObj = property.serializedObject.targetObject;

		System.Type currType = currObj.GetType();

		FieldInfo currField = null;

		string[] path = property.propertyPath.Split('.');

		for(int i=0 ; i<path.Length ; i++)
		{
			currType = currObj.GetType();

			currField = currType.GetField(path[i],flags);

			if(currField == null)
			{
				Debug.LogError("Field "+path[i]+" not found in type "+currType.FullName);
				return null;
			}

			refProp._parentObject = currObj;
			currObj = currField.GetValue(currObj);

			if(currObj == null)
			{
				Debug.LogError("Can not access to object at path "+property+", "+path[i]+" is null");
				return null;
			}
		}

		refProp._object = currObj;
		refProp._field = currField;

		return refProp;
	}
}


#endif