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
	public class GameObjectRef
	{
		public enum AccessMethod
		{
			USING_NAME,
			USING_TAG,
			USING_REF,
		}

		#region Private Members

		/// <summary>
		/// Defines the method of accessing the game object.
		/// </summary>
		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private AccessMethod m_access;

		/// <summary>
		/// Name used to find gameobject when access method is USING_NAME.
		/// </summary>
		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private string m_name;

		/// <summary>
		/// Tag used to find gameobject when access method is USING_TAG.
		/// </summary>
		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private string m_tag;

		/// <summary>
		/// Encapsulated game object.
		/// </summary>
		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private GameObject m_gameObject; 

		#endregion

		#region Properties

		/// <summary>
		/// Encapsulated game object
		/// </summary>
		/// <value>The game object.</value>
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
			}
		}

		#endregion

		#region Find Object

		private void FindObject()
		{
			switch(m_access)
			{
			case AccessMethod.USING_NAME:
				FindObjectUsingName();
				break;
			case AccessMethod.USING_TAG:
				FindObjectUsingTag();
				break;
			case AccessMethod.USING_REF:
				FindObjectUsingRef();
				break;
			}
		}

		private void FindObjectUsingRef()
		{
			if(m_gameObject == null)
			{
				Debug.LogWarning("No gameobject reference were provided");
			}
		}

		private void FindObjectUsingName()
		{
			m_gameObject = GameObject.Find(m_name);

			if(m_gameObject == null)
			{
				Debug.LogWarning("GameObject with name "+m_name+" not found.");
			}
		}

		private void FindObjectUsingTag()
		{
			m_gameObject = GameObject.FindGameObjectWithTag(m_tag);

			if(m_gameObject == null)
			{
				Debug.LogWarning("GameObject with tag "+m_tag+" not found.");
			}
		}

		#endregion
	}
}
