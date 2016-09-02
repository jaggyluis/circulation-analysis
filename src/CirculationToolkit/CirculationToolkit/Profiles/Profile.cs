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
    public abstract class Profile
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
        public Profile(string name, string type)
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

    /// <summary>
    /// FloorProfile sub-class that stores Floor Entity information
    /// from the Rhino environment
    /// </summary>
    public class FloorProfile : Profile
    {
        /// <summary>
        /// FloorProfile Constructor that takes a name for this Floor Entity
        /// </summary>
        /// <param name="name"></param>
        public FloorProfile(string name)
            : base(name, "floor")
        {
        }
    }

    /// <summary>
    /// BarrierProfile sub-class that stores Barrier Entity information
    /// from the Rhino environment
    /// </summary>
    public class BarrierProfile : Profile
    {
        /// <summary>
        /// BarrierProfile Constructor that takes a name for the Floor Entity
        /// that this Barrier Entity is on
        /// </summary>
        /// <param name="floorName"></param>
        public BarrierProfile(string floorName)
            : base("barrier")
        {
            SetAttribute("floor", floorName);
        }
    }

    /// <summary>
    /// NodeProfile sub-class that stores Node Entity information
    /// from the Rhino environment
    /// </summary>
    public class NodeProfile : Profile
    {
        /// <summary>
        /// NodeProfile Constructor that takes a name for this Node Entity and a name
        /// for the Floor Entity that it is on
        /// </summary>
        /// <param name="name"></param>
        /// <param name="floorName"></param>
        public NodeProfile(string name, string floorName)
            : base(name, "node")
        {
            SetAttribute("floor", floorName);
        }
    }

    /// <summary>
    /// TemplateProfile sub-class that stores Template Entity information
    /// </summary>
    public class TemplateProfile : Profile
    {
        /// <summary>
        /// TemplateProfile Constructor that takes no input but initializes attributes
        /// later
        /// </summary>
        public TemplateProfile()
            : base("template")
        {

        }
    }

    /// <summary>
    /// AgentProfile sub-class that stores Agent Entity attributes
    /// </summary>
    public class AgentProfile : Profile
    {
        /// <summary>
        /// AgentProfile Constructor that takes a name for this Agent Entity and a name
        /// for the Floor Entity that it is on
        /// </summary>
        /// <param name="name"></param>
        /// <param name="floorName"></param>
        public AgentProfile(string name, string floorName)
            : base(name, "agent")
        {
            SetAttribute("floor", floorName);
        }
    }
}
