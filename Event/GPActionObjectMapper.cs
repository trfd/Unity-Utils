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

        private GPActionObjectMap m_actionObjectMap;

	    #endregion

	    #region Public Members

        

	    #endregion

	    #region Properties

	    #endregion

	    #region Constructors

        public GPActionObjectMapper()
        {
            m_actionObjectMap = new GPActionObjectMap();
        }

	    #endregion

        #region Public Interface
        
        public void AddEventHandler(EventHandler handler)
        {
            m_actionObjectMap.Dictionary.Add(handler, CreateGPActionHolderObject(handler));
        }

        public T AddAction<T>(EventHandler handler) where T : GPAction
        {
            if (handler == null)
                throw new System.ArgumentNullException();

            GameObject holder;

            if (!m_actionObjectMap.Dictionary.TryGetValue(handler, out holder))
                throw new GPActionHolderObjectNotFoundException();

            return holder.AddComponent<T>();
        }

#if UNITY_EDITOR

        public void ImportGPActionObjectHolderPrefab(EventHandler handler,Object prefab)
        {
            try
            {
                m_actionObjectMap.Dictionary[handler] = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
            }
            catch (KeyNotFoundException) { throw new GPActionHolderObjectNotFoundException();  }
        }

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

        public void ApplyGPActionObjectHolderToPrefab()

#endif

        #endregion

        #region Protected Interface

        protected GameObject CreateGPActionHolderObject(EventHandler handler)
        {
            GameObject holder = new GameObject(handler._eventID.Name);

            InitGPActionHolderObject(holder);

            return holder;
        }

        protected GameObject CreateGPActionHolderObject(EventHandler handler, GameObject prefab)
        {
            GameObject holder = (GameObject) GameObject.Instantiate(prefab);

            InitGPActionHolderObject(holder);

            return holder;
        }

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
