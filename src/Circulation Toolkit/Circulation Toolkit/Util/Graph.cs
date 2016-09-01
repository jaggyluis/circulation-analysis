using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CirculationToolkit.Entities;

namespace CirculationToolkit.Util
{
    /// <summary>
    /// Graph Class that handles abstract connections
    /// </summary>
    public class Graph
    {
        public Graph()
        {

        }
    }

    /// <summary>
    /// Map Graph that handles Graphs on a Floor
    /// </summary>
    public class Map : Graph
    {

        private Floor _floor;

        public Map(Floor floor)
        {
            Floor = floor;
        }

        #region properties
        /// <summary>
        /// The floor property of the Map Graph
        /// </summary>
        public Floor Floor
        {
            get
            {
                return _floor;
            }
            set
            {
                _floor = value;
            }
        }
        #endregion
    }
}
