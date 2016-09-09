﻿using System;
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
            pManager.AddParameter(new Entity_Param(), "Entities", "E", "Environment Entities", GH_ParamAccess.list);
            pManager.AddNumberParameter("Resolution", "R", "Environment Resolution", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("test", "test", "test", GH_ParamAccess.item);
            pManager.AddMeshParameter("sdf", "sdf", "sdf", GH_ParamAccess.item);
            pManager.AddNumberParameter("agent", "cs", "asc", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Entity_Goo> entityGoos = new List<Entity_Goo>();
            double resolution = default(double);

            DA.GetDataList(0, entityGoos);
            DA.GetData(1, ref resolution);

            SimulationEnvironment env = new SimulationEnvironment(resolution);

            foreach (Entity_Goo g in entityGoos)
            {
                env.AddEntity(g.Value);
            }

            env.BuildEnvironment();

            env.RunEnvironment();

            Floor fl = (Floor)env.Floors[0];

            if (env.Agents.Count > 0)
            {
                Agent a1 = (Agent)env.Agents[0];
                              

                DA.SetDataList(2, a1.Path);

            }
            

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