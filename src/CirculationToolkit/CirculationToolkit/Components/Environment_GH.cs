using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using CirculationToolkit.Entities;

namespace CirculationToolkit.Components
{
    public class Environment_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Environment_GH class.
        /// </summary>
        public Environment_GH()
          : base("Environment", "Environment",
              "The Simulation Environment",
              "Circulation", "Simulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Entity_Param(), "Floors", "F", "Environment Floors", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("test", "test", "test", GH_ParamAccess.item);
            pManager.AddMeshParameter("sdf", "sdf", "sdf", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Entity_Goo> entityGoos = new List<Entity_Goo>();

            DA.GetDataList(0, entityGoos);

            SimulationEnvironment env = new SimulationEnvironment(1);

            foreach (Entity_Goo g in entityGoos)
            {
                env.AddEntity(g.Value);
            }

            env.BuildEnvironment();

            Floor fl = (Floor)env.Floors[0];

            DA.SetData(0, env.ToString());
            DA.SetData(1, fl.Mesh);
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
            get { return new Guid("{574ae86e-0540-4d04-a950-446193c2ad2f}"); }
        }
    }
}