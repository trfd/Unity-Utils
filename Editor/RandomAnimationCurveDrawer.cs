//
// RandomAnimationCurveDrawer.cs
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
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(Utils.RandomAnimationCurve))]
public class RandomAnimationCurveDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                             GUIContent label)
    { 
        SerializedProperty random = property.FindPropertyRelative("_usesRandom");

        if (random.boolValue)
            return base.GetPropertyHeight(property, label) + 3f * (EditorGUIUtility.singleLineHeight);
        else
            return base.GetPropertyHeight(property, label) + 2f * (EditorGUIUtility.singleLineHeight);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.LabelField(position,label);
        position.height = EditorGUIUtility.singleLineHeight;
        position.y += EditorGUIUtility.singleLineHeight;

        EditorGUI.indentLevel++;

        SerializedProperty random = property.FindPropertyRelative("_usesRandom");

        random.boolValue = EditorGUI.ToggleLeft(position,"Use Random", random.boolValue);

        position.y += EditorGUIUtility.singleLineHeight;
        
        if (!random.boolValue)
        {
            SerializedProperty curve = property.FindPropertyRelative("_curve");
            curve.animationCurveValue = EditorGUI.CurveField(position,"Curve", curve.animationCurveValue);
        }
        else
        {
            SerializedProperty mincurve = property.FindPropertyRelative("_minRandomCurve");
            SerializedProperty maxcurve = property.FindPropertyRelative("_maxRandomCurve");

            mincurve.animationCurveValue = EditorGUI.CurveField(position,"Min Curve", mincurve.animationCurveValue);
           
            position.y += EditorGUIUtility.singleLineHeight;

            maxcurve.animationCurveValue = EditorGUI.CurveField(position, "Max Curve", maxcurve.animationCurveValue);
        }

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}
