//
// GPActionMaterialAnimationInspector.cs
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

namespace Utils.Event
{
	[GPActionInspectorAttribute(typeof(GPActionMaterialAnimation))]
	public class GPActionMaterialAnimationInspector : GPActionDefaultInspector
    {
        #region Private Members

        private GPActionInspector m_implInspector;

        #endregion

        protected override void OnInspectorGUI()
		{
			GPActionMaterialAnimation anim = (GPActionMaterialAnimation) TargetAction;

			anim.UseThisObject = EditorGUILayout.Toggle("Use This Object",anim.UseThisObject);


			anim.Material = (Material) EditorGUILayout.ObjectField("Material",anim.Material,typeof(Material),true);

			anim._duration = EditorGUILayout.FloatField("Duration",anim._duration);

			anim._animatedVariable = EditorGUILayout.TextField("Property",anim._animatedVariable);

			if(anim.Material != null && !anim.Material.HasProperty(anim._animatedVariable))
			{
				EditorGUILayout.HelpBox("Property "+anim._animatedVariable+" is not in Material "+anim.Material.name,MessageType.Error);
			}

            GPActionMaterialAnimation.FieldType prevAnimType = anim.AnimationType;
            anim.AnimationType = (GPActionMaterialAnimation.FieldType)EditorGUILayout.EnumPopup("Type", prevAnimType);

            if(prevAnimType != anim.AnimationType || m_implInspector == null)
            {
                CreateImplInspector();
            }

            if (m_implInspector == null)
                EditorGUILayout.LabelField("Null implementation");
            else
                m_implInspector.DrawInspectorSimple();

			if(anim.UseThisObject && anim.ParentGameObject.GetComponent<Renderer>())
			{
				EditorGUILayout.HelpBox("'Use This Action' requires a Renderer in the GameObject",MessageType.Error);
			}
		}

	    private void CreateImplInspector()
	    {
            GPActionMaterialAnimation anim = (GPActionMaterialAnimation)TargetAction;

			if(anim.Implementation == null)
				return;

            System.Type inspectorType = GPActionInspectorManager.InspectorTypeForAction(anim.Implementation);

            m_implInspector = (GPActionInspector)System.Activator.CreateInstance(inspectorType);

            if (m_implInspector == null)
                throw new System.NullReferenceException();

	        m_implInspector.TargetAction = anim.Implementation;

	        m_implInspector.HideNameField = true;
	    }
	}
}
