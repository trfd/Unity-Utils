//
// GPConditionCounterInspector.cs
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
using System.Collections;
using System.Collections.Generic;

namespace Utils.Event
{
	[GPConditionInspector(typeof(GPConditionCounter))]
    public class GPConditionCounterInspector : GPConditionInspector
    {
        ValueProviderEditor<int> m_provideEditor;
        ValueComparerEditor<int> m_comparerEditor;


        protected override void OnInspectorGUI()
        {
            GPConditionCounter conditionCounter = (GPConditionCounter) Condition;

            // Provider

            if (m_provideEditor == null)
            {
                m_provideEditor = new ValueProviderEditor<int>();
                m_provideEditor.Provider = conditionCounter._provider;
            }

            if (m_provideEditor.Provider != conditionCounter._provider)
                m_provideEditor.Provider = conditionCounter._provider;


            m_provideEditor.Display();


            // Comparer

            if(m_comparerEditor == null)
                m_comparerEditor = new ValueComparerEditor<int>();

            System.Type newType = m_comparerEditor.Display(conditionCounter._comparer.GetType());

            if (newType != conditionCounter._comparer.GetType())
                conditionCounter._comparer = (IntComparer) System.Activator.CreateInstance(newType);

        }
    }
}
