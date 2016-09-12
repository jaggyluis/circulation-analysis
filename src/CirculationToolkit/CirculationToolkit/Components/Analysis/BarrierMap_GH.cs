using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using CirculationToolkit.Entities;

namespace CirculationToolkit.Components.Analysis
{
    public class BarrierMap_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BarrierMap_GH class.
        /// </summary>
        public BarrierMap_GH()
          : base("Barrier Map", "BMap",
              "The Environment Barrier Map",
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
            pManager.AddNumberParameter("Values", "V", "BarrierMap Values", GH_ParamAccess.list);
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
                List<double> values = new List<double>();
                List<int> keys = new List<int>(fl.FloorGraph.BarrierMap.Keys);

                keys.Sort();

                for (int i = 0; i < keys.Count; i++)
                {
                    values.Add(fl.FloorGraph.BarrierMap[keys[i]]);
                }

                DA.SetData(0, fl.Mesh);
                DA.SetDataList(1, values);
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
            get { return new Guid("{73e64243-5053-4731-9db0-96703e5601c5}"); }
        }
    }
}