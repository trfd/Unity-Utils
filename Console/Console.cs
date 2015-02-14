//
// Console.cs
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

public class Console : MonoBehaviour 
{
	private static Console m_instance;

	public static Console Instance
	{ get{ return m_instance;}}


	// Settings
	public Font _consoleFont;
	private static GUIStyle s_textAreaStyle;
	private static GUIStyle s_textFieldStyle;

	public string output = "";
	public string stack = "";

	public bool logExtern = true;

	public bool m_active = false;
	public bool m_ignoreInputs = false;  

	private ConsoleText m_ctext;

	public int _displaySize = 5;

	public static bool isActive
	{get{return m_instance.m_active;}
	 set{m_instance.m_active = value;}}

	private string m_consoleInput = "";
	private float m_consoleScrollValue;
	private float m_consoleScrollSize;
	private int m_currentLine = 0;

	Dictionary<string,string> m_errorStack;

	void Awake()
	{
		if(m_instance == null)
		{
			m_instance = this;
			Init();
		}
		else
			Destroy(this.gameObject);
	}

	void Init()
	{
		m_ctext = new ConsoleText();
		m_errorStack = new Dictionary<string, string>();

		GameObject.DontDestroyOnLoad(this.gameObject);
	}

	public void Clear()
	{
		m_ctext.Clear();
		m_errorStack.Clear();
		m_consoleScrollValue = 1f;
		m_consoleScrollSize = 1f;
		m_currentLine = 0;
	}

	void InitGUI()
	{	
		s_textAreaStyle = new GUIStyle(GUI.skin.textArea);
		s_textAreaStyle.richText = true;
		s_textAreaStyle.font = _consoleFont;
		
		s_textFieldStyle = new GUIStyle(GUI.skin.textField);
		s_textFieldStyle.font = _consoleFont;
	}

	void OnEnable() 
	{
		Application.RegisterLogCallback(HandleLog);
	}
	
	void OnDisable() 
	{
		Application.RegisterLogCallback(null);
	}

	void HandleLog(string logString, string stackTrace, LogType type) 
	{
		string prefix;

		string uid = "NoUID";

		switch(type)
		{
		case LogType.Error:
		{
		 	uid = GetUID();
			prefix = "[Error:"+uid+"]";
			m_errorStack.Add(uid,stackTrace);
			break;
		}
		case LogType.Exception:
		{
			uid = GetUID();
			prefix = "[Except.:"+uid+"]";
			m_errorStack.Add(uid,stackTrace);
			break;
		}
		case LogType.Warning:
			prefix = "[WARNING]";
			break;

		case LogType.Assert:
			prefix = "[ASSERT]";
			break;

		case LogType.Log:
			prefix = "[LOG]";
			break;

		default:
			prefix = "";
			break;
		}

		m_ctext.AppendLine("<color="+GetColor(type)+">"+prefix+logString+"</color>");
	}

	string GetUID()
	{
		int rand =  Random.Range(0,16)        |
				   (Random.Range(0,16) << 4 ) |
				   (Random.Range(0,16) << 8 ) |
				   (Random.Range(0,16) << 12);

		return rand.ToString("X4");
	}

	string GetColor(LogType logtype)
	{
		switch(logtype)
		{
		case LogType.Error:
			return "#ff0000ff";
		case LogType.Log:
			return "#ffffffff";
		case LogType.Exception:
			return "#00ffffff";
		case LogType.Warning:
			return "#ffa500ff";
		case LogType.Assert:
			return "#008080ff";
		}

		return "#ff00ffff";
	}

	void Update()
	{
		if(Input.GetKey(KeyCode.F1) && !m_ignoreInputs)
		{
			m_active = !m_active;
			m_ignoreInputs = true;
		}
		else if( Input.GetKeyUp(KeyCode.F1))
			m_ignoreInputs = false;
	}

	void OnGUI() 
	{
		if(!m_active)
			return;

		if(s_textAreaStyle == null || s_textFieldStyle == null)
			InitGUI();


		GUILayout.BeginArea(new Rect(0,0, Camera.main.pixelWidth, 200));

		GUI.SetNextControlName("ConsoleInput");
		m_consoleInput = GUILayout.TextField(m_consoleInput,s_textFieldStyle,GUILayout.Width(Camera.main.pixelWidth-10));

		GUILayout.BeginHorizontal();

		m_consoleScrollSize = Mathf.Min(1f, (float)_displaySize / (float)m_ctext.lineCount);
		m_consoleScrollSize = Mathf.Max(0.01f, m_consoleScrollSize);


		GUILayout.TextArea(GetConsoleDisplay(m_consoleScrollValue),s_textAreaStyle,
		                   GUILayout.Height(110),GUILayout.MaxWidth(Camera.main.pixelWidth-20));
	
		m_consoleScrollValue = GUILayout.VerticalScrollbar(m_consoleScrollValue,m_consoleScrollSize,0f,1f,
		                                                   GUILayout.Height(110));

		GUILayout.EndHorizontal();

		GUILayout.EndArea();


		if(GUI.GetNameOfFocusedControl() == "ConsoleInput")
		{
			GUIFocusOnConsoleInput();
		}
	}

	void GUIFocusOnConsoleInput()
	{
		Event ev = Event.current;

		if(!ev.isKey)
			return;

		if(ev.keyCode == KeyCode.UpArrow)
			SetScrollToLine( m_currentLine - 1);
		else if(ev.keyCode == KeyCode.DownArrow)
			SetScrollToLine( m_currentLine + 1);
		else if(ev.keyCode == KeyCode.Return)
		{
			ConsoleCommand.Input(m_consoleInput);
			m_consoleInput = "";
		}
	}

	void SetScrollToLine(int line)
	{
		float fline = (float) line/(float)(m_ctext.lineCount-_displaySize);
	
		m_consoleScrollValue = Mathf.Clamp01(fline);
	}

	string GetConsoleDisplay(float displayStart)
	{
		int startLine = Mathf.RoundToInt((m_ctext.lineCount - _displaySize) * displayStart);

		m_currentLine = Mathf.Max(0, startLine);

		return m_ctext.GetString(m_currentLine,_displaySize);
	}
}
