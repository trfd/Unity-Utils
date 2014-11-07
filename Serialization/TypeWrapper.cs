//
// TypeWrapper.cs
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
	#region TypeWrapper

	/// <summary>
	/// Wraps System.Reflection.FieldInfo to enable Unity serialization.
	/// </summary>
	[System.Serializable]
	public class TypeWrapper
	{
		#region Static

		static System.Type sInvalid = typeof(InvalidType);

		/// <summary>
		/// Checks if the string matches any 
		/// existing type.
		/// </summary>
		/// <returns>Whether or not a type of name <c>str</c> exists</returns>
		/// <param name="str">String.</param>
		public static bool Exists(string str)
		{
			return (FindType(str) != sInvalid);
		}

		/// <summary>
		/// Find the type of name <c>str</c>
		/// </summary>
		/// <returns>The type found if any.CK.Utils.InvalidType otherwise</returns>
		/// <param name="str">String.</param>
		public static System.Type FindType(string str)
		{
			System.Type t = CurrentAssemblySearch(str);

			if(t != sInvalid)
				return t;

			return AllAssembliesSearch(str);
		}

		/// <summary>
		/// Searches a type of name <c>str</c> in the current Assembly.
		/// </summary>
		/// <returns>The type value found if any, CK.Utils.InvalidType otherwise</returns>
		/// <param name="str">String name of the type.</param>
		protected static System.Type CurrentAssemblySearch(string str)
		{
			try
			{
				System.Type t = System.Type.GetType(str);
				
				if(t != null)
					return t;
			}
			catch(System.Exception excpt)
			{}
			
			return typeof(InvalidType);
		}

		/// <summary>
		/// Searches a type of name <c>str</c> in the all Assemblies.
		/// </summary>
		/// <returns>The type value found if any, CK.Utils.InvalidType otherwise</returns>
		/// <param name="str">String name of the type.</param>
		protected static System.Type AllAssembliesSearch(string str)
		{
			try
			{
				System.Type t;

				Assembly[] asms = System.AppDomain.CurrentDomain.GetAssemblies();

				for(int i=0 ; i<asms.Length ; i++)
				{
					if((t = asms[i].GetType(str)) != null)
					{
						return t;
					}
				}
			}
			catch(System.SystemException excpt)
			{}

			return sInvalid;
		}

		#endregion

		#region Private Members

		/// <summary>
		/// String name of the type.
		/// </summary>
		[UnityEngine.SerializeField]
		private string m_name;

		/// <summary>
		/// Actual Type
		/// </summary>
		private System.Type m_type;

		/// <summary>
		/// Dirty flag for myType.
		/// Used only via reflection.
		/// </summary>
		private  bool m_dirty;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the name of the type.
		/// When the setter is used, TypeWrapper will immediatly
		/// match the new string value to an existing type. 
		/// If it fails, the value will be set to System.Object
		/// and an Error will be logged with the handled Exception message.
		/// </summary>
		/// <value>The name of the type.</value>
		[System.Xml.Serialization.XmlIgnore]
		public string TypeName
		{ 
			get
			{ 
				return m_name; 
			}

			set
			{ 
				SetType(value);
			}
		}

		/// <summary>
		/// Gets or sets the actual type of TypeWrapper instance.
		/// This is the safest way of setting both name and type values,
		/// since setting through TypeWrapper.typeName can fail.
		/// Type value and name are set to CK.InvalidType if
		/// value provided doesn't match any existing type.
		/// </summary>
		/// <value>Type value</value>
		[System.Xml.Serialization.XmlIgnore]
		public System.Type Type
		{
			get
			{
				if(m_dirty)
					SetType(m_name);

				return m_type;
			}

			set
			{
				if(value != null)
					m_type = value;
				else
					m_type = typeof(InvalidType);
				
				m_name = m_type.Name;
			}
		}

		public bool IsValid
		{
			get{ return (m_type != typeof(InvalidType)); }
		}

		#endregion

		#region Constructors

		public TypeWrapper()
		{
			// Setting to default values
			m_type = typeof(InvalidType);
			m_name = m_type.Name;
		}

		public TypeWrapper(System.Type pType)
		{
			// Using property
			Type = pType;
		}

		public TypeWrapper(string pTypeName)
		{
			// Using property
			TypeName = pTypeName;
		}
	
		#endregion

		#region Private Methods

		/// <summary>
		/// Sets the type using its string name.
		/// Type value and name are set to CK.InvalidType if
		/// value provided doesn't match any existing type.
		/// </summary>
		/// <param name="value">Value.</param>
		private void SetType(string value)
		{
			try
			{
				m_type = FindType(value);
				m_name = value;
				m_dirty = false;
			}
			catch(System.Exception excpt)
			{
				Debug.LogError("TypeWrapper string name "+value+" doesn't correspond to any type. "+
				               "Values will be set to default ones.Exception: "+excpt.Message);
				
				// Reset value to default
				m_type = typeof(InvalidType);
				m_name = m_type.Name;
				m_dirty = false;
			}
		}

		#endregion

		#region Casts

		// String casts

		public static implicit operator string(TypeWrapper wrap)
		{
			return wrap.TypeName;
		}

		public static implicit operator TypeWrapper(string strName)
		{
			return new TypeWrapper(strName);
		}

		// System.Type cast

		public static implicit operator System.Type(TypeWrapper wrap)
		{
			return wrap.Type;
		}

		public static implicit operator TypeWrapper(System.Type sType)
		{
			return new TypeWrapper(sType);
		}

		#endregion

	}

	#endregion

	#region InvalidType

	public class InvalidType
	{
	}

	#endregion
}

