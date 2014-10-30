//
// DictonaryWrapper.cs
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
	/// <summary>
	/// Wrapping of .Net dictionary enabling Unity serialization / deserialization.
	/// </summary>
	public class DictionaryWrapper<Key,Value> : UnityEngine.ISerializationCallbackReceiver
	{
		#region Private Members

		/// <summary>
		/// List of all keys. Only used for serialization / deserialization.
		/// </summary>
		[UnityEngine.SerializeField]
		private List<Key> m_keys;
			
		/// <summary>
		/// List of all values. Only used for serialization / deserialization.
		/// </summary>
		[UnityEngine.SerializeField]
		private List<Value> m_values;

		/// <summary>
		/// Wrapped dictionary.
		/// </summary>
		private Dictionary<Key,Value> m_dict;

		#endregion

		#region Properties

		/// <summary>
		/// Access to wrapped dictionnary.
		/// </summary>
		/// <value>The dictionary.</value>
		public Dictionary<Key,Value> Dictionary
		{
			get{ return m_dict; }
		}

		#endregion

		#region Constructors

		public DictionaryWrapper()
		{
			m_dict = new Dictionary<Key, Value>();
			m_keys = new List<Key>();
			m_values = new List<Value>();
		}

		public DictionaryWrapper(Dictionary<Key,Value> dict)
		{
			m_dict = dict;
			m_keys = new List<Key>();
			m_values = new List<Value>();
		}

		#endregion

		#region Serialization

		/// <summary>
		/// Raises the before serialize event.
		/// </summary>
		public void OnBeforeSerialize()
		{
			m_keys.Clear();
			m_values.Clear();

			foreach(KeyValuePair<Key,Value> pair in m_dict)
			{
				m_keys.Add(pair.Key);
				m_values.Add(pair.Value);
			}
		}

		/// <summary>
		/// Raises the after deserialize event.
		/// </summary>
		public void OnAfterDeserialize()
		{
			if(m_keys.Count != m_values.Count)
			{
				Debug.LogError("Serialization error: Key and value lists doesn't match");
				return;
			}

			m_dict.Clear();

			for(int i=0 ; i<m_keys.Count ; ++i)
			{
				m_dict.Add(m_keys[i],m_values[i]);
			}
		}

		#endregion
	}
}