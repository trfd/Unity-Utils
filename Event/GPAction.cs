using UnityEngine;
using System.Collections;

namespace Utils.Event
{
    [System.Serializable]
    public class GPAction : UnityEngine.MonoBehaviour
    {
        public enum ActionState
        {
            NONE,
            RUNNNING,
            TERMINATED
        }

        #region Private Members

		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private EventHandler m_parentHandler;

        /// <summary>
        /// Current state of action
        /// </summary>
        private ActionState m_currState = ActionState.NONE;

        #endregion 

		#region Public Members

		public string _name;

		#endregion

        #region Properties

		public string EditionName
		{
			get; set;
		}

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

		/// <summary>
		/// Parent game object
		/// </summary>
		/// <value>The parent game object.</value>
		public GameObject ParentGameObject
		{
			get{ return m_parentHandler.gameObject; }
		}

		/// <summary>
		/// Parent event handler.
		/// </summary>
		/// <value>The parent handler.</value>
		public EventHandler ParentHandler
		{
			get{ return m_parentHandler; }
			set
			{
				SetParentHandler(value);
			}
		}

        #endregion

		#region Constructor

		public GPAction()
		{
		}

		#endregion

        #region Public Interface

        public void Trigger()
        {
            m_currState = ActionState.RUNNNING;
            OnTrigger();
        }

        public void Update()
        {
            if(HasEnded)
                return;
                
            OnUpdate();
        }

		/// <summary>
		/// Interrupt action
		/// </summary>
		public void Stop()
		{
			if(m_currState == ActionState.RUNNNING)
				OnInterrupt();
		}

        #endregion

        #region Override Interface

		public virtual void SetParentHandler(EventHandler handler)
		{
			m_parentHandler = handler;
		}

		/// <summary>
		/// Raised each time action is triggered
		/// </summary>
        protected virtual void OnTrigger()
        {
        }

		/// <summary>
		/// Raised each frame while action is running.
		/// Calling GPAction.End or GPAction.Stop will stop updates.
		/// </summary>
		/// <param name="dt">Dt.</param>
        protected virtual void OnUpdate()
        {
        }

		/// <summary>
		/// Raised when GPAction.Stop is called.
		/// </summary>
		protected virtual void OnInterrupt()
		{
		}

		/// <summary>
		/// Raised when action ended (typically when GPAction.End is called)
		/// </summary>
		protected virtual void OnTerminate()
		{
		}

		public virtual void OnDrawGizmos()
		{
		}

		public virtual void OnDrawGizmosSelected()
		{
		}

		/// <summary>
		/// Should be called by subclass to terminate action.
		/// </summary>
        protected virtual void End()
        {
            m_currState = ActionState.TERMINATED;
			OnTerminate();
        }
         
        #endregion

		#region MonoBehaviour

		void OnValidate()
		{
			hideFlags = HideFlags.HideInInspector;
			this.enabled = false;
		}


		#endregion
    }

    public static class GPActionUtils
    {
        #region Static Members

        /// <summary>
        /// Name used to designate the GPActionObject (that is the object holding GPAction)
        /// </summary>
        public static const string c_ActionObjectName = "__ActionObject__";

        #endregion

        #region Interface

        /// <summary>
        /// Creates a default GPActionObject if not existing.
        /// Returns the created GameObject or the existing GPActionObject
        /// </summary>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        public static GameObject CreateActionObject(GameObject parentObj)
        {
            GameObject gpaObj = GetGPActionObject(parentObj);

            if (gpaObj != null)
                return gpaObj;

            gpaObj = new GameObject(c_ActionObjectName);

            InitGPActionObject(parentObj,gpaObj);

            return gpaObj;
        }

        /// <summary>
        /// Init a GameObject owner GPAction (a so-called "GPActionObject").
        /// That is, the object is hid in the hierarchy and parent transform is 
        /// set to parent gameobject
        /// </summary>
        /// <param name="gpactionParent"></param>
        /// <param name="gpactionObj"></param>
        public static void InitGPActionObject(GameObject gpactionParent, GameObject gpactionObj)
        {
            gpactionObj.hideFlags = HideFlags.HideInHierarchy;

            gpactionObj.transform.parent = gpactionParent.transform;
        }

        public static void DestroyActionObject(GameObject parentObj)
        {
            GameObject gpaObj = GetGPActionObject(parentObj);

            if (gpaObj == null)
                return;

            GameObject.Destroy(gpaObj);
        }

        public static bool HasGPActionObject(GameObject parentObj)
        {
            return (GetGPActionObject(parentObj) != null);
        }

        public static GameObject GetGPActionObject(GameObject parentObj)
        {
            GameObject gpactionObj = null;

            for (int i = 0; i < parentObj.transform.childCount; i++)
                if (parentObj.transform.GetChild(i).gameObject.name == c_ActionObjectName)
                    gpactionObj = parentObj.transform.GetChild(i).gameObject;

            return gpactionObj;
        }

      

        #endregion
    }
}

