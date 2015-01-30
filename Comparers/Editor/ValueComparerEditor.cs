//
// ValueComparerEditor.cs
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
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using Utils.Reflection;

namespace Utils
{
    public class ValueComparerEditor<T>
    {
        System.Type[] m_types;
        string[] m_typeNames;

        int m_selectedIndex;

        public System.Type Display(ValueComparer<T> c)
        {
            if(m_types == null)
                UpdateList();

            m_selectedIndex = System.Array.IndexOf(m_types, c.GetType());

            if (m_types.Length == 0)
                return null;

            return m_types[m_selectedIndex];
        }

        protected void UpdateList()
        {
            m_types = TypeManager.Instance.ListChildrenTypesOf(typeof(ValueComparer<T>));

            m_typeNames = new string[m_types.Length];

            for(int i = 0 ; i<m_typeNames.Length ; i++)
            {
                object[] aliases = m_types[i].GetCustomAttributes(typeof(ValueComparerAliasAttribute),false);

                if (aliases.Length == 0)
                    m_typeNames[i] = m_types[i].Name;
                else
                    m_typeNames[i] = ((ValueComparerAliasAttribute)aliases[aliases.Length - 1])._name;
            }

        }
    }
}
