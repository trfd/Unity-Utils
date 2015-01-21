//
// NestedDataMemberWrapper.cs
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
using System.Collections;
using System.Collections.Generic;

namespace Utils
{
    public class NestedDataMemberWrapper
    {
        #region Private Members

        [HideInInspector]
        [SerializeField]
        private DataMemberWrapper[] m_dataMembers;

        #endregion

        #region Properties

        public DataMemberWrapper[] DataMembers
        {
            get { return m_dataMembers; }
            set { m_dataMembers = value; }
        }

        #endregion

        #region Interface

        public System.Object GetValue(System.Object instance)
        {
            System.Object currObj = instance;

            for(int i=0 ; i<m_dataMembers.Length ; i++)
            {
                currObj = m_dataMembers[i].GetValue(currObj);
            }

            return currObj;
        }

        public void SetValue(System.Object instance, System.Object value)
        {
            System.Object currObj = instance;

            for (int i = 0; i < m_dataMembers.Length-1; i++)
            {
                currObj = m_dataMembers[i].GetValue(currObj);
            }

            m_dataMembers[m_dataMembers.Length - 1].SetValue(currObj, value);
        }

        #endregion
    }

    public class ComponentNestedDataMemberWrapper
    {
        #region Private Members

        [HideInInspector]
        [SerializeField]
        private Component m_component;

        [HideInInspector]
        [SerializeField]
        private NestedDataMemberWrapper m_nestedDataMember;

        #endregion 

        #region Properties

        public Component Component
        {
            get { return m_component;  }
            set { m_component = value; }
        }

        public NestedDataMemberWrapper NestedDataMember
        {
            get { return m_nestedDataMember;  }
            set { m_nestedDataMember = value; }
        }

        #endregion
    }
}
