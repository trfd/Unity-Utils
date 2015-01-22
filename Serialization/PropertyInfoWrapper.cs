using UnityEngine;
using System.Collections;
using System.Reflection;

namespace Utils
{
	[System.Serializable]
	public class PropertyInfoWrapper : ISerializationCallbackReceiver
	{
		#region Private Members

		/// <summary>
		/// Property
		/// </summary>
		private PropertyInfo m_info;
		
		/// <summary>
		/// Type of property's declaring type. Used for serialization/deserialization.
		/// </summary>
		[UnityEngine.SerializeField]
		private TypeWrapper m_type;
		
		/// <summary>
		/// Name of wrapped property. Used for serialization/deserialization.
		/// </summary>
		[UnityEngine.SerializeField]
		private string m_propertyName;
		
		#endregion
		#region Properties
		
		[System.Xml.Serialization.XmlIgnore]
		public PropertyInfo PropertyInfo
		{
			get{ return m_info;  }
			set{ m_info = value; }
		}
		
		#endregion

		#region Constructors
		
		public PropertyInfoWrapper()
		{
			m_info = null;
		}
		
		public PropertyInfoWrapper(PropertyInfo info)
		{
			m_info = info;
		}
		
		#endregion

#if UNITY_EDITOR
		
		public static void SetSerializedPropertyValue(UnityEditor.SerializedProperty property, 
		                                              PropertyInfoWrapper prop)
		{
			TypeWrapper.SetSerializedPropertyValue(property.FindPropertyRelative("m_type"),prop.m_type);
			
			property.FindPropertyRelative("m_propertyName").stringValue = prop.m_propertyName;
		}
		
#endif
		
		#region Serialization / Deserialization
		
		/// <summary>
		/// Raises the before serialization event.
		/// </summary>
		public void OnBeforeSerialize()
		{
			if(m_info == null)
			{
				m_propertyName = "";
				return;
			}
			
			m_type = new TypeWrapper(m_info.DeclaringType);
			m_propertyName = m_info.Name;
		}
		
		/// <summary>
		/// Raises the after serialization event.
		/// </summary>
		public void OnAfterDeserialize()
		{
			m_info = m_type.Type.GetProperty(m_propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

			if(m_info == null)
			{
				Debug.LogWarning("Property '"+m_propertyName+"' not found in type "+m_type.TypeName);
			}
		}
		
		#endregion
	}
}