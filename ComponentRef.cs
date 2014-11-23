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

		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private GameObjectRef m_gameObjectRef;
	
		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private TypeWrapper m_componentType;

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
			}
		}

		#endregion

		public T ComponentAs<T>() where T : Component
		{
			return (T) Component;
		}

		#region Serialization callbacks
		
		public void OnBeforeSerialize()
		{
			m_component = null;
		}
		
		public void OnAfterDeserialize()
		{
			FindComponent();
		}
		
		#endregion
		
		#region Find Object
		
		protected virtual void FindComponent()
		{
			if(m_gameObjectRef.GameObject == null)
				return;

			m_component = m_gameObjectRef.GameObject.GetComponent(m_componentType.Type);
		}
		
		#endregion
	}
}
