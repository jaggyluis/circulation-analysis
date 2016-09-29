using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CirculationToolkit.Entities;
using Rhino.Geometry;

namespace CirculationToolkit.Graph
{
    /// <summary>
    /// Graph Class that handles abstract connections
    /// </summary>
    public class Graph<NodeType, DistanceType>
    {
        private HashSet<NodeType> _nodes;
        private Dictionary<NodeType, HashSet<NodeType>> _edges;
        private Dictionary<Tuple<NodeType, NodeType>, DistanceType> _distances;

        #region constructors
        /// <summary>
        /// Graph class Constructor
        /// </summary>
        public Graph()
        {
            _nodes = new HashSet<NodeType>();
            _edges = new Dictionary<NodeType, HashSet<NodeType>>();
            _distances = new Dictionary<Tuple<NodeType, NodeType>, DistanceType>();
        }
        #endregion

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
        /// Removes a Node and all occurances of that Node from the Graph
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(NodeType node)
        {
            if (Nodes.Contains(node))
            {
                Nodes.Remove(node);
            }

            RemoveNodeReferences(node);
        }

        /// <summary>
        /// Removes all occurances of this node in the Edges and Distances of the Graph
        /// </summary>
        public void RemoveNodeReferences(NodeType node)
        {
            if (Edges.ContainsKey(node))
            {
                List<NodeType> edges = Edges[node].ToList();

                for (int i = edges.Count - 1; i >= 0; i--)
                {
                    RemoveEdge(node, edges[i], false);
                }
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
                AddEdge(n2, n1, distance, true);
            }
        }

        /// <summary>
        /// Removes an edge from the Graph
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="removeDirected"></param>
        public void RemoveEdge(NodeType n1, NodeType n2, bool removeDirected=true)
        {
            Tuple<NodeType, NodeType> key;

            if (Edges[n1].Contains(n2))
            {
                Edges[n1].Remove(n2);
                key = new Tuple<NodeType, NodeType>(n1, n2);

                if (Distances.ContainsKey(key)) {
                    Distances.Remove(key);
                }
            }
            if (removeDirected)
            {
                RemoveEdge(n2, n1, false);
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




}
