using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using CirculationToolkit.Entities;


namespace CirculationToolkit.Components.Analysis
{
    public class DensityMap_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BarrierMap_GH class.
        /// </summary>
        public DensityMap_GH()
          : base("Density Map", "DMap",
              "The Normalized Density Map",
              "Circulation", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Env_Param(), "Environment", "E", "Simulation Environment", GH_ParamAccess.item);
            pManager.AddTextParameter("Floor Name", "N", "The name of the Floor Entity to generate the Density Map on.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Generation", "G", "The generation to output density for. If left blank, this component will output the average density across all generations.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("NonZero", "O", "Toggle NonZero Density. Nonzero Density will output the average density only for time occupied. Default is false.", GH_ParamAccess.item);

            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Floor as Mesh", GH_ParamAccess.item);
            pManager.AddNumberParameter("Values", "V", "Density Map Values represent the density per grid unit area", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Env_Goo envGoo = null;
            int gen = -1;
            bool nonZero = false;
            string floorName = null;

            if (!DA.GetData(0, ref envGoo)) { return; }
            if (!DA.GetData(1, ref floorName)) { return; }
            if (!DA.GetData(2, ref gen)) { gen = -1; }
            if (!DA.GetData(3, ref nonZero)) { nonZero = false; }

            List<Floor> floors = envGoo.Value.GetEntities<Floor>(floorName);

            if (floors.Count == 1)
            {
                Floor floor = floors[0];
                double area = Math.Pow(floor.GridSize, 2);            

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
                                int count = occupancy[i];
                                double density = count / area;

                                values.Add(density);

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
                    Dictionary<int, List<double>> valueDict = new Dictionary<int, List<double>>();
                    List<double> values = new List<double>();

                    foreach (int generation in floor.FloorGraph.OccupancyMap.Keys)
                    {
                        for (int i = 0; i < floor.Grid.Count; i++)
                        {
                            if (!valueDict.ContainsKey(i))
                            {
                                valueDict[i] = new List<double>();
                            }

                            if (floor.FloorGraph.OccupancyMap[generation].ContainsKey(i))
                            {
                                int count = floor.FloorGraph.OccupancyMap[generation][i];
                                double density = count / area;

                                valueDict[i].Add(density);
                            }
                            else if (!nonZero)
                            {
                                valueDict[i].Add(0);
                            }
                        }
                    }

                    for (int i = 0; i < floor.Grid.Count; i++)
                    {
                        int count = valueDict[i].Count;
                        double sum = 0;

                        for (int j=0; j<count; j++)
                        {
                            sum += valueDict[i][j];
                        }

                        values.Add(sum / count);
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
            get { return new Guid("{0845a93c-7e27-4e67-b683-e9e4ab309b37}"); }
        }
    }
}