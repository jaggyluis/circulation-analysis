using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using CirculationToolkit.Entities;
using Grasshopper;
using Grasshopper.Kernel.Data;

namespace CirculationToolkit.Components.Analysis
{
    public class AgentPaths_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BarrierMap_GH class.
        /// </summary>
        public AgentPaths_GH()
          : base("Agent Paths", "APaths",
              "Agent Entity Paths",
              "Circulation", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Env_Param(), "Environment", "E", "Simulation Environment", GH_ParamAccess.item);
            pManager.AddTextParameter("Agent Name", "N", "The name of the Agent to output paths for", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Floor as Mesh", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Indexes", "I", "AgentPath Indexes", GH_ParamAccess.tree);
            pManager.AddTextParameter("Log", "L", "Agent Log", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Env_Goo envGoo = null;
            string agentName = null;

            if (!DA.GetData(0, ref envGoo)) { return; }
            if (!DA.GetData(1, ref agentName)) { return; }

            List<Agent> agents = envGoo.Value.GetAgents(agentName);

            DataTree<int> pathTree = new DataTree<int>();
            DataTree<string> logTree = new DataTree<string>();

            if (agents.Count > 0)
            {
                DA.SetData(0, agents[0].Floor.Mesh);
            }

            for (int i=0; i<agents.Count; i++)
            {
                Agent agent = agents[i];
                List<string> agentLog = new List<string> (agent.Log());
                List<int> agentPath = agent.Path;

                GH_Path path = new GH_Path(i);

                for (int j=0; j<agentPath.Count; j++)
                {
                    pathTree.Add(agentPath[j], path);
                }

                for (int k=0; k<agentLog.Count; k++)
                {
                    logTree.Add(agentLog[k], path);
                }
            }

            DA.SetDataTree(1, pathTree);
            DA.SetDataTree(2, logTree);
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
            get { return new Guid("{248191a2-bd24-4c2a-97b9-94eaa313079e}"); }
        }
    }
}