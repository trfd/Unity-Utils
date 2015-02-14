//
// ConsoleCommand.cs
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

public partial class ConsoleCommand 
{

	/// <summary>
	/// Singleton instance
	/// </summary>
	private static ConsoleCommand m_instance;


	/// <summary>
	/// Access to singleton's instance
	/// </summary>
	protected static ConsoleCommand GetInstance()
	{
		if(m_instance == null)
			m_instance = new ConsoleCommand();
		
		return m_instance;
	}

	/// <summary>
	/// Input a command (usualy from the console)
	/// </summary>
	/// <param name="str">Command input. Will be parsed</para>
	public static void Input(string str)
	{
		GetInstance().Input_impl(str);
	}


	/// 
	/// Command object
	///
	delegate void Command(params string[] args);

	/// <summary>
	/// List of registered command
	/// The keys are command's name
	/// </summary>
	private Dictionary<string,Command> m_cmd;

	/// <summary>
	/// Constructor.
	/// Initialize all known commands
	/// </summary>
	public ConsoleCommand()
	{
		m_cmd = new Dictionary<string, Command>();

		//RegisterCommands();
	}

	/// <summary>
	/// Process the command inputs sent via Input(string)
	/// </summary>
	/// <param name="str">Command Input</param>
	private void Input_impl(string str)
	{
		string[] bits = str.Split(' ');

		Command cmd;

		if(m_cmd.TryGetValue( bits[0].ToLower() , out cmd))
			cmd(bits);
		else
			Debug.Log("Unknown command: "+str);
	}
}
