using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CirculationToolkit.Profiles;
using Rhino.Geometry;

namespace CirculationToolkit.Entities
{
    /// <summary>
    /// Main Template Class that stores information for the Templates in the scene
    /// </summary>
    public class Template : Entity
    {
        private List<Tuple<Point3d, Point3d>> _edges;

        /// <summary>
        /// Template Entity Constructor that takes a TemplateProfile and a list of
        /// edges as Tuples of Point3ds
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="edges"></param>
        public Template(Profile profile, List<Tuple<Point3d, Point3d>> edges)
            : base(profile)
        {
            Edges = edges;
        }

        /// <summary>
        /// Duplicate this Template Entity
        /// </summary>
        /// <returns></returns>
        public override Entity Duplicate()
        {
            Template duplicate = new Template(Profile, Edges);

            return duplicate;
        }

        #region properties
        /// <summary>
        /// Returns the list of edges associated with this Template Entity
        /// </summary>
        public List<Tuple<Point3d, Point3d>> Edges
        {
            get
            {
                return _edges;
            }
            set
            {
                _edges = value;
            }
        }

        /// <summary>
        /// Returns whether the template is directec or not
        /// </summary>
        public bool Directed
        {
            get
            {
                bool directed = false;
                bool parsed = bool.TryParse(GetAttribute("directed"), out directed);

                if (parsed)
                {
                    return parsed;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion
    }
}
