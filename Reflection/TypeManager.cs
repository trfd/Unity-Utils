//
// VSReflectionTypeManager.cs
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

using System;
using System.Reflection;
using System.Collections.Generic;

namespace Utils.Reflection
{
    public class TypeManager
    {
        #region Singleton
			
        /// <summary>
        /// Instance of singleton
        /// </summary>
        private static TypeManager m_instance;
			
        public static TypeManager Instance
        {
            get
            {
                if(m_instance == null)
                    m_instance = new TypeManager();
					
                return m_instance;
            }
        }
			
        #endregion

        #region Private Members

        Type[] m_types;

        #endregion

        #region Load

        /// <summary>
        /// Loads all types.
        /// </summary>
        private void LoadAllTypes()
        {
            if(m_types != null)
                return;

            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
				
            List<Type> types = new List<Type>();
				
            for(int i=0 ; i<asms.Length ; i++)
            {
                types.AddRange(asms[i].GetTypes());
            }

            m_types = types.ToArray();
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Returns the list of all types inheriting from a base type
        /// </summary>
        /// <returns>The list children types.</returns>
        /// <param name="parentType">Parent type.</param>
        /// <param name="includeParent">Whether or not the parentType 
        /// type should be in the list returned</param>
        public Type[] ListChildrenTypesOf(Type parentType, bool includeParent = true)
        {
            LoadAllTypes();

            List<Type> types = new List<Type>();

            for(int j = 0 ; j<m_types.Length ; j++)
            {
                if(parentType.IsAssignableFrom(m_types[j]))
                {
                    if(!includeParent && parentType.Equals(m_types[j]))
                        continue;

                    types.Add(m_types[j]);
                }
            }
				
            return types.ToArray();
        }

        public Type[] ListTypesWithAttribute(Type attributeType)
        {
            LoadAllTypes();
				
            List<Type> types = new List<Type>();
				
            for(int j = 0 ; j<m_types.Length ; j++)
            {
                Object[] customAttributes = m_types[j].GetCustomAttributes(attributeType,false);

                if(customAttributes.Length > 0)
                {
                    types.Add(m_types[j]);
                }
            }
				
            return types.ToArray();
        }

        #endregion
    }
}