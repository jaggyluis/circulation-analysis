using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CirculationToolkit.Graph;
using CirculationToolkit.Geometry;
using CirculationToolkit.Profiles;
using Rhino.Geometry;


namespace CirculationToolkit.Entities
{
    /// <summary>
    /// Main Floor Class that stores information for the Circulation Environment
    /// </summary>
    public class Floor : Entity
    {
        private Curve _geometry;
        private Bounds2d _bounds;
        private Mesh _mesh;
        private List<Point3d> _grid;
        private double _gridSize;
        private FloorGraph<int> _floorGraph;
        private Dictionary<Tuple<double, double>, int> _coordinates;

        #region constructors
        /// <summary>
        /// Floor Entity constructor
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="geometry"></param>
        /// <param name="coordinates"></param>
        /// <param name="grid"></param>
        private Floor(Profile profile, Curve geometry, Dictionary<Tuple<double, double>, int> coordinates, List<Point3d> grid)
            : base (profile)
        {
            _geometry = geometry;
            _coordinates = coordinates;
            _grid = grid;
            _bounds = new Bounds2d(geometry);

            Position = Bounds.Origin;
        }

        /// <summary>
        /// Floor Entity constructor that takes a FloorProfile and a Geometry curve
        /// representing the edge of the floor
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="geometry"></param>
        public Floor(Profile profile, Curve geometry)
            : this (profile, geometry, new Dictionary<Tuple<double, double>, int>(), new List<Point3d>())
        {
        }

        /// <summary>
        /// Duplicate this Floor
        /// </summary>
        /// <returns></returns>
        public override Entity Duplicate()
        {
            Floor floor = new Floor(Profile, Geometry, Coordinates, Grid);

            floor.Mesh = Mesh;
            floor.GridSize = GridSize;
            floor.FloorGraph = FloorGraph;

            return floor;
        }
        #endregion

        #region properties
        /// <summary>
        /// Returns the Floor Entity Geometry
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
        /// Returns the index of a Point3d in the Grid from a set of Coordinates
        /// </summary>
        public Dictionary<Tuple<double, double>, int> Coordinates
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
        /// Returns the Floor Entity FloorGraph
        /// </summary>
        public FloorGraph<int> FloorGraph
        {
            get
            {
                return _floorGraph;
            }
            set
            {
                _floorGraph = value;
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
        #endregion

        #region utility methods
        /// <summary>
        /// Creates the Grid object used for Agent Entity movement
        /// </summary>
        /// <param name="gridSize"></param>
        public void SetGrid(double gridSize)
        {
            GridSize = gridSize;
            Grid = new List<Point3d>();
            Mesh = Bounds.GetGrid(gridSize);
            FloorGraph = new FloorGraph<int>(this);

            List<int> indexes = new List<int>();
            double denom = Math.Sqrt(2 * Math.Pow(gridSize, 2));

            MeshingParameters parameters = new MeshingParameters();
            Mesh mesh = Mesh.CreateFromPlanarBoundary(Geometry, parameters);

            for (int i=0; i<Mesh.Faces.Count; i++)
            {
                Point3d pt = Mesh.Faces.GetFaceCenter(i);
                Line line = new Line(pt, new Vector3d(0, 0, -1));
                int[] intersections;

                Rhino.Geometry.Intersect.Intersection.MeshLine(mesh, line, out intersections);
                if (intersections == null)
                {
                    indexes.Add(i);
                }
            }

            Mesh.Faces.DeleteFaces(indexes);

            for (int i=0; i<Mesh.Faces.Count; i++)
            {
                Point3d pt = Mesh.Faces.GetFaceCenter(i);

                Grid.Add(pt);
                SetCoord(GetCoord(pt), i);
            }

            for (int i=0; i< Mesh.Vertices.Count; i++)
            {
                int[] edges = Mesh.TopologyVertices.ConnectedFaces(i);

                foreach (int e1 in edges)
                {
                    foreach (int e2 in edges)
                    {
                        if (e1 != e2)
                        {
                            double weight = Grid[e1].DistanceTo(Grid[e2]);
                            FloorGraph.AddEdge(e1,e2, weight);
                        }
                    }
                }
            }        
        }

        /// <summary>
        /// Returns the Point3d on the grid at the given key coordinates
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Point3d GetCoordPoint(Tuple<double, double> coord)
        {
            return Grid[Coordinates[coord]];
        }

        /// <summary>
        /// Returns the Point3d on the grid at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Point3d? GetGridIndexPoint(int index)
        {
            if (Grid[index] != null)
            {
                return Grid[index];
            }

            return null;
        }

        /// <summary>
        /// Returns the grid coordinate of a grid index
        /// - this should be optimized
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Tuple<double, double> GetGridIndexCoord(int index)
        {
            Dictionary<int, Tuple<double, double>> mappedDict = new Dictionary<int, Tuple<double, double>>();

            foreach (Tuple<double, double> key in Coordinates.Keys)
            {
                mappedDict[Coordinates[key]] = key;
            }

            return mappedDict[index];
        }

        /// <summary>
        /// Returns a Bounds2d representation of a point on the Grid
        /// for containment tests
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private Bounds2d GetGridUnit(Point3d pt)
        {
            return Bounds2d.FromCenterPoint(pt, GridSize, GridSize);
        }

        /// <summary>
        /// Assigns (X,Y) values to an index on the grid 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="coord"></param>
        public void SetCoord(Tuple<double, double> key, int coord)
        {
            Coordinates[key] = coord;
        }

        /// <summary>
        /// Returns the (X,Y) coordinates of a Point3d
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Tuple<double, double> GetCoord(Point3d pt)
        {
            double gridX = Bounds.DimX / Math.Floor(Bounds.DimX / GridSize);
            double gridY = Bounds.DimY / Math.Floor(Bounds.DimY / GridSize);

            //double csX = Math.Floor(Bounds.DimX / gridX);
            //double csY = Math.Floor(Bounds.DimY / gridY);

            double ptX = Math.Floor((pt.X - Position.X) / gridX);
            double ptY = Math.Floor((pt.Y - Position.Y) / gridY);

            return new Tuple<double, double>(ptX, ptY);
        }

        /// <summary>
        /// Return the Coordinates of all neighbors at a coordinate
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<Tuple<double, double>> GetCoordNeighbors(Tuple<double, double> key)
        {
            List<Tuple<double, double>> neighbors = new List<Tuple<double, double>>();

            for (int i=-1; i<2; i++)
            {
                for (int j=-1; j<2; j++)
                {
                    neighbors.Add(new Tuple<double, double>(key.Item1 + i, key.Item2 + j));
                }
            }

            return neighbors;

        }

        /// <summary>
        /// Returns the Index of a Point3d in the Grid fron a Coordinate and a Point3d
        /// This is a helper method for GetPointIndex
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        private int? GetCoordGridIndex(Tuple<double, double> key, Point3d pt)
        {
            int? index = null;

            if (Coordinates.ContainsKey(key))
            {
                index = Coordinates[key];
            }
            else
            {
                Tuple<double, double> nearest = Search(key, pt);

                if (nearest != null)
                {
                    index = Coordinates[nearest];
                }
            }

            return index;
        }

        /// <summary>
        /// Returns the Index of a Point3d in the Grid
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public int? GetPointGridIndex(Point3d pt)
        {
            return GetCoordGridIndex(GetCoord(pt), pt);
        }

        /// <summary>
        /// Returns the closest grid coordinates to a given Point3d
        /// This is a helper method for GetCoordIndex
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        private Tuple<double, double> Search(Tuple<double, double> key, Point3d pt)
        {
            List<Tuple<double, double>> neighbors;
            List<Tuple<Tuple<double, double>, Point3d>> values;

            neighbors = GetCoordNeighbors(key);
            values = new List<Tuple<Tuple<double, double>, Point3d>>();

            foreach (Tuple<double, double> n in neighbors)
            {
                if (Coordinates.ContainsKey(n))
                {
                    values.Add(new Tuple<Tuple<double, double>, Point3d>(n, GetCoordPoint(n)));
                }
            }

            values.Sort(delegate(Tuple<Tuple<double, double>, Point3d> t1, 
                Tuple<Tuple<double, double>, Point3d> t2)
            {
                double d1 = pt.DistanceTo(t1.Item2);
                double d2 = pt.DistanceTo(t2.Item2);

                if (d2 < d1)
                {
                    return -1;
                }
                else if (d2 > d1)
                {
                    return 1;   
                }
                else
                {
                    return 0;
                }
            });

            if (values.Count != 0)
            {
                return values[0].Item1;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region tests
        /// <summary>
        /// Tests a Point3d for containment on a Floor Entity
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool ContainsPoint(Point3d pt)
        {
            if (Bounds.Contains(pt))
            {
                if (Geometry.Contains(pt) == PointContainment.Inside)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region FloorGraph methods
        /// <summary>
        /// Tests a Floor point for edge conditions
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        protected bool IsEdgeVertex(int testIndex)
        {
            List<double> barrierValues = new List<double>();

            foreach(int nodeIndex in FloorGraph.Edges[testIndex])
            {
                barrierValues.Add(FloorGraph.GetBarrierMapNodeValue(nodeIndex));
            }
            if (barrierValues.Count != 8 || barrierValues.Contains(double.MaxValue))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates the Weighted Floor edges for Agent Decisions
        /// </summary>
        public void AddEdgeMap()
        {
            for (var i=0; i<Grid.Count; i++)
            {
                if (FloorGraph.GetBarrierMapNodeValue(i) != double.MaxValue)
                {
                    List<int> keys = new List<int> { i };
                    int maxSearchDepth = 5;
                    Dictionary<int, int?> search = FloorGraph.ShallowSearch(keys, IsEdgeVertex, maxDepth: maxSearchDepth);
                    List<int> values = new List<int>();

                    foreach (int? v in search.Values)
                    {
                        if (v != null)
                        {
                            values.Add((int)v);
                        }
                    }
                    values.Sort();
                    if (values.Count > 0)
                    {
                        double weight = 1 / Math.Pow((GridSize * (values[0] + 1)), 2);
                        FloorGraph.AddBarrierMapNodeValue(i, weight);
                    }
                }
            }
        }

        /// <summary>
        /// Adds Barrier Entities to the floor
        /// This should be upadated to use a quad-tree
        /// </summary>
        /// <param name="barrier"></param>
        public List<int> __AddBarrierMap(Barrier barrier)
        {
            List<Point3d> points = barrier.Geometry.DivideEquidistant(GridSize/2).ToList();
            List<int> indexes = new List<int>();

            for (var i=0; i<Grid.Count; i++)
            {
                Bounds2d unitBounds = GetGridUnit(Grid[i]);

                if (barrier.Bounds.Intersects(unitBounds))
                {
                    bool contains = false;
                    
                    for (int j = 0; j < unitBounds.Points.Count; j++)
                    {
                        if (barrier.Geometry.Contains(unitBounds.Points[j]) == PointContainment.Inside)
                        {
                            FloorGraph.AddBarrierMapNodeValue(i, double.MaxValue);
                            indexes.Add(i);
                            contains = true;
                            break;
                        }
                    }

                    if (contains) continue;
                                     
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (unitBounds.Contains(points[k]))
                        {
                            FloorGraph.AddBarrierMapNodeValue(i, double.MaxValue);
                            indexes.Add(i);
                            contains = true;
                            break;
                        }
                    }
                }
            }

            return indexes;        
        }

        /// <summary>
        /// Temporary quick fix
        /// </summary>
        /// <param name="barrier"></param>
        public List<int> _AddBarrierMap(Barrier barrier)
        {
            List<int> indexes = new List<int>();

            for (var i = 0; i < Grid.Count; i++)
            {
                if (barrier.Geometry.Contains(Grid[i]) == PointContainment.Inside)
                {
                    FloorGraph.AddBarrierMapNodeValue(i, double.MaxValue);
                    indexes.Add(i);
                }
            }

            return indexes;
        }

        public List<int> AddBarrierMap(Barrier barrier)
        {
            List<Point3d> points = barrier.Geometry.DivideEquidistant(GridSize / 2).ToList();
            List<int> indexes = new List<int>();

            for (var i = 0; i < Grid.Count; i++)
            {
                Bounds2d unitBounds = GetGridUnit(Grid[i]);

                if (barrier.Bounds.Intersects(unitBounds))
                {
                    int count = 0;

                    for (int j = 0; j < unitBounds.Points.Count; j++)
                    {
                        if (barrier.Geometry.Contains(unitBounds.Points[j]) == PointContainment.Inside)
                        {
                            count++;
                        }
                    }

                    if (count > 2 || barrier.Geometry.Contains(Grid[i]) == PointContainment.Inside)
                    {
                        FloorGraph.AddBarrierMapNodeValue(i, double.MaxValue);
                        indexes.Add(i);
                    }
                }
            }

            return indexes;
        }

        /// <summary>
        /// Adds Occupancy at specific generations
        /// </summary>
        /// <param name="index"></param>
        /// <param name="gen"></param>
        public void AddOccupancy(int index, int gen)
        {
            FloorGraph.AddOccupancyMapNodeValue(index, gen);
        }

        public void RemoveOccupancy(int index, int gen)
        {
            FloorGraph.RemoveOccupancyMapNodeValue(index, gen);
        }

        /// <summary>
        /// Returns the Occupancy at a grid index at a generation
        /// </summary>
        /// <param name="index"></param>
        /// <param name="gen"></param>
        /// <returns></returns>
        public int GetOccupancy(int index, int gen)
        {
            return FloorGraph.GetOccupancyMapNodeValue(index, gen);
        }
        #endregion
    }
}
