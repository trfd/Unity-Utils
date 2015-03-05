//
// BehaviourEditor.cs
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

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;

[CustomEditor(typeof(UnityEngine.MonoBehaviour),true),CanEditMultipleObjects]
public class BehaviourEditor : UnityEditor.Editor 
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

	    foreach(UnityEngine.Object obj in targets)
	    {
            if (!(obj is MonoBehaviour))
            {
                Debug.LogWarning("Object "+obj.name +" of type " + obj.GetType().FullName+" found in BehaviourEditor's targets");
                continue;
            }

            MonoBehaviour beh = (MonoBehaviour)obj;

            EditorUtils.DrawMethodGUIButton(beh);
            EditorUtils.DrawMemberValue(beh);
	    }
	}
}
