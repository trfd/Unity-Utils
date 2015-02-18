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
using System.Reflection;
using System.Collections.Generic;

namespace Utils
{
    using Reflection;

    public interface IConsoleCommandHolder
    {
    }

    public class ConsoleCommandManager 
    {
	    /// <summary>
	    /// Singleton instance
	    /// </summary>
	    private static ConsoleCommandManager m_instance;


	    /// <summary>
	    /// Access to singleton's instance
	    /// </summary>
	    protected static ConsoleCommandManager GetInstance()
	    {
		    if(m_instance == null)
			    m_instance = new ConsoleCommandManager();
		
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
	    private Dictionary<string,MethodInfo> m_cmd;

	    /// <summary>
	    /// Constructor.
	    /// Initialize all known commands
	    /// </summary>
	    public ConsoleCommandManager()
	    {
		    RegisterCommands();
	    }

	    /// <summary>
	    /// Process the command inputs sent via Input(string)
	    /// </summary>
	    /// <param name="str">Command Input</param>
	    private void Input_impl(string str)
	    {
		    string[] bits = str.Split(' ');
            string[] args = new string[bits.Length - 1];

            System.Array.Copy(bits, 1, args, 0, args.Length);

		    MethodInfo cmd;

            if (m_cmd.TryGetValue(bits[0].ToLower(), out cmd))
                Console.DisplayMessage((string)cmd.Invoke(null, new object[]{args}));
            else
                Debug.Log("Unknown command: "+str);
	    }

        private void RegisterCommands()
        {
            m_cmd = new Dictionary<string, MethodInfo>();

            System.Type[] holders = TypeManager.Instance.ListChildrenTypesOf(typeof(IConsoleCommandHolder));

            List<MethodInfo> commands = new List<MethodInfo>();

            // Look for every types implmenting IConsoleCommandHolder
            foreach(System.Type type in holders)
            {
                // Get All Static Methods in an IConsoleCommandHolder
                MethodInfo[] allMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                foreach(MethodInfo method in allMethods)
                {
                    // Check if method has attribute ConsoleCommandAttribute.
                    // i.e. a Command Definition
                    ConsoleCommandAttribute[] commandDefs =(ConsoleCommandAttribute[]) method.GetCustomAttributes(typeof(ConsoleCommandAttribute), false);
                    
                    if (commandDefs.Length == 0)
                        continue;

                    // Check if method returns string
                    if(method.ReturnType != typeof(string))
                    {
                        Debug.LogWarning("Console Command " + method.Name + " ( " + method.DeclaringType.FullName + ") must return a string");
                        continue;
                    }

                    // Check if method accepts string[] in parameters
                    ParameterInfo[] allParams = method.GetParameters();
                    if (allParams.Length != 1 || allParams[0].ParameterType != typeof(string[]) )
                    {
                        Debug.LogWarning("Console Command " + method.Name + " (" + method.DeclaringType.FullName + ") must accepts a single parameter of type string[]");
                        continue;
                    }

                    // Foreach definition attribute 
                    // adds to the list of commands
                    foreach(ConsoleCommandAttribute def in commandDefs)
                        m_cmd.Add(def._name, method);
                }
            }

        }

    }
}