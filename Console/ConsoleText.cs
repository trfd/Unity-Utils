//
// ConsoleText.cs
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
using System.Text;

namespace Utils
{
    public class ConsoleText
    {
        StringBuilder m_stringBuilder;

        List<int> m_lineIndex;

        public int lineCount
        { get { return m_lineIndex.Count; } }

        public ConsoleText()
        {
            m_stringBuilder = new StringBuilder();
            m_lineIndex = new List<int>();
        }

        // Returns the number of lines
        public int AppendLine(string str,string color)
        {
            string[] lines = str.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                m_stringBuilder.AppendLine("<color="+color+">"+lines[i]+"</color>");
                m_lineIndex.Add(m_stringBuilder.Length);
            }

            return lines.Length;
        }

        public void Clear()
        {
            m_lineIndex.Clear();
            // Clear not implemented in Unity's .NET
            //m_stringBuilder.Clear();
            m_stringBuilder = new StringBuilder();
        }

        public string GetString(int startLine, int lineCnt)
        {
            startLine = Mathf.Max(0, startLine);

            if (startLine >= m_lineIndex.Count)
            {
                Debug.LogError("ConsoleText out of bound request line in GetString");
                return "";
            }
            
            int endIdx;

            if (startLine + lineCnt > m_lineIndex.Count)
            {
                lineCnt = m_lineIndex.Count - startLine;

                endIdx = m_stringBuilder.Length;
            }
            else
            {
                endIdx = m_lineIndex[startLine + lineCnt - 1];
            }

            int charLength = endIdx - m_lineIndex[startLine];

            return m_stringBuilder.ToString(m_lineIndex[startLine], charLength);
        }
    }
}