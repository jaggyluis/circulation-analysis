using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CirculationToolkit.Entities;
using Rhino.Geometry;

namespace CirculationToolkit.Util
{
    /// <summary>
    /// Graph Class that handles abstract connections
    /// </summary>
    public class Graph<NodeType, DistanceType>
    {
        private HashSet<NodeType> _nodes;
        private Dictionary<NodeType, HashSet<NodeType>> _edges;
        private Dictionary<Tuple<NodeType, NodeType>, DistanceType> _distances;
        
        /// <summary>
        /// Graph class Constructor
        /// </summary>
        public Graph()
        {
            _nodes = new HashSet<NodeType>();
            _edges = new Dictionary<NodeType, HashSet<NodeType>>();
            _distances = new Dictionary<Tuple<NodeType, NodeType>, DistanceType>();
        }

        #region properties
        /// <summary>
        /// String representation of the Graph
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Graph : " + 
                Nodes.Count.ToString() + " nodes, " +
                Edges.Count.ToString() + " edges";
        }

        /// <summary>
        /// Returns a list of all the Graph nodes
        /// </summary>
        public HashSet<NodeType> Nodes
        {
            get
            {
                return _nodes;
            }
        }

        /// <summary>
        /// Returns a dictionary of all the Graph edges by node
        /// </summary>
        public Dictionary<NodeType, HashSet<NodeType>> Edges
        {
            get
            {
                return _edges;
            }
        }

        /// <summary>
        /// Returns a dictionary of all the Graph distances by tuple key
        /// </summary>
        public Dictionary<Tuple<NodeType, NodeType>, DistanceType> Distances
        {
            get
            {
                return _distances;
            }
        }
        #endregion

        #region utlity methods
        /// <summary>
        /// Adds a node to the Graph
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(NodeType node)
        {
            Nodes.Add(node);

            if (!Edges.ContainsKey(node))
            {
                Edges[node] = new HashSet<NodeType>();
            }
        }

        /// <summary>
        /// Adds and edge to the Graph
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="distance"></param>
        /// <param name="directed"></param>
        public void AddEdge(NodeType n1, NodeType n2, DistanceType distance, bool directed=false)
        {
            AddNode(n1);
            AddNode(n2);

            Edges[n1].Add(n2);
            Distances[new Tuple<NodeType, NodeType>(n1, n2)] = distance;

            if (!directed)
            {
                Edges[n2].Add(n1);
                Distances[new Tuple<NodeType, NodeType>(n2, n1)] = distance;
            }
        }

        /// <summary>
        /// Returns the distance between two nodes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gen"></param>
        /// <returns></returns>
        public virtual DistanceType GetDistance(Tuple<NodeType, NodeType> key, int gen=0)
        {
            return Distances[key];
        }

