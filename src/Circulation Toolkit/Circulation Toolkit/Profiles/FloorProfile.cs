using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Profiles
{
    /// <summary>
    /// FloorProfile sub-class that stores Floor information
    /// fron the Rhino environment
    /// </summary>
    public class FloorProfile : Profile
    {

        public FloorProfile() 
            : base("default", "floor")
        {
        }
        public FloorProfile(string name)
            : base(name, "floor")
        {
        }
    }
}
