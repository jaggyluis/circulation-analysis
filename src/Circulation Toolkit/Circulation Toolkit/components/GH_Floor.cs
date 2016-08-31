using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;

namespace Circulation_Toolkit
{
    public class GH_Floor : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically beWelcome1LMJ created.
        /// </summary>
        public GH_Floor()
          : base("Floor", "Floor Entity",
              "A floor entity for circulation analysis",
              "Circulation", "Simulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Boundary", "Boundary", "The boundary curve for this floor", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "Name", "The name of this floor", GH_ParamAccess.item);

            // Use this to make parameters optional
            // pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Floor", "Floor", "Created floor entity", GH_ParamAccess.item);
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

            FloorProfile profile = new FloorProfile(name, boundary);

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

    /*
    public class ParamFloor : GH_Param<GooFloor>
    {
        public ParamFloor() : base(new GH_InstanceDescription("Temp", "temp","temp","temp")) 
        {

        }

    }

    public class GooFloor : GH_Goo<FloorProfile>
    {
        public GooFloor()
        {

        }
    }
    */
}
