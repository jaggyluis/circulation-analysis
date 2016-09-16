using CirculationToolkit.Util;
using CirculationToolkit.Profiles;
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
     
        private bool _isActive;
        private bool _isComplete;

        private List<string> _states;
        private List<List<int>> _paths;
        private List<Node> _visited;

        private Stack<int> _stack;

        private List<string> _log;

        private Node _last;
        private Node _curr;
        private Node _next;

        public Agent(AgentProfile profile)
            : base (profile)
        {
        }

        public override Entity Duplicate()
        {
            Agent duplicate = new Agent((AgentProfile)Profile);

            duplicate.Paths = Paths;
            duplicate.Visited = Visited;

            duplicate.Age = Age;
            duplicate._states = _states;
            duplicate.IsActive = IsActive;
            duplicate.IsComplete = IsComplete;

            duplicate.Stack = Stack;

            duplicate.Last = Last;
            duplicate.Current = Current;
            duplicate.Next = Next;


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
        /// Returns the last Node this Agent was at
        /// </summary>
        public Node Last
        {
            get
            {
                return _last;
            }
            set
            {
                _last = value;
            }
        }

        /// <summary>
        /// Returns the current Node this Agent is at,
        /// which is used during path calculations and floor calculations
        /// </summary>
        public Node Current
        {
            get
            {
                return _curr;
            }
            set
            {
                _curr = value;
            }
        }

        /// <summary>
        /// Returns the next node this Agent will be at
        /// </summary>
        public Node Next
        {
            get
            {
                return _next;
            }
            set
            {
                _next = value;
            }
        }

        /// <summary>
        /// The Floor Entity that this Agent is currently on
        /// </summary>
        public Floor Floor
        {
            get
            {
                return Environment.GetFloor(Current.Floor);
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
                return _states.Count > 0 ? _states[_states.Count - 1] : "None";
            }
            set
            {
                Log("state change - " + State + " to " + value + " at age " + Age.ToString());

                _states.Add(value);
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
        /// Return the previous state
        /// </summary>
        /// <returns></returns>
        public string GetLastState()
        {
            if (_states.Count > 1)
            {
                return _states[_states.Count - 2];
            }
            else
            {
                return _states[0];
            }
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
        private Tuple<Node, List<int>> Wait(Node position)
        {
            //
            // waiting distribution needs to be implemeted
            //

            List<int> path = new List<int>();
            Random random = new Random(Guid.NewGuid().GetHashCode());

            double waitTime = random.Next(1, 10);

            Log("waiting " + waitTime.ToString());

            for (int i=0; i<waitTime; i++)
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
        private Tuple<Node, List<int>> Move(Node position)
        {
            List<Node> goalNodes = Environment.NodeGraph.Edges[position].ToList();
            Node goal = Destination;

            int positionIndex = (int)Floor.GetPointGridIndex(position.Position);           

            for (int i=goalNodes.Count-1; i>=0; i--)
            {
                if (Visited.Contains(goalNodes[i]))
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
                }
            }
            else
            {
                goal = Destination;
                              
                ToggleComplete();
            }
            
            int goalIndex = (int)Floor.GetPointGridIndex(goal.Position);
            List<int> path = Floor.FloorGraph.ShortestPath(positionIndex, goalIndex, startIndex:Age);           
            
            return new Tuple<Node, List<int>>(goal, path);
        }
        #endregion

        #region main methods
        /// <summary>
        /// Sets a string to the log
        /// </summary>
        /// <param name="logInfo"></param>
        public void Log(string logInfo)
        {
            _log.Add(logInfo);
        }

        /// <summary>
        /// Returns the Agent log
        /// </summary>
        /// <returns></returns>
        public List<string> Log()
        {
            return _log;
        }
        /// <summary>
        /// Initialize this Agent on the Environment
        /// </summary>
        public void Init(SimulationEnvironment environment)
        {
            Environment = environment;
            
            Paths = new List<List<int>>();
            Visited = new List<Node>();
            _states = new List<string>();
            _log = new List<string>();

            Visited.Add(Origin);
            Position = Origin.Position;
            Current = Origin;
            Age = 0;

            State = "waiting";

            ToggleActive();
        }

        /// <summary>
        /// Shift the Agent Path based on density and velocity
        /// </summary>
        /// <param name="path"></param>
        /// <param name="age"></param>
        /// <returns></returns>
        private List<int> Shift(List<int> path, int age)
        {
            List<int> shiftedPath = new List<int>();

            int maxDensity = 1;
            int maxCount = (int)Math.Floor(maxDensity * Math.Pow(Floor.GridSize, 2));
            int i = 0;
            int j = age;

            while (i < path.Count)
            {            
                if (1 < i  && i < path.Count-1)
                {                 
                    int lastCount = Floor.GetOccupancy(path[i - 1], j-1);
                    int nextCount = Floor.GetOccupancy(path[i], j);

                    if (nextCount > maxCount &&
                        lastCount <= maxCount)
                    {
                        i--;
                    }
                    else if (nextCount >= 4*maxCount)
                    {
                        i--;
                    }
                }

                shiftedPath.Add(path[i]);

                i++;
                j++;
            }

            return shiftedPath;
        }

        private List<int> Flow(List<int> path, int age)
        {
            /// Pedestrian Traffic flow equation : f = s/a where
            /// f = volume in pedestrians per width per minute
            /// s = average pedestrian speed in meters per min. Average is 1.26
            /// a = average area per pedestrian in square meters. Average is 2.32

            List<int> shiftedPath = new List<int>();
            
            double speed = (Floor.GridSize / 1);
            double area = Math.Pow(Floor.GridSize, 2);

            int i = 0;

            while (i < path.Count)
            {
                if (1 < i && i < path.Count -1 )
                {
                    int j = 0;   
                                           
                    while (j < speed)
                    {
                        shiftedPath.Add(path[i]);
                        age++;
                        j++;

                        /*
                        if (j == speed - 1 )
                        {
                            int lastCount = Floor.GetOccupancy(path[i - 1], age - 1);
                            int nextCount = Floor.GetOccupancy(path[i], age);

                            if (nextCount > maxCount &&
                                lastCount <= maxCount)
                            {
                                j--;
                            }
                            else if (nextCount >= 2 * maxCount)
                            {
                                j--;
                            }
                        }
                        */                      
                    }                  
                }
                else
                {
                    shiftedPath.Add(path[i]);
                    age++;
                }

                i++;
            }

            return shiftedPath;
        }

        /// <summary>
        /// Sets the position and directive during the active state
        /// of the Agent
        /// </summary>
        /// <param name="directive"></param>
        private void SetDirective(Tuple<Node, List<int>> directive)
        {
            Next = directive.Item1;
            List<int> path = directive.Item2;

            if (State != "waiting")
            {
                path = Shift(path, Age);
                //path = Flow(path, Age);
            }            

            path.Reverse();
            Stack = new Stack<int>(path);
            
            path.Reverse();
            Paths.Add(path);

            for (int i = 0; i < path.Count; i++)
            {
                Floor.AddOccupancy(path[i], Age + i);
                SetPosition((Point3d)Floor.GetGridIndexPoint(path[i]), Age + i);
            }       
            
            State = "active";
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
                    SetDirective(Move(Current));
                }
                else if (State == "waiting")
                {
                    SetDirective(Wait(Current));
                }
                else if (State == "active")
                {
                    if (Stack.Count > 0)
                    {
                        Stack.Pop();
                        Age++;
                    }
                    else
                    {
                        Visited.Add(Current);
                        Last = Current;
                        Current = Next;

                        if (GetLastState() == "ready")
                        {
                            State = "waiting";
                        }
                        else if (GetLastState() == "waiting")
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
