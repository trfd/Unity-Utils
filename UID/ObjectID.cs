//
// VSObjectID.cs
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

#define ID_CHECK_RUNTIME

using UnityEngine;
using System.Collections;

namespace Utils
{
	public class ObjectID : MonoBehaviour , ISerializationCallbackReceiver
	{
		#region Private Members
	
		[UnityEngine.SerializeField]
		private bool m_isRegistered;

		[UnityEngine.SerializeField]
		private UID m_id;
		
		#endregion
		
		#region Properties
		
		public UID ID
		{
			get{ return m_id; }
		}

		public bool IsRegistered
		{
			get{ return m_isRegistered; }
		}

		#endregion
		
		#region MonoBehaviour
		
		void Awake()
		{
			if(GetComponents<ObjectID>().Length != 1)
			{
				Debug.LogError("Incorrect number of ObjectID");
			}
			
			if(m_id == null || m_id == UID.Invalid)
			{
				m_id = UIDManager.NewUID();
			}

			if(!m_isRegistered)
				Register();
		}

		void Start()
		{
#if ID_CHECK_RUNTIME
			CheckRegistration();
#endif
		}

		void OnDestroy()
		{
			if(m_isRegistered)
				Unregister();
		}

		#endregion

		#region ID Management

		public void Regen()
		{
			m_id = UIDManager.NewUID();
		}

		public void Set(long newID)
		{
			//m_id.Stamp = newID;
		}

		#endregion

		#region Serialization

		public void OnBeforeSerialize()
		{
			if(m_id == null || m_id == UID.Invalid)
				Regen();
		}
		
		public void OnAfterDeserialize()
		{
			if(m_id == null || m_id == UID.Invalid)
				Regen();
		}
		
		#endregion

		#region Registration

		public void Register(bool force = false)
		{
			if(m_isRegistered && !force)
				return;

			GameObjectManager.Instance.Register(this);
			m_isRegistered = true;
		}

		public void Unregister(bool force = false)
		{
			if(!m_isRegistered && !force)
				return;

			GameObjectManager inst = GameObjectManager.Instance;

			if(inst != null) // GameObjectManager might already have been destroyed
				inst.Unregister(this);

			m_isRegistered = false;
		}

		public void CheckRegistration()
		{
			GameObject obj = GameObjectManager.Instance.InstanceIDToObject(m_id);

			if(obj == null)
			{
				Debug.LogWarning("ObjectID "+m_id.Stamp.ToString("X")+" has register flag " +
				                 "set true but is not registered to the GameObjectManager. ObjectID will register now!");
				Register(true);
				return;
			}
			if(obj != this.gameObject)
			{
				Debug.LogWarning("GameObject ID mismatch registration");
			}
		}

		#endregion
	}

}