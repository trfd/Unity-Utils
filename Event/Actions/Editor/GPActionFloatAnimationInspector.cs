using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Utils.Event
{
	[GPActionInspector(typeof(GPActionFloatAnimation))]
	public class GPActionFloatAnimationInspector : GPActionDefaultInspector
	{
		#region Private Members

		private FieldInfo[] m_fieldInfoList;

		private string[] m_fieldNameList;

		private int m_currFieldListIndex;

		#endregion

		protected override void OnInspectorGUI()
		{
			GPActionFloatAnimation anim = (GPActionFloatAnimation) TargetAction;

			SerializedProperty compProp = SerialObject.FindProperty("_component");

			Object prevObj = compProp.objectReferenceValue;

			EditorGUILayout.PropertyField(compProp);

			if(prevObj != compProp.objectReferenceValue)
				CreateFieldList((Component)compProp.objectReferenceValue);

			if(m_fieldInfoList != null && m_fieldInfoList.Length > 0)
			{
					m_currFieldListIndex = EditorGUILayout.Popup("Field",m_currFieldListIndex,m_fieldNameList);
					anim._field.FieldInfo = m_fieldInfoList[m_currFieldListIndex];
			}
			else if(compProp.objectReferenceValue != null)
				EditorGUILayout.HelpBox("Type "+compProp.objectReferenceValue.GetType().FullName+
				                        " doesn't contain Float fields",MessageType.Warning);
			else
				EditorGUILayout.HelpBox("",MessageType.None);

			EditorGUILayout.PropertyField(SerialObject.FindProperty("_duration"));
			EditorGUILayout.PropertyField(SerialObject.FindProperty("_curve"));
		}

		private void CreateFieldList(Component comp)
		{
			GPActionFloatAnimation anim = (GPActionFloatAnimation) TargetAction;

			if(comp == null)
			{
				m_fieldInfoList = null;
				m_fieldNameList = null;
				
				m_currFieldListIndex = 0;
				
				return;
			}
			
			FieldInfo[] infos = comp.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

			List<FieldInfo> dInfos = new List<FieldInfo>();
			List<string> dInfosName = new List<string>();

			m_currFieldListIndex = 0;

			foreach(FieldInfo info in infos)
			{
				if(info.FieldType == typeof(float))
				{
					dInfos.Add(info);
					dInfosName.Add(info.Name);

					if(anim._field.FieldInfo == info)
						m_currFieldListIndex = dInfos.Count-1;
				}
			}

			m_fieldInfoList = dInfos.ToArray();
			m_fieldNameList = dInfosName.ToArray();
		}
	}
}
