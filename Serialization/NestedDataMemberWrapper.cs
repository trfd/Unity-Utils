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
using System.Reflection;
using System.Linq;

using Utils.Reflection;

namespace Utils
{
	[System.Serializable]
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

#if UNITY_EDITOR
		
		public static void SetSerializedPropertyValue(UnityEditor.SerializedProperty property, 
		                                              NestedDataMemberWrapper member)
		{
			UnityEditor.SerializedProperty dataMemberProp = property.FindPropertyRelative("m_dataMembers");

			dataMemberProp.ClearArray();

			for(int i=0 ; i<member.m_dataMembers.Length ; i++)
			{
				dataMemberProp.InsertArrayElementAtIndex(i);
				DataMemberWrapper.SetSerializedPropertyValue(dataMemberProp.GetArrayElementAtIndex(i),
				                                             member.m_dataMembers[i]);
			}
		}

#endif

        #region Interface

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="instance">Instance.</param>
        public System.Object GetValue(System.Object instance)
        {
            System.Object currObj = instance;

            for(int i=0 ; i<m_dataMembers.Length ; i++)
            {
                currObj = m_dataMembers[i].GetValue(currObj);
            }

            return currObj;
        }

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="instance">Instance.</param>
		/// <param name="value">Value.</param>
        public void SetValue(System.Object instance, System.Object value)
        {
            System.Object currObj = instance;

            for (int i = 0; i < m_dataMembers.Length-1; i++)
            {
                currObj = m_dataMembers[i].GetValue(currObj);
            }

            m_dataMembers[m_dataMembers.Length - 1].SetValue(currObj, value);
        }

		/// <summary>
		/// Append the specified field.
		/// </summary>
		/// <param name="field">Field.</param>
		public NestedDataMemberWrapper Append(FieldInfo field)
		{
			if(field == null)
				return this;

			List<DataMemberWrapper> members;
			
			if(this.m_dataMembers == null)
				members = new List<DataMemberWrapper>();
			else
				members = new List<DataMemberWrapper>(this.m_dataMembers);

			if(members.Count >0 && !field.DeclaringType.IsAssignableFrom( members.Last().GetMemberType() ))
				throw new System.ArgumentException("NestedDataMember append fail: new member declaring type ("+
				                                   field.DeclaringType+") not assignable from last member type ("+
				                                   members.Last().GetMemberType()+
				                                   ")");

			members.Add(new DataMemberWrapper(field));

			NestedDataMemberWrapper newMember = new NestedDataMemberWrapper();
			newMember.m_dataMembers = members.ToArray();
			
			return newMember;
		}

		/// <summary>
		/// Append the specified property.
		/// </summary>
		/// <param name="property">Property.</param>
		public NestedDataMemberWrapper Append(PropertyInfo property)
		{
			if(property == null)
				return this;
			
			List<DataMemberWrapper> members;

			if(this.m_dataMembers == null)
				members = new List<DataMemberWrapper>();
			else
				members = new List<DataMemberWrapper>(this.m_dataMembers);
			
			if(members.Count >0 && !property.DeclaringType.IsAssignableFrom( members.Last().GetMemberType() ))
				throw new System.ArgumentException("NestedDataMember append fail: new member declaring type ("+
				                                   property.DeclaringType+") not assignable from last member type ("+
				                                   members.Last().GetMemberType()+
				                                   ")");
			
			members.Add(new DataMemberWrapper(property));
			
			NestedDataMemberWrapper newMember = new NestedDataMemberWrapper();
			newMember.m_dataMembers = members.ToArray();
			
			return newMember;
		}

		public string EditorDisplayName()
		{
			if(m_dataMembers.Length == 0)
				return "";

			string str = "";
			MemberInfo info;

			for(int i=0 ; i<m_dataMembers.Length-1 ; i++)
			{
				info = m_dataMembers[i].GetMember();

				if(info != null)
					str += info.Name+"/";
				else
					str += "(ERROR: null member)";
			}

			info = m_dataMembers[m_dataMembers.Length-1].GetMember();
			
			if(info != null)
				str += info.Name;
			else
				str += "(ERROR: null member)";


			return str;
		}

		public bool HasIntermediateMemberOfType(System.Type type)
		{
			if(m_dataMembers == null)
				return false;

			foreach(DataMemberWrapper member in m_dataMembers)
				if(member.GetMember().DeclaringType.IsAssignableFrom(type) || 
				   type.IsAssignableFrom(member.GetMember().DeclaringType))
					return true;

			return false;
		}

        public virtual System.Object Invoke()
        {
            throw new System.NotImplementedException();
        }

        #endregion

		#region System.Object
		
		public override bool Equals(System.Object obj)
		{
			if (obj == null)
			{
				return false;
			}
			
			NestedDataMemberWrapper p 
				= obj as NestedDataMemberWrapper;
			if ((System.Object)p == null)
			{
				return false;
			}

			return ( Enumerable.SequenceEqual( p.m_dataMembers, this.m_dataMembers) );
		}
		
		public bool Equals(NestedDataMemberWrapper p)
		{
			// If parameter is null return false:
			if ((object)p == null)
			{
				return false;
			}

			return ( Enumerable.SequenceEqual( p.m_dataMembers, this.m_dataMembers) );
		}
		
