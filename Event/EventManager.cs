using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Utils
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

        public delegate void EventDelegate(string evtName);

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

        public void PlayEvent(string name)
        {
            EventDelegate value;

            if (m_eventMap.TryGetValue(name, out value))
            {
                value(name);
            }
            else
            {
                Debug.LogError("Event manager does not contain the event name : " + name);
            }
        }

        #endregion
    }
}
