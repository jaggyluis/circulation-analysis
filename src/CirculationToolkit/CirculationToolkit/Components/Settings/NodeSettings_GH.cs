using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using CirculationToolkit.Profiles;

namespace CirculationToolkit.Components.Settings
{
    public class NodeSettings_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AgentSettings_GH class.
        /// </summary>
        public NodeSettings_GH()
          : base("Node Settings", "Settings",
              "Settings for the Node Entity",
              "Circulation", "Settings")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Max Density", "D", "The Maximum Density at this Node. If left blank, default will be 0.4.", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Distribution Interval", "I", "The amount of time spent at this Node. If left blank, the default will be 0 to 1 generations.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Geometry", "G", "An optional Geometry for density analysis. If left blank, max density will defualt to infinite.", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Settings_Param(), "Settings", "S", "Node Settings", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int capacity = -1;
            double maxDensity = 0.4;
            Interval ival = new Interval(0, 1);
            Curve geometry = null;

            Dictionary<string, string> attributes = new Dictionary<string, string>();

            if (!DA.GetData(0, ref maxDensity)) { }
            if (!DA.GetData(1, ref ival)) { }
            if (!DA.GetData(2, ref geometry)) { }

            Tuple<int, int> distribution = new Tuple<int, int>((int)ival.T0, (int)ival.T1);
            

            if (geometry != null)
            {
                if (geometry.IsClosed && geometry.IsPlanar())
                {
                    double area = AreaMassProperties.Compute(geometry).Area;

                    capacity = (int)Math.Floor(area * maxDensity);
                }
            }

            NodeProfile profile = new NodeProfile(null, attributes, distribution, capacity);

            profile.Geometry = geometry;

            DA.SetData(0, profile);
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
            get { return new Guid("{23801c49-6bbf-45dc-97ad-958a7897620e}"); }
        }
    }
}