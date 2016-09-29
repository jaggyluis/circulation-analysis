using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CirculationToolkit.Profiles;
using Rhino.Geometry;
using CirculationToolkit.Environment;

namespace CirculationToolkit.Entities
{
    /// <summary>
    /// Main Node Class that stores goal locations for the Circulation Environment
    /// </summary>
    public class Node : Entity
    {
        private Floor _floor;
        private List<int> _indexes;

        #region constructors
        /// <summary>
        /// Node Entity Constructor that takes a NodeProfile and a Point3d
        /// representing the location of this node
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="geometry"></param>
        public Node(Profile profile, Point3d geometry)
            : base(profile)
        {
            _indexes = new List<int>();

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
        #endregion

        #region properties
        /// <summary>
        /// Returns the Agent capacity at this Node
        /// </summary>
        public int Capacity
        {
            get
            {
                NodeProfile profile = (NodeProfile)Profile;

                int capacity = profile.Capacity;

                if (capacity < 0)
                {
                    capacity = int.MaxValue;
                }

                return capacity;
            }
        }

        /// <summary>
        /// Returns the Floor Entity that this Node is on
        /// </summary>
        public Floor Floor
        {
            get
            {
                return _floor;
            }
        }

        #endregion

        #region utility methods
        /// <summary>
        /// Returns the number of Agents at this Node at a Generation
        /// </summary>
        public int Count(int gen)
        {
            int? index = Floor.GetPointGridIndex(Position);
            int count = 0;

            if (index != null)
            {
                count = Floor.GetOccupancy((int)index, gen);
            }
          
            return count;
        }

        /// <summary>
        /// Returns the Node's Geometry if is has any
        /// </summary>
        public Curve Geometry
        {
            get
            {
                NodeProfile profile = (NodeProfile)Profile;
                
                if (profile.Geometry != null)
                {
                    return profile.Geometry;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns whether this node is a Zone
        /// </summary>
        public bool IsZone
        {
            get
            {
                return Geometry != null;
            }
        }

        /// <summary>
        /// Returns a list of indexes for this node's geometry location on the floor,
        /// if any.
        /// </summary>
        public List<int> Indexes
        {
            get
            {
                return _indexes;
            }
            set
            {
                _indexes = value;
            }
        }
        #endregion

        #region main methods
        /// <summary>
        /// Initialize this Node with an environment
        /// </summary>
        /// <param name="environment"></param>
        public void Init(Floor floor)
        {
            _floor = floor;
        }
        #endregion;
    }
}
