using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CirculationToolkit.Profiles;
using CirculationToolkit.Util;
using Rhino.Geometry;


namespace CirculationToolkit.Entities
{
    /// <summary>
    /// Main Floor class that stores information for the Circulation Environment
    /// </summary>
    public class Floor : Entity
    {

        private Curve _boundary;
        private Mesh _mesh;
        private double _gridSize;
        private Dictionary<int, int> _coordinates;
        private Bounds2d _bounds;
        private Map _map;
        private List<Point3d> _grid;      

        public Floor(FloorProfile profile, Curve boundary)
            : base(profile)
        {
            Boundary = boundary;
            Coordinates = new Dictionary<int, int>();
            //SetPosition()
        }

        #region properties
        /// <summary>
        /// Returns the Floor Entity boundary
        /// </summary>
        public Curve Boundary
        {
            get
            {
                return _boundary;
            }
            set
            {
                _boundary = value;
            }
        }

        /// <summary>
        /// Returns the mesh representation of this Floor Entity
        /// </summary>
        public Mesh Mesh
        {
            get
            {
                return _mesh;
            }
            set
            {
                _mesh = value;
            }
        }

        /// <summary>
        /// Returns the current grid size for this Floor Entity
        /// </summary>
        public double GridSize
        {
            get
            {
                return _gridSize;
            }
            set
            {
                _gridSize = value;
            }
        }

        /// <summary>
        /// Returns the list of all points currently on the Floor grid
        /// </summary>
        public List<Point3d> Grid
        {
            get
            {
                return _grid;
            }
            set
            {
                _grid = value;
            }
        }


        /// <summary>
        /// Returns the Floor Entity coordinates
        /// </summary>
        public Dictionary<int, int> Coordinates
        {
            get
            {
                return _coordinates;
            }
            set
            {
                _coordinates = value;
            }
        }

        /// <summary>
        /// Returns the Floor Entity Map Graph
        /// </summary>
        public Map Map
        {
            get
            {
                return _map;
            }
            set
            {
                _map = value;
            }
        }

        /// <summary>
        /// Returns the Floor Entity Bounds2d
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


        //public Grid2d Grid
        #endregion

        #region utility methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridSize"></param>
        public void SetGrid(double gridSize)
        {
            GridSize = gridSize;
            Mesh = Bounds.GetGrid(gridSize);
            Map = new Map(this);

            List<int> indexes = new List<int>();
            double denom = Math.Sqrt(2 * Math.Pow(gridSize, 2));

            MeshingParameters parameters = new MeshingParameters();
            Mesh mesh = Mesh.CreateFromPlanarBoundary(Boundary, parameters);

            for (int i=0; i<mesh.Faces.Count; i++)
            {
                Point3d pt = mesh.Faces.GetFaceCenter(i);
                Line line = new Line(pt, new Vector3d(0, 0, -1));
                int[] intersections;

                Rhino.Geometry.Intersect.Intersection.MeshLine(mesh, line, out intersections);
                if (intersections.Count() == 0)
                {
                    indexes.Add(i);
                }
            }

            mesh.Faces.DeleteFaces(indexes);

            for (int i=0; i<mesh.Faces.Count; i++)
            {
                Point3d pt = mesh.Faces.GetFaceCenter(i);

                Grid.Add(pt);



            }

            
            


        }

        public void SetCoord(string key, int coord)
        {

        }

        public void GetCoord(Point3d pt)
        {
            double gridX = Bounds.DimX / Math.Floor(Bounds.DimX / GridSize);
            double gridY = Bounds.DimY / Math.Floor(Bounds.DimY / GridSize);
        }
        #endregion
    }


}
