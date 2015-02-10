using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utils
{
    [System.Serializable]
    public class ObjectFilter
    {
        public enum Method
        {
            NONE,
            ALL,
            OBJECTS,
            TAGS
        }

        #region Private Members

        public Method _filterMethod;
        public List<GameObject> _objects;
        public List<string> _tags;

        #endregion

        public bool IsValid(GameObject obj)
        {
            switch(_filterMethod)
            {
                case Method.ALL: return true;
                case Method.OBJECTS: return _objects.Contains(obj);
                case Method.TAGS: return _tags.Contains(obj.tag);
                default: return false;
            }
        }
    }
}

