using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace Utils
{
    public class BasicCommands : IConsoleCommandHolder
    {
        [ConsoleCommand("mock")]
        public static string Mock(string[] args)
        {
            return "Hello";
        }


        // Log the value of a member
        // argument is of the form:
        // log gameObjectName.ComponentType.field
        // log gameObjectName.ComponentType[i].field
        // log gameObjectName.ComponentType.property
        [ConsoleCommand("log")]
        public static string Log(string[] args)
        {
            if (args.Length != 1)
                return "'log' requires a single argument of the form gameObjectName.ComponentType/[i]/.member";

            string[] bits = args[0].Split('.');

            if(bits.Length <= 3)
                return "'log' requires a single argument of the form gameObjectName.ComponentType/[i]/.member";

            GameObject obj 

        }


        /// <summary>
        /// Returns a gameobject access via a console syntax string
        /// </summary>
        /// 
        /// "myObjName" will look for object with the name "myObjName"
        /// "myObjName[i]" returns the ith object with the name "myObjName"
        /// "#myObjTag" returns the first object with tag "myObjectTag"
        /// "#myObjTag[i]" return the i-th object with tag "myObjectTag"
        /// "myObj>myChildObj" returns the first child object of "myObj" named "myChildObj"
        /// tags "#" and list "[]" can be combined with child access ">"
        /// <param name="?"></param>
        /// <returns></returns>
        public static GameObject GetGameObjectFromExpression(string exp)
        {
            string[] singleExp = exp.Split('>');
        }

        /// <summary>
        /// Returns a game object found using a single console syntax expression. 
        /// That is, it doesn't support children access '>'. Moreover it requires a 
        /// research context. If a parent is provided the expression parsing will search
        /// in the children of the specified object, otherwise it will search via
        /// GameObject.Find... methods
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static GameObject GetGameObjectFromSingleExpression(string exp, GameObject parent)
        {
            Regex tagSearchPattern   = new Regex(@"^\#.*");
            Regex arraySearchPattern = new Regex(@"\#?(.*)\[([0-9]*)\]$");
            exp = exp.Trim(' ');

            Match arrayMatch = arraySearchPattern.Match(exp);
            bool tagSearch = tagSearchPattern.IsMatch(exp);
            bool arraySearch = arrayMatch.Success;
            int arrayIndex = 0;
            
            string name = exp;

            if(tagSearch)
                name = name.Replace("$","");

            if(arraySearch)
            {
                arrayIndex = int.Parse(arrayMatch.Groups[1].ToString());
                name = arrayMatch.Groups[0].ToString();
            }

            if (parent == null)
                GetRootGameObjectFromSingleExpression(name, tagSearch, arrayIndex);
            else
                GetChildGameObjectFromSingleExpression(parent,name, tagSearch, arrayIndex);
        }

        private static GameObject GetRootGameObjectFromSingleExpression(string name, bool tagSearch, int arrayIndex)
        {
            if(tagSearch)
            {
                GameObject[] obj = GameObject.FindGameObjectsWithTag(name);

                if (arrayIndex >= obj.Length && arrayIndex < 0)
                    return null;
                return obj[arrayIndex];
            }
            else
                return GameObject.Find(name);
        }

        private static GameObject GetChildGameObjectFromSingleExpression(GameObject parent, string name, bool tagSearch, int arrayIndex)
        {
            List<GameObject> children = new List<GameObject>();

            for (int i = 0; i < parent.transform.childCount ; i++)
            {
                if(tagSearch && parent.transform.GetChild(i).tag == name)
                    children.Add(parent.transform.GetChild(i).gameObject);
                else if(!tagSearch &&  parent.transform.GetChild(i).name == name)
                    children.Add(parent.transform.GetChild(i).gameObject);
            }

            if(arrayIndex < 0 && arrayIndex >= children.Count)
                return null;

            return children[arrayIndex];
        }
    }
}
