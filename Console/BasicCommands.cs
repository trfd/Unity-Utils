using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Utils
{
    public class BasicCommands : IConsoleCommandHolder
    {
        [ConsoleCommand("mock")]
        public static string Mock(string[] args)
        {
            return "Hello";
        }

        [ConsoleCommand("list")]
        public static string List(string[] args)
        {
            if (args.Length != 2)
                return "list: argument of the form 'list components gameObject' or 'list children gameObject'";

            GameObject obj = GetGameObjectFromExpression(args[1]);

            if (obj == null)
                return "list: object with expression " + args[1] + " not found";

            if(args[0] == "components")
            {
                Component[] comps = obj.GetComponents<Component>();

                string list = "";

                foreach (Component comp in comps)
                    list += comp.GetType().FullName + "\n";

                return list;
            } 
            else if(args[0] == "children")
            {
                string list = "";

                for(int i=0 ; i<obj.transform.childCount ; i++)
                {
                    list += obj.transform.GetChild(i).name + "\n";
                }
                
                return list;
            }

            return "list: unsupported argument " + args[0];
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

            if(bits.Length < 3)
                return "'log' requires an argument of the form gameObjectName.ComponentType/[i]/.member";

            GameObject obj = GetGameObjectFromExpression(bits[0]);

            if(obj == null)
                return "log: object with expression "+bits[0]+" not found";

            Component comp = obj.GetComponent(bits[1]);

            if (comp == null)
                return "log: component of type " + bits[1] + " not found in object " + obj.name;

            ComponentNestedDataMemberWrapper dataMemberWrapper = new ComponentNestedDataMemberWrapper(comp);
            dataMemberWrapper.NestedDataMember.DataMembers = new DataMemberWrapper[0];

            System.Type type = comp.GetType();

            for(int i=2 ; i<bits.Length ; i++)
            {
                FieldInfo field = type.GetField(bits[i]);
                PropertyInfo property = type.GetProperty(bits[i]);

                if (field != null)
                {
                    dataMemberWrapper = dataMemberWrapper.Append(field);
                    type = field.FieldType;
                }
                else if (property != null)
                {
                    dataMemberWrapper = dataMemberWrapper.Append(property);
                    type = property.PropertyType;
                }
                else
                    return "log: field or property " + bits[i] + " not found in " + bits[i - 1];
                    
            }

            return dataMemberWrapper.GetValue().ToString();
        }

        [ConsoleCommand("disable")]
        public static string Disable(string[] args)
        {
            if (args.Length != 1)
                return "'disable' requires a single argument of the form gameObjectName.ComponentType/[i]";

            string[] bits = args[0].Split('.');

            if (bits.Length > 2)
                return "'disable' requires an argument of the form gameObjectName.ComponentType/[i]/";

            GameObject gobj = GetGameObjectFromExpression(bits[0]);

            if(bits.Length == 2)
            {
                Component comp = gobj.GetComponent(bits[1]);

                if (comp == null)
                    return "disable: component of type " + bits[1] + " not found in object " + gobj.name;

                if(!(comp is MonoBehaviour))
                    return "disable: can not disable component "+bits[1]+". Not a MonoBehaviour";

                ((MonoBehaviour)comp).enabled = false;

                return "Disable "+bits[1]+" in "+ gobj.name;
            }
            else
            {
                gobj.SetActive(false);
                return "Disable " + gobj.name;
            }
        }

        [ConsoleCommand("enable")]
        public static string Enable(string[] args)
        {
            if (args.Length != 1)
                return "'disable' requires a single argument of the form gameObjectName.ComponentType/[i]";

            string[] bits = args[0].Split('.');

            if (bits.Length > 2)
                return "'disable' requires an argument of the form gameObjectName.ComponentType/[i]/";

            GameObject gobj = GetGameObjectFromExpression(bits[0]);

            if (bits.Length == 2)
            {
                Component comp = gobj.GetComponent(bits[1]);

                if (comp == null)
                    return "disable: component of type " + bits[1] + " not found in object " + gobj.name;

                if (!(comp is MonoBehaviour))
                    return "disable: can not disable component " + bits[1] + ". Not a MonoBehaviour";

                ((MonoBehaviour)comp).enabled = false;

                return "Disable " + bits[1] + " in " + gobj.name;
            }
            else
            {
                gobj.SetActive(false);
                return "Disable " + gobj.name;
            }
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

            GameObject obj = null;

            foreach(string sExp in singleExp)
            {
                obj = GetGameObjectFromSingleExpression(sExp , obj);
            }

            return obj;
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
                return GetRootGameObjectFromSingleExpression(name, tagSearch, arrayIndex);
            
            return GetChildGameObjectFromSingleExpression(parent,name, tagSearch, arrayIndex);
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
