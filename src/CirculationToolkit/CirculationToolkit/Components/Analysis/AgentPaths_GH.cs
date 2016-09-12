using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using CirculationToolkit.Entities;

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
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Floor as Mesh", GH_ParamAccess.item);
            pManager.AddNumberParameter("Indexes", "I", "AgentPath Indexes", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Env_Goo envGoo = null;
            if (!DA.GetData(0, ref envGoo)) { return; }

            if (envGoo.Value.Floors.Count > 0)
            {
                Floor fl = (Floor)envGoo.Value.Floors[0];
                DA.SetData(0, fl.Mesh);
            }

            if (envGoo.Value.Agents.Count > 0)
            {
                Agent a1 = (Agent)envGoo.Value.Agents[0];
                DA.SetDataList(1, a1.Path);
            }
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