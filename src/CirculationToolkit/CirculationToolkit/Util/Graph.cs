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
    public class Graph<K>
    {
        private HashSet<K> _nodes;
        private Dictionary<K, HashSet<K>> _edges;
        private Dictionary<Tuple<K, K>, double> _distances;
        /// <summary>
        /// Graph Class Constructor
        /// </summary>
        public Graph()
        {
            _nodes = new HashSet<K>();
            _edges = new Dictionary<K, HashSet<K>>();
            _distances = new Dictionary<Tuple<K, K>, double>();
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
        public List<K> Nodes
        {
            get
            {
                return _nodes.ToList();
            }
        }

        /// <summary>
        /// Returns a dictionary of all the Graph edges by node
        /// </summary>
        public Dictionary<K, HashSet<K>> Edges
        {
            get
            {
                return _edges;
            }
        }

        /// <summary>
        /// Returns a dictionary of all the Graph distances by tuple key
        /// </summary>
        public Dictionary<Tuple<K, K>, double> Distances
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
        public void AddNode(K node)
        {
            Nodes.Add(node);
        }

        /// <summary>
        /// Adds and edge to the Graph
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="distance"></param>
        /// <param name="directed"></param>
        public void AddEdge(K n1, K n2, double distance, bool directed=false)
        {
            AddNode(n1);
            AddNode(n2);

            Edges[n1].Add(n2);
            Distances[new Tuple<K, K>(n1, n2)] = distance;

            if (!directed)
            {
                Edges[n2].Add(n1);
                Distances[new Tuple<K, K>(n2, n1)] = distance;
            }
        }

        /// <summary>
        /// Returns the distance between two nodes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gen"></param>
        /// <returns></returns>
        public double GetDistance(Tuple<K, K> key, int gen=0)
        {
            return Distances[key];
        }

        /// <summary>
        /// Void method for override
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="goal"></param>
        /// <param name="route"></param>
        public void GetStep(K initial, K goal, Dictionary<K, K> route)
        {
            return;
        }
        #endregion

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
        private Dictionary<K,V> MergeDict<V>(Dictionary<K,V> to, Dictionary<K, V> from)
        {
            foreach(K key in from.Keys)
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
        /// <typeparam name="K"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="visited"></param>
        /// <param name="depth"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public Dictionary<K, int?> DeepSearch(K key, 
            Func<K, bool> func, 
            Dictionary<K, int?> visited = null,
            int depth=0,
            int maxDepth=10)
        {
            if(visited == null)
            {
                visited = new Dictionary<K, int?>();
            }
            visited[key] = null;

            if(func(key))
            {
                visited[key] = depth;
            }
            else if(depth < maxDepth) {
                if(Edges.ContainsKey(key))
                {
                    foreach (K edge in Edges[key])
                    {
                        if (!visited.ContainsKey(edge))
                        {
                            visited = MergeDict(visited, 
                                DeepSearch(edge, 
                                func, 
                                visited, 
                                depth+1));
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
        /// <typeparam name="K"></typeparam>
        /// <param name="keys"></param>
        /// <param name="func"></param>
        /// <param name="visited"></param>
        /// <param name="depth"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public Dictionary<K, int?> ShallowSearch(List<K> keys, 
            Func<K, bool> func, 
            Dictionary<K, int?> visited = null,
            int depth=0,
            int maxDepth=int.MaxValue)
        {
            if (visited == null)
            {
                visited = new Dictionary<K, int?>();
            }
            foreach(K key in keys)
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
                HashSet<K> keySet = new HashSet<K>();

                foreach(K key in keys)
                {
                    foreach(K k in Edges[key])
                    {
                        keySet.Add(k);
                    }
                }

                visited = MergeDict(visited, 
                    ShallowSearch(keySet.ToList(),
                    func,
                    visited,
                    depth+1));
            }
            return visited;
        }
        #endregion

        public void Dijsktra(K initial, K goal, int startIndex)
        {
            Dictionary<K, int> visited = new Dictionary<K, int>() {{initial, 0}};
            Dictionary<K, K> path = new Dictionary<K, K>();
            HashSet<K> nodes = new HashSet<K>(Nodes);

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
