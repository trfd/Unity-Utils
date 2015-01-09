//
// Log.cs
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

#define HTML_LOGGER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

public class Log
{
	public enum MessageType
	{
		INFO,
		DEBUG_INFO,
		ERROR,
		WARNING,
		EXCEPTION
	}

	public enum Category
	{
		NONE = 0,

		AI,
		AUDIO,
		CLIENT,
		FRAMEWORK,
		GAMELOGIC,
		GRAPHICS,
		GUI,
		INPUT,
		NETWORK,
		PHYSICS,
		SERVER,
		SYSTEM
	}

	#region Private Members

	private static string ms_logPath = System.IO.Path.Combine(Application.streamingAssetsPath, "log.html");

	[System.ThreadStatic]
	private static Stopwatch ms_watch;

	private static ILogger ms_logger;

	#endregion

	#region Public Interface

	public static void Error(string msg,Category cat = Category.NONE)
	{
		ToLogger(MessageType.ERROR,msg,cat);
	}

	public static void Debug(string msg,Category cat = Category.NONE)
	{
		ToLogger(MessageType.DEBUG_INFO,msg,cat);
	}

	public static void Warning(string msg,Category cat = Category.NONE)
	{
		ToLogger(MessageType.WARNING,msg,cat);
	}

	public static void Info(string msg,Category cat = Category.NONE)
	{
		ToLogger(MessageType.INFO,msg,cat);
	}

	public static void Clear()
	{
		ms_logger.Clear();
	}

	#endregion

	#region Private Interface

	static Log()
	{
#if HTML_LOGGER
		if (ms_logPath.Contains("://")) 
			ms_logger = new UnityLogger();
		else
			ms_logger = new HTMLLogger(ms_logPath);
#else
		ms_logger = new UnityLogger();
#endif
		ms_watch = new Stopwatch();
	}

	private static void ToLogger(Log.MessageType type, string msg, Category cat)
	{
		if(msg == null)
			msg = "## NULL ##";
		else if (msg == "")
			msg = "## EMPTY ##";

		// Stack Trace

		// Note that the first stack frame return is the third (index 2)
		// since there is two intermediate calls

		StackTrace st = new StackTrace(true);

		StackFrame frame = st.GetFrame(2);

		string firstStackFrame = frame.GetFileName()+"("+frame.GetFileLineNumber()+"): "+frame.GetMethod();
		string stackTrace = "";

		for(int i=2; i< st.FrameCount; i++ )
		{
			frame = st.GetFrame(i);
			stackTrace += frame.GetFileName()+"("+frame.GetFileLineNumber()+"): "+frame.GetMethod()+"\n";
		}

		// Logging

		ms_logger.Log(type,msg,stackTrace,firstStackFrame,cat,TimeString(),ThreadString());
	}

	internal static string TimeString()
	{
		// Time
		if(ms_watch == null)
			ms_watch = new Stopwatch();
		else
			ms_watch.Stop();
		
		string time = System.DateTime.Now.ToString("hh:mm:ss.fff")+" ["+ms_watch.ElapsedMilliseconds+"ms]";
		
		ms_watch.Reset();
		ms_watch.Start();

		return time;
	}

	internal static string ThreadString()
	{
		return System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
	}

	#endregion
}

internal interface ILogger
{
	void Clear();
	void Log(Log.MessageType type, string msg, string stackTrace, string firstStackFrame , Log.Category category, string time, string thread);
}

internal class UnityLogger : ILogger
{
	public void Clear()
	{
	}

	public void Log(Log.MessageType type, string msg, string stackTrace, string firstStackFrame , Log.Category category, string time, string thread)
	{
		switch(type)
		{
		case global::Log.MessageType.DEBUG_INFO:
			UnityEngine.Debug.Log("["+category+"]"+msg+"\n{ "+firstStackFrame+" }");
			break;
		case global::Log.MessageType.EXCEPTION:
		case global::Log.MessageType.ERROR:
			UnityEngine.Debug.LogError("["+category+"]"+msg+"\n{ "+firstStackFrame+" }");
			break;
		case global::Log.MessageType.WARNING:
			UnityEngine.Debug.LogWarning("["+category+"]"+msg+"\n{ "+firstStackFrame+" }");
			break;
		}
	}
}

internal class HTMLLogger : ILogger
{
	#region Private Members
	
	private static object ms_lockObject = new object();

	private StreamWriter m_file;

	private int m_entry = 0;

	#endregion

	public HTMLLogger(string path)
	{
		Application.RegisterLogCallback(UnityLogCallback);

		lock(ms_lockObject)
			m_file = new StreamWriter(path,false);

		Clear();
	}

