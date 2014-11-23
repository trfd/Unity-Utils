//
// GameObjectRegistrator.cs
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
	public class GameObjectRegistrator : MonoBehaviour
	{
		#region Private Member 

		/// <summary>
		/// Holds whether or not the registrator has registered 
		/// the game object to the GameObjectManager
		/// </summary>
		private bool m_isRegistred;

		#endregion

		#region Properties

		/// <summary>
		/// Instance ID of parent game object.
		/// </summary>
		/// <value>The game object I.</value>
		[InspectorLabel]
		public int GameObjectID
		{
			get{ return this.gameObject.GetInstanceID(); }
		}

		/// <summary>
		/// Whether or not the registrator has registered 
		/// the game object to the GameObjectManager
		/// </summary>
		/// <value><c>true</c> if this instance is registered; otherwise, <c>false</c>.</value>
		[InspectorLabel]
		public bool IsRegistered
		{
			get{ return m_isRegistred; }
		}

		#endregion

		#region MonoBehaviour Override

		void Awake()
		{
			Register();
		}

		void OnDestroy()
		{
			Unregister();
		}

		#endregion

		#region Registration

		/// <summary>
		/// Registers parent GameObject to the GameObjectManager.
		/// </summary>
		public void Register()
		{
			if(m_isRegistred)
				return;

			GameObjectManager.Instance.Register(this);
			m_isRegistred = true;
		}

		/// <summary>
		/// Unregisters parent GameObject to the GameObjectManager
		/// </summary>
		public void Unregister()
		{
			if(!m_isRegistred)
				return;

			GameObjectManager.Instance.Unregister(this);
			m_isRegistred = false;

		}

		/// <summary>
		/// Checks if the GameObject is correctly registered by checking if GameObjectManager 
		/// returns the same GameObject as the parent GameObject for the parent GameObject's 
		/// instance ID.
		/// </summary>
		/// <returns><c>true</c>, if registration is correct, <c>false</c> otherwise.</returns>
		public bool CheckRegistration()
		{
			return (GameObjectManager.Instance.InstanceIDToObject(this.gameObject.GetInstanceID()) == this.gameObject);
		}

		#endregion
	}
}