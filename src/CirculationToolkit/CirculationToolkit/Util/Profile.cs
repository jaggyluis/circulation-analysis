using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Util
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

        /// <summary>
        /// Profile Constructor that takes a type only
        /// </summary>
        /// <param name="type"></param>
        public Profile(string type)
        {
            Type = type;
            Attributes = new Dictionary<string, string>();
        }
        /// <summary>
        /// Profile Constructor that takes a name and type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public Profile(string type, string name)
        {
            Name = name;
            Type = type;
            Attributes = new Dictionary<string, string>();
        }

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

    public class AgentProfile : Profile
    {
        private Dictionary<string, double> _propensities;

        /// <summary>
        /// AgentProfile Constructor for Agent Entities that 
        /// extends the Profile class with Agent methods
        /// </summary>
        public AgentProfile(string name)
            : base ("agent", name)
        {
            _propensities = new Dictionary<string, double>();
        }

        #region properties
        /// <summary>
        /// Returns the dictionary of key value pairs for Agent Entity
        /// Decision making
        /// </summary>
        public Dictionary<string, double> Propensities
        {
            get
            {
                return _propensities;
            }
        }
        #endregion

        #region utility methods
        /// <summary>
        /// Returns the propensity of an Agent to make a certain decision
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public double GetPropensity(string type)
        {
            if (Propensities.ContainsKey(type))
            {
                return Propensities[type];
            }

            return 1;
        }

        /// <summary>
        /// Adds a propensity to the Agent's profile for decision making
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void AddPropensity(string type, double value)
        {
            Propensities[type] = value;
        }
        #endregion

    }
}