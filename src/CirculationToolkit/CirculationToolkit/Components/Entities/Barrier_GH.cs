using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using CirculationToolkit.Entities;
using Grasshopper.Kernel.Types;
using CirculationToolkit.Util;

namespace CirculationToolkit.Components
{
    public class Barrier_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Barrier_GH class.
        /// </summary>
        public Barrier_GH()
          : base("Barrier", "Barrier",
              "A Barrier Entity for circulation analysis",
              "Circulation", "Entities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Boundary", "B", "The boundary curve for this Barrier", GH_ParamAccess.item);
            pManager.AddTextParameter("Floor", "F", "The name of the Floor this Barrier is on", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Entity_Param(), "Barrier", "B", "Barrier Entity");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve boundary = null;
            string floorName = null;

            DA.GetData(0, ref boundary);
            DA.GetData(1, ref floorName);

            Profile profile = new Profile("barrier");
            profile.SetAttribute("floor", floorName);

            Barrier barrier = new Barrier(profile, boundary);

            DA.SetData(0, barrier);
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
            get { return new Guid("{e32a8136-50fc-45f1-a4e7-7a30ed35d64c}"); }
        }
    }
}