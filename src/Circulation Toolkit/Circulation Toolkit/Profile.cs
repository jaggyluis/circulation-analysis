using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Circulation_Toolkit
{
    /// <summary>
    /// Main Profile class for storing input information
    /// from the Rhino environment
    /// </summary>
    class Profile
    {
        public string Name;
        public string Type;
        public Dictionary<string, string> Attributes;

        public Profile(string name, string type)
        {
            this.Name = name;
            this.Type = type;
            this.Attributes = new Dictionary<string, string>();
        }

        public bool HasAttribute (string attribute)
        {
            if (this.Attributes.ContainsKey(attribute))
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
            if (this.Attributes.ContainsValue(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// FloorProfile sub-class that stores Floor information
    /// fron the Rhino environment
    /// </summary>
    class FloorProfile : Profile
    {
        private Curve _boundary;

        public FloorProfile(string name, Curve boundary)
            : base (name, "floor")
        {
            this._boundary = boundary;
        }

        public Curve Boundary
        {
            get
            {
                return this._boundary;
            }
        }
    }
}
