using UnityEngine;
using System.Collections;

namespace Utils.Event
{
	[System.Serializable]
	public class GPActionRef : ISerializationCallbackReceiver
	{
		#region Private Members

		/// <summary>
		/// Action encapsulated in reference.
		/// </summary>
		private GPAction m_action;

		/// <summary>
		/// Action Name
		/// </summary>
		[UnityEngine.SerializeField]
		private string m_actionName;

		#endregion

		#region Constructors

		public GPActionRef()
		{}

		public GPActionRef(GPAction action)
		{
			SetAction(action);
		}

		public GPActionRef(string actionName)
		{
			m_actionName = actionName;
		}

		#endregion

		#region Public Interface

		public GPAction Action(GameObject containerObj)
		{
			if(m_action == null)
				FindAction(containerObj);

			return m_action;
		}

		public void SetAction(GPAction action)
		{
			m_action = action;

            if(m_action != null)
                m_actionName = m_action._name;
		}

		public bool FindAction(GameObject containerObj)
		{
			GPAction[] actions = containerObj.GetComponents<GPAction>();

			foreach(GPAction action in actions)
			{
				if(action._name == m_actionName)
				{
					m_action = action;
					return true;
				}
			}

			return false;
		}

		#endregion

		#region ISerializationCallbackReceiver

		public void OnBeforeSerialize()
		{
			if(m_action == null)
				return;

			m_actionName = m_action._name;
		}

		public void OnAfterDeserialize()
		{}

		#endregion
	}
}