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

        public Agent(AgentProfile profile, SimulationEnvironment environment)
            : base (profile)
        {
            Environment = environment;
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
        }

        /// <summary>
        /// The Floor Entity that this Agent is currently on
        /// </summary>
        public Floor Floor
        {
            get
            {
                return Environment.GetFloor(Profile.GetAttribute("floor"));
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
                return "temp";
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
        /// Returns all the grid indexes that this agent has walked
        /// </summary>
        public List<int> Path
        {
            get
            {
                List<int> path = new List<int>();

                foreach (List<int> _p in _paths)
                {
                    foreach (int i in _p)
                    {
                        path.Add(i);
                    }
                }

                return path;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void ToggleActive()
        {
            _isActive = !_isActive;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ToggleComplete()
        {
            _isComplete = !_isComplete;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private Tuple<Node, List<int>> Move(List<Node> nodes)
        {
            List<int> path = new List<int>();
            List<Node> goalNodes = new List<Node>(nodes);

            Node position = Environment.GetNode(Position);
            Node goal;

            foreach (Node node in goalNodes)
            {
                if (Visited.Contains(node))
                {
                    goalNodes.Remove(node);
                }
            }

            if (goalNodes.Count > 0)
            {
                Dictionary<string, double> propensities = Propensities;
                List<string> types = new List<string>(Propensities.Keys);
                Random random = new Random();

                foreach (string type in types)
                {
                    if (random.NextDouble() < propensities[type])
                    {
                        types.Remove(type);
                    }
                }

                // still need to filter by whether node exists

                if (types.Count > 0)
                {

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

        /// <summary>
        /// This is the Agent's main movement method
        /// </summary>
        public void Step()
        {
            if (isActive)
            {

            }
            else
            {

            }
        }
    }
}
