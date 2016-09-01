using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Profiles
{
    /// <summary>
    /// Main Profile class for storing input information
    /// from the Rhino environment
    /// </summary>
    public class Profile
    {
        public string Name;
        public string Type;
        public Dictionary<string, string> Attributes;

        public Profile(string name, string type)
        {
            Name = name;
            Type = type;
            Attributes = new Dictionary<string, string>();
        }

        public bool HasAttribute (string attribute)
        {
            if (Attributes.ContainsKey(attribute))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasValue (string value)
        {
            if (Attributes.ContainsValue(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
