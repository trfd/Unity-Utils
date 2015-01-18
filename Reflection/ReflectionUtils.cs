//
// ReflectionUtils.cs
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

using System.Reflection;

namespace Utils.Reflection
{
	public static class ReflectionUtils 
	{
		#region Static Interface

		#region Public Static Interface

		public static bool HasAttribute(this MemberInfo info, System.Type type,bool inherit = false)
		{
			return (info.GetCustomAttributes(type,inherit).Length != 0);
		}

		public static FieldInfo[] GetAllFields(System.Type type)
		{
			System.Type currType = type;
			
			List<FieldInfo> fields = new List<FieldInfo>();
			
			// Adds public fields
			
			fields.AddRange(currType.GetFields(BindingFlags.Public | BindingFlags.Instance));
			
			// Adds private fields of all parent classes
			
			BindingFlags bindFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			
			while(currType != null && currType != typeof(System.Object))
			{
				fields.AddRange(currType.GetFields(bindFlags));
				
				currType = currType.BaseType;
			}
			
			return fields.ToArray();
		}
		
		public static PropertyInfo[] GetAllProperties(System.Type type)
		{
			System.Type currType = type;
			
			List<PropertyInfo> properties = new List<PropertyInfo>();
			
			// Adds public fields
			
			properties.AddRange(currType.GetProperties(BindingFlags.Public | BindingFlags.Instance));
			
			// Adds private fields of all parent classes
			
			BindingFlags bindFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			
			while(currType != null && currType != typeof(System.Object))
			{
				properties.AddRange(currType.GetProperties(bindFlags));
				
				currType = currType.BaseType;
			}
			
			return properties.ToArray();
		}

		public static MethodInfo[] GetAllMethods(System.Type type)
		{
			System.Type currType = type;
			
			List<MethodInfo> methods = new List<MethodInfo>();
			
			// Adds public fields
			
			methods.AddRange(currType.GetMethods(BindingFlags.Public | BindingFlags.Instance));
			
			// Adds private fields of all parent classes
			
			BindingFlags bindFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			
			while(currType != null && currType != typeof(System.Object))
			{
				methods.AddRange(currType.GetMethods(bindFlags));
				
				currType = currType.BaseType;
			}
			
			return methods.ToArray();
		}

		/// <summary>
		/// Find the type of name <c>str</c>
		/// </summary>
		/// <returns>The type found if any.CK.Utils.InvalidType otherwise</returns>
		/// <param name="str">String.</param>
		public static System.Type FindType(string str)
		{
			System.Type t = CurrentAssemblySearch(str);
			
			if(t != typeof(InvalidType))
				return t;
			
			return AllAssembliesSearch(str);
		}
		
		/// <summary>
		/// Searches a type of name <c>str</c> in the current Assembly.
		/// </summary>
		/// <returns>The type value found if any, CK.Utils.InvalidType otherwise</returns>
		/// <param name="str">String name of the type.</param>
		public static System.Type CurrentAssemblySearch(string str)
		{
			try
			{
				System.Type t = System.Type.GetType(str);
				
				if(t != null)
					return t;
			}
			catch(System.Exception)
			{}
			
			return typeof(InvalidType);
		}
		
		/// <summary>
		/// Searches a type of name <c>str</c> in the all Assemblies.
		/// </summary>
		/// <returns>The type value found if any, CK.Utils.InvalidType otherwise</returns>
		/// <param name="str">String name of the type.</param>
		public static System.Type AllAssembliesSearch(string str)
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
			
			throw new System.Exception();
			
			return typeof(InvalidType);
		}

		#endregion

		#endregion
	}

}