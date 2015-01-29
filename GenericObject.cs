//
// GenericObject.cs
//
// Author(s):
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


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utils
{
	[System.Serializable]
	public class GenericObject : ISerializationCallbackReceiver
	{
		public enum Type
		{
			UNDEFINED,
			BOOL,
			INT,
			FLOAT,
			ENUM,
			VECTOR2,
			VECTOR3,
			VECTOR4,
			QUATERNION,
			COLOR,
			RECT,
			STRING,
			OBJECT
		}

		#region Private Members

		private System.Object m_value;

		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private Type m_type;

		#endregion

		#region Public Members

		public bool _boolValue;
		public int _intValue;
		public float _floatValue;
		public int _enumIndex;
		public TypeWrapper _enumType;
		public Vector2 _vector2Value;
		public Vector3 _vector3Value;
		public Vector4 _vector4Value;
		public Quaternion _quaternionValue;
		public Color _colorValue;
		public Rect _rectValue;
		public string _stringValue;
		public Object _objectValue;

		#endregion

		#region Properties

		public System.Object Value
		{
			get{ return GetValue(); }
			set{ SetValue(value);   }
		}

		public Type ObjectType
		{
			get{ return m_type;  }
			set{ SetType(value); }
		}

		#endregion

		#region Private Methods

		private void SetType(Type t)
		{
			m_type = t;

			SetValue(m_value);
		}

		private System.Object GetValue()
		{
			switch(m_type)
			{
			case Type.UNDEFINED : return m_value;
			case Type.BOOL:       return _boolValue;
			case Type.INT:        return _intValue;
			case Type.FLOAT:      return _floatValue;
			case Type.ENUM:       return _enumIndex;
			case Type.VECTOR2:    return _vector2Value;
			case Type.VECTOR3:    return _vector3Value;
			case Type.VECTOR4:    return _vector4Value;
			case Type.QUATERNION: return _quaternionValue;
			case Type.COLOR:      return _colorValue;
			case Type.RECT:       return _rectValue;
			case Type.STRING:     return _stringValue;
			case Type.OBJECT:     return _objectValue;
				
			default: return m_value;
			}
		}

		private void SetValue(System.Object value)
		{
			m_value = value;

			if(m_value == null)
				return;

			switch(m_type)
			{
			case Type.UNDEFINED : return;
			case Type.BOOL:       _boolValue       = (bool) m_value; break;
			case Type.INT:        _intValue        = (int) m_value; break;
			case Type.FLOAT:      _floatValue      = (float) m_value; break;
			case Type.ENUM:       _enumIndex 	   = (int) m_value; _enumType = new TypeWrapper(m_value.GetType()); break;
			case Type.VECTOR2:    _vector2Value    = (Vector2) m_value; break;
			case Type.VECTOR3:    _vector3Value    = (Vector3) m_value; break;
			case Type.VECTOR4:    _vector4Value    = (Vector4) m_value; break;
			case Type.QUATERNION: _quaternionValue = (Quaternion) m_value; break;
			case Type.COLOR:      _colorValue      = (Color) m_value; break;
			case Type.RECT:       _rectValue       = (Rect) m_value; break;
			case Type.STRING:     _stringValue     = (string) m_value; break;
			case Type.OBJECT:     _objectValue     = (UnityEngine.Object) m_value; break;

			default:
				Debug.LogWarning("Unsupported type "+m_value.GetType());
				break;
			}
		}

		#endregion

		#region Serialization Callbacks

		public void OnBeforeSerialize()
		{
			SetValue(m_value);
		}

		public void OnAfterDeserialize()
		{
			m_value = GetValue();
		}

		#endregion
	}
}
