using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Utils.Event
{
	public class EventManager : MonoBehaviour
	{
		[System.Serializable]
		public class EventIDMap : Utils.DictionaryWrapper<string, GPEventID>
		{
		}

        #region Singleton

		private static EventManager m_instance;

		public static EventManager Instance 
        {
			get 
            {
				if (m_instance == null) 
                {
					m_instance = GameObject.FindObjectOfType<EventManager>();
					//DontDestroyOnLoad(m_instance.gameObject);
				}

				return m_instance;
			}
		}

		void Awake ()
		{
			if (m_instance == null) 
            {
				m_instance = this;
				//DontDestroyOnLoad(this);
			} 
            else
            {
				if (this != m_instance)
					Destroy (this.gameObject);
			}
		}

        #endregion

        #region Delegates

		public delegate void EventDelegate (GPEvent evt);

        #endregion

        #region Private Members

		/// <summary>
		/// Whether or not the list of GPEventID is dirty.
		/// </summary>
		private bool m_isEventIDMapDirty = true;

		/// <summary>
		/// Map GPEventID.ID to listener delegates
		/// </summary>
		private Dictionary<int, EventDelegate> m_eventMap;
        
		/// <summary>
		/// Maps event string name to GPEventID for ease of use.
		/// </summary>
		private EventIDMap m_eventIDMap;

		/// <summary>
		/// List of all GPEventID declared.
		/// </summary>
		[UnityEngine.SerializeField]
		private List<GPEventID> m_eventIDList;

		/// <summary>
		/// List of event names
		/// </summary>
		private string[] m_eventIDNameList;

        #endregion

		#region Properties

		public Dictionary<string,GPEventID> EventMap 
		{
			get
			{ 
				if(m_isEventIDMapDirty)
					CreateEventIDMap();
			
				return m_eventIDMap.Dictionary; 
			}
		}

		public GPEventID[] EventIDs 
		{
		    get
		    {
                if(m_eventIDList == null)
                    m_eventIDList = new List<GPEventID>();

		        return m_eventIDList.ToArray();
		    }
		}

		public string[] EventNames 
		{
			get 
			{
				if(m_isEventIDMapDirty)
					CreateEventIDMap();

				return m_eventIDNameList;
			}
		}

		#endregion

        #region Registration

		public void Register (int evtID, EventDelegate del)
		{
			try
            {
				m_eventMap [evtID] += del;
			} 
            catch (KeyNotFoundException) 
            {
				Debug.Log ("Can not register for event " + evtID);
			}
		}

		public void Register (string evtName, EventDelegate del)
		{
			try 
            {
				GPEventID evtID = EventNameToID (evtName);

				if (evtID.Equals (GPEventID.Invalid))
					throw new KeyNotFoundException ();		

				Register (evtID.ID, del);
			} 
            catch (KeyNotFoundException) 
            {
				Debug.Log ("Can not register for event " + evtName);
			}
		}

		public void Unregister (int evtID, EventDelegate del)
		{
			try 
            {
				m_eventMap [evtID] -= del;
			} 
            catch (KeyNotFoundException) 
            {
				Debug.Log ("Can not unregister for event " + evtID);
			}
		}

		public void Unregister (string evtName, EventDelegate del)
		{
			try 
			{
				GPEventID evtID = EventNameToID (evtName);
				
				if (evtID == GPEventID.Invalid)
					throw new KeyNotFoundException ();		

				Unregister (evtID.ID, del);
			} 
			catch (KeyNotFoundException) 
			{
				Debug.Log ("Can not unregister for event " + evtName);
			}
		}

		public void RefreshIDList()
		{
			CreateEventIDMap();
		}

        #endregion

		#region EventID Management

		public GPEventID EventNameToID (string evtName)
		{
			try 
			{
				return m_eventIDMap.Dictionary [evtName];
			}
			catch (KeyNotFoundException) 
			{
				return GPEventID.Invalid;
			}
		}

		public void AddEventName ()
		{
			int maxID = 0;

			foreach(GPEventID evtID in m_eventIDList) 
			{
                int id = evtID.ID;

				if (maxID <= id)
					maxID = id;
			}

			string newEventName = "Unnammed "+(maxID+1).ToString();

			m_eventIDList.Add(new GPEventID{ ID=maxID+1, Name=newEventName });

			m_isEventIDMapDirty = true;
		}

		public void RemoveEventName(GPEventID id)
		{
			m_eventIDList.Remove(id);

			m_isEventIDMapDirty = true;
		}

		public void CheckNames(GPEventID id)
		{
			Debug.Log ("Map matching: " + (m_eventIDMap.Dictionary [id.Name].ID == id.ID));

			foreach (GPEventID eventId in m_eventIDList) 
			{
				if (eventId.ID == id.ID) 
				{
					Debug.Log ("List matching: " + (eventId.Name == id.Name));
					break;
				}
			}
		}

		private void CreateEventIDMap()
		{
			m_eventIDMap = new EventIDMap();
			m_eventIDNameList = new string[m_eventIDList.Count];

			for(int i=0 ; i<m_eventIDList.Count ; i++)
			{
				m_eventIDMap.Dictionary.Add(m_eventIDList[i].Name,m_eventIDList[i]);
				m_eventIDNameList[i] = m_eventIDList[i].Name;
			}

			m_isEventIDMapDirty = false;
		}

		public int IndexOfEventID (GPEventID id)
		{
			if (m_isEventIDMapDirty)
				CreateEventIDMap();

			return m_eventIDList.IndexOf(id);
		}

		public int IndexOfID (int id)
		{
			if (m_isEventIDMapDirty)
				CreateEventIDMap();

			for (int i=0; i<m_eventIDList.Count; i++) {
				if (m_eventIDList [i].ID == id)
					return i;
			}

			return -1;
		}

		#endregion

        #region Post Events

		public void PostEvent (string evtName, GameObject obj = null)
		{
			EventDelegate value;
			
			try 
			{
				GPEventID evtID = EventNameToID (evtName);
				
				if (evtID.ID < 0)
					throw new KeyNotFoundException ();
				
				if (m_eventMap.TryGetValue (evtID.ID, out value)) 
				{
					GPEvent evt = new GPEvent();

					evt.EventID = evtID;
					evt.RelatedObject = obj;

					value (evt);
				} 
				else
					throw new KeyNotFoundException ();
			} 
			catch (KeyNotFoundException) 
			{
				Debug.LogError ("Event manager does not contain the event name: " + name);
			}
		}

        #endregion


	}
}
