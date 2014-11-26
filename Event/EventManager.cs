using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Utils.Event
{
    public class EventManager : MonoBehaviour
    {
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

        private Dictionary<string, EventDelegate> m_eventMap;

        #endregion

        private void Init()
        {
            m_eventMap = new Dictionary<string, EventDelegate>();
        }

        #region Registration

        public void Register(string evtName, EventDelegate del)
        {
            try
            {
                m_eventMap[evtName] += del;
            }
            catch (KeyNotFoundException)
            {
                m_eventMap.Add(evtName, del);
            }
        }

        public void Unregister(string evtName, EventDelegate del)
        {
            try
            {
                m_eventMap[evtName] -= del;
            }
            catch (KeyNotFoundException)
            { }
        }

        #endregion

        #region Post Events

        public void PostEvent(string name , UnityEngine.Object obj = null)
        {
			PostEvent(new GPEvent{ Name=name, RelatedObject=obj});
        }
	
		public void PostEvent(GPEvent evt)
		{
			EventDelegate value;
			
			if (m_eventMap.TryGetValue(evt.Name, out value))
			{
				value(evt);
			}
			else
			{
				Debug.LogWarning("Event manager does not contain the event name : " + name);
			}
		}

        #endregion
    }
}
