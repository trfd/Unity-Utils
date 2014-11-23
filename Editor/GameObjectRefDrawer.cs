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

	private int m_lastTimeID;
	private int m_lastUniqueID;
	private GameObject m_lastObject;

	#endregion

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
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
		
		m_lastObject = (GameObject) EditorGUILayout.ObjectField(label,m_lastObject,typeof(GameObject),true);

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
}
