using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CirculationToolkit.Components.Settings
{
    public class AgentSettings_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AgentSettings_GH class.
        /// </summary>
        public AgentSettings_GH()
          : base("Agent Settings", "Agent Settings",
              "Settings for the Agent Entities",
              "Circulation", "Settings")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path Nodes", "N", "The name of the Nodes to mvoe to", GH_ParamAccess.list);
            pManager.AddNumberParameter("Node Propensities", "P", "The likelyhood of an Agent to visit this node", GH_ParamAccess.list);
            pManager.AddIntervalParameter("Distribution", "D", "the amount of time these agents take to start", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Count", "C", "The number of Agents to generate. The default is 1", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> nodes = new List<string>();
            List<double> propensities = new List<double>();
            Interval ival = default(Interval);
            int count = 1;

            if (!DA.GetDataList(0, nodes) && !DA.GetDataList(1, propensities)) { return; }
            if (nodes.Count != propensities.Count) { return; } // for now

            if (!DA.GetData(2, ref ival)) {  }
            if (!DA.GetData(3, ref count)) {  }




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
            get { return new Guid("{5e0fad35-6a66-4298-8acf-832923cb0fa3}"); }
        }
    }
}