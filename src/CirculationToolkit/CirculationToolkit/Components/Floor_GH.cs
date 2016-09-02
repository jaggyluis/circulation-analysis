using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;

using CirculationToolkit.Profiles;
using CirculationToolkit.Entities;

namespace CirculationToolkit.Components
{
    public class Floor_GH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically beWelcome1LMJ created.
        /// </summary>
        public Floor_GH()
          : base("Floor", "Floor",
              "A floor entity for circulation analysis",
              "Circulation", "Simulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Boundary", "B", "The boundary curve for this floor", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "The name of this floor", GH_ParamAccess.item);

            // Use this to make parameters optional
            // pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Floor_Param(), "Floor", "F", "Floor Entity");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve boundary = null;
            string name = null;

            DA.GetData(0, ref boundary);
            DA.GetData(1, ref name);

            Profile profile = new Profile("floor", name);
            Floor floor = new Floor(profile, boundary);

            DA.SetData(0, floor);

        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{26352a7c-1fc7-47a2-a9eb-70ae70442523}"); }
        }
    }

    /// <summary>
    /// Floor_Param wrapper
    /// </summary>
    public class Floor_Param : GH_Param<Floor_Goo>, IGH_PreviewObject
    {
        #region constructors
        /// <summary>
        /// 
        /// </summary>
        public Floor_Param() : base(new GH_InstanceDescription("Floor", 
            "Floor", 
            "A floor entity for circulation analysis", 
            "Circulation",
            "Simulation")) 
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// 
        /// </summary>
        public BoundingBox ClippingBox
        {
            get
            {
                return Preview_ComputeClippingBox();
            }
        }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("682d479f-b0b2-473d-9371-b3e3b20a8f3d");
            }
        }
        /// <summary>
        /// This makes sure that the parameter does not show up in toolbar
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }
        #endregion

        #region preview methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
        }

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            Preview_DrawWires(args);
        }
        
        private bool m_hidden = false;
        public bool Hidden
        {
            get
            {
                return m_hidden;
            }
            set
            {
                m_hidden = value;
            }
        }
        public bool IsPreviewCapable
        {
            get
            {
                return true;
            }
        }
        #endregion
    }
    /// <summary>
    /// Floor_Goo wrapper
    /// </summary>
    public class Floor_Goo : GH_GeometricGoo<Floor>, IGH_PreviewData
    {
        #region constructors
        /// <summary>
        /// Constructor methods
        /// </summary>
        /// <param name="name"></param>
        /// <param name="boundary"></param>
        public Floor_Goo()
        {
            Value = new Floor();
        }
        public Floor_Goo(Floor floor)
        {
            if (floor == null)
            {
                floor = new Floor();
            }
            Value = floor;
        }
        #endregion

        #region properties
        /// <summary>
        /// Property methods
        /// </summary>
        /// <param name="xform"></param>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
            {
                return "Null Floor";
            }
            else
            {
                return Value.ToString();
            }
        }

        public override string TypeName
        {
            get
            {
                return ("Floor");
            }
        }

        public override string TypeDescription
        {
            get
            {
                return ("Defines a Floor Entity for circulation simulation");
            }
        }
        #endregion

        #region casting methods
        /// <summary>
        /// 
        /// </summary>
        public override bool CastTo<Q>(out Q target)
        {
            //Cast to Floor
            if (typeof(Q).IsAssignableFrom(typeof(Floor)))
            {
                if (Value == null)
                {
                    target = default(Q);
                }
                else
                {
                    target = (Q)(object)Value;
                }
                return true;
            }

            target = default(Q);
            return false;
        }

        public override bool CastFrom(object source)
        {
            if (source == null)
            {
                return false;
            }
            if (typeof(Floor).IsAssignableFrom(source.GetType()))
            {
                Value = (Floor)source;
                return true;
            }

            return false;
        }
        #endregion

        #region transformation methods
        /// <summary>
        /// 
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value == null)
                {
                    return BoundingBox.Empty;
                }
                if (Value.Geometry == null)
                {
                    return BoundingBox.Empty;
                }
                return Value.Geometry.GetBoundingBox(true);
            }
        }

        public BoundingBox ClippingBox
        {
            get
            {
                return Boundingbox;
            }
        }

        public override BoundingBox GetBoundingBox(Transform xform)
        {
            if (Value == null)
            {
                return BoundingBox.Empty;
            }
            if (Value.Geometry == null)
            {
                return BoundingBox.Empty;
            }
            return Value.Geometry.GetBoundingBox(xform);
        }

        public override IGH_GeometricGoo Transform(Transform xform)
        {
            throw new NotImplementedException();
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region duplication methods
        /// <summary>
        /// Duplication methods
        /// </summary>
        /// <returns></returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateFloor();
        }
        public Floor_Goo DuplicateFloor()
        {
            return new Floor_Goo(Value == null ? new Floor() : Value.Duplicate());
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
        }
        #endregion
    }

}
