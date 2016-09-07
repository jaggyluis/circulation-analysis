using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;

using CirculationToolkit.Profiles;
using CirculationToolkit.Entities;

namespace CirculationToolkit.Components
{
    public class Floor_GH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically beWelcome1LMJ created.
        /// </summary>
        public Floor_GH()
          : base("Floor", "Floor",
              "A Floor Entity for circulation analysis",
              "Circulation", "Simulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Boundary", "B", "The boundary curve for this Floor", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "The name of this Floor", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Entity_Param(), "Floor", "F", "Floor Entity");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve boundary = null;
            string name = null;

            DA.GetData(0, ref boundary);
            DA.GetData(1, ref name);

            Profile profile = new Profile("floor", name);
            Floor floor = new Floor(profile, boundary);

            DA.SetData(0, floor);

        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{26352a7c-1fc7-47a2-a9eb-70ae70442523}"); }
        }
    }
}
