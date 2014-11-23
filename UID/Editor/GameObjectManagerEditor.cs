//
// GameObjectManagerEditor.cs
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
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Utils.GameObjectManager))]
public class GameObjectManagerEditor : Editor
{
	#region Private Member

	private bool m_displayObjectMap;

	#endregion

	public override void OnInspectorGUI()
	{
		if(m_displayObjectMap)
		{
			if(GUILayout.Button("Hide Map"))
			{
				m_displayObjectMap = false;
			}
			else
			{
				Dictionary<Utils.UID,GameObject> dict = Utils.GameObjectManager.Instance.ObjectMap;

				foreach(KeyValuePair<Utils.UID,GameObject> kvp in dict)
				{
					EditorGUILayout.ObjectField(kvp.Key.Stamp.ToString("X"),kvp.Value,typeof(GameObject));
				}
			}
		}
		else
		{
			if(GUILayout.Button("Show Map"))
			{
				m_displayObjectMap = true;
			}
		}
	}
}
