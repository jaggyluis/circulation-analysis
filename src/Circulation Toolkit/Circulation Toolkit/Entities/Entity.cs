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
    public class Entity
    {
        private Profile _profile;
        private Dictionary<int, Point3d> _positions;

        public Entity(Profile profile)
        {
            Profile = profile;
            Positions = new Dictionary<int, Point3d>();
        }

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

        public Point3d GetPosition(int gen)
        {
            if (Positions.ContainsKey(gen))
            {
                return Positions[gen];
            }
            else
            {
                // needs to be nullable
                return null;
            }
        }

        public void SetPosition(Point3d position, int gen = -1)
        {
            if (gen == -1)
            {
                Positions[Positions.Last().Key] = position;
            }
            else
            {
                Positions[gen] = position;         
            }
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
                return "None";
            }
        }

        public void SetAttribute(string attribute, string value)
        {
            Profile.Attributes[attribute] = value;
        }

        public bool HasValue(string value)
        {
            return Profile.HasValue(value);
        }
    }

}
