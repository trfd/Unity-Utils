//
// GPActionMaterialAnimation.cs
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

namespace Utils.Event
{
	[System.Serializable]
	[GPActionAlias("Animation/MaterialAnimation")]
	public class GPActionMaterialAnimation : GPAction 
	{
		public enum FieldType
		{
			NONE,
			COLOR,
			FLOAT
		}

		#region Implementation

		[System.Serializable]
		[GPActionHide]
		public class Impl : GPAction
		{
			#region Protected Members

			/// <summary>
			/// Animation Timer.
			/// </summary>
			protected Timer m_timer;

			#endregion

			#region Public Members

			public GPActionMaterialAnimation _parent;

			#endregion
		}

		/// <summary>
		/// Anim shader color
		/// </summary>
		[System.Serializable]
		[GPActionHide]
		public class ColorImpl : Impl
		{
			#region Public Members

			public Gradient _colorMap;

			public AnimationCurve _curve;

			#endregion

			#region Override Action

			protected override void OnTrigger()
			{
				m_timer = new Timer(_parent._duration);
			}

			protected override void OnUpdate()
			{
				if(m_timer.IsElapsedLoop)
				{
					End();
					m_timer = null;
					return;
				}

				Color c = _colorMap.Evaluate(
					_curve.Evaluate(1f - m_timer.CurrentNormalized));

				_parent.m_material.SetColor(_parent._animatedVariable,c);
			}

			#endregion 
		}

		/// <summary>
		/// Anim shader float
		/// </summary>
		[System.Serializable]
		[GPActionHide]
		public class FloatImpl : Impl
		{
			#region Public Members
			
			public AnimationCurve _curve;
			
			#endregion
			
			#region Override Action
			
			protected override void OnTrigger()
			{
				m_timer = new Timer(_parent._duration);
			}
			
			protected override void OnUpdate()
			{
				if(m_timer.IsElapsedLoop)
				{
					End();
					m_timer = null;
					return;
				}

				_parent.m_material.SetFloat(_parent._animatedVariable,
				                            _curve.Evaluate(1f - m_timer.CurrentNormalized));
			}
			
			#endregion 
		}
		

		#endregion

		#region Private Members

		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private Impl m_impl;

		/// <summary>
		/// The type of animation
		/// </summary>
		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private FieldType m_animationType;

		/// <summary>
		/// Whether or not the animation uses the shared material of this object.
		/// </summary>
		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private bool m_useShared;

		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private bool m_useThis;

		/// <summary>
		/// Material animated.
		/// </summary>
		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private Material m_material;

		#endregion

		#region Public Members

		/// <summary>
		/// Duration of animation.
		/// </summary>
		public float _duration;
		
		/// <summary>
		/// The key of animated variable.
		/// </summary>
		public string _animatedVariable;
	
		#endregion

		#region Properties

		public FieldType AnimationType
		{
			get{ return m_animationType; }
			set
			{
				if(m_animationType == value)
					return;

				m_animationType = value;

				ChangeImpl();
			}
		}

		public Impl Implementation
		{
			get{ return m_impl; }
		}

		public bool UseThisObject
		{
			get{ return m_useThis; }
			set
			{ 
				if(m_useThis == value)
					return;

				m_useThis = value; 

				ChangeMaterial();
			}
		}

		/// <summary>
		/// Whether or not the animation uses the shared material of this object.
		/// </summary>
		public bool UseShared
		{
			get{ return m_useShared; }
			set
			{
				if(m_useShared == value)
					return;

				m_useShared = value;

				ChangeMaterial();
			}
		}

		public Material Material
		{
			get{ return m_material;  }
			set{ m_material = value; CheckMaterial(); }
		}

		#endregion

		#region Action Override

		protected override void OnTrigger()
		{
			m_impl.Trigger();
		}

		protected override void OnUpdate()
		{
			m_impl.Update();
		}

		#endregion

		#region Internal

		private void ChangeImpl()
		{
			m_impl = null;

			System.Type implType;

			switch(m_animationType)
			{
			case FieldType.NONE:
				return;
			case FieldType.COLOR:
				implType = typeof(ColorImpl);
				break;
			case FieldType.FLOAT:
				implType = typeof(FloatImpl);
				break;
			default:
				Debug.LogError("Unsupported type: "+m_animationType);
				return;
			}

			m_impl = (Impl) ScriptableObject.CreateInstance(implType);

			m_impl._parent = this;
			m_impl.ParentHandler = ParentHandler;
		}

		private void ChangeMaterial()
		{
			if(m_useThis)
			{
				Renderer thisRenderer = ParentGameObject.GetComponent<Renderer>();

				if(thisRenderer == null)
					return;

				if(m_useShared)
					m_material = thisRenderer.sharedMaterial;
				else
					m_material = thisRenderer.material;
			}
		}

		private void CheckMaterial()
		{
			Renderer thisRenderer = ParentGameObject.GetComponent<Renderer>();

			if(thisRenderer == null)
			{
				m_useThis = false;
				m_useShared = false;
			}
			else
			{
				if(thisRenderer.sharedMaterial == m_material)
				{
					m_useThis = true;
					m_useShared = false;
				}
				else if(thisRenderer.material == m_material)
				{
					m_useThis = true;
					m_useShared = false;
				}
				else
				{
					m_useThis = false;
					m_useShared = false;
				}
			}
		}

		#endregion
	}
}
