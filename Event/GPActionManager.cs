//
// GPActionManager.cs
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

using Utils.Reflection;

namespace Utils.Event
{
    public static class GPActionManager 
    {
	    #region Public Static Members

        public static System.Type[] s_gpactionTypes;

        public static string[] s_gpactionTypeNames;

        #endregion

        #region Static Constructor

        static GPActionManager()
        {
            System.Type[] types = TypeManager.Instance.ListChildrenTypesOf(typeof(GPAction),false);

            List<System.Type> visibleTypes = new List<System.Type>();
            List<string> visibleTypeNames = new List<string>();

            foreach (System.Type type in types)
            {
                if(type.GetCustomAttributes(typeof (GPActionHideAttribute), false).Length == 0)
                {
                    visibleTypes.Add(type);
                    visibleTypeNames.Add(type.Name);
                }
            }

            s_gpactionTypes = visibleTypes.ToArray();
            s_gpactionTypeNames = visibleTypeNames.ToArray();
        }

        #endregion

    }
}

