using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Profiles
{
    /// <summary>
    /// The Environment Profile class overrides and handles general Environment settings
    /// </summary>
    class EnvironmentProfile : Profile
    {
        private Tuple<double, double> _distribution;

        #region constructors
        /// <summary>
        /// Environment Profile Construvtor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="distrubution"></param>
        public EnvironmentProfile(string type, Tuple<double,double> distrubution)
            : base (type)
        {
            _distribution = distrubution;
        }

        /// <summary>
        /// Environment Profile Constructor
        /// </summary>
        /// <param name="distribution"></param>
        public EnvironmentProfile(Tuple<double,double> distribution)
            : this ("environment", distribution)
        {
        }
        /// <summary>
        /// Enviroment Profile Constructor
        /// </summary>
        public EnvironmentProfile()
            : base ("environment")
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// Returns the Environment distribution for agents. This represents the 
        /// cutoff for agent tasks and decisions
        /// </summary>
        public Tuple<double, double> Distribution
        {
            get
            {
                return _distribution;
            }
        }
        #endregion
    }
}
