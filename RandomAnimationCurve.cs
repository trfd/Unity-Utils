//
// RandomAnimationCurve.cs
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

namespace Utils
{
    [System.Serializable]
    public class RandomAnimationCurve
    {
        #region Private Members

        /// <summary>
        /// Holds whether or not the random curve has been generated
        /// </summary>
        private bool m_curveGenerated;

        #endregion

        #region Public Members

        /// <summary>
        /// Holds whether or not the animation curve is generated.
        /// </summary>
        public bool _usesRandom;

        /// <summary>
        /// Actual animation curve. If uses random this curve is generated
        /// </summary>
        public AnimationCurve _curve;

        /// <summary>
        /// The curve defining the maximum of random range
        /// </summary>
        public AnimationCurve _maxRandomCurve;

        /// <summary>
        /// The curve defining the minimum of random range
        /// </summary>
        public AnimationCurve _minRandomCurve;

        #endregion

        #region Public Interface

        public float Evaluate(float t)
        {
            if (_usesRandom && !m_curveGenerated)
                GenerateCurve();

            return _curve.Evaluate(t);
        }

        [InspectorButton("Generate")]
        public void GenerateCurve()
        {
            m_curveGenerated = true;

            _curve = new AnimationCurve();

            int minCurveLength = _minRandomCurve.length;
            int maxCurveLength = _maxRandomCurve.length;

            for (int i = 0; i < minCurveLength; i++)
            {
                Keyframe key = _minRandomCurve[i];
                _maxRandomCurve.AddKey(key.time, _maxRandomCurve.Evaluate(key.time));
            }

            for (int i = 0; i < maxCurveLength; i++)
            {
                Keyframe key = _maxRandomCurve[i];
                _minRandomCurve.AddKey(key.time, _minRandomCurve.Evaluate(key.time));
            }

            int length = _minRandomCurve.length;

            if (_maxRandomCurve.length != length)
                return;

            for (int i = 0; i < length; i++)
            {
                Keyframe minKey = _minRandomCurve[i];
                Keyframe maxKey = _maxRandomCurve[i];

                float lerpValue = Random.Range(0f, 1f);

                float value = (maxKey.value - minKey.value) * lerpValue + minKey.value;

                float inTangent = (maxKey.inTangent - minKey.inTangent) * lerpValue + minKey.inTangent;
                float outTangent = (maxKey.outTangent - minKey.outTangent) * lerpValue + minKey.outTangent;

                _curve.AddKey(new Keyframe(minKey.time, value, inTangent, outTangent));
            }
        }

        #endregion
    }
}
