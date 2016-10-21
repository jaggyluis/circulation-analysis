using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CirculationToolkit.Profiles;
using Rhino.Geometry;

namespace CirculationToolkit.Entities
{
    public class Link : Entity
    {
        Point3d _start;
        Point3d _end;

        public Link(Point3d start, Point3d end)
            : base (new Profile("link"))
        {
            _start = start;
            _end = end;
        }
    }
}
