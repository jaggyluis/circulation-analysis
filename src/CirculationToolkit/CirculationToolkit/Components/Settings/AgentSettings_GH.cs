using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using CirculationToolkit.Profiles;

namespace CirculationToolkit.Components.Settings
{
    public class AgentSettings_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AgentSettings_GH class.
        /// </summary>
        public AgentSettings_GH()
          : base("Agent Settings", "Settings",
              "Settings for the Agent Entities",
              "Circulation", "Settings")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path Nodes", "N", "The name of the Nodes to move to", GH_ParamAccess.list);
            pManager.AddNumberParameter("Node Propensities", "P", "The likelyhood of an Agent to visit this node (0 to 1)", GH_ParamAccess.list);
            pManager.AddIntervalParameter("Distribution", "D", "The Agent Entity spawning distribution", GH_ParamAccess.item);
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
            pManager.RegisterParam(new Settings_Param(), "Settings", "S", "Agent Settings", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> nodes = new List<string>();
            List<double> values = new List<double>();
            Interval ival = new Interval(0,10);
            int count = 1;

            if (!DA.GetDataList(0, nodes)) { return; }
            if (!DA.GetDataList(1, values)) { values.Add(1); }
            if (!DA.GetData(2, ref ival)) {  }
            if (!DA.GetData(3, ref count)) {  }

            nodes.Reverse();
            values.Reverse();

            Stack<string> nodeStack = new Stack<string>(nodes);
            Stack<double> valueStack = new Stack<double>(values);

            Tuple<int, int> distribution = new Tuple<int, int>((int)ival.T0, (int)ival.T1);
            Dictionary<string, double> propensities = new Dictionary<string, double>();
            Dictionary<string, string> attributes = new Dictionary<string, string>(); // this could be added to later

            while (nodeStack.Count > 0)
            {
                string node = nodeStack.Pop();
                double value;

                if (valueStack.Count > 1)
                {
                    value = valueStack.Pop();
                }
                else
                {
                    value = valueStack.Peek();
                }

                propensities[node] = value;    
            }

            AgentProfile profile = new AgentProfile(null, attributes, propensities, distribution, count);

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
            get { return new Guid("{5e0fad35-6a66-4298-8acf-832923cb0fa3}"); }
        }
    }
}