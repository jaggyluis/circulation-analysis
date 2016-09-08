﻿using CirculationToolkit.Entities;
using CirculationToolkit.Util;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit
{
    public class SimulationEnvironment
    {
        private double _resolution;

        private Dictionary<string, List<Entity>> _entities =
            new Dictionary<string, List<Entity>>()
             {
                {"floor", new List<Entity>() },
                {"barrier", new List<Entity>() },
                {"node", new List<Entity>() },
                {"template", new List<Entity>() },
                {"agent", new List<Entity>() },
            };

        private Dictionary<Tuple<Node, Node>, List<int>> _nodeConnections =
            new Dictionary<Tuple<Node, Node>, List<int>>();

        private Dictionary<Node, Dictionary<int, int>> _nodeShortestPaths =
            new Dictionary<Node, Dictionary<int, int>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution"></param>
        public SimulationEnvironment(double resolution)
        {
            Resolution = resolution;          
        }

        /// <summary>
        /// Returns the string representation of the Environment
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Environment: " +
                " {" +
                Floors.Count + "f" + 
                Barriers.Count + "b" +
                Nodes.Count + "n" +
                " }";
        }

        #region properties
        /// <summary>
        /// Returns the Floor Resolution for this Environment
        /// </summary>
        public double Resolution
        {
            get
            {
                return _resolution;
            }
            set
            {
                _resolution = value;
            }
        }

        /// <summary>
        /// Returns the dictionary of connections between Node Entities
        /// </summary>
        public Dictionary<Tuple<Node, Node>, List<int>> NodeConnections
        {
            get
            {
                return _nodeConnections;
            }
        }

        /// <summary>
        /// Returns a dictionary of all the paths on the floor to Each Node Entity
        /// </summary>
        public Dictionary<Node, Dictionary<int, int>> NodeShortestPaths
        {
            get
            {
                return _nodeShortestPaths;
            }
        }

        /// <summary>
        /// Accessor for all the Entities in the Environment
        /// </summary>
        public Dictionary<string, List<Entity>> Entities
        {
            get
            {
                return _entities;
            }
        }

        /// <summary>
        /// Returns a list of all the Floor Entities in this Environment
        /// </summary>
        public List<Entity> Floors
        {
            get
            {
                return Entities["floor"];
            }
        }

        /// <summary>
        /// Returns a list of all the Barrier Entities in this Environment
        /// </summary>
        public List<Entity> Barriers
        {
            get
            {
                return Entities["barrier"];
            }
        }     

        /// <summary>
        /// Returns a list of all the Node Entities in this Environment
        /// </summary>
        public List<Entity> Nodes
        {
            get
            {
                return Entities["node"];
            }
        }

        /// <summary>
        /// Returns a list of all the Template Entities in this Environment
        /// </summary>
        public List<Entity> Templates
        {
            get
            {
                return Entities["template"];
            }
        }
        #endregion

        #region Entity methods
        /// <summary>
        /// Generice add Entity method adds all Entities to the
        /// Respective Lists
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntity(Entity entity)
        {
            Entities[entity.Type].Add(entity);
        }
                
        /// <summary>
        /// Returns a mapped dictionary of all Floor Entities by name
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Floor> GetFloorDictByName()
        {
            Dictionary<string, Floor> floorDict = new Dictionary<string, Floor>();

            foreach(Floor floor in Floors)
            {
                floorDict[floor.Name] = floor;
            }

            return floorDict;
        }

        /// <summary>
        /// Returns a Floor Entity by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Floor GetFloor(string name)
        {
            Dictionary<string, Floor> floorDict = GetFloorDictByName();

            if (floorDict.ContainsKey(name))
            {
                return floorDict[name];
            }

            return null;
        }

        /// <summary>
        /// Returns a mapped Dictionary of all Node Entities by name
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Node> GetNodeDictByName()
        {
            Dictionary<string, Node> nodeDict = new Dictionary<string, Node>();

            foreach (Node node in Nodes)
            {
                nodeDict[node.Name] = node;
            }

            return nodeDict;
        }

        /// <summary>
        /// Returns a mapped Dictionary of all Node Entities by position
        /// </summary>
        /// <returns></returns>
        private Dictionary<Point3d, Node> GetNodeDictByPosition()
        {
            Dictionary<Point3d, Node> nodeDict = new Dictionary<Point3d, Node>();

            foreach (Node node in Nodes)
            {
                nodeDict[node.Position] = node;
            }

            return nodeDict;
        }

        /// <summary>
        /// Returns a Node Entity by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Node GetNode(string name)
        {
            Dictionary<string, Node> nodeDict = GetNodeDictByName();

            if (nodeDict.ContainsKey(name))
            {
                return nodeDict[name];
            }

            return null;
        }

        /// <summary>
        /// Returns a Node Entity by position
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Node GetNode(Point3d position)
        {
            Dictionary<Point3d, Node> nodeDict = GetNodeDictByPosition();

            if (nodeDict.ContainsKey(position))
            {
                return nodeDict[position];
            }

            return null;
        }

        /// <summary>
        /// Stores a list of all shortest paths to this node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        private void SetNodeShortestPaths(Node node, Dictionary<int,int> path)
        {
            NodeShortestPaths[node] = path;
        }

        /// <summary>
        /// Returns the shortest path between two Node Entities on a single floor
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="toNode"></param>
        /// <returns></returns>
        public List<int> GetNodeShortestPath(Node fromNode, Node toNode)
        {
            if (toNode.Floor == fromNode.Floor)
            {
                Floor floor = GetFloor(toNode.Floor);

                if (floor != null)
                {
                    int? fromNodeGridIndex = floor.GetPointGridIndex(toNode.Position);
                    int? toNodeGridIndex = floor.GetPointGridIndex(fromNode.Position);

                    if (toNodeGridIndex != null && fromNodeGridIndex != null)
                    {
                        List<int> path =
                            floor.Map.ShortestPath((int)fromNodeGridIndex, (int)toNodeGridIndex);

                        return path;
                    }

                }
            }

            return null;
        }
        #endregion

        #region main methods
        /// <summary>
        /// Handles all processess needed to integrate Floors in the Environment
        /// </summary>
        private void BuildFloors()
        {
            foreach (Floor floor in Floors)
            {
                floor.SetGrid(Resolution);
                //floor.AddEdgeMap();
            }
        }

        /// <summary>
        /// Handles all processess needed to integrate Barriers in the Environment
        /// </summary>
        private void BuildBarriers()
        {
            foreach (Barrier barrier in Barriers)
            {
                Floor floor = GetFloor(barrier.Floor);

                if (floor != null)
                {
                    floor.AddBarrierMap(barrier);
                }
            }
        }

        /// <summary>
        /// Handles all processes needed to integrate Nodes into the Environment
        /// </summary>
        private void BuildNodes()
        {
            foreach (Node node in Nodes)
            {
                Floor floor = GetFloor(node.Floor);

                if (floor != null && floor.ContainsPoint(node.Position))
                {
                    int? nodeGridIndex = floor.GetPointGridIndex(node.Position);

                    if (nodeGridIndex != null)
                    {
                        Dictionary<int, int> paths = 
                            floor.Map.Dijsktra((int)nodeGridIndex, (int)nodeGridIndex).Item2;

                        SetNodeShortestPaths(node, paths);
                    }    
                }
            }
        }

        /// <summary>
        /// Handles all processes needed to integrate Templates in the Environment
        /// </summary>
        private void BuildTemplates()
        {
            if (Templates.Count != 0)
            {
                foreach (Template template in Templates)
                {
                    bool directed = bool.Parse(template.GetAttribute("directed"));

                    foreach (Tuple<Point3d, Point3d> edge in template.Edges)
                    {
                        Node fromNode = GetNode(edge.Item1);
                        Node toNode = GetNode(edge.Item2);

                        if (fromNode != null && toNode != null)
                        {
                            List<int> path = GetNodeShortestPath(fromNode, toNode);
                            NodeConnections[new Tuple<Node, Node>(fromNode, toNode)] = path;

                            if (!directed)
                            {
                                List<int> reversedPath = new List<int>(path);
                                reversedPath.Reverse();
                                NodeConnections[new Tuple<Node, Node>(toNode, fromNode)] = path;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Node fromNode in Nodes)
                {
                    foreach (Node toNode in Nodes)
                    {
                        if (!fromNode.Equals(toNode))
                        {
                            List<int> path = GetNodeShortestPath(fromNode, toNode);
                            NodeConnections[new Tuple<Node, Node>(fromNode, toNode)] = path;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Builds the Environment
        /// </summary>
        public void BuildEnvironment()
        {
            BuildFloors();
            BuildBarriers();
            BuildNodes();
            BuildTemplates();
        }
        #endregion

    }
}
