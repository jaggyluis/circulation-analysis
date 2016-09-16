using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Profiles
{
    /// <summary>
    /// Agent profile Extension class that handles Agent profile
    /// information from the Rhino Environment
    /// </summary>
    public class AgentProfile : Profile
    {
        private Dictionary<string, double> _propensities;
        private List<string> _nodes;
        private Tuple<int, int> _distribution;
        private int _count;

        #region constructors
        /// <summary>
        /// AgentProfile Constructors for Agent Entities that 
        /// extend the Profile class with Agent methods
        /// </summary>
        public AgentProfile(string name, Dictionary<string, double> propensities, List<string> nodes, Tuple<int, int> distribution, int count)
            : base("agent", name)
        {
            _propensities = propensities;
            _nodes = nodes;
            _distribution = distribution;
            _count = count;
        }

        public AgentProfile(string name)
            : this(name, new Dictionary<string, double>(), new List<string>(), new Tuple<int, int>(0, 10), 1)
        {
        }
        #endregion

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
            set
            {
                _propensities = value;
            }
        }

        /// <summary>
        /// Returns the list of all portals to visit
        /// </summary>
        public List<string> Nodes
        {
            get
            {
                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }

        /// <summary>
        /// Accessor for the amount of agents of this type to create
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
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

        /// <summary>
        /// Adds a portal to the list of 
        /// </summary>
        /// <param name="portal"></param>
        public void AddNode(string name)
        {
            if (!Nodes.Contains(name))
            {
                Nodes.Add(name);
            }
        }
        #endregion

    }
}
