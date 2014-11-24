using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utils.Event
{
    public class EventHandler : MonoBehaviour
    {
        #region Enum Definition
	
        /// <summary>
        /// State of the handler
        /// </summary>
        public enum HandlerState
        {
            NONE,
            RUNNING,
            SLEEPING,
            TERMINATED
        }

        #endregion

        #region Static Members

        public const int s_kindFixedCountMask = 0x01;
        public const int s_kindLockMask       = 0x02;

        #endregion 

        #region Private Members

		/// <summary>
		/// Whether or not the handler can fire a fixed or infinite number of time
		/// its action.
		/// </summary>
		[UnityEngine.SerializeField]
		private bool m_usesFixedCount;

		/// <summary>
		/// Whether or not the handler can fire its action while it's already running.
		/// </summary>
		[UnityEngine.SerializeField]
		private bool m_usesLockUntilCompletion;

        /// <summary>
        /// Number of times the event has been triggered
        /// </summary>
        private int m_triggerCount;

        /// <summary>
        /// Current state of the handler
        /// </summary>
        private HandlerState m_currState = HandlerState.NONE;

        #endregion

        #region Public Members
		
		/// <summary>
		/// Action to trigger
		/// </summary>
		public GPAction _action;

        /// <summary>
        /// Name of the event the handler is listening
        /// </summary>
        public string _eventName;

        /// <summary>
        /// Maximum number of time the event can be triggered
        /// </summary>
        public int _maxTriggerCount;

        #endregion

        #region Properties

		[System.Obsolete("Use EventHandler._action instead",true)]
		public GPAction Action
		{
			get{ return _action;  }
			set{ _action = value; }
		}

        /// <summary>
        /// Readonly access to the handler state.
        /// </summary>
        public HandlerState State
        {
            get { return m_currState; }
        }

        #endregion

        #region MonoBehaviour

        void Start()
        {
            Init();
        }

        void Update()
        {
            if(_action == null)
                return;

            if(_action.HasEnded)
            {
            	if(HasReachedMaxTriggerCount())
             		m_currState = HandlerState.TERMINATED;
            	else
            		m_currState = HandlerState.SLEEPING;
			
				return;
            }

			if(_action.IsRunning)
				_action.Update();
        }

		void OnDrawGizmos()
		{
			_action.OnDrawGizmos();
		}

		void OnDrawGizmosSelected()
		{
			_action.OnDrawGizmosSelected();
		}

        #endregion

        #region Event Listening

        public void Init()
        {
            if(string.IsNullOrEmpty(_eventName))
                throw new System.Exception("Null event name");

            EventManager.Instance.Register(_eventName, EventTrigger);

			if(_action != null)
				_action.SetParentHandler(this);
        }

        public void EventTrigger(string evtName)
        {
            if (evtName != _eventName || _action == null)
                return;

           if(CanTriggerAction())
               TriggerAction();
        }

        #endregion

        #region Private Utils

        private bool CanTriggerAction()
        {
            if(HasReachedMaxTriggerCount())
                return false;
           
			if (m_usesLockUntilCompletion && _action.IsRunning)
                return false;

            return true;
        }

        private bool HasReachedMaxTriggerCount()
        {
        	// Check if handler kind has fixed count and if current count allows to run one more time.

            return (m_usesFixedCount && m_triggerCount >= _maxTriggerCount);
        }

        private void TriggerAction()
        {
            m_currState = HandlerState.RUNNING;
       
            m_triggerCount++;
            _action.Trigger();
        }

        #endregion
    }
}
