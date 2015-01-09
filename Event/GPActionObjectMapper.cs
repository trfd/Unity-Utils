//
// GPActionObjectMapper.cs
//
// Author:
//       Baptiste Dupy <baptiste.dupy@gmail.com>
//
// Copyright (c) 2014 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utils.Event
{
    public class GPActionObjectMapper : MonoBehaviour 
    {
        [System.Serializable]
        public class GPActionObjectMap : DictionaryWrapper<EventHandler,GameObject>
        {}

	    #region Private Members

        /// <summary>
        /// Maps EventHanlder to GameObject. Those game object are GPActionHolderObject
        /// </summary>
        private GPActionObjectMap m_actionObjectMap;

	    #endregion

	    #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GPActionObjectMapper()
        {
            m_actionObjectMap = new GPActionObjectMap();
        }

	    #endregion

        #region Public Interface

        #region Context Menu

        [ContextMenu("Show Children")]
        private void ShowAllActions()
        {
            for (int i = 0; i < this.transform.childCount; ++i)
                this.transform.GetChild(i).gameObject.hideFlags = HideFlags.None;
        }


        [ContextMenu("Hide Children")]
        private void HideAllActions()
        {
            for (int i = 0; i < this.transform.childCount; ++i)
                this.transform.GetChild(i).gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        #endregion

        /// <summary>
        /// Adds an EventHandler to the mapping.
        /// </summary>
        /// <param name="handler"></param>
        public void AddEventHandler(EventHandler handler)
        {
            m_actionObjectMap.Dictionary.Add(handler, CreateGPActionHolderObject(handler));
        }

        /// <summary>
        /// Add an action to the EventHandler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public GPAction AddAction(EventHandler handler,System.Type actionType)
        {
            if (handler == null)
                throw new System.ArgumentNullException();

            if (!typeof(GPAction).IsAssignableFrom(actionType))
                throw new System.ArgumentException("Type 'actionType' must inherit from GPAction");

            GameObject holder;

            if (!m_actionObjectMap.Dictionary.TryGetValue(handler, out holder))
                holder = CreateGPActionHolderObject(handler);

            return (GPAction) holder.AddComponent(actionType);
        }

#if UNITY_EDITOR

        /// <summary>
        /// Imports an action prefab for the specified EventHandler
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="prefab"></param>
        public void ImportGPActionObjectHolderPrefab(EventHandler handler,Object prefab)
        {
            try
            {
                m_actionObjectMap.Dictionary[handler] = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
            }
            catch (KeyNotFoundException) { throw new GPActionHolderObjectNotFoundException();  }
        }

        /// <summary>
        /// Exports the current GPActionHolderObject as action prefab
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public Object ExportGPActionObjectHolderPrefab(EventHandler handler)
        {
            try
            {
                string str = EditorUtility.SaveFilePanel("Export Action","Assets","New Action","prefab");

                if(string.IsNullOrEmpty(str))
                    return null;

                GameObject obj = m_actionObjectMap.Dictionary[handler];

                return PrefabUtility.CreatePrefab(str, obj, ReplacePrefabOptions.ConnectToPrefab);
            }
            catch (KeyNotFoundException) { throw new GPActionHolderObjectNotFoundException(); }
        }

        /// <summary>
        /// Applies the modification of GPActionHolderObject to its prefab if any.
        /// </summary>
        /// <param name="handler"></param>
        public void ApplyGPActionObjectHolderToPrefab(EventHandler handler)
        {
            try
            {
                GameObject obj = m_actionObjectMap.Dictionary[handler];

                Object prefab = PrefabUtility.GetPrefabObject(obj);

                PrefabUtility.ReplacePrefab(obj, prefab);
            }
            catch (KeyNotFoundException) { throw new GPActionHolderObjectNotFoundException(); }
        }

        /// <summary>
        /// Resets all GPActionHolderObject modifications to the prefab values
        /// </summary>
        /// <param name="handler"></param>
        public void ResetGPActionObjectHolderToPrefab(EventHandler handler)
        {
            try
            {
                GameObject obj = m_actionObjectMap.Dictionary[handler];

                Object prefab = PrefabUtility.GetPrefabObject(obj);

                PrefabUtility.ResetToPrefabState(obj);
            }
            catch (KeyNotFoundException) { throw new GPActionHolderObjectNotFoundException(); }
        }

#endif

        #endregion

        #region Protected Interface

        /// <summary>
        /// Create a GPActionHolderObject in the hierarchy
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        protected GameObject CreateGPActionHolderObject(EventHandler handler)
        {
            GameObject holder = new GameObject(handler._eventID.Name);

            InitGPActionHolderObject(holder);

            return holder;
        }

        /// <summary>
        /// Init a GPActionHolderObject properties.
        /// </summary>
        /// <param name="holder"></param>
        protected void InitGPActionHolderObject(GameObject holder)
        {
            holder.transform.parent = this.transform;
            holder.hideFlags = HideFlags.HideInHierarchy;
        }

        #endregion
    }

    public class GPActionHolderObjectNotFoundException : System.Exception
    {
        public GPActionHolderObjectNotFoundException() 
            : base("GPAction Holder object not found")
        {}
    }
}
