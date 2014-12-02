﻿//
// GPActionCompound.cs
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

namespace Utils.Event
{
	[GPActionHide]
	[System.Serializable]
	public class GPActionCompound : GPAction
	{
		#region Public Members

		/// <summary>
		/// List of GPAction of compound action
		/// </summary>
		public List<GPAction> _actions;

		#endregion

		#region Constructor

		public GPActionCompound()
		{
			_actions = new List<GPAction>();
		}

		#endregion

		#region GPAction Override

		public override void SetParentHandler(EventHandler handler)
		{
			base.SetParentHandler(handler);

			foreach(GPAction action in _actions)
			{
				action.SetParentHandler(handler);
			}
		}

		public override void OnDrawGizmos()
		{
			foreach(GPAction action in _actions)
			{
				action.OnDrawGizmos();
			}
		}
		
		public override void OnDrawGizmosSelected()
		{
			foreach(GPAction action in _actions)
			{
				action.OnDrawGizmosSelected();
			}
		}

		#endregion
	}
}