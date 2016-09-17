using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CirculationToolkit.Entities;

namespace CirculationToolkit.Graph
{
    /// <summary>
    /// The FloorGraph Class is an extension to the Graph class that has
    /// Floor specific methods implemeted
    /// </summary>
    public class FloorGraph<NodeType> : SearchGraph<NodeType>
    {

        private Floor _floor;
        private Dictionary<NodeType, double> _barrierMap;
        private Dictionary<int, Dictionary<NodeType, int>> _occupancyMap;

        #region constructors
        /// <summary>
        /// FloorGraph Constructor that inherits graph functionality and applies it
        /// to a Floor Entity
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="barrierMap"></param>
        /// <param name="occupancyMap"></param>
        private FloorGraph(Floor floor, Dictionary<NodeType, double> barrierMap, Dictionary<int, Dictionary<NodeType, int>> occupancyMap)
        {
            _floor = floor;
            _barrierMap = barrierMap;
            _occupancyMap = occupancyMap;
        }
        

        /// <summary>
        /// FloorGraph Constructor that inherits graph functionality and applies it
        /// to a Floor Entity
        /// </summary>
        /// <param name="floor"></param>
        public FloorGraph(Floor floor)
            : this (floor, new Dictionary<NodeType, double>(), new Dictionary<int, Dictionary<NodeType, int>>())
        {
        }
        #endregion

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

        /// <summary>
        /// Returns the Barrier map on this Map
        /// </summary>
        public Dictionary<NodeType, double> BarrierMap
        {
            get
            {
                return _barrierMap;
            }
            set
            {
                _barrierMap = value;
            }
        }

        /// <summary>
        /// Returns the Occupancy at all grid points for all generations of
        /// the simulation
        /// </summary>
        public Dictionary<int, Dictionary<NodeType, int>> OccupancyMap
        {
            get
            {
                return _occupancyMap;
            }
            set
            {
                _occupancyMap = value;
            }
        }
        #endregion

        #region map methods
        /// <summary>
        /// Returns a list of all the Map node BarrierMap values
        /// </summary>
        /// <returns></returns>
        public List<double> GetBarrierMapNodeValueList()
        {
            return BarrierMap.Values.ToList();
        }

        /// <summary>
        /// Returns the BarrierMap value at a Map node
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double GetBarrierMapNodeValue(NodeType key)
        {
            if (!BarrierMap.ContainsKey(key))
            {
                BarrierMap[key] = 0;
            }
            return BarrierMap[key];
        }

        /// <summary>
        /// Increase a Map node BarrierMap value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddBarrierMapNodeValue(NodeType key, double value)
        {
            if (!BarrierMap.ContainsKey(key))
            {
                BarrierMap[key] = 0;
            }
            if (BarrierMap[key] != double.MaxValue)
            {
                if (BarrierMap[key] + value >= double.MaxValue)
                {
                    BarrierMap[key] = double.MaxValue;
                }
                else
                {
                    BarrierMap[key] += value;
                }
            }
        }

        /// <summary>
        /// Returns a list of all the Map node OccupancyMap values
        /// of a generation
        /// </summary>
        /// <param name="gen"></param>
        /// <returns></returns>
        public List<int> GetOccupancyMapNodeValueList(int gen)
        {
            List<int> valueList = new List<int>();

            foreach (NodeType node in Nodes)
            {
                valueList.Add(GetOccupancyMapNodeValue(node, gen));
            }

            return valueList;
        }

        /// <summary>
        /// Returns the OccupancyMap value at a Map node at a generation
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gen"></param>
        /// <returns></returns>
        public int GetOccupancyMapNodeValue(NodeType key, int gen)
        {
            if (!OccupancyMap.ContainsKey(gen))
            {
                OccupancyMap[gen] = new Dictionary<NodeType, int>();
            }
            if (!OccupancyMap[gen].ContainsKey(key))
            {
                OccupancyMap[gen][key] = 0;
            }

            return OccupancyMap[gen][key];
        }

        /// <summary>
        /// Increase the Map node OccupancyMap value at a generation
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gen"></param>
        public void AddOccupancyMapNodeValue(NodeType key, int gen)
        {
            if (!OccupancyMap.ContainsKey(gen))
            {
                OccupancyMap[gen] = new Dictionary<NodeType, int>();
            }
            if (!OccupancyMap[gen].ContainsKey(key))
            {
                OccupancyMap[gen][key] = 0;
            }

            OccupancyMap[gen][key]++;
        }

        /// <summary>
        /// Decrease the Map node OccupancyMap value at a generation
        /// This is used primarily for agent path shifting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gen"></param>
        public void RemoveOccupancyMapNodeValue(NodeType key, int gen)
        {
            if (OccupancyMap.ContainsKey(gen))
            {
                if (OccupancyMap[gen].ContainsKey(key))
                {
                    if (OccupancyMap[gen][key] != 0)
                    {
                        OccupancyMap[gen][key]--;
                    }
                }
            }
        }
        #endregion

        #region shortest path methods
        /// <summary>
        /// Gets the length of the current mid calculation shortest path in order to compute
        /// the current step count at that point
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="goal"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public override int GetStep(NodeType initial, NodeType goal, Dictionary<NodeType, NodeType> route)
        {
            List<NodeType> path = new List<NodeType>()
            {
                initial
            };
            while (!initial.Equals(goal))
            {
                path.Add(route[initial]);
                initial = route[initial];
            }
            return path.Count;
        }

        /// <summary>
        /// Overrides the GetDistance method from the Graph class in order to use the Map
        /// to modify shortest path values updated to densities and barriers
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gen"></param>
        /// <returns></returns>
        public override double GetDistance(Tuple<NodeType, NodeType> key, int gen = 0)
        {
            double distance = Distances[key];
            double weight = distance / Floor.GridSize;

            double area = Math.Pow(Floor.GridSize, 2);
            double occupancy = GetOccupancyMapNodeValue(key.Item2, gen);
            double density = occupancy / area;

            double barriers = GetBarrierMapNodeValue(key.Item2);

            return weight + barriers + density;
        }
        #endregion
    }
}
