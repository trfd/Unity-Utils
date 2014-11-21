using UnityEngine;
using System.Collections;

namespace Utils.Event
{
    [System.Serializable]
    public abstract class GPAction : UnityEngine.ScriptableObject
    {
        public enum ActionState
        {
            NONE,
            RUNNNING,
            TERMINATED
        }

        #region Private Members

        /// <summary>
        /// Current state of action
        /// </summary>
        private ActionState m_currState = ActionState.NONE;

        #endregion 

        #region Properties

        /// <summary>
        /// Returns whether or not the action is currently running
        /// </summary>
        public bool IsRunning
        {
            get { return m_currState == ActionState.RUNNNING; }
        }


        /// <summary>
        /// Returns whether or not the action has ended
        /// </summary>
        public bool HasEnded
        {
            get { return m_currState == ActionState.TERMINATED; }
        }

        /// <summary>
        /// Returns whether or not the action has ended.
        /// </summary>
        public bool HasStarted
        {
            get 
            { return (m_currState == ActionState.RUNNNING || 
                      m_currState == ActionState.TERMINATED) ; 
            }
        }

        #endregion

        #region Public Interface

        public void Trigger()
        {
            m_currState = ActionState.RUNNNING;
            OnTrigger();
        }

        public void Update(float dt)
        {
            if(HasEnded)
            {
                OnDestroy();
                return;
            }
                
            OnUpdate(dt);
        }

        #endregion

        #region Override Interface

        protected virtual void OnTrigger()
        {
        }

        protected virtual void OnUpdate(float dt)
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected void End()
        {
            m_currState = ActionState.TERMINATED;
        }
         
        #endregion
    }
}

