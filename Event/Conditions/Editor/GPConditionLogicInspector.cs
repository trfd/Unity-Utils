//
// GPConditionLogicInspector.cs
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
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Utils.Event
{
	[GPConditionInspector(typeof(GPConditionAND))]
	public class GPConditionANDInspector : GPConditionInspector
	{
		int m_selectedAIndex;
		int m_selectedBIndex;

		GPConditionInspector m_aInspector;
		GPConditionInspector m_bInspector;

		protected override void OnInspectorGUI()
		{
			GPConditionAND condition = (GPConditionAND) Condition;

			EditorGUI.indentLevel++;

			m_selectedAIndex = GPConditionInspector.CreateConditionField(ref condition._a,m_selectedAIndex,condition.Handler);
			m_selectedBIndex = GPConditionInspector.CreateConditionField(ref condition._b,m_selectedBIndex,condition.Handler);

			if(m_aInspector == null && condition._a != null)
				m_aInspector = GPConditionInspector.CreateInspector(condition._a);

			if(m_bInspector == null && condition._b != null)
				m_bInspector = GPConditionInspector.CreateInspector(condition._b);

			if(m_aInspector != null)
				m_aInspector.DrawInspector();

			if(m_aInspector != null && m_bInspector != null)
				EditorGUILayout.LabelField("AND");

			if(m_bInspector != null)
				m_bInspector.DrawInspector();

			EditorGUI.indentLevel--;
		}
	}

	[GPConditionInspector(typeof(GPConditionOR))]
	public class GPConditionORInspector : GPConditionInspector
	{
		int m_selectedAIndex;
		int m_selectedBIndex;
		
		GPConditionInspector m_aInspector;
		GPConditionInspector m_bInspector;
		
		protected override void OnInspectorGUI()
		{
			GPConditionOR condition = (GPConditionOR) Condition;
			
			EditorGUI.indentLevel++;
			
			m_selectedAIndex = GPConditionInspector.CreateConditionField(ref condition._a,m_selectedAIndex,condition.Handler);
			m_selectedBIndex = GPConditionInspector.CreateConditionField(ref condition._b,m_selectedBIndex,condition.Handler);
			
			if(m_aInspector == null && condition._a != null)
				m_aInspector = GPConditionInspector.CreateInspector(condition._a);
			
			if(m_bInspector == null && condition._b != null)
				m_bInspector = GPConditionInspector.CreateInspector(condition._b);
			
			if(m_aInspector != null)
				m_aInspector.DrawInspector();
			
			if(m_aInspector != null && m_bInspector != null)
				EditorGUILayout.LabelField("OR");
			
			if(m_bInspector != null)
				m_bInspector.DrawInspector();
			
			EditorGUI.indentLevel--;
		}
	}

	[GPConditionInspector(typeof(GPConditionNOT))]
	public class GPConditionNOTInspector : GPConditionInspector
	{
		int m_selectedAIndex;
		
		GPConditionInspector m_aInspector;
		
		protected override void OnInspectorGUI()
		{
			GPConditionNOT condition = (GPConditionNOT) Condition;
			
			EditorGUI.indentLevel++;
			
			m_selectedAIndex = GPConditionInspector.CreateConditionField(ref condition._a,m_selectedAIndex,condition.Handler);
			
			if(m_aInspector == null && condition._a != null)
				m_aInspector = GPConditionInspector.CreateInspector(condition._a);
			
			if(m_aInspector != null)
			{
				EditorGUILayout.LabelField("NOT");

				m_aInspector.DrawInspector();
			}

			EditorGUI.indentLevel--;
		}
	}
}