	public void Clear()
	{
		lock(ms_lockObject)
		{	
			m_file.Write("<head><link href=\"http://maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css\" rel=\"stylesheet\">"+
			             "<script type=\"text/JavaScript\">"+
			             "function timedRefresh(timeoutPeriod) {"+
			             "setTimeout(\"location.reload(true);\",timeoutPeriod);"+
			             "} function scrolldown(){"+
			             "window.scrollTo(0,document.body.scrollHeight);"+
			             "}</script>"+
			             "<script language=\"javascript\" src=\"log/log_js\"></script>"+
			             "<link rel=\"stylesheet\" type=\"text/css\" href=\"log/log.css\"></head>");
			
			m_file.Write("<div class=\"Header\">"+
			             "<INPUT TYPE=\"BUTTON\" VALUE=\"GUI\"           CLASS=\"GUI BUTTON\"           ONCLICK=\"HIDE_CLASS('GUI')\"          >"+
			             "<INPUT TYPE=\"BUTTON\" VALUE=\"PHYSICS\"       CLASS=\"PHYSICS BUTTON\"       ONCLICK=\"HIDE_CLASS('PHYSICS')\"      >"+
			             "<INPUT TYPE=\"BUTTON\" VALUE=\"GRAPHICS\"      CLASS=\"GRAPHICS BUTTON\"      ONCLICK=\"HIDE_CLASS('GRAPHICS')\"     >"+
			             "<INPUT TYPE=\"BUTTON\" VALUE=\"AUDIO\"         CLASS=\"AUDIO BUTTON\"         ONCLICK=\"HIDE_CLASS('AUDIO')\"        >"+
			             "<INPUT TYPE=\"BUTTON\" VALUE=\"NETWORKING\"    CLASS=\"NETWORKING BUTTON\"    ONCLICK=\"HIDE_CLASS('NETWORKING')\"   >"+
			             "<INPUT TYPE=\"BUTTON\" VALUE=\"NETWORKSERVER\" CLASS=\"NETWORKSERVER BUTTON\" ONCLICK=\"HIDE_CLASS('NETWORKSERVER')\">"+
			             "<INPUT TYPE=\"BUTTON\" VALUE=\"NETWORKCLIENT\" CLASS=\"NETWORKCLIENT BUTTON\" ONCLICK=\"HIDE_CLASS('NETWORKCLIENT')\">"+
			             "<INPUT TYPE=\"BUTTON\" VALUE=\"GAMELOGIC\"     CLASS=\"GAMELOGIC BUTTON\"     ONCLICK=\"HIDE_CLASS('GAMELOGIC')\"    >"+
			             "<INPUT TYPE=\"BUTTON\" VALUE=\"AI\"            CLASS=\"AI BUTTON\"            ONCLICK=\"HIDE_CLASS('AI')\"           >"+
			             "<INPUT TYPE=\"BUTTON\" VALUE=\"SYSTEM\"        CLASS=\"SYSTEM BUTTON\"        ONCLICK=\"HIDE_CLASS('SYSTEM')\"       >"+
			             "<INPUT TYPE=\"BUTTON\" VALUE=\"INPUT\"         CLASS=\"INPUT BUTTON\"         ONCLICK=\"HIDE_CLASS('INPUT')\"        >"+
			             "<br></div>\n");

			m_file.Write("<h1 onload=\"JavaScript:timedRefresh(5000); scrolldown(); \">"+System.DateTime.Now.ToString("hh:mm:ss.fff")+"</h1>");
		}
	}

	public void UnityLogCallback(string logString, string stackTrace, LogType type)
	{
		Log.MessageType msgType;

		switch(type)
		{
		case LogType.Assert:
		case LogType.Log:
			msgType = global::Log.MessageType.INFO;
			break;
		case LogType.Error:
		case LogType.Exception:
			msgType = global::Log.MessageType.ERROR;
			break;
		case LogType.Warning:
			msgType = global::Log.MessageType.WARNING;
			break;
		default:
			msgType = global::Log.MessageType.DEBUG_INFO;
			break;
		}

		Log (msgType,logString,stackTrace,"",global::Log.Category.NONE, global::Log.TimeString(), global::Log.ThreadString());
	}

	public void Log(Log.MessageType type, string msg, string stackTrace, string firstStackFrame , Log.Category category,string time, string thread)
	{
		string icon = "";

		switch(type)
		{
		case global::Log.MessageType.INFO:
			icon = "<i class=\"fa fa-comment-o fa-1x\"></i>";
			break;
		case global::Log.MessageType.WARNING:
			icon = "<i class=\"fa fa-exclamation-triangle fa-1x\"></i>";
			break;
		case global::Log.MessageType.ERROR:
			icon = "<i class=\"fa fa-times-circle fa-1x\"></i>";
			break;
		case global::Log.MessageType.DEBUG_INFO:
			icon = "<i class=\"fa fa-bug fa-1x\"></i>";
			break;
		}


		lock(ms_lockObject)
		{
			string shortMsg = "";

			int lineIdx = -1;
		
			if((lineIdx = msg.IndexOf('\n')) > 0)
				shortMsg = msg.Substring(0,lineIdx)+"...";
			else if(msg.Length > 100)
				shortMsg = msg.Substring(0,100)+"...";
			
			m_file.Write("<p class=\""+category+"\"><span class=\"time\">"+time+"</span>" +
			             "<a onclick=\"hide('trace"+m_entry+"')\">TRACE</a>");

			// replace returns

			if(!string.IsNullOrEmpty(shortMsg))
			{
				m_file.Write("<a onclick=\"hide('full"+m_entry+"')\">More</a>");
				m_file.Write(icon+shortMsg);
				m_file.Write("<span class=\"sub-info\">[Thread: "+thread+"] "+firstStackFrame+"</span></p>");
				m_file.Write("<p class=\"full-msg "+category+"\" id=\"full"+m_entry+"\">"+msg.Replace("\n","<br/>")+"</p>");
			}
			else
			{
				m_file.Write(icon+msg);
				m_file.Write("<span class=\"sub-info\">[Thread: "+thread+"] "+firstStackFrame+"</span></p>");
			}

			m_file.Write("<pre class=\"stack-trace\" id=\"trace"+m_entry+"\">"+stackTrace+"</pre>");

			m_file.Flush();

			m_entry++;
		}
	}
}
