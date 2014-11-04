//
// FieldInfoWrapper.cs
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
using System.Runtime.Serialization;
using System.Reflection;

namespace Utils
{
	#region FieldInfoWrapper

	/// <summary>
	/// Wraps System.Reflection.FieldInfo to enable Unity serialization.
	/// </summary>
	[System.Serializable]
	public class FieldInfoWrapper : UnityEngine.ISerializationCallbackReceiver
	{
		#region Private Members

		/// <summary>
		/// Wrapped field info.
		/// </summary>
		private FieldInfo m_info;

		/// <summary>
		/// Type of field's declaring type. Used for serialization/deserialization.
		/// </summary>
		[UnityEngine.SerializeField]
		private TypeWrapper m_type;

		/// <summary>
		/// Name of wrapped field. Used for serialization/deserialization.
		/// </summary>
		[UnityEngine.SerializeField]
		private string m_fieldName;

		#endregion

		#region Properties

		[System.Xml.Serialization.XmlIgnore]
		public FieldInfo FieldInfo
		{
			get{ return m_info;  }
			set{ m_info = value; }
		}

		#endregion

		#region Constructors

		public FieldInfoWrapper()
		{
			m_info = null;
		}

		public FieldInfoWrapper(FieldInfo info)
		{
			m_info = info;
		}

		#endregion

		#region Serialization / Deserialization

		/// <summary>
		/// Raises the before serialization event.
		/// </summary>
		public void OnBeforeSerialize()
		{
			if(m_info == null)
			{
				m_fieldName = "";
				return;
			}

			m_type = new TypeWrapper(m_info.DeclaringType);
			m_fieldName = m_info.Name;
		}

		/// <summary>
		/// Raises the after serialization event.
		/// </summary>
		public void OnAfterDeserialize()
		{
			if(m_fieldName == "")
				m_info = null;

			m_info = m_type.Type.GetField(m_fieldName);

			if(m_info == null)
			{
				Debug.LogError("Serialization error in FieldInfoWrapper");
			}
		}

		#endregion
	}

	#endregion
}

