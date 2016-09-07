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
        public Node(Profile profile, Point3d geometry)
            : base(profile)
        {
            Position = geometry;
        }

        /// <summary>
        /// Duplicate this Node
        /// </summary>
        /// <returns></returns>
        public override Entity Duplicate()
        {
            return new Node(Profile, Position);
        }

        #region properties
        /// <summary>
        /// Returns the Agent capacity at this Node
        /// </summary>
        public int Capacity
        {
            get
            {
                int capacity;
                bool parsed = Int32.TryParse(GetAttribute("capacity"), out capacity);

                if (!parsed)
                {
                    capacity = int.MaxValue;
                }

                return capacity;
            }
        }
        #endregion
    }
}
