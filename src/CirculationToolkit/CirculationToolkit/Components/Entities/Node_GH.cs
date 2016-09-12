using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using CirculationToolkit.Util;
using CirculationToolkit.Entities;
using Grasshopper.Kernel.Types;

namespace CirculationToolkit.Components
{
    public class Node_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Node_GH class.
        /// </summary>
        public Node_GH()
          : base("Node", "Node",
              "A Node Entity for circulation analysis",
              "Circulation", "Entities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Position", "P", "The position of this Node", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "The name of this Node", GH_ParamAccess.item);
            pManager.AddTextParameter("Floor", "F", "The name of the Floor this Node is on", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Capacity", "C", "The Agent Capacity at this Node", GH_ParamAccess.item);

            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Entity_Param(), "Node", "N", "Node Entity");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d position = new Point3d();
            string name = null;
            string floorName = null;
            int? capacity = null;

            DA.GetData(0, ref position);
            DA.GetData(1, ref name);
            DA.GetData(2, ref floorName);
            DA.GetData(3, ref capacity);

            Profile profile = new Profile("node", name);
            if (capacity != null)
            {
                profile.SetAttribute("capacity", capacity.ToString());
            }
            profile.SetAttribute("floor", floorName);

            Node node = new Node(profile, position);

            DA.SetData(0, node);
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
            get { return new Guid("{879e09a5-8a07-4b0b-9002-95b8733933a3}"); }
        }
    }
}