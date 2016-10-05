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
            pManager.AddTextParameter("Floor Name", "N", "The name of the Floor Entity to generate the Barrier Map on", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Floor as Mesh", GH_ParamAccess.item);
            pManager.AddNumberParameter("Values", "V", "Barrier Map Values", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Env_Goo envGoo = null;
            string floorName = null;

            if (!DA.GetData(0, ref envGoo)) { return; }
            if (!DA.GetData(1, ref floorName)) { return; }

            List<Floor> floors = envGoo.Value.GetEntities<Floor>(floorName);

            if (floors.Count == 1)
            {
                Floor floor = floors[0];
                List<double> values = new List<double>();
                List<int> keys = new List<int>(floor.FloorGraph.BarrierMap.Keys);

                keys.Sort();

                for (int i = 0; i < keys.Count; i++)
                {
                    values.Add(floor.FloorGraph.BarrierMap[keys[i]]);
                }

                DA.SetData(0, floor.Mesh);
                DA.SetDataList(1, values);
            }
            else if (floors.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Floor not found: " + floorName);
            }
            else if (floors.Count > 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Floor name not unique: " + floorName);
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