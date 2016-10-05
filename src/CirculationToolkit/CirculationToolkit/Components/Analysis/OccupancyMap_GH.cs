using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using CirculationToolkit.Entities;

namespace CirculationToolkit.Components.Analysis
{
    public class OccupancyMap_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BarrierMap_GH class.
        /// </summary>
        public OccupancyMap_GH()
          : base("Occupancy Map", "OMap",
              "The Environment Occupancy Map",
              "Circulation", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Env_Param(), "Environment", "E", "Simulation Environment", GH_ParamAccess.item);
            pManager.AddTextParameter("Floor Name", "N", "The name of the Floor Entity to generate the Occupancy Map on", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Generation", "G", "The generation to output occupancy for. If left blank, this component will output the total occupancy for all generations", GH_ParamAccess.item);

            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Floor as Mesh", GH_ParamAccess.item);
            pManager.AddNumberParameter("Values", "V", "Ocucpancy Map Values represent the number of Agents per grid unit", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Env_Goo envGoo = null;
            int gen = -1;
            string floorName = null;

            if (!DA.GetData(0, ref envGoo)) { return; }
            if (!DA.GetData(1, ref floorName)) { return; }
            if (!DA.GetData(2, ref gen)) { gen = -1; }

            List<Floor> floors = envGoo.Value.GetEntities<Floor>(floorName);

            if (floors.Count == 1)
            {
                Floor floor = floors[0];
                
                //
                // All of this should be implemeted as Floor methods
                //

                if (gen != -1)
                {
                    if (floor.FloorGraph.OccupancyMap.ContainsKey(gen))
                    {
                        Dictionary<int, int> occupancy = floor.FloorGraph.OccupancyMap[gen];
                        List<double> values = new List<double>();

                        for (int i = 0; i < floor.Grid.Count; i++)
                        {
                            if (occupancy.ContainsKey(i))
                            {
                                values.Add(occupancy[i]);
                            }
                            else
                            {
                                values.Add(0);
                            }
                        }

                        DA.SetData(0, floor.Mesh);
                        DA.SetDataList(1, values);
                    }
                }
                else
                {
                    Dictionary<int, int> valueDict = new Dictionary<int, int>();
                    List<double> values = new List<double>();

                    foreach (int generation in floor.FloorGraph.OccupancyMap.Keys)
                    {
                        for (int i = 0; i < floor.Grid.Count; i++)
                        {
                            if (!valueDict.ContainsKey(i))
                            {
                                valueDict[i] = 0;
                            }

                            if (floor.FloorGraph.OccupancyMap[generation].ContainsKey(i))
                            {
                                valueDict[i] += floor.FloorGraph.OccupancyMap[generation][i];
                            }
                        }
                    }

                    for (int i = 0; i < floor.Grid.Count; i++)
                    {
                        values.Add(valueDict[i]);
                    }

                    DA.SetData(0, floor.Mesh);
                    DA.SetDataList(1, values);
                }
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
            get { return new Guid("{6e30b167-6788-48a9-a93e-ecfe222b6bd5}"); }
        }
    }
}