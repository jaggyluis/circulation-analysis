using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using CirculationToolkit.Util;
using CirculationToolkit.Entities;

namespace CirculationToolkit.Components
{
    public class Agent_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Agent_GH class.
        /// </summary>
        public Agent_GH()
          : base("Agent", "Agent",
              "An Agent Entity for circulation analysis",
              "Circulation", "Entities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "The name of this Agent", GH_ParamAccess.item);
            pManager.AddTextParameter("Origin", "O", "The name of the Origin Node this Agent is on", GH_ParamAccess.item);
            pManager.AddTextParameter("Destination", "D", "The name of this Agent's Destination Node", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Count", "C", "Optional number of Agents to create", GH_ParamAccess.item);

            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Entity_Param(), "Agent", "A", "Agent Entity", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = null;
            string origin = null;
            string destination = null;
            int count = 1;

            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref origin)) { return; }
            if (!DA.GetData(2, ref destination)) { return; }
            if (!DA.GetData(3, ref count)) { count = 1; }

            AgentProfile profile = new AgentProfile(name);
            profile.SetAttribute("origin", origin);
            profile.SetAttribute("destination", destination);

            List<Entity> agentlist = new List<Entity>();

            for (int i=0; i<count; i++)
            {
                Agent agent = new Agent(profile);
                agentlist.Add(agent);
            }

            DA.SetDataList(0, agentlist);

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
            get { return new Guid("{e2438adb-6b90-48df-8c91-3c5bb75683eb}"); }
        }
    }
}