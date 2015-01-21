using UnityEngine;
using System.Collections;
using System.Reflection;

namespace Utils
{
	[System.Serializable]
	public class DataMemberWrapper 
	{
		#region Private Members

		/// <summary>
		/// Field Info
		/// </summary>
		[UnityEngine.SerializeField]
		private FieldInfoWrapper m_field;

		/// <summary>
		/// Property Info
		/// </summary>
		[UnityEngine.SerializeField]
		private PropertyInfoWrapper m_property;
			
		#endregion	

		#region Constructor

		public DataMemberWrapper()
		{
			m_field = new FieldInfoWrapper();
			m_property = new PropertyInfoWrapper();

			m_field.FieldInfo = null;
			m_property.PropertyInfo = null;
		}

		public DataMemberWrapper(PropertyInfo pInfo)
		{
			m_field = new FieldInfoWrapper();
			m_property = new PropertyInfoWrapper();

			m_property.PropertyInfo = pInfo;
			m_field.FieldInfo = null;
		}

		public DataMemberWrapper(FieldInfo fInfo)
		{
			m_field = new FieldInfoWrapper();
			m_property = new PropertyInfoWrapper();

			m_field.FieldInfo = fInfo;
			m_property.PropertyInfo = null;
		}

		#endregion

		#region Accessor
		
		public virtual MemberInfo GetMember()
		{
			if(m_field.FieldInfo != null)
				return m_field.FieldInfo;
			else if(m_property.PropertyInfo != null)
				return m_property.PropertyInfo;

			return null;
		}

        public virtual FieldInfo GetField()
		{
			return m_field.FieldInfo;
		}

        public virtual PropertyInfo GetProperty()
		{
			return m_property.PropertyInfo;
		}

        public virtual void SetMember(MemberInfo member)
		{
			if(member is PropertyInfo)
			{
				m_property.PropertyInfo = (PropertyInfo) member;
				m_field.FieldInfo = null;
			}
			else if(member is FieldInfo)
			{
				m_field.FieldInfo = (FieldInfo) member;
				m_property.PropertyInfo = null;
			}
			else
				Debug.Log("DataMember can not use member of type "+member.GetType().FullName);
		}

        public virtual void SetField(FieldInfo field)
		{
			m_field.FieldInfo = field;
			m_property.PropertyInfo = null;
		}

        public virtual void SetProperty(PropertyInfo prop)
		{
			m_field.FieldInfo = null;
			m_property.PropertyInfo = prop;
		}

        public virtual System.Object GetValue(System.Object instance)
		{
			if(m_field.FieldInfo != null)
				m_field.FieldInfo.GetValue(instance);
			else if(m_property.PropertyInfo != null)
				m_property.PropertyInfo.GetValue(instance,null);

			return null;
		}

        public virtual void SetValue(System.Object instance, System.Object value)
		{
			if(m_field.FieldInfo != null)
				m_field.FieldInfo.SetValue (instance,value);
			else if(m_property.PropertyInfo != null)
				m_property.PropertyInfo.SetValue(instance,value,null);
		}

		#endregion

	}
}
