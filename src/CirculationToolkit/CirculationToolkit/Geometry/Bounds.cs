﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rhino.Geometry;

namespace CirculationToolkit.Geometry
{
    /// <summary>
    /// 2 Dimensional Bounds Class
    /// </summary>
    public class Bounds2d
    {
        private Dictionary<string, double> _corners;

        #region constructors
        /// <summary>
        /// Bounds2d Constructor that takes a Curve as geometry
        /// </summary>
        /// <param name="geo"></param>
        public Bounds2d(Curve geo)
        {
            Polyline pline = null;
            geo.TryGetPolyline(out pline);

            _corners = GetCorners(pline.ToList());
        }
        /// <summary>
        /// Bounds2d Constructor that takes a list of Point3ds as geometry
        /// </summary>
        /// <param name="points"></param>
        public Bounds2d(List<Point3d> points)
        {
            _corners = GetCorners(points);
        }
        #endregion

        #region properties
        /// <summary>
        /// Returns the 4 corner points of the Bounds2d object
        /// </summary>
        public List<Point3d> Points
        {
            get
            {
                return new List<Point3d>()
                {
                    new Point3d(Corners["x1"], Corners["y1"], 0),
                    new Point3d(Corners["x2"], Corners["y1"], 0),
                    new Point3d(Corners["x2"], Corners["y2"], 0),
                    new Point3d(Corners["x1"], Corners["y2"], 0),
                };
            }
        }

        /// <summary>
        /// Returns a dictionary of the 4 corner points of the Bounds2d
        /// </summary>
        public Dictionary<string, double> Corners
        {
            get
            {
                return _corners;
            }
            set
            {
                _corners = value;
            }
        }

        /// <summary>
        /// Returns the X dimension of the Bounds2d object
        /// </summary>
        public double DimX
        {
            get
            {
                return Corners["x2"] - Corners["x1"];
            }
        }

        /// <summary>
        /// Returns the Y dimension of the Bounds2d object
        /// </summary>
        public double DimY
        {
            get
            {
                return Corners["y2"] - Corners["y1"];
            }
        }

        /// <summary>
        /// Returns the X Dimension Interval of this Bounds2d
        /// </summary>
        public Interval IvalX
        {
            get
            {
                return new Interval(Corners["x1"], Corners["x2"]);
            }
        }

        /// <summary>
        /// Returns the X Dimension Interval of this Bounds2d
        /// </summary>
        public Interval IvalY
        {
            get
            {
                return new Interval(Corners["y1"], Corners["y2"]);
            }
        }

        /// <summary>
        /// Returns the origin point of the Bounds2d object
        /// This is the bottom left corner
        /// </summary>
        public Point3d Origin
        {
            get
            {
                return Points[0];
            }
        }

        /// <summary>
        /// Returns the centroid of the corner points of the Bounds2d object
        /// </summary>
        public Point3d Centroid
        {
            get
            {
                double x = (Corners["x2"] - Corners["x1"]) / 2 + Corners["x1"];
                double y = (Corners["y2"] - Corners["y1"]) / 2 + Corners["y1"];

                return new Point3d(x, y, 0);
            }
        }

        /// <summary>
        /// Returns the plane of the Bounds 2d Object
        /// </summary>
        public Plane Plane
        {
            get
            {
                return new Plane(Origin, new Vector3d(0, 0, 1));
            }
        }

        /// <summary>
        /// Returns the Rectangle boundary of the Bounds2d object
        /// </summary>
        public Rectangle3d Rectangle
        {
            get
            {
                return new Rectangle3d(Plane, DimX, DimY);
            }
        }
        #endregion

        #region util methods
        /// <summary>
        /// Returns the mesh representation of this Bounds2d with a given grid size
        /// </summary>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        public Mesh GetGrid( double gridSize)
        {
            return MeshFromRect(Rectangle, gridSize);
        }
        
        /// <summary>
        /// Calculates the bounding corners of a set of Point3d objects
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public Dictionary<string, double> GetCorners(List<Point3d> points)
        {
            double x1 = points[0].X;
            double x2 = points[0].X;
            double y1 = points[0].Y;
            double y2 = points[0].Y;

            foreach (Point3d point in points)
            {
                if (point.X < x1)
                {
                    x1 = point.X;
                }
                if (point.X > x2)
                {
                    x2 = point.X;
                }
                if (point.Y < y1)
                {
                    y1 = point.Y;
                } 
                if (point.Y > y2)
                {
                    y2 = point.Y;
                }
            }

            return new Dictionary<string, double>()
            {
                { "x1" , x1 },
                { "x2" , x2 },
                { "y1" , y1 },
                { "y2" , y2}
            };
        }

        /// <summary>
        /// Returns a Mesh from a Rectangle with a given grid size
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        private Mesh MeshFromRect(Rectangle3d rect, double gridSize)
        {
            MeshingParameters parameters = new MeshingParameters();
            parameters.MaximumEdgeLength = gridSize;
            parameters.MinimumEdgeLength = gridSize;
            parameters.GridAspectRatio = 1;

            Curve boundary = rect.ToNurbsCurve();
            Mesh mesh = Mesh.CreateFromPlanarBoundary(boundary, parameters);

            return mesh;
        }
        #endregion

        #region tests
        /// <summary>
        /// Tests Point3d containment within the Bounds2d
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Point3d point)
        {
            return (IvalX.Includes(point.X) && IvalY.Includes(point.Y));
        }

        /// <summary>
        /// Test intersection with another Bounds2d
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool Intersects(Bounds2d bounds)
        {
            return IvalX.Intersects(bounds.IvalX) &&
                IvalY.Intersects(bounds.IvalY);
        }
        #endregion

        #region static methods
        /// <summary>
        /// Creates a Bounds2d object from a center point and dimensions
        /// </summary>
        /// <param name="center"></param>
        /// <param name="dimX"></param>
        /// <param name="dimY"></param>
        /// <returns></returns>
        public static Bounds2d FromCenterPoint(Point3d center, double dimX, double dimY)
        {
            List<Point3d> points = new List<Point3d>()
            {
                new Point3d(center.X-dimX/2, center.Y-dimY/2, center.Z),
                new Point3d(center.X+dimX/2, center.Y-dimY/2, center.Z),
                new Point3d(center.X+dimX/2, center.Y+dimY/2, center.Z),
                new Point3d(center.X-dimX/2, center.Y+dimY/2, center.Z),
            };

            return new Bounds2d(points);
        }
        #endregion
    }
}
