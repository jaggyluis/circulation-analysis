using CirculationToolkit.Util;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Entities
{
    public class Agent : Entity
    {
        private SimulationEnvironment _environment;

        private int _age;
        private string _state;
        private bool _isActive;
        private bool _isComplete;

        private List<List<int>> _paths;
        private List<Node> _visited;

        private Stack<int> _stack;

        private Node _last;
        private Node _curr;
        private Node _next;

        public Agent(AgentProfile profile)
            : base (profile)
        {
            Paths = new List<List<int>>();
            Visited = new List<Node>();
        }

        public override Entity Duplicate()
        {
            Agent duplicate = new Agent((AgentProfile)Profile);

            duplicate.Paths = Paths;
            duplicate.Visited = Visited;

            duplicate.Age = Age;
            duplicate.State = State;
            duplicate.IsActive = IsActive;
            duplicate.IsComplete = IsComplete;

            duplicate.Stack = Stack;

            return duplicate;
        }

        #region properties
        /// <summary>
        /// This is the Accessor for all things environment related for
        /// this Agent Entity
        /// </summary>
        public SimulationEnvironment Environment
        {
            get
            {
                return _environment;
            }
            set
            {
                _environment = value;
            }
        }

        /// <summary>
        /// The Node Entity that this Agent starts at
        /// </summary>
        public Node Origin
        {
            get
            {
                return Environment.GetNode(GetAttribute("origin"));
            }
        }

        /// <summary>
        /// The Node Entity that this Agent ends at
        /// </summary>
        public Node Destination
        {
            get
            {
                return Environment.GetNode(GetAttribute("destination"));
            }
        }

        /// <summary>
        /// Returns the List of all Node entities that this agent has visited
        /// </summary>
        public List<Node> Visited
        {
            get
            {
                return _visited;
            }
            set
            {
                _visited = value;
            }
        }

        /// <summary>
        /// The Floor Entity that this Agent is currently on
        /// </summary>
        public Floor Floor
        {
            get
            {
                Node position = Environment.GetNode(Position);

                return Environment.GetFloor(position.Floor);
            }
        }

        /// <summary>
        /// Returns the Grid idex of the Agent position on the current FLoor
        /// </summary>
        public int? GridIndex
        {
            get
            {
                return Floor.GetPointGridIndex(Position);
            }
        }

        /// <summary>
        /// Returns the Agent's current state
        /// </summary>
        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        /// <summary>
        /// Returns the Agent age
        /// </summary>
        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                _age = value;
            }
        }

        /// <summary>
        /// Returns the Agent's current stack of grid indexes to visit
        /// </summary>
        public Stack<int> Stack
        {
            get
            {
                return _stack;
            }
            set
            {
                _stack = value;
            }
        }

        /// <summary>
        /// Returns all the Agent Paths
        /// </summary>
        public List<List<int>> Paths
        {
            get
            {
                return _paths;
            }
            set
            {
                _paths = value;
            }
        }
          
        /// <summary>
        /// Returns all the grid indexes that this agent has walked
        /// </summary>
        public List<int> Path
        {
            get
            {
                List<int> path = new List<int>();

                foreach (List<int> p in Paths)
                {
                    foreach (int i in p)
                    {
                        path.Add(i);
                    }
                }

                return path;
            }
        }

        /// <summary>
        /// Returns the Agent's Profile propensities
        /// </summary>
        public Dictionary<string, double> Propensities
        {
            get
            {
                AgentProfile profile = (AgentProfile)Profile;

                return profile.Propensities;
            }
        }

        /// <summary>
        /// Returns whether this agent is active
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }

        /// <summary>
        /// Returns whether this agent has completed its journey
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return _isComplete;
            }
            set
            {
                _isComplete = value;
            }
        }
        #endregion

        #region utility methods
        /// <summary>
        /// Returns a decision making propensity
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public double GetPropensity(string type)
        {
            AgentProfile profile = (AgentProfile)Profile;

            return profile.GetPropensity(type);
        }

        /// <summary>
        /// Adds a list of grid indexes to the Agent paths
        /// </summary>
        /// <param name="path"></param>
        public void AddPath(List<int> path)
        {
            _paths.Add(path);
        }

        /// <summary>
        /// Toggle whether this Agent Entity is active in the Simulation
        /// </summary>
        public void ToggleActive()
        {
            _isActive = !_isActive;
        }

        /// <summary>
        /// Toggle whether this Agent Entity has completed its journey
        /// </summary>
        public void ToggleComplete()
        {
            _isComplete = !_isComplete;
        }
        #endregion

        #region step methods
        /// <summary>
        /// Returns a list of grid indexes for waiting
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private Tuple<Node, List<int>> Wait(int time)
        {
            List<int> path = new List<int>();

            Node position = Environment.GetNode(Position);

            for (int i=0; i<time; i++)
            {
                path.Add((int)GridIndex);
            }

            return new Tuple<Node, List<int>> (position, path);
        }

        /// <summary>
        /// Returns a list of grid indexes for moving
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private Tuple<Node, List<int>> Move(List<Node> nodes)
        {
            List<int> path = new List<int>();
            List<Node> goalNodes = new List<Node>(nodes);

            Node position = Environment.GetNode(Position);
            Node goal = Destination;

            for (int i=goalNodes.Count-1; i>=0; i--)
            {
                if (Visited.Contains(goalNodes[i]) ||
                    !Propensities.Keys.Contains(goalNodes[i].Name))
                {
                    goalNodes.RemoveAt(i);
                }
            }

            if (goalNodes.Count > 0)
            {
                Random random = new Random();
                
                for (int i = goalNodes.Count - 1; i >= 0; i--)
                {
                    if (random.NextDouble() > GetPropensity(goalNodes[i].Name))
                    {
                        goalNodes.RemoveAt(i);
                    }
                }

                if (goalNodes.Count > 0)
                {
                    goalNodes.Sort(delegate (Node a, Node b)
                    {
                        Tuple<Node, Node> ca = new Tuple<Node, Node>(position, a);
                        Tuple<Node, Node> cb = new Tuple<Node, Node>(position, b);

                        int la = Environment.NodeGraph.Distances[ca].Count;
                        int lb = Environment.NodeGraph.Distances[cb].Count;

                        return lb - la;

                    });

                    Stack<Node> goalNodesStack = new Stack<Node>(goalNodes);

                    while (goalNodes.Count > 0)
                    {
                        goal = goalNodesStack.Pop();

                        //int goalPopulation = goal.Population;

                        int goalPopulation = 0;
                        int goalCapacity = goal.Capacity;

                        if (goalPopulation < goalCapacity)
                        {
                            break;
                        }
                        else
                        {
                            if (random.NextDouble() < GetPropensity("queuing"))
                            {
                                path = Environment.GetNodeShortestPath(position, goal);

                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    goal = Destination;
                    path = Environment.GetNodeShortestPath(position, goal);
                }
            }
            else
            {
                goal = Destination;
                path = Environment.GetNodeShortestPath(position, goal);
                ToggleComplete();
            }

            return new Tuple<Node, List<int>>(goal, path);
        }
        #endregion

        #region main methods
        /// <summary>
        /// Initialize this Agent on the Environment
        /// </summary>
        public void Init(SimulationEnvironment environment)
        {
            Environment = environment;
            
            Paths = new List<List<int>>();
            Visited = new List<Node>();

            Visited.Add(Origin);
            Position = Origin.Position;
            State = "waiting";

            ToggleActive();
        }

        /// <summary>
        /// This is the Agent's main movement method
        /// </summary>
        public void Step()
        {
            if (IsActive)
            {
                if (State == "ready")
                {
                    Node position = Environment.GetNode(Position);
                    List<Node> edges = Environment.NodeGraph.Edges[position].ToList();
                    Tuple<Node, List<int>> move = Move(edges);

                    // add density here

                    Node goal = move.Item1;
                    List<int> path = move.Item2;

                    Paths.Add(path);
                    path.Reverse();

                    Stack = new Stack<int>(path);
                    State = "active";

                }
                else if (State == "waiting")
                {
                    Node position = Environment.GetNode(Position);
                    Tuple<Node, List<int>> wait = Wait(1);

                    // add density here

                    Node goal = wait.Item1;
                    List<int> path = wait.Item2;

                    Paths.Add(path);
                    path.Reverse();

                    Stack = new Stack<int>(path);
                    State = "active";

                }
                else if (State == "active")
                {
                    if (Stack.Count > 0)
                    {
                        int next = Stack.Pop();

                        // do shifting calculations here

                        Age++;

                    }
                    else
                    {
                        if (false)
                        {
                            State = "waiting";
                        }
                        else if (true)
                        {
                            State = "ready";
                        }
                        if (IsComplete)
                        {
                            State = "complete";

                            ToggleActive();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
