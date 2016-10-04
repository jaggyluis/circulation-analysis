using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using CirculationToolkit.Environment;
using Grasshopper.Kernel.Data;
using CirculationToolkit.Exceptions;

namespace CirculationToolkit.Components
{
    public class Env_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Environment_GH class.
        /// </summary>
        public Env_GH()
          : base("Environment", "Env",
              "The Simulation Environment",
              "Circulation", "Environment")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Entity_Param(), "Entities", "E", "Environment Entities", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Resolution", "R", "Environment Resolution. This must be above 1 for simulations to be accurate.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run", "R", "Run the simulation", GH_ParamAccess.item);

            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Env_Param(), "Environment", "E", "Simulation Environment", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<Entity_Goo> entityGoos = new GH_Structure<Entity_Goo>();
            double resolution = default(double);
            bool run = false;

            if (!DA.GetDataTree(0, out entityGoos)) { return; };
            if (!DA.GetData(1, ref resolution)) { return; } ;
            if (!DA.GetData(2, ref run)) { return; };

            if (resolution < 1) {

                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Resolution below threshold - setting resolution to 1");

                resolution = 1;               
            }

            SimulationEnvironment env = new SimulationEnvironment(resolution);

            foreach (Entity_Goo g in entityGoos.AllData(true))
            {
                env.AddEntity(g.Value.Duplicate());
            }

            try
            {
                if (env.BuildEnvironment())
                {
                    if (run)
                    {
                        try
                        {
                            env.RunEnvironment();
                        }
                        catch (MaxStepReachedException e)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
                        }
                    }

                    DA.SetData(0, env);
                }
            }
            catch (FloorNotFoundException e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
            }
            catch (NodePathNotPossibleException e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
            }
            catch (NodeNotOnFloorException e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
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
            get { return new Guid("{574ae86e-0540-4d04-a950-446193c2ad2f}"); }
        }
    }
}