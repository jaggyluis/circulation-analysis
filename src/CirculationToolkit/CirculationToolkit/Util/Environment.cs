using CirculationToolkit.Entities;
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

        private Dictionary<string, List<Entity>> _entities;

        public SimulationEnvironment(double resolution)
        {
            Entities = new Dictionary<string, List<Entity>>()
            {
                {"floor", new List<Entity>() },
                {"barrier", new List<Entity>() },
                {"node", new List<Entity>() },
                {"template", new List<Entity>() },
                {"agent", new List<Entity>() },
            };
        }

        /// <summary>
        /// Returns the string representation of the Environment
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Environment: " +
                Floors.Count + " floors" + ", " + 
                Barriers.Count + " barriers" ;
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
        #endregion

        #region main methods
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        private void BuildBarriers()
        {
            foreach (Barrier barrier in Barriers)
            {
                foreach (Floor floor in Floors)
                {
                    if (floor.Name == barrier.Floor)
                    {
                        floor.AddBarrierMap(barrier);
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
        }
        #endregion

    }
}
