//
// VSUIDManager.cs
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

using UnityEngine;
using System;
using System.Collections;

namespace Utils
{
	[System.Serializable]
	public class UID : ISerializationCallbackReceiver
	{
		#region Static

		public static readonly UID Invalid = new UID();

		#endregion

		#region Private Members
	
		[UnityEngine.SerializeField]
		private int m_timeStamp;

		[UnityEngine.SerializeField]
		private int m_uniqueStamp;

		internal Int64 m_stamp;

		#endregion

		public UID()
		{
			m_stamp = Int64.MaxValue;
			m_timeStamp = Int32.MaxValue;
			m_uniqueStamp = Int32.MaxValue;
		}

		public UID(UID copyID)
		{
			m_stamp = copyID.m_stamp;
			m_timeStamp = copyID.m_timeStamp;
			m_uniqueStamp = copyID.m_uniqueStamp;
		}

		public UID(Int32 timeStamp, Int32 uniqueStamp)
		{
			m_timeStamp = timeStamp;
			m_uniqueStamp = uniqueStamp;

			UpdateStamp();
		}

		public int TimeStamp
		{
			get{ return m_timeStamp; }
			set{ m_timeStamp = value; UpdateStamp(); }
		}

		public int UniqueStamp
		{
			get{ return m_uniqueStamp; }
			set{ m_uniqueStamp = value; UpdateStamp(); }
		}

		public Int64 Stamp
		{
			get{ UpdateStamp(); return m_stamp; }
		}

		#region Serialization

		private void UpdateStamp()
		{
			m_stamp = ((Int64) m_timeStamp) | (((Int64)m_uniqueStamp)<<32);
		}

		public void OnBeforeSerialize()
		{
			m_timeStamp = (int)(m_stamp & 0xFFFFFFFF);
			m_uniqueStamp = (int)(m_stamp >> 32);
		}

		public void OnAfterDeserialize()
		{
			UpdateStamp();
		}

		#endregion

		#region System.Object overload

		public override int GetHashCode()
		{
			return m_stamp.GetHashCode();
		}

		public override string ToString()
		{
			return m_stamp.ToString();
		}

		#endregion

		#region Value Equality overload

		public override bool Equals(System.Object obj)
		{
			if(obj == null)
				return false;

			UID otherID = obj as UID;
			if(otherID == null)
				return false;

			return m_stamp == otherID.m_stamp;
		}
		
		public bool Equals(UID otherID)
		{
			if(otherID == null)
				return false;

			return m_stamp == otherID.m_stamp;
		}

		#endregion

		#region Equality Operators Overload

		public static bool operator ==(UID a, UID b)
		{
			if(System.Object.ReferenceEquals(a,b))
				return true;

			if(((object)a == null) || ((object)b == null))
			{
				return false;
			}
			
			return a.m_stamp == b.m_stamp;
		}
		
		public static bool operator !=(UID a, UID b)
		{
			return !(a == b);
		}

		#endregion
	}

	public class UIDManager
	{
		#region Singleton
		
		/// <summary>
		/// Instance of singleton
		/// </summary>
		private static UIDManager m_instance;
		
		public static UIDManager Instance
		{
			get
			{
				if(m_instance == null)
					m_instance = new UIDManager();
				
				return m_instance;
			}
		}

		#endregion

		#region Private Members

		private Int32 m_lastTimeStamp = 0; 
		private Int32 m_lastUnique    = 0;

		#endregion

		public Int32 UnixTimeNow()
		{
			return (Int32)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
		}
	
		public static UID NewUID()
		{
			return Instance.NewUID_impl();
		}

		private UID NewUID_impl()
		{
			Int32 timeStamp = UnixTimeNow();
			m_lastUnique++;

			if(timeStamp != m_lastTimeStamp)
			{
				m_lastTimeStamp = timeStamp;
				m_lastUnique = 0;
			}

			return new UID(m_lastTimeStamp,m_lastUnique);
		}
	}
}