//
// GameObjectRef.cs
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
using System.Collections;

namespace Utils
{
	[System.Serializable]
	public class GameObjectRef : ISerializationCallbackReceiver
	{
		#region Private Members

		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private int m_instanceID;

		/// <summary>
		/// Encapsulated game object.
		/// </summary>
		private GameObject m_gameObject; 

		#endregion

		#region Properties

		/// <summary>
		/// Encapsulated game object
		/// </summary>
		/// <value>The game object.</value>
		[UnityEngine.SerializeField]
		public GameObject GameObject
		{
			get
			{ 
				if(m_gameObject == null)
					FindObject();

				return m_gameObject;  
			}
			set
			{ 
				if(m_gameObject == value)
					return;

				m_gameObject = value; 

				UpdateGameObject();
			}
		}

		#endregion

		#region Serialization callbacks

		public void OnBeforeSerialize()
		{
			UpdateGameObject();
			m_gameObject = null;
		}

		public void OnAfterDeserialize()
		{
			FindObject();
		}

		#endregion

		#region Update links

		private void UpdateGameObject()
		{
			if(m_gameObject != null)
				m_instanceID = m_gameObject.GetInstanceID();
			else
				CheckObject();
		}

		#endregion

		#region Find Object

		private void FindObject()
		{
#if UNITY_EDITOR
			if(!UnityEditor.EditorApplication.isPlaying)
				FindObjectEditor();
			else
				FindObjectRuntime();
#else
			FindObjectRuntime();
#endif
		}

		private void FindObjectEditor()
		{
#if UNITY_EDITOR
			m_gameObject = (GameObject) UnityEditor.EditorUtility.InstanceIDToObject(m_instanceID);

			if(m_gameObject == null)
			{
				Debug.LogWarning("GameObject with ID "+m_instanceID+
				                 " not found. Please check that object have a component GameObjectRegistrator");
			}
#endif
		}

		private void FindObjectRuntime()
		{
			m_gameObject = GameObjectManager.Instance.InstanceIDToObject(m_instanceID);

			if(m_gameObject == null)
			{
				Debug.LogWarning("GameObject with ID "+m_instanceID+
				                 " not found. Please check that object have a component GameObjectRegistrator");
			}
		}

		#endregion

		#region Check Object

		private void CheckObject()
		{
			if(m_gameObject == null)
				return;

			GameObjectRegistrator reg = m_gameObject.GetComponent<GameObjectRegistrator>();

			if(reg == null)
			{
				Debug.LogWarning("No GameObjectRegistrator found for linking GameObject to GameObjectRef, one will be added");
				reg = m_gameObject.AddComponent<GameObjectRegistrator>();
			}
		}

		#endregion
	}
}
