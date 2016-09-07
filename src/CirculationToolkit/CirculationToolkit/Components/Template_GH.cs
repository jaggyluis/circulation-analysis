using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using CirculationToolkit.Profiles;
using CirculationToolkit.Entities;
using Grasshopper.Kernel.Types;

namespace CirculationToolkit.Components
{
    public class Template_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Template_GH class.
        /// </summary>
        public Template_GH()
          : base("Template", "Template",
              "A Template for Node Entity graph direction",
              "Circulation", "Simulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Lines", "L", "A collection of lines describing connections between nodes", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Directed", "D", "Optional directed graph toggle. Default is False", GH_ParamAccess.item);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Entity_Param(), "Template", "T", "Template Entity");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> lines = new List<Curve>();
            List<Tuple<Point3d, Point3d>> edges = new List<Tuple<Point3d, Point3d>>();
            bool directed = false;

            DA.GetDataList(0, lines);
            DA.GetData(1, ref directed);

            foreach(Curve line in lines)
            {
                Point3d spt = line.PointAtStart;
                Point3d ept = line.PointAtEnd;

                edges.Add(new Tuple<Point3d, Point3d>(spt, ept));
            }

            Profile profile = new Profile("Template");
            profile.SetAttribute("directed", directed.ToString());
            Template template = new Template(profile, edges);

            DA.SetData(0, template);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{00267260-3b60-4c27-bbc7-820f0ec8bf1c}"); }
        }
    }
}