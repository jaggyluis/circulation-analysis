using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using CirculationToolkit.Entities;
using CirculationToolkit.Profiles;
using CirculationToolkit.Geometry;

namespace CirculationToolkit.Components.Analysis
{
    public class NodeMap_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BarrierMap_GH class.
        /// </summary>
        public NodeMap_GH()
          : base("Node Map", "NMap",
              "The Normalized Density Map at a Node",
              "Circulation", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Env_Param(), "Environment", "E", "Simulation Environment", GH_ParamAccess.item);
            pManager.AddTextParameter("Node Name", "N", "The name of the Node Entity to generate the Density Map for.", GH_ParamAccess.item);
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
            pManager.AddMeshParameter("Mesh", "M", "Node Geometry as Mesh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Values", "V", "Density Map Values represent the density for the entire Zone", GH_ParamAccess.list);
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
            string nodeName = null;

            if (!DA.GetData(0, ref envGoo)) { return; }
            if (!DA.GetData(1, ref nodeName)) { return; }
            if (!DA.GetData(2, ref gen)) { gen = -1; }
            if (!DA.GetData(3, ref nonZero)) { nonZero = false; }

            List<Mesh> outMeshes = new List<Mesh>();
            List<double> outValues = new List<double>();

            if (envGoo.Value.GetEntities<Node>(nodeName).Count != 0)
            {
                List<Node> nodes = envGoo.Value.GetEntities<Node>(nodeName);      

                if (nodes.Count > 0)
                {
                    for (int i=0; i<nodes.Count; i++)
                    {
                        Node node = nodes[i];

                        if (node.Geometry == null) { continue; }

                        double area = AreaMassProperties.Compute(node.Geometry).Area;

                        if (gen != -1)
                        {
                            if (node.Floor.FloorGraph.OccupancyMap.ContainsKey(gen))
                            {
                                int nodeCount = node.Count(gen);

                                Mesh mesh = new Mesh();
                                mesh.CopyFrom(node.Floor.Mesh);
                                List<int> removeIndexes = new List<int>();

                                for (int j=0; j<mesh.Faces.Count; j++)
                                {
                                    if (!node.Indexes.Contains(j))
                                    {
                                        removeIndexes.Add(j);
                                    }
                                }

                                mesh.Faces.DeleteFaces(removeIndexes);

                                outMeshes.Add(mesh);
                                outValues.Add(nodeCount / area);
                            }
                        }
                        else
                        {
                            List<double> values = new List<double>();
                            double sum = 0;

                            Mesh mesh = new Mesh();
                            mesh.CopyFrom(node.Floor.Mesh);
                            List<int> removeIndexes = new List<int>();

                            for (int j = 0; j < mesh.Faces.Count; j++)
                            {
                                if (!node.Indexes.Contains(j))
                                {
                                    removeIndexes.Add(j);
                                }
                            }

                            mesh.Faces.DeleteFaces(removeIndexes);

                            foreach (int generation in node.Floor.FloorGraph.OccupancyMap.Keys)
                            {
                                int count = node.Count(generation);

                                if (count != 0)
                                {
                                    values.Add(count / area);
                                }
                                else
                                {
                                    if (!nonZero)
                                    {
                                        values.Add(count);
                                    }
                                }
                            }

                            for (int j=0; j<values.Count; j++)
                            {
                                sum += values[j];
                            }

                            outMeshes.Add(mesh);
                            outValues.Add(sum / values.Count);
                        }
                    }
                }

                DA.SetDataList(0, outMeshes);
                DA.SetDataList(1, outValues);
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
            get { return new Guid("{1b041ef2-91f6-44a2-a38c-fdf132a449f3}"); }
        }
    }
}