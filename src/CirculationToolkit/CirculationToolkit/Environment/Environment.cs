using CirculationToolkit.Entities;
using CirculationToolkit.Exceptions;
using CirculationToolkit.Graph;
using CirculationToolkit.Profiles;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CirculationToolkit.Environment
{
    public class SimulationEnvironment
    {
        private double _resolution;
        private int _generations;

        private Dictionary<string, List<Entity>> _entities =
            new Dictionary<string, List<Entity>>()
             {
                {"floor", new List<Entity>() },
                {"barrier", new List<Entity>() },
                {"node", new List<Entity>() },
                {"template", new List<Entity>() },
                {"agent", new List<Entity>() },
            };

        private Graph<Node, List<int>> _nodeGraph =
            new Graph<Node, List<int>>();

        private Dictionary<Node, Dictionary<int, int>> _nodeShortestPaths =
            new Dictionary<Node, Dictionary<int, int>>();

        /// <summary>
        /// Constructor for a Simulation Environment that handles null Environments
        /// </summary>
        public SimulationEnvironment()
        {
            _generations = 0;
        }

        /// <summary>
        /// Constructor for a Simulation Environment Object
        /// </summary>
        /// <param name="resolution"></param>
        public SimulationEnvironment(double resolution)
            : this()
        {
            Resolution = resolution;          
        }

        public SimulationEnvironment Duplicate()
        {
            SimulationEnvironment environment = new SimulationEnvironment(Resolution);

            environment.Entities = Entities;
            environment.NodeGraph = NodeGraph;
            environment.NodeShortestPaths = NodeShortestPaths;

            return environment;
        }

        /// <summary>
        /// Returns the string representation of the Environment
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Environment: " +
                "{ floors: "                + Floors.Count +
                ", barriers: "              + Barriers.Count +
                ", nodes: "                 + Nodes.Count +
                ", agents: "                + Agents.Count +
                "} Generations Computed: "  + Generations.ToString();
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
        public Graph<Node, List<int>> NodeGraph
        {
            get
            {
                return _nodeGraph;
            }
            set
            {
                _nodeGraph = value;
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
            set
            {
                _nodeShortestPaths = value;
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
            set
            {
                _entities = value;
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

        /// <summary>
        /// Returns a list of all the Agent Entities in this Environment
        /// </summary>
        public List<Entity> Agents
        {
            get
            {
                return Entities["agent"];
            }
        }
        public int Generations
        {
            get
            {
                return _generations;
            }
            set
            {
                _generations = value;
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
        /// Returns a mapped dictionary of type T entities by their property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private Dictionary<PropertyType, List<EntityType>> GetEntityDictByProperty<EntityType, PropertyType>(string propertyName)
        {
            Type type = typeof(EntityType);
            string typeName = type.Name.ToLower();

            Dictionary<PropertyType, List<EntityType>> entityDict = new Dictionary<PropertyType, List<EntityType>>();            

            foreach(Entity entity in Entities[typeName])
            {
                PropertyType value = (PropertyType)Convert.ChangeType(type.GetProperty(propertyName).GetValue(entity, null), typeof(PropertyType));

                if (!entityDict.ContainsKey(value))
                {
                    entityDict[value] = new List<EntityType>();
                }

                entityDict[value].Add((EntityType)Convert.ChangeType(entity, typeof(EntityType)));
            }           

            return entityDict;
        }

        /// <summary>
        /// Returns a list of type T Entities matching a name property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<EntityType> GetEntities<EntityType>(string name)
        {
            Dictionary<string, List<EntityType>> entityDict = GetEntityDictByProperty<EntityType, string>("Name");

            if (entityDict.ContainsKey(name))
            {
                return entityDict[name];
            }

            return new List<EntityType>();
        }

        /// <summary>
        /// Returns a list of type T Entities matching a position property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <returns></returns>
        public List<EntityType> GetEntities<EntityType>(Point3d position)
        {
            Dictionary<Point3d, List<EntityType>> entityDict = GetEntityDictByProperty<EntityType, Point3d>("Position");

            if (entityDict.ContainsKey(position))
            {
                return entityDict[position];
            }

            return new List<EntityType>();
        }
        #endregion

        #region main methods
        /// <summary>
        /// Stores a list of all shortest paths to this node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        private void SetNodeShortestPaths(Node node, Dictionary<int, int> path)
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
                Floor floor = toNode.Floor;

                if (floor != null)
                {
                    int? fromNodeGridIndex = floor.GetPointGridIndex(toNode.Position);
                    int? toNodeGridIndex = floor.GetPointGridIndex(fromNode.Position);

                    if (toNodeGridIndex != null && fromNodeGridIndex != null)
                    {
                        List<int> path;

                        try
                        {
                            if (NodeShortestPaths.Keys.Contains(fromNode))
                            {
                                path = floor.FloorGraph.ShortestPath(
                                    (int)toNodeGridIndex,
                                    (int)fromNodeGridIndex,
                                    0,
                                    NodeShortestPaths[fromNode]);

                                path.Reverse();
                            }
                            else if (NodeShortestPaths.Keys.Contains(toNode))
                            {
                                path = floor.FloorGraph.ShortestPath(
                                    (int)fromNodeGridIndex,
                                    (int)toNodeGridIndex,
                                    0,
                                    NodeShortestPaths[toNode]);
                            }
                            else
                            {
                                path = floor.FloorGraph.ShortestPath(
                                    (int)fromNodeGridIndex,
                                    (int)toNodeGridIndex);
                            }

                            return path;
                        }
                        catch (KeyNotFoundException e)
                        {
                            throw new NodePathNotPossibleException("Path not possible - " + fromNode + " to " + toNode, e);
                        }
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Handles all processes needed to store shortest paths between nodes
        /// </summary>
        /// <returns></returns>
        private bool BuildNodeShortestPaths()
        {
            if (Nodes.Count > 0)
            {
                foreach (Node node in Nodes)
                {
                    if (node.Floor != null && node.Floor.ContainsPoint(node.Position))
                    {
                        int? nodeGridIndex = node.Floor.GetPointGridIndex(node.Position);

                        if (nodeGridIndex != null)
                        {
                            Dictionary<int, int> paths =
                                node.Floor.FloorGraph.Dijsktra((int)nodeGridIndex, (int)nodeGridIndex).Item2;

                            SetNodeShortestPaths(node, paths);
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Handles all processess needed to integrate Floors in the Environment
        /// </summary>
        private bool BuildFloors()
        {
            if (Floors.Count > 0)
            {
                foreach (Floor floor in Floors)
                {
                    floor.SetGrid(Resolution);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Handles all processes needed to create the edge maps that affect shortest paths
        /// </summary>
        /// <returns></returns>
        private bool BuildFloorBarrierMaps()
        {
            if (Floors.Count > 0)
            {
                foreach (Floor floor in Floors)
                {
                    floor.AddBarrierMap();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Handles all processess needed to integrate Barriers in the Environment
        /// </summary>
        private bool BuildBarriers()
        {
            if (Barriers.Count > 0)
            {
                foreach (Barrier barrier in Barriers)
                {
                    string floorName = barrier.Floor;
                    List<Floor> floors = GetEntities<Floor>(floorName);

                    if (floors.Count > 1)
                    {
                        throw new EntityNameNotUniquException("Muliple Floor Entities with same name detected:" + floorName);
                    }
                    else if  (floors.Count == 0)
                    {
                        throw new FloorNotFoundException("Barrier Entity floor not found: " + floorName);
                    }
                    else if (floors.Count == 1)
                    {
                        barrier.Indexes = floors[0].AddBarrier(barrier);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Handles all processes needed to integrate Nodes into the Environment
        /// </summary>
        private bool BuildNodes()
        {
            if (Nodes.Count > 0)
            {
                foreach (Node node in Nodes)
                {
                    string floorName = node.GetAttribute("floor");
                    List<Floor> floors = GetEntities<Floor>(floorName);

                    if (floors.Count > 1)
                    {
                        throw new EntityNameNotUniquException("Muliple Floor Entities with same name detected:" + floorName);
                    }
                    else if (floors.Count == 0)
                    {
                        throw new FloorNotFoundException("Node Entity floor not found: " + floorName);
                    }
                    else if (floors.Count == 1)
                    {
                        node.Init(floors[0]);

                        if (node.GridIndex == null)
                        {
                            throw new NodeNotOnFloorException(node + " was flagged as not on a Floor");
                        }

                        if (node.IsZone)
                        {
                            node.Indexes = floors[0].AddZone(node);
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Handles all processes needed to integrate Templates in the Environment
        /// </summary>
        private bool BuildTemplates()
        {
            if (Templates.Count > 0)
            {
                foreach (Template template in Templates)
                {
                    bool directed = bool.Parse(template.GetAttribute("directed"));

                    foreach (Tuple<Point3d, Point3d> edge in template.Edges)
                    {
                        List<Node> fromNodes = GetEntities<Node>(edge.Item1);
                        List<Node> toNodes = GetEntities<Node>(edge.Item2);

                        if (fromNodes.Count != 0 && toNodes.Count != 0)
                        {                           
                            for (int i=0; i<fromNodes.Count; i++)
                            {
                                for (int j=0; j<toNodes.Count; j++)
                                {
                                    Node fromNode = fromNodes[i];
                                    Node toNode = toNodes[j];

                                    try
                                    {
                                        List<int> path = GetNodeShortestPath(fromNode, toNode);
                                        NodeGraph.AddEdge(fromNode, toNode, path);

                                        if (!directed)
                                        {
                                            List<int> reversedPath = new List<int>(path);
                                            reversedPath.Reverse();
                                            NodeGraph.AddEdge(toNode, fromNode, reversedPath);
                                        }
                                    }
                                    catch (NodePathNotPossibleException e)
                                    {
                                        throw e;
                                    }

                                }
                            }
                        }
                    }
                }

                return true;
            }
            else
            {
                foreach (Node fromNode in Nodes)
                {
                    foreach (Node toNode in Nodes)
                    {
                        if (!fromNode.Equals(toNode))
                        {
                            try
                            {
                                List<int> path = GetNodeShortestPath(fromNode, toNode);
                                NodeGraph.AddEdge(fromNode, toNode, path);
                            }
                            catch (NodePathNotPossibleException e)
                            {
                                throw e;
                            }
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Handles all processes needed to integrate Agents in the Environment
        /// </summary>
        private bool BuildAgents()
        {
            if (Agents.Count > 0)
            {
                foreach (Agent agent in Agents)
                {
                    agent.Init(this);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Builds the Environment
        /// </summary>
        public bool BuildEnvironment()
        {

            ///
            /// Try to build the floors return a warning if 
            /// no floors are found
            if (!BuildFloors())
            {
                throw new FloorNotFoundException("Environment needs at least one floor");
            }


            if (!BuildBarriers()) { }

            ///
            /// Try to build the nodes on the Floor
            /// and return a warning if a node is not on the floor
            /// 
            try
            {
                if (!BuildNodes()) { }
            }
            catch (NodeNotOnFloorException e)
            {
                throw e;
            }
            
            if (!BuildFloorBarrierMaps()) { }
            if (!BuildNodeShortestPaths()) { }
            
            ///
            /// Try to build the connections between nodes
            /// and return an warning if a path is not possible
            /// 
            try
            {
                if (!BuildTemplates()) { }
            }
            catch (NodePathNotPossibleException e)
            {
                throw e;
            }
            
            if (!BuildAgents()) { }

            return true;
        }

        /// <summary>
        /// Steps one generation of the Environment Entities
        /// </summary>
        private bool Step()
        {
            bool isComplete = true;

            foreach (Agent agent in Agents)
            {
                agent.Step();

                if (agent.IsActive)
                {
                    isComplete = false;
                }
            }

            return isComplete;
        }

        /// <summary>
        /// Runs the Environment
        /// </summary>
        public bool RunEnvironment(Func<bool> iterFunc)
        {
            bool isComplete = false;

            while (!isComplete)
            {
                isComplete = Step();
                Generations++;

                if (Generations >= 100000)
                {
                    ///
                    /// force the simulation to end after 100000 generations
                    /// as a precauction. This should maybe be set.
                    ///
                    throw new MaxStepReachedException("Maximum step count reached - " + Generations);
                }                        

                iterFunc();
            }

            return isComplete;

        }
        #endregion

    }
}
