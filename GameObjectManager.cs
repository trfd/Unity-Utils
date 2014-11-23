//
// GameObjectManager.cs
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
using System.Collections.Generic;

namespace Utils
{
	public class GameObjectManager
	{
		#region Private Members

		/// <summary>
		/// Maps game object instance ID with gameobject instance.
		/// </summary>
		private Dictionary<int,GameObject> m_objectMap;

		#endregion

		#region Singleton

		private static GameObjectManager instance;
		
		private GameObjectManager()
		{
			Init();
		}
		
		public static GameObjectManager Instance
		{
			get 
			{
				if (instance == null)
				{
					instance = new GameObjectManager();
				}
				return instance;
			}
		}

		#endregion

		#region Initialization

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		private void Init()
		{
			m_objectMap = new Dictionary<int, GameObject>();
		}

		#endregion

		#region Registration Interface

		/// <summary>
		/// Register the specified registrator.
		/// </summary>
		/// <param name="registrator">Registrator.</param>
		public void Register(GameObjectRegistrator registrator)
		{
			m_objectMap.Add(registrator.gameObject.GetInstanceID() , registrator.gameObject);
		}

		/// <summary>
		/// Unregister the specified registrator.
		/// </summary>
		/// <param name="registrator">Registrator.</param>
		public void Unregister(GameObjectRegistrator registrator)
		{
			m_objectMap.Remove(registrator.gameObject.GetInstanceID());
		}

		#endregion

		#region Access

		/// <summary>
		/// Returns the GameObject corresponding to instance ID if any found.
		/// </summary>
		/// <returns>The identifier to object.</returns>
		/// <param name="id">Identifier.</param>
		public GameObject InstanceIDToObject(int id)
		{
			GameObject obj;
			if(m_objectMap.TryGetValue(id,out obj))
				return obj;

			return null;
		}

		#endregion
	}
}
