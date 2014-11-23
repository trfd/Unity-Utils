//
// ComponentRef.cs
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
	public class ComponentRef : ISerializationCallbackReceiver
	{
		#region Private Members

		/// <summary>
		/// Instance ID of game object owning component.
		/// </summary>
		[UnityEngine.SerializeField]
		[HideInInspector]
		private int m_gameObjectInstanceID;

		/// <summary>
		/// Instance ID of component.
		/// </summary>
		[UnityEngine.SerializeField]
		[HideInInspector]
		private int m_componentInstanceID;

		/// <summary>
		/// Encapsulated component.
		/// </summary>
		private Component m_component;

		#endregion

		#region Properties

		public Component Component
		{
			get
			{
				if(m_component == null)
					FindComponent();

				return m_component;
			}

			set
			{
				if(m_component == value)
					return;

				m_component = value;

				UpdateComponent();
			}
		}

		#endregion

		#region Serialization callbacks
		
		public void OnBeforeSerialize()
		{
			UpdateComponent();
			m_component = null;
		}
		
		public void OnAfterDeserialize()
		{
			FindComponent();
		}
		
		#endregion
		
		#region Update links
		
		private void UpdateComponent()
		{
			if(m_component != null)
			{
				m_componentInstanceID = m_component.GetInstanceID();
				m_gameObjectInstanceID = m_component.gameObject.GetInstanceID();
			}
		}
		
		#endregion
		
		#region Find Object
		
		private void FindComponent()
		{
			FindComponentRuntime();

			/*
#if UNITY_EDITOR
			if(!UnityEditor.EditorApplication.isPlaying)
				FindComponentEditor();
			else
				FindComponentRuntime();
#else
			FindObjectRuntime();
#endif
			*/
		}
		/*
		private void FindComponentEditor()
		{
#if UNITY_EDITOR
			m_component = GameObjectManager.Instance.InstanceIDToObject(m_componentInstanceID);

			if(m_component == null)
			{
				Debug.LogWarning("Component with ID "+m_componentInstanceID+
				                 " not found. Please check that parent game object have a component GameObjectRegistrator");
			}
#endif
		}
		*/
		
		private void FindComponentRuntime()
		{/*
			GameObject obj = GameObjectManager.Instance.InstanceIDToObject(m_gameObjectInstanceID);

			if(obj == null)
			{
				Debug.LogWarning("GameObject with ID "+m_gameObjectInstanceID+
				                 " not found. Please check that object have a component GameObjectRegistrator");
				return;
			}

			Component[] components = obj.GetComponents<Component>();

			foreach(Component component in components)
			{
				if(component.GetInstanceID() == m_componentInstanceID)
				{
					m_component = component;
					return;
				}
			}

			Debug.LogWarning("Component with ID "+m_componentInstanceID+" not found in object "+obj.name);
			*/
		}
		
		#endregion
	}
}