        /// <summary>
        /// Void method for override
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="goal"></param>
        /// <param name="route"></param>
        public virtual int GetStep(NodeType initial, NodeType goal, Dictionary<NodeType, NodeType> route)
        {
            return 0;
        }
        #endregion           
    }

    /// <summary>
    /// The SearchGraph Class is an extension to the Graph class that has
    /// search methods implemented
    /// </summary>
    /// <typeparam name="NodeType"></typeparam>
    public class SearchGraph<NodeType> : Graph<NodeType, double>
    {
        /// <summary>
        /// SearchGraph Constructor that inherits Graph functionality
        /// and has search methods implemented
        /// </summary>
        public SearchGraph()
            : base ()
        {
        }

        #region search methods
        /// <summary>
        /// MergeLeft two Dictionaries
        /// This does not replace any keys and keeps the original keys from
        /// the leftmost Dictionary
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        private Dictionary<K, V> MergeDict<K, V>(Dictionary<K, V> to, Dictionary<K, V> from)
        {
            foreach (K key in from.Keys)
            {
                if (!to.ContainsKey(key))
                {
                    to[key] = from[key];
                }
            }

            return to;
        }

        /// <summary>
        /// Dictionray DeepSearch method that does a deep search of the Graph
        /// using a boolean function until it reaches a result
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="visited"></param>
        /// <param name="depth"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public Dictionary<NodeType, int?> DeepSearch(NodeType key,
            Func<NodeType, bool> func,
            Dictionary<NodeType, int?> visited = null,
            int depth = 0,
            int maxDepth = 10)
        {
            if (visited == null)
            {
                visited = new Dictionary<NodeType, int?>();
            }
            visited[key] = null;

            if (func(key))
            {
                visited[key] = depth;
            }
            else if (depth < maxDepth)
            {
                if (Edges.ContainsKey(key))
                {
                    foreach (NodeType edge in Edges[key])
                    {
                        if (!visited.ContainsKey(edge))
                        {
                            visited = MergeDict(visited,
                                DeepSearch(edge,
                                func,
                                visited,
                                depth + 1));
                        }
                    }
                }
            }
            return visited;
        }

        /// <summary>
        /// Dictionary ShallowSearch that does a shallow search of the Graph
        /// using a boolean function until it reaches as result
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="keys"></param>
        /// <param name="func"></param>
        /// <param name="visited"></param>
        /// <param name="depth"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public Dictionary<NodeType, int?> ShallowSearch(List<NodeType> keys,
            Func<NodeType, bool> func,
            Dictionary<NodeType, int?> visited = null,
            int depth = 0,
            int maxDepth = int.MaxValue)
        {
            if (visited == null)
            {
                visited = new Dictionary<NodeType, int?>();
            }
            foreach (NodeType key in keys)
            {
                if (!visited.ContainsKey(key))
                {
                    visited[key] = null;
                }
                if (func(key))
                {
                    visited[key] = depth;
                    return visited;
                }
            }
            if (depth < maxDepth)
            {
                HashSet<NodeType> keySet = new HashSet<NodeType>();

                foreach (NodeType key in keys)
                {
                    foreach (NodeType k in Edges[key])
                    {
                        keySet.Add(k);
                    }
                }

                visited = MergeDict(visited,
                    ShallowSearch(keySet.ToList(),
                    func,
                    visited,
                    depth + 1));
            }
            return visited;
        }
        #endregion

        #region shortest path methods
        /// <summary>
        /// Dijsktra Shortest Path Algorythm returns the shortest path tree between
        /// an Initial node and all nodes
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="goal"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public Tuple<Dictionary<NodeType, double>, Dictionary<NodeType, NodeType>>
            Dijsktra(NodeType initial, NodeType goal, int startIndex = 0)
        {
            Dictionary<NodeType, double> visited = new Dictionary<NodeType, double>() { { initial, 0 } };
            Dictionary<NodeType, NodeType> path = new Dictionary<NodeType, NodeType>();
            HashSet<NodeType> nodes = new HashSet<NodeType>(Nodes);
            Tuple<Dictionary<NodeType, double>, Dictionary<NodeType, NodeType>> tup =
                new Tuple<Dictionary<NodeType, double>, Dictionary<NodeType, NodeType>>
                (visited, path);

            while (nodes.Count > 0)
            {
                NodeType minNode = default(NodeType);

                foreach (NodeType node in nodes)
                {
                    if (visited.ContainsKey(node))
                    {
                        if (minNode.Equals(default(NodeType)))
                        {
                            minNode = node;
                        }
                        else if (visited[node] < visited[minNode])
                        {
                            minNode = node;
                        }
                    }
                }
                if (minNode.Equals(default(NodeType)))
                {
                    break;
                }
                nodes.Remove(minNode);

                double currWeight = visited[minNode];
                int currGeneration = GetStep(minNode, initial, path);

                foreach (NodeType edge in Edges[minNode])
                {
                    double distance = GetDistance(new Tuple<NodeType, NodeType>(minNode, edge),
                        currGeneration + startIndex);
                    double weight = currWeight + distance;

                    if (!visited.ContainsKey(edge) || weight < visited[edge])
                    {
                        visited[edge] = weight;
                        path[edge] = minNode;
                    }

                    //
                    // Right now this is using goal=initial to determine whether to
                    // complete the algorythm - this should implement a nullable nodeType or
                    // a default value as an option to determine end results.
                    //

                    if (!goal.Equals(initial) && edge.Equals(goal))
                    {
                        return tup;
                    }
                }
            }
            return tup;
        }

        /// <summary>
        /// Shortest Path between two nodes
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="goal"></param>
        /// <param name="startIndex"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<NodeType> ShortestPath(NodeType initial,
            NodeType goal,
            int startIndex = 0,
            Dictionary<NodeType, NodeType> path = null)
        {
            if (path == null)
            {
                path = Dijsktra(initial, goal, startIndex).Item2;
            }
            List<NodeType> route = new List<NodeType>()
            {
                goal
            };
            while (!goal.Equals(initial))
            {
                route.Add(path[goal]);
                goal = path[goal];
            }
            route.Reverse();

            return route;
        }
        #endregion
    }

    /// <summary>
    /// The FloorGraph Class is an extension to the Graph class that has
    /// Floor specific methods implemeted
    /// </summary>
    public class FloorGraph<NodeType> : SearchGraph<NodeType>
    {

        private Floor _floor;
        private Dictionary<NodeType, double> _barrierMap;
        private Dictionary<NodeType, Dictionary<int, int>> _occupancyMap;

        /// <summary>
        /// FloorGraph Constructor that inherits graph functionality and applies it
        /// to a Floor Entity
        /// </summary>
        /// <param name="floor"></param>
        public FloorGraph(Floor floor)
        {
            Floor = floor;
            BarrierMap = new Dictionary<NodeType, double>();
            OccupancyMap = new Dictionary<NodeType, Dictionary<int, int>>();
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
        public Dictionary<NodeType, Dictionary<int, int>> OccupancyMap
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
            BarrierMap[key] += value;
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

            foreach(NodeType node in Nodes)
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
            if (!OccupancyMap.ContainsKey(key))
            {
                OccupancyMap[key] = new Dictionary<int, int>();
            }
            if (!OccupancyMap[key].ContainsKey(gen))
            {
                OccupancyMap[key][gen] = 0;
            }

            return OccupancyMap[key][gen];
        }

        /// <summary>
        /// Increase the Map node OccupancyMap value at a generation
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gen"></param>
        public void AddOccupancyMapNodeValue(NodeType key, int gen)
        {
            if (!OccupancyMap.ContainsKey(key))
            {
                OccupancyMap[key] = new Dictionary<int, int>();
            }
            if (!OccupancyMap[key].ContainsKey(gen))
            {
                OccupancyMap[key][gen] = 0;
            }

            OccupancyMap[key][gen]++;
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
            double area = Math.Pow(Floor.GridSize, 2);
            double occupancy = GetOccupancyMapNodeValue(key.Item2, gen);
            double density = occupancy / area;
            double barriers = GetBarrierMapNodeValue(key.Item2);

            return distance + barriers + density;
        }
        #endregion
    }
}
