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
	public class GameObjectManager : MonoBehaviour, ISerializationCallbackReceiver
	{
		[System.Serializable]
		public class GameObjectUIDMap : DictionaryWrapper<UID,GameObject>
		{}

		#region Private Members

		/// <summary>
		/// Maps game object instance ID with gameobject instance.
		/// </summary>
		[UnityEngine.SerializeField]
		private GameObjectUIDMap m_objectMap;

		#endregion

		#region Singleton
		
		private static GameObjectManager m_instance;
		
		public static GameObjectManager Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = GameObject.FindObjectOfType<GameObjectManager>();

					if(m_instance == null) return null;

					if(m_instance.m_objectMap == null)
						m_instance.Init();

					//DontDestroyOnLoad(m_instance.gameObject);
				}
				
				return m_instance;
			}
		}
		
		void Awake()
		{
			if (m_instance == null)
			{
				m_instance = this;
				//DontDestroyOnLoad(this);
			}
			else
			{
				if (this != m_instance)
					Destroy(this.gameObject);
			}
		}
		
		#endregion

		#region Properties

		public Dictionary<UID,GameObject> ObjectMap
		{
			get{ return m_objectMap.Dictionary; }
		}

		#endregion

		#region Initialization

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		private void Init()
		{
			m_objectMap = new GameObjectUIDMap();
		}

		#endregion

		#region Registration Interface

		/// <summary>
		/// Register the specified registrator.
		/// </summary>
		/// <param name="registrator">Registrator.</param>
		public void Register(ObjectID registrator)
		{
			try
			{
				m_objectMap.Dictionary.Add(registrator.ID , registrator.gameObject);
			}
			catch
			{}
		}

		/// <summary>
		/// Unregister the specified registrator.
		/// </summary>
		/// <param name="registrator">Registrator.</param>
		public void Unregister(ObjectID registrator)
		{
			m_objectMap.Dictionary.Remove(registrator.ID);
		}

		#endregion

		#region Serialization Callback

		public void OnBeforeSerialize()
		{

		}

		public void OnAfterDeserialize()
		{

		}

		#endregion

		#region Access

		/// <summary>
		/// Returns the GameObject corresponding to instance ID if any found.
		/// </summary>
		/// <returns>The identifier to object.</returns>
		/// <param name="id">Identifier.</param>
		public GameObject InstanceIDToObject(UID uid)
		{
			GameObject obj;
			if(m_objectMap.Dictionary.TryGetValue(uid,out obj))
				return obj;

			return null;
		}

		#endregion
	}
}
