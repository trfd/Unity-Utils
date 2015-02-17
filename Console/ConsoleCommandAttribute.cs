using UnityEngine;
using System.Collections;

namespace Utils
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class ConsoleCommandAttribute : System.Attribute
    {
        public string _name;

        public ConsoleCommandAttribute(string name)
        {
            _name = name;
        }
    }
}
