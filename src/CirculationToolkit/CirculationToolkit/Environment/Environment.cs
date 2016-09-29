using CirculationToolkit.Entities;
using CirculationToolkit.Graph;
using CirculationToolkit.Profiles;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Returns a mapped dictionary of all Agent Entities by name
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<Agent>> GetAgentDictByName()
        {
            Dictionary<string, List<Agent>> agentDict = new Dictionary<string, List<Agent>>();

            foreach(Agent agent in Agents)
            {
                if (!agentDict.ContainsKey(agent.Name))
                {
                    agentDict[agent.Name] = new List<Agent>();
                }
                agentDict[agent.Name].Add(agent);
            }

            return agentDict;
        }

        /// <summary>
        /// Returns a list of Agent Entities by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Agent> GetAgents(string name)
        {
            Dictionary<string, List<Agent>> agentDict = GetAgentDictByName();

            if (agentDict.ContainsKey(name))
            {
                return agentDict[name];
            }

            return new List<Agent>();
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
        private Dictionary<string, List<Node>> GetNodeDictByName()
        {
            Dictionary<string, List<Node>> nodeDict = new Dictionary<string, List<Node>>();

            foreach (Node node in Nodes)
            {
                if (!nodeDict.ContainsKey(node.Name))
                {
                    nodeDict[node.Name] = new List<Node>();
                }
                nodeDict[node.Name].Add(node);
            }

            return nodeDict;
        }

        /// <summary>
        /// Returns a mapped Dictionary of all Node Entities by position
        /// </summary>
        /// <returns></returns>
        private Dictionary<Point3d, List<Node>> GetNodeDictByPosition()
        {
            Dictionary<Point3d, List<Node>> nodeDict = new Dictionary<Point3d, List<Node>>();

            foreach (Node node in Nodes)
            {
                if (!nodeDict.ContainsKey(node.Position))
                {
                    nodeDict[node.Position] = new List<Node>();
                }
                nodeDict[node.Position].Add(node);
            }

            return nodeDict;
        }

        /// <summary>
        /// Returns a Node Entity by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Node> GetNodes(string name)
        {
            Dictionary<string, List<Node>> nodeDict = GetNodeDictByName();

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
        public List<Node> GetNodes(Point3d position)
        {
            Dictionary<Point3d, List<Node>> nodeDict = GetNodeDictByPosition();

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
                Floor floor = toNode.Floor;

                if (floor != null)
                {
                    int? fromNodeGridIndex = floor.GetPointGridIndex(toNode.Position);
                    int? toNodeGridIndex = floor.GetPointGridIndex(fromNode.Position);

                    if (toNodeGridIndex != null && fromNodeGridIndex != null)
                    {
                        List<int> path;

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
                }
            }

            return null;
        }
        #endregion

        #region main methods
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
                    Floor floor = GetFloor(barrier.Floor);

                    if (floor != null)
                    {
                        barrier.Indexes = floor.AddBarrier(barrier);
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
                    Floor floor = GetFloor(node.GetAttribute("floor"));                   

                    if (floor != null)
                    {
                        node.Init(floor);

                        if (node.IsZone)
                        {
                            floor.AddZone(node);
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
                        List<Node> fromNodes = GetNodes(edge.Item1);
                        List<Node> toNodes = GetNodes(edge.Item2);

                        if (fromNodes.Count != 0 && toNodes.Count != 0)
                        {                           
                            for (int i=0; i<fromNodes.Count; i++)
                            {
                                for (int j=0; j<toNodes.Count; j++)
                                {
                                    Node fromNode = fromNodes[i];
                                    Node toNode = toNodes[j];

                                    List<int> path = GetNodeShortestPath(fromNode, toNode);
                                    NodeGraph.AddEdge(fromNode, toNode, path);

                                    if (!directed)
                                    {
                                        List<int> reversedPath = new List<int>(path);
                                        reversedPath.Reverse();
                                        NodeGraph.AddEdge(toNode, fromNode, reversedPath);
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
                            List<int> path = GetNodeShortestPath(fromNode, toNode);
                            NodeGraph.AddEdge(fromNode, toNode, path);
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
            if (!BuildFloors()) { return false; }
            if (!BuildBarriers()) { }
            if (!BuildNodes()) { }
            if (!BuildFloorBarrierMaps()) { }
            if (!BuildNodeShortestPaths()) { }
            if (!BuildTemplates()) { }
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
        public void RunEnvironment()
        {
            bool isComplete = false;

            while (!isComplete)
            {
                isComplete = Step();

                if (Generations >= 10000)
                {
                    // force the simulation to end after 10000 generations
                    // as a precauction
                    break;
                }

                Generations++;
            }

        }
        #endregion

    }
}
