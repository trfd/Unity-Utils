using UnityEngine;
using System.Collections;

namespace Utils
{   
    public class EventTrigger : MonoBehaviour
    {
        #region Public Member

        /// <summary>
        /// Name of event to trigger
        /// </summary>
        public string _eventName;

        #endregion

        #region Trigger Interface

        public void Trigger()
        {
            if (_eventName == "" || _eventName == null)
                throw new System.NullReferenceException("Null event name");

            EventManager.Instance.PlayEvent(_eventName);
        }

        #endregion
    }
}