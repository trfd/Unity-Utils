using UnityEngine;
using System.Collections;

namespace Utils
{
    /// Set of simple data structure made for math serialization
    
    [System.Serializable]
    public class SimpleVector2
    {
        #region Public Members

        public float _x; public float _y;

        #endregion

        #region Constructor

        public SimpleVector2(){ _x = 0; _y = 0;}
        public SimpleVector2(float x, float y){ _x = x; _y = y;}

        public SimpleVector2(Vector2 v) { _x = v.x; _y = v.y; }

        #endregion

        #region Cast Operator

        public static implicit operator Vector2(SimpleVector2 sv) { return new Vector2(sv._x,sv._y); }
        public static implicit operator SimpleVector2(Vector2 v) { return new SimpleVector2(v); }

        #endregion
    }

    [System.Serializable]
    public class SimpleVector3
    {
        #region Public Members

        public float _x; public float _y; public float _z;

        #endregion

        #region Constructor

        public SimpleVector3() { _x = 0; _y = 0; _z = 0; }
        public SimpleVector3(float x, float y, float z) { _x = x; _y = y; _z = z; }

        public SimpleVector3(Vector3 v) { _x = v.x; _y = v.y; _z = v.z; }

        #endregion

        #region Cast Operator

        public static implicit operator Vector3(SimpleVector3 sv) { return new Vector3(sv._x, sv._y, sv._z); }
        public static implicit operator SimpleVector3(Vector3 v) { return new SimpleVector3(v); }

        #endregion
    }

    [System.Serializable]
    public class SimpleVector4
    {
        #region Public Members

        public float _x; public float _y; public float _z; public float _w;

        #endregion

        #region Constructor

        public SimpleVector4() { _x = 0; _y = 0; _z = 0; _w = 0; }
        public SimpleVector4(float x, float y, float z, float w) { _x = x; _y = y; _z = z; _w = w; }

        public SimpleVector4(Vector4 v) { _x = v.x; _y = v.y; _z = v.z; _w = v.w; }
        public SimpleVector4(Quaternion q) { _x = q.x; _y = q.y; _z = q.z; _w = q.w; }

        #endregion

        #region Cast Operator

        public static implicit operator Vector4(SimpleVector4 sv) { return new Vector4(sv._x, sv._y, sv._z, sv._w); }
        public static implicit operator SimpleVector4(Vector4 v) { return new SimpleVector4(v); }

        public static implicit operator Quaternion(SimpleVector4 sv) { return new Quaternion(sv._x, sv._y, sv._z, sv._w); }
        public static implicit operator SimpleVector4(Quaternion q) { return new SimpleVector4(q); }

        #endregion
    }
}