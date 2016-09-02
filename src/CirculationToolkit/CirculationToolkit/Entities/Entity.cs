using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CirculationToolkit.Profiles;
using Rhino.Geometry;

namespace CirculationToolkit.Entities
{
    /// <summary>
    /// Main Entity class that stores information for the Circulation Environment
    /// </summary>
    public abstract class Entity
    {
        private Profile _profile;
        private Dictionary<int, Point3d> _positions;

        /// <summary>
        /// Entity Constructor that takes a Profile containing Entity Attributes
        /// </summary>
        /// <param name="profile"></param>
        public Entity(Profile profile)
        {
            Profile = profile;
            Positions = new Dictionary<int, Point3d>();
        }

        #region properties
        /// <summary>
        /// Returns the Entity type
        /// </summary>
        public string Type
        {
            get
            {
                return _profile.Type;
            }
            set
            {
                _profile.Type = value;
            }
        }

        /// <summary>
        /// Returns the Entity name
        /// </summary>
        public string Name
        {
            get
            {
                return _profile.Name;
            }
            set
            {
                _profile.Name = value;
            }
        }

        /// <summary>
        /// Returns the Entity string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Type + " entity" + ": " + Name;
        }

        public Profile Profile
        {
            get
            {
                return _profile;
            }
            set
            {
                _profile = value;
            }
        }

        /// <summary>
        /// Returns the last/current position assigned to the Entity
        /// </summary>
        public Point3d Position
        {
            get
            {
                List<int> keys = Positions.Keys.ToList();
                keys.Sort();
                keys.Reverse();

                for (int i=0; i< keys.Count; i++)
                {
                    Point3d? pos = GetPosition(keys[i]);

                    if (pos.HasValue)
                    {
                        return (Point3d)pos;
                    }
                }

                return new Point3d(0, 0, 0);
            }
            set
            {
                Positions[Positions.Last().Key] = value;
            }
        }

        /// <summary>
        /// Returns A Dictionary of all the Entity's positions
        /// during the simulation with their step keys
        /// </summary>
        public Dictionary<int, Point3d> Positions
        {
            get
            {
                return _positions;
            }
            set
            {
                _positions = value;
            }
        }
        #endregion

        #region utility methods
        /// <summary>
        /// Returns a specific Position at a given generation
        /// or null if it does not exist
        /// </summary>
        /// <param name="gen"></param>
        /// <returns></returns>
        public Point3d? GetPosition(int gen)
        {
            if (Positions.ContainsKey(gen))
            {
                return Positions[gen];
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// Sets the position at a specified generation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="gen"></param>
        public void SetPosition(Point3d position, int gen)
        {
            Positions[gen] = position;         
        }

        public bool HasAttribute(string attribute)
        {
            return Profile.HasAttribute(attribute);
        }

        public string GetAttribute(string attribute)
        {
            if (HasAttribute(attribute))
            {
                return Profile.Attributes[attribute];
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Sets an Attribute of the Entity's Profile
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public void SetAttribute(string attribute, string value)
        {
            Profile.SetAttribute(attribute, value);
        }

        /// <summary>
        /// Returns whether an Entity's attributes conatin a value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool HasValue(string value)
        {
            return Profile.HasValue(value);
        }
        #endregion
    }

}
