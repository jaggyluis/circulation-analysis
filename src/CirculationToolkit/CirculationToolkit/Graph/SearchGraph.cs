using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Graph
{
    /// <summary>
    /// The SearchGraph Class is an extension to the Graph class that has
    /// search methods implemented
    /// </summary>
    /// <typeparam name="NodeType"></typeparam>
    public class SearchGraph<NodeType> : Graph<NodeType, double>
    {
        #region constructors
        /// <summary>
        /// SearchGraph Constructor that inherits Graph functionality
        /// and has search methods implemented
        /// </summary>
        public SearchGraph()
            : base()
        {
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
            for (int i = 0; i < keys.Count; i++)
            {
                if (!visited.ContainsKey(keys[i]))
                {
                    visited[keys[i]] = null;
                }
                if (func(keys[i]))
                {
                    visited[keys[i]] = depth;
                    return visited;
                }
            }
            if (depth < maxDepth)
            {
                HashSet<NodeType> nextKeys = new HashSet<NodeType>();

                for (int i = 0; i < keys.Count; i++)
                {
                    List<NodeType> edges = Edges[keys[i]].ToList();

                    for (int j = 0; j < edges.Count; j++)
                    {
                        nextKeys.Add(edges[j]);
                    }
                }

                visited = MergeDict(visited,
                    ShallowSearch(nextKeys.ToList(),
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
                bool isMinNodeSet = false;

                foreach (NodeType node in nodes)
                {
                    if (visited.ContainsKey(node))
                    {
                        if (!isMinNodeSet)
                        {
                            minNode = node;
                            isMinNodeSet = true;
                        }
                        else
                        {
                            if (visited[node] < visited[minNode])
                            {
                                minNode = node;
                            }
                        }
                    }
                }
                if (!isMinNodeSet)
                {
                    break;
                }
                nodes.Remove(minNode);

                double currWeight = visited[minNode];
                int currGeneration = GetStep(minNode, initial, path);

                foreach (NodeType edge in Edges[minNode])
                {
                    int gen = currGeneration + startIndex;
                    double distance = GetDistance(new Tuple<NodeType, NodeType>(minNode, edge), gen);
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
                try
                {
                    route.Add(path[goal]);
                    goal = path[goal];
                }
                catch (KeyNotFoundException e)
                {
                    throw e;
                }
            }
            route.Reverse();

            return route;
        }
        #endregion
    }
}
