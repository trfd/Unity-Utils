//
// MethodInfoWrapper.cs
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
	#region MethodInfoWrapper

	/// <summary>
	/// This class is a simple wrapping adaptator
	/// of .NET's System.Type to string.
	/// In this way the type will be serialized
	/// as a basic string. 
	/// In order to allow custom drawer for Unity and
	/// restriction on the possible type value it can
	/// get, the wrapper can not be a simple extension 
	/// of System.string
	/// </summary>
	[System.Serializable]
	public class MethodInfoWrapper : UnityEngine.ISerializationCallbackReceiver
	{
		#region Private Members

		/// <summary>
		/// Wrapped method info.
		/// </summary>
		private MethodInfo m_info;

		/// <summary>
		/// Type of method's declaring type. Used for serialization/deserialization.
		/// </summary>
		[UnityEngine.SerializeField]
		private TypeWrapper m_type;

		/// <summary>
		/// Name of wrapped method. Used for serialization/deserialization.
		/// </summary>
		[UnityEngine.SerializeField]
		private string m_methodName;

		/// <summary>
		/// Type of parameters of wrapped method.
		/// </summary>
		[UnityEngine.SerializeField]
		private TypeWrapper[] m_paramTypes;

		#endregion

		#region Properties

		[System.Xml.Serialization.XmlIgnore]
		public MethodInfo MethodInfo
		{
			get{ return m_info;  }
			set{ m_info = value; }
		}

		#endregion

		#region Constructors

		public MethodInfoWrapper()
		{
			m_info = null;
		}

		public MethodInfoWrapper(MethodInfo info)
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
				m_methodName = "";
				return;
			}
			m_type = new TypeWrapper(m_info.DeclaringType);
			m_methodName = m_info.Name;

			ParameterInfo[] paramsInfo = m_info.GetParameters();

			m_paramTypes = new TypeWrapper[paramsInfo.Length];

			for(int i = 0 ; i < m_paramTypes.Length ; i++) 
			{
				m_paramTypes[i] = new TypeWrapper(paramsInfo[i].ParameterType);
			}
		}

		/// <summary>
		/// Raises the after serialization event.
		/// </summary>
		public void OnAfterDeserialize()
		{
			if(m_methodName == "")
				m_info = null;

			System.Type[] paramTypes = new System.Type[m_paramTypes.Length];

			for(int i=0 ; i<paramTypes.Length ; i++)
			{
				paramTypes[i] = m_paramTypes[i].Type;
			}

			m_info = m_type.Type.GetMethod(m_methodName,paramTypes);

			if(m_info == null)
			{
				Debug.LogError("Serialization error in MethodInfoWrapper");
			}
		}

		#endregion
	}

	#endregion
}

