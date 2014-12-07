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

		private MemberInfo[] m_memberInfoList;

		private string[] m_memberNameList;

		private int m_currMemberListIndex;

		#endregion

		protected override void OnInspectorGUI()
		{
			GPActionFloatAnimation anim = (GPActionFloatAnimation) TargetAction;

			SerializedProperty compProp = SerialObject.FindProperty("_component");

			Object prevObj = compProp.objectReferenceValue;

			EditorGUILayout.PropertyField(compProp);

			if(prevObj != compProp.objectReferenceValue)
				CreateFieldList((Component)compProp.objectReferenceValue);

			if(m_memberInfoList != null && m_memberInfoList.Length > 0)
			{
					m_currMemberListIndex = EditorGUILayout.Popup("Field",m_currMemberListIndex,m_memberNameList);
					anim._member.SetMember( m_memberInfoList[m_currMemberListIndex] );
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
				m_memberInfoList = null;
				m_memberNameList = null;
				
				m_currMemberListIndex = 0;
				
				return;
			}
			
			FieldInfo[] fInfos = comp.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			PropertyInfo[] pInfos = comp.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);


			List<MemberInfo> dInfos = new List<MemberInfo>();
			List<string> dInfosName = new List<string>();

			m_currMemberListIndex = 0;

			foreach(FieldInfo info in fInfos)
			{
				if(info.FieldType == typeof(float))
				{
					dInfos.Add(info);
					dInfosName.Add(info.Name);

					if(anim._member.GetMember() == info)
						m_currMemberListIndex = dInfos.Count-1;
				}
			}

			foreach(PropertyInfo info in pInfos)
			{
				if(info.PropertyType == typeof(float))
				{
					dInfos.Add(info);
					dInfosName.Add(info.Name);
					
					if(anim._member.GetMember() == info)
						m_currMemberListIndex = dInfos.Count-1;
				}
			}

			m_memberInfoList = dInfos.ToArray();
			m_memberNameList = dInfosName.ToArray();
		}
	}
}
