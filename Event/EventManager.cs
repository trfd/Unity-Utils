using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Utils.Event
{
    public class EventManager : MonoBehaviour
    {
		[System.Serializable]
		public class EventIDMap : DictionaryWrapper<string, int>
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

        public delegate void EventDelegate(string evtName);

        #endregion

        #region Private Members

        private Dictionary<EventID, EventDelegate> m_eventMap;
	
		[UnityEngine.SerializeField]
		private EventIDMap m_eventIDMap;

        #endregion

		#region Properties

		public int[] EventIDs
		{
			get{ return m_eventIDMap.Dictionary.Keys; }
		}

		public string[] EventNames
		{
			get{ return m_eventIDMap.Dictionary.Values; }
		}

		public Dictionary<int,string> EventMap
		{
			get{ return m_eventIDMap.Dictionary; }
		}

		#endregion

        private void Init()
        {
			m_eventIDMap = new EventIDMap();
			m_eventMap = new Dictionary<EventID, EventDelegate>();
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
				int evtID = EventNameToID(evtName);

				if(evtID<0)
					throw new KeyNotFoundException();		

				Register(evtID,del);
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
				int evtID = EventNameToID(evtName);
				
				if(evtID<0)
					throw new KeyNotFoundException();		

				Unregister(evtID,del);
            }
            catch (KeyNotFoundException)
            {
				Debug.Log("Can not unregister for event "+evtName);
			}
        }

        #endregion

		#region EventID Management

		public void EventNameToID(string evtName)
		{
			try
			{
				return m_eventIDMap.Dictionary[evtName];
			}
			catch (KeyNotFoundException)
			{
				return -1;
			}
		}

		public void AddEventName()
		{
			int maxID = 0;

			for(int i=0 ; i<m_eventMap.Count ; i++)
			{
				int id = m_eventMap.Keys[i];

				if(maxID <= id)
					maxID = id;
			}

			m_eventMap.Add(maxID+1,"New_Event");
		}

		#endregion

        #region Post Events

		public void PlayEvent(string evtName)
        {
            EventDelegate value;
		
			try
			{
				int evtID = EventNameToID(evtName);
				
				if(evtID<0)
					throw new KeyNotFoundException();	

				if (m_eventMap.TryGetValue(name, out value))
				{
					value(name);
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
