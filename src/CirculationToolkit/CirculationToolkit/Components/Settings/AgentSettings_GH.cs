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
            pManager.AddTextParameter("Visit Nodes", "N", "The name of the Nodes to visit", GH_ParamAccess.list);
            pManager.AddNumberParameter("Visit Node Propensities", "P", "The likelyhood of an Agent to visit each node (0 to 1)", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Visit Node Count", "V", "The number of times the Agent should visit each node", GH_ParamAccess.list);
            pManager.AddNumberParameter("Queuing Propensity", "Q", "The likelyhood of an Agent to choose a node with many Agents at it (0 to 1)", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Distribution Interval", "I", "The Agent Entity spawning distribution", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Agent Count", "C", "The number of Agents to generate. The default is 1", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
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
            List<double> propensityValues = new List<double>();
            List<int> visitValues = new List<int>();
            double queuing = 0;
            Interval ival = new Interval(0,1);
            int count = 1;

            if (!DA.GetDataList(0, nodes)) { }
            if (!DA.GetDataList(1, propensityValues)) { propensityValues.Add(1); }
            if (!DA.GetDataList(2, visitValues)) { visitValues.Add(1); }
            if (!DA.GetData(3, ref queuing)) { }
            if (!DA.GetData(4, ref ival)) {  }
            if (!DA.GetData(5, ref count)) {  }

            nodes.Reverse();
            propensityValues.Reverse();
            visitValues.Reverse();

            Stack<string> nodeStack = new Stack<string>(nodes);
            Stack<double> propensityValueStack = new Stack<double>(propensityValues);
            Stack<int> visitValueStack = new Stack<int>(visitValues);

            Tuple<int, int> distribution = new Tuple<int, int>((int)ival.T0, (int)ival.T1);
            Dictionary<string, double> propensities = new Dictionary<string, double>();
            Dictionary<string, int> visits = new Dictionary<string, int>();
            Dictionary<string, string> attributes = new Dictionary<string, string>(); // this could be added to later

            propensities.Add("queuing", queuing);

            while (nodeStack.Count > 0)
            {
                string node = nodeStack.Pop();
                double propensity;
                int visit;

                if (propensityValueStack.Count > 1)
                {
                    propensity = propensityValueStack.Pop();
                }
                else
                {
                    propensity = propensityValueStack.Peek();
                }

                if (visitValueStack.Count > 1)
                {
                    visit = visitValueStack.Pop();
                }
                else
                {
                    visit = visitValueStack.Peek();
                }

                propensities[node] = propensity;
                visits[node] = visit; 
            }

            AgentProfile profile = new AgentProfile(null, attributes, propensities, visits, distribution, count);         

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