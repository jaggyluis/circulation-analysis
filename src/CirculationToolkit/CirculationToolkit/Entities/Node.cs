using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CirculationToolkit.Profiles;
using CirculationToolkit.Util;
using Rhino.Geometry;


namespace CirculationToolkit.Entities
{
    /// <summary>
    /// Main Node Class that stores goal locations for the Circulation Environment
    /// </summary>
    public class Node : Entity
    {
        /// <summary>
        /// Node Entity Constructor that takes a NodeProfile and a Point3d
        /// representing the location of this node
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="geometry"></param>
        public Node(NodeProfile profile, Point3d geometry)
            : base(profile)
        {
            Position = geometry;
        }
    }
}
