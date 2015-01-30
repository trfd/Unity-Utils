//
// GPConditionInspector.cs
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
	public class GPConditionInspector 
	{
		#region Private Members

		private GPCondition m_condition;

		#endregion

		#region Properties

		public GPCondition Condition
		{
			get{ return m_condition;  } 
			set{ m_condition = value; }
		}

		#endregion

		public void DrawInspector()
		{
			OnInspectorGUI();
		}
		
		protected virtual void OnInspectorGUI()
		{}

		#region Static Interface

		public static int CreateConditionField(ref GPCondition condition, int selectedIndex, EventHandler handler)
		{
			if(condition != null)
				return selectedIndex;

			EditorGUILayout.BeginHorizontal();

			int idx = EditorGUILayout.Popup(selectedIndex,GPConditionManager.s_gpconditionTypeNames);

			if(GUILayout.Button("Add"))
				condition = AddCondition(GPConditionManager.s_gpconditionTypes[idx],handler);

			EditorGUILayout.EndHorizontal();

			return idx;
		}

		public static GPCondition AddCondition(System.Type type, EventHandler handler)
		{
			GPCondition condition = (GPCondition) handler.GetGPActionObjectOrCreate().AddComponent(type);		

			condition.SetHandler(handler);

			return condition;
		}

		public static GPConditionInspector CreateInspector(GPCondition condition)
		{
			GPConditionInspector insp = (GPConditionInspector) System.Activator.CreateInstance(
					GPConditionInspectorManager.InspectorTypeForCondition(condition));
			insp.Condition = condition;

			return insp;
		}

		#endregion
	}
}