		#endregion
    }

	[System.Serializable]
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

		#region Constructor

		public ComponentNestedDataMemberWrapper()
		{
			m_nestedDataMember = new NestedDataMemberWrapper();
		}

		public ComponentNestedDataMemberWrapper(Component c)
		{
			m_component = c;

			m_nestedDataMember = new NestedDataMemberWrapper();
		}
	
		#endregion

#if UNITY_EDITOR

		public static void SetSerializedPropertyValue(UnityEditor.SerializedProperty property, 
		                                              ComponentNestedDataMemberWrapper member)
		{
			property.FindPropertyRelative("m_component").objectReferenceValue = member.m_component;

			NestedDataMemberWrapper.SetSerializedPropertyValue(property.FindPropertyRelative("m_nestedDataMember"),
			                                                   member.m_nestedDataMember);
		}

#endif

		public ComponentNestedDataMemberWrapper Append(FieldInfo field)
		{
			ComponentNestedDataMemberWrapper newMember = 
				new ComponentNestedDataMemberWrapper(this.m_component);

			newMember.m_nestedDataMember = this.m_nestedDataMember.Append(field);

			return newMember;
		}

		public ComponentNestedDataMemberWrapper Append(PropertyInfo property)
		{
			ComponentNestedDataMemberWrapper newMember = 
				new ComponentNestedDataMemberWrapper(this.m_component);

			newMember.m_nestedDataMember = this.m_nestedDataMember.Append(property);
			
			return newMember;
		}

		public string EditorDisplayName()
		{
			return m_component.GetType().Name+"/"+m_nestedDataMember.EditorDisplayName();
		}

		public bool HasIntermediateMemberOfType(System.Type type)
		{
			return m_nestedDataMember.HasIntermediateMemberOfType(type);
		}

        public virtual System.Object Invoke()
        {
            throw new System.NotImplementedException();
        }


		#region System.Object

		public override bool Equals(System.Object obj)
		{
			if(obj == null)
			{
				return false;
			}

			ComponentNestedDataMemberWrapper p 
				= obj as ComponentNestedDataMemberWrapper;
			if((System.Object)p == null)
			{
				return false;
			}

			return (p.m_component == this.m_component && 
			        p.m_nestedDataMember.Equals(this.m_nestedDataMember));
		}
		
		public bool Equals(ComponentNestedDataMemberWrapper p)
		{
			if((object)p == null)
			{
				return false;
			}

			return (p.m_component == this.m_component && 
			        p.m_nestedDataMember == this.m_nestedDataMember);
		}
		
		#endregion

        #region Static Interface

        /// <summary>
        /// Creates the member list for all components of the specified GameObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ComponentNestedDataMemberWrapper[] CreateGameObjectMemberList(GameObject obj, System.Type restrictedType)
        {
            List<ComponentNestedDataMemberWrapper> members = new List<ComponentNestedDataMemberWrapper>();

            if (obj == null)
                return members.ToArray();

            foreach (Component comp in obj.GetComponents<Component>())
            {
				members.AddRange(CreateComponentMemberList(comp,restrictedType));
            }

            return members.ToArray();
        }

        /// <summary>
        /// Creates the component member list.
        /// </summary>
        /// <returns>The component member list.</returns>
        /// <param name="comp">Comp.</param>
		public static ComponentNestedDataMemberWrapper[] CreateComponentMemberList(Component comp, System.Type restrictedType)
        {
			return CreateTypeMemberList(comp.GetType(), new ComponentNestedDataMemberWrapper(comp),restrictedType, 3);
        }

        /// <summary>
        /// Creates the type member list.
        /// </summary>
        /// <returns>The type member list.</returns>
        /// <param name="type">Type.</param>
        /// <param name="parentMember">Parent member.</param>
        private static ComponentNestedDataMemberWrapper[] CreateTypeMemberList(System.Type type,
                                                                               ComponentNestedDataMemberWrapper parentMember,
                                                                               System.Type restrictedType,
		                                                                       int level)
        {
            List<ComponentNestedDataMemberWrapper> members = new List<ComponentNestedDataMemberWrapper>();

            if (level < 0)
                return members.ToArray();

            FieldInfo[] fields = ReflectionUtils.GetAllFields(type);

            foreach (FieldInfo field in fields)
            {
                if (field.DeclaringType == typeof(Component))
                    continue;

                ComponentNestedDataMemberWrapper fieldMember = parentMember.Append(field);

				if(restrictedType.IsAssignableFrom(field.FieldType))
                	members.Add(fieldMember);

                members.AddRange(CreateTypeMemberList(field.FieldType, fieldMember, restrictedType, level - 1));
            }

            PropertyInfo[] properties = ReflectionUtils.GetAllProperties(type);

            foreach (PropertyInfo property in properties)
            {
                if (property.DeclaringType == typeof(Component))
                    continue;

                ComponentNestedDataMemberWrapper propertyMember = parentMember.Append(property);

				if(restrictedType.IsAssignableFrom(property.PropertyType))
                	members.Add(propertyMember);

				members.AddRange(CreateTypeMemberList(property.PropertyType, propertyMember,restrictedType, level - 1));
            }

            return members.ToArray();
        }

        #endregion 
    }
}
