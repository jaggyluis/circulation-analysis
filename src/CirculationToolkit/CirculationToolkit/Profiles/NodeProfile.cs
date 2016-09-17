using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Profiles
{
    class NodeProfile : Profile
    {
        private Tuple<int, int> _distribution;
        private int _capacity;
        private Curve _geometry; // maybe not the best idea to bring Rhino into this file - substitute with area or bounds?

        #region constructors
        /// <summary>
        /// NodeProfile Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        /// <param name="distribution"></param>
        /// <param name="capacity"></param>
        public NodeProfile(string name, Dictionary<string, string> attributes, Tuple<int, int> distribution, int capacity)
            : base ("node", name, attributes)
        {
            _distribution = distribution;
            _capacity = capacity;
        }

        /// <summary>
        /// NodeProfile Constructor
        /// </summary>
        /// <param name="name"></param>
        public NodeProfile(string name)
            : this (name, new Dictionary<string, string>(), new Tuple<int, int>(0, 10), int.MaxValue)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// Returns the amount of time Agent Entities spend at this Node
        /// </summary>
        public Tuple<int, int> Distribution
        {
            get
            {
                return _distribution;
            }

            set
            {
                _distribution = value;
            }
        }

        /// <summary>
        /// Returns the maximum amount of Agents at this location
        /// </summary>
        public int Capacity
        {
            get
            {
                return _capacity;
            }

            set
            {
                _capacity = value;
            }
        }

        /// <summary>
        /// Returns a Geometric representation of this Node
        /// </summary>
        public Curve Geometry
        {
            get
            {
                return _geometry;
            }

            set
            {
                _geometry = value;
            }
        }
        #endregion
    }
}
