//
// ValueProviderEditor.cs
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

namespace Utils
{
    using Event;

    public class ValueProviderEditor<T>
    {
        #region Private Members

        ComponentNestedDataMemberWrapper[] m_dataMembers;
        string[] m_dataMembersName;

        GPActionVariable[] m_actionVars;
        string[] m_actionVarsName;

        int m_selectedDataMember;
        int m_selectedActionVariable;

        #endregion

        #region Properties

        public ValueProvider<T> Provider
        {
            get;
            set;
        }
        
        #endregion

        public void Display()
        {
            if(Provider == null)
                return;

            ProviderKind kind = (ProviderKind)EditorGUILayout.EnumPopup("Kind", Provider._kind);

            if (kind != Provider._kind)
                ChangeKind(kind);

            GameObject obj = (GameObject)EditorGUILayout.ObjectField(Provider._object, typeof(GameObject));

            if (obj != Provider._object)
                ChangeObject(obj);

            DisplaySpecificFields();
        }

        public void DisplaySpecificFields()
        {
            switch(Provider._kind)
            {
                case ProviderKind.OBJECT_MEMBER:   DisplayDataMemberSelection();     break;
                case ProviderKind.ACTION_VARIABLE: DisplayActionVariableSelection(); break;
                case ProviderKind.CONSTANT_VALUE:  DisplayConstantValueField();      break;
            }
        }

        protected void DisplayDataMemberSelection()
        {
            m_selectedDataMember = EditorGUILayout.Popup(m_selectedDataMember, m_dataMembersName);

            Provider._nestedDataMember = m_dataMembers[m_selectedDataMember];
        }
        protected void DisplayActionVariableSelection()
        {
            m_selectedActionVariable = EditorGUILayout.Popup(m_selectedActionVariable, m_actionVarsName);

            Provider._actionVariable = m_actionVars[m_selectedActionVariable];
        }

        protected void DisplayConstantValueField()
        {
            Provider._constValue = (T)EditorUtils.DrawField(new GUIContent(""), Provider._constValue);
        }

        #region Update Methods

        private void ChangeKind(ProviderKind newKind)
        {
            switch(newKind)
            {
                case ProviderKind.OBJECT_MEMBER: UpdateDataMembers();
                    break;
                case ProviderKind.ACTION_VARIABLE: UpdateActionVariables();
                    break;
            }

            Provider._kind = newKind;
        }

        private void ChangeObject(GameObject obj)
        {
            switch (Provider._kind)
            {
                case ProviderKind.OBJECT_MEMBER: UpdateDataMembers();
                    break;
                case ProviderKind.ACTION_VARIABLE: UpdateActionVariables();
                    break;
            }

            Provider._object = obj;
        }

        private void UpdateDataMembers()
        {
            m_dataMembers = ComponentNestedDataMemberWrapper.CreateGameObjectMemberList(Provider._object);

            m_dataMembersName = new string[m_dataMembers.Length];

            for(int i=0 ; i<m_dataMembers.Length ; i++)
                m_dataMembersName[i] = m_dataMembers[i].EditorDisplayName();
        }

        private void UpdateActionVariables()
        {
            m_actionVars = ValueProvider<T>.ActionVariablesInGameObject(Provider._object);
            m_actionVarsName = new string[m_actionVars.Length];

            for (int i = 0; i < m_actionVars.Length; i++)
                m_actionVarsName[i] = m_actionVars[i]._varName;
        }

        #endregion
    }

}