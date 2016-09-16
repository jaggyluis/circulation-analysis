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
        private string _name;
        private string _type;
        private Dictionary<string, string> _attributes;

        #region constructors
        /// <summary>
        /// Profile Constructors
        /// </summary>
        /// <param name="type"></param>
        public Profile(string type, string name, Dictionary<string, string> attributes)
        {
            _name = name;
            _type = type;
            _attributes = new Dictionary<string, string>();
        }

        public Profile(string type, string name)
            : this (type, name, new Dictionary<string, string>())
        {
        }

        public Profile(string type)
            : this (type, null, new Dictionary<string, string>())
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// Return the name of the Entity
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Return the Entity Type
        /// </summary>
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// Return the Entity Attributes
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get
            {
                return _attributes;
            }

            set
            {
                _attributes = value;
            }
        }
        #endregion

        #region utiliy methods
        /// <summary>
        /// Returns a boolean of whether the profile has an attribute key
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public bool HasAttribute(string attribute)
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

        /// <summary>
        /// Returns a given attribute from the Attributes, or an empty string
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public string GetAttribute(string attribute)
        {
            if (HasAttribute(attribute))
            {
                return Attributes[attribute];
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Sets the value of an Entity attribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public void SetAttribute(string attribute, string value)
        {
            Attributes[attribute] = value;
        }

        /// <summary>
        /// Returns a boolean of whether the profile has an attribute value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool HasValue(string value)
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
        #endregion
    }
}
