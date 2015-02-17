using UnityEngine;
using System.Collections;

namespace Utils
{
    public class BasicCommands : IConsoleCommandHolder
    {
        [ConsoleCommand("mock")]
        public static string Mock(string[] args)
        {
            return "Hello";
        }
    }
}
