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
        private Dictionary<string, int> _visits;
        private Tuple<int, int> _distribution;
        private int _count;

        #region constructors
        /// <summary>
        /// AgentProfile Constructors for Agent Entities that 
        /// extend the Profile class with Agent methods
        /// </summary>
        public AgentProfile(string name, Dictionary<string, string> attributes, Dictionary<string, double> propensities, Dictionary<string, int> visits, Tuple<int, int> distribution, int count)
            : base("agent", name, attributes)
        {
            _propensities = propensities;
            _visits = visits;
            _distribution = distribution;
            _count = count;
        }

        /// <summary>
        /// AgentProfile Constructors for Agent Entities that 
        /// extend the Profile class with Agent methods
        /// </summary>
        /// <param name="name"></param>
        public AgentProfile(string name)
            : this(name, new Dictionary<string, string>(),  new Dictionary<string, double>(), new Dictionary<string, int>(), new Tuple<int, int>(0, 1), 1)
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

        /// <summary>
        /// Returns the distribution for this Agent initialization
        /// </summary>
        public Tuple<int,int> Distribution
        {
            get
            {
                return _distribution;
            }
        }

        /// <summary>
        /// Returns the number of visits per node in this Agent's journey
        /// </summary>
        public Dictionary<string, int> Visits
        {
            get
            {
                return _visits;
            }

            set
            {
                _visits = value;
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

            return 0;
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
        /// Return the number of visits an agent should make to a specific Node
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetVisit(string type)
        {
            if (Visits.ContainsKey(type))
            {
                return Visits[type];
            }

            return 0;
        }

        /// <summary>
        /// Add a number of visits to a Node that this Agent should make
        /// </summary>
        /// <param name="type"></param>
        /// <param name="num"></param>
        public void AddVisit(string type, int num)
        {
            Visits[type] = num;
        }
        #endregion

    }
}
