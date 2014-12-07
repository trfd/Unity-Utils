using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Utils.Event
{
	[CustomPropertyDrawer(typeof(Component))]
	public class ComponentDrawer : PropertyDrawer
	{
		#region Private Members
		
		Component[] m_components;
		
		string[] m_componentList;

		int m_componentPopupIndex;

		bool m_thisObject;

		Component m_thisComponent;

		GameObject m_currGameObject;

		#endregion

		#region PropertyDrawer Override

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if(property.objectReferenceValue == null)
				return 3f*EditorGUIUtility.singleLineHeight;
			else
				return 4f*EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.LabelField(position,label);
			position.y += EditorGUIUtility.singleLineHeight;

			EditorGUI.indentLevel++;

			if(m_thisComponent == null)
				m_thisComponent = (Component) property.serializedObject.targetObject;

			if(m_thisComponent == null)
			{
				Debug.LogError("Can not use ComponentDrawer on non-component object");
				return;
			}

			Component comp = (Component) property.objectReferenceValue;

			GameObject newObject = m_currGameObject;

			if(comp != null)
				m_currGameObject = comp.gameObject;

			m_thisObject = (m_thisComponent.gameObject == m_currGameObject);

			bool newThisObject = EditorGUI.ToggleLeft(position,"Use This GameObject",m_thisObject);
			position.y += EditorGUIUtility.singleLineHeight;

			if(m_thisObject != newThisObject)
			{
				m_thisObject = newThisObject;

				if(m_thisObject)
					newObject = m_thisComponent.gameObject;
				else
					newObject = null;
			}

			newObject = (GameObject) EditorGUI.ObjectField(position,"Object",newObject,typeof(GameObject));
			position.y += EditorGUIUtility.singleLineHeight;

			if(m_currGameObject != newObject)
			{
				m_currGameObject = newObject;
			
				if(m_currGameObject == null)
				{
					property.objectReferenceValue = null;
					m_componentList = null;
				}

				CreateComponentList();
			}

			if(m_componentList == null)
				CreateComponentList();

			if(m_componentList != null)
			{
				m_componentPopupIndex = EditorGUI.Popup(position,"Component",m_componentPopupIndex,m_componentList);
				position.y += EditorGUIUtility.singleLineHeight;

				property.objectReferenceValue = m_components[m_componentPopupIndex];
			}

			EditorGUI.indentLevel--;
		}

		#endregion
		
		private void CreateComponentList()
		{	
			if(m_currGameObject == null)
				return;
			
			m_components = m_currGameObject.GetComponents<Component>();
			
			m_componentList = new string[m_components.Length];
			
			m_componentPopupIndex = 0;
			
			for(int i = 0 ; i < m_components.Length ; i++)
			{
				if(m_components[i] == m_thisComponent)
					m_componentPopupIndex = i;
				
				m_componentList[i] = m_components[i].GetType().Name+" ("+m_components[i].GetInstanceID()+")";
			}
		}

	}
}