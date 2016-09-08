using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CirculationToolkit.Util;
using Rhino.Geometry;


namespace CirculationToolkit.Entities
{
    /// <summary>
    /// Main Barrier Class that stores information for the Barriers in the scene
    /// </summary>
    public class Barrier : Entity
    {
        private Curve _geometry;
        private Bounds2d _bounds;

        /// <summary>
        /// Barrier Entity Constructor that takes a Barrier Profile and a Curve
        /// representing the edge of the Barrier
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="geometry"></param>
        public Barrier(Profile profile, Curve geometry)
            : base (profile)
        { 
            Geometry = geometry;
            Bounds = new Bounds2d(Geometry);
        }

        /// <summary>
        /// Duplicate this Barrier Entity
        /// </summary>
        /// <returns></returns>
        public override Entity Duplicate()
        {
            Barrier duplicate = new Barrier(Profile, Geometry);

            return duplicate;
        }

        #region properties
        /// <summary>
        /// Returns the Barrier Entity Geometry
        /// </summary>
        public Curve Geometry
        {
            get
            {
                return _geometry;
            }
            set
            {
                _geometry = value;
            }
        }

        /// <summary>
        /// Returns the Barrier Entity Bounds2d
        /// </summary>
        public Bounds2d Bounds
        {
            get
            {
                return _bounds;
            }
            set
            {
                _bounds = value;
            }
        }

        /// <summary>
        /// Returns the name of the Floor Entity that this Barrier is on
        /// </summary>
        public string Floor
        {
            get
            {
                return GetAttribute("floor");
            }
        }
        #endregion
    }
}
