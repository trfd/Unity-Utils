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

        void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
                m_instance.Init();
                //DontDestroyOnLoad(this);
            }
            else
            {
                if (this != m_instance)
                    Destroy(this.gameObject);
            }
        }

        #endregion

        #region Delegates

        public delegate void EventDelegate(GPEvent evt);

        #endregion

        #region Private Members

        /// <summary>
        /// Whether or not the list of GPEventID is dirty.
        /// </summary>
        private bool m_isEventIDListDirty = true;

        /// <summary>
        /// Map GPEventID.ID to listener delegates
        /// </summary>
        private Dictionary<int, EventDelegate> m_eventMap;
        
        /// <summary>
        /// Maps event string name to GPEventID for ease of use.
        /// </summary>
        [UnityEngine.SerializeField]
		private EventIDMap m_eventIDMap;

        /// <summary>
        /// List of all GPEventID declared.
        /// </summary>
        private GPEventID[] m_eventIDList;

        /// <summary>
        /// List of event names
        /// </summary>
        private string[] m_eventIDNameList;

        #endregion

		#region Properties

		public Dictionary<string,GPEventID> EventMap
		{
			get{ return m_eventIDMap.Dictionary; }
		}

        public GPEventID[] EventIDs
        {
            get
            {
                if (m_isEventIDListDirty)
                    CreateEventIDList();

                return m_eventIDList;
            }
        }

        public string[] EventNames
        {
            get
            {
                if(m_isEventIDListDirty)
                    CreateEventIDList();

                return m_eventIDNameList;
            }
        }

		#endregion

        private void Init()
        {
			m_eventIDMap = new EventIDMap();
			m_eventMap = new Dictionary<int, EventDelegate>();
        }

        #region Registration

		public void Register(int evtID, EventDelegate del)
		{
			try
			{
				m_eventMap[evtID] += del;
			}
			catch (KeyNotFoundException)
			{
				Debug.Log("Can not register for event "+evtID);
			}
		}

        public void Register(string evtName, EventDelegate del)
        {
            try
            {
				GPEventID evtID = EventNameToID(evtName);

				if(evtID.Equals(GPEventID.Invalid))
					throw new KeyNotFoundException();		

				Register(evtID.ID,del);
            }
            catch (KeyNotFoundException)
            {
				Debug.Log("Can not register for event "+evtName);
            }
        }

		public void Unregister(int evtID, EventDelegate del)
		{
			try
			{
				m_eventMap[evtID] -= del;
			}
			catch (KeyNotFoundException)
			{
				Debug.Log("Can not unregister for event "+evtID);
			}
		}

        public void Unregister(string evtName, EventDelegate del)
        {
            try
            {
				GPEventID evtID = EventNameToID(evtName);
				
				if(evtID == GPEventID.Invalid)
					throw new KeyNotFoundException();		

				Unregister(evtID.ID,del);
            }
            catch (KeyNotFoundException)
            {
				Debug.Log("Can not unregister for event "+evtName);
			}
        }

        #endregion

		#region EventID Management

        public GPEventID EventNameToID(string evtName)
		{
			try
			{
				return m_eventIDMap.Dictionary[evtName];
			}
			catch (KeyNotFoundException)
			{
				return GPEventID.Invalid;
			}
		}

		public void AddEventName()
		{
			int maxID = 0;

            foreach (KeyValuePair<string, GPEventID> kvp in m_eventIDMap.Dictionary)
		    {
		        int id = kvp.Value.ID;

				if(maxID <= id)
					maxID = id;
			}

			m_eventIDMap.Dictionary.Add("Unammed",new GPEventID{ ID=maxID+1, Name= "Unammed"});

		    m_isEventIDListDirty = true;
		}

        public void RemoveEventName(GPEventID id)
        {
            m_eventIDMap.Dictionary.Remove(id.Name);

            m_isEventIDListDirty = true;
        }

        public void CheckNames(GPEventID id)
        {
           Debug.Log("Map matching: "+(m_eventIDMap.Dictionary[id.Name].ID == id.ID));

            foreach(GPEventID eventId in m_eventIDList)
            {
                if(eventId.ID == id.ID)
                {
                    Debug.Log("List matching: " + (eventId.Name == id.Name));
                    break;
                }
            }
        }

        private void CreateEventIDList()
        {
            m_eventIDList = new GPEventID[m_eventIDMap.Dictionary.Count];
            m_eventIDMap.Dictionary.Values.CopyTo(m_eventIDList ,0);
            m_isEventIDListDirty = false;
        }

        public int IndexOfEventID(GPEventID id)
        {
            if(m_isEventIDListDirty)
                CreateEventIDList();

            return Array.IndexOf(m_eventIDList,id);
        }

        public int IndexOfID(int id)
        {
            if (m_isEventIDListDirty)
                CreateEventIDList();

            for(int i=0 ; i<m_eventIDList.Length ; i++)
            {
                if (m_eventIDList[i].ID == id)
                    return i;
            }

            return -1;
        }

		#endregion

        #region Post Events

		public void PostEvent(string evtName)
        {
            EventDelegate value;
		
			try
			{
				GPEventID evtID = EventNameToID(evtName);
				
				if(evtID.ID<0)
					throw new KeyNotFoundException();

			    if (m_eventMap.TryGetValue(evtID.ID, out value))
			    {
			        value(new GPEvent {EventID = evtID});
                }
				else
					throw new KeyNotFoundException();
			}
			catch(KeyNotFoundException)
			{
				Debug.LogError("Event manager does not contain the event name: " + name);
			}
        }

        #endregion


    }
}
