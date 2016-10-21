using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Components
{
    class Env_Param : GH_Param<Env_Goo>, IGH_PreviewObject
    {
        #region constructors
        public Env_Param() : base(new GH_InstanceDescription("Environment",
            "Environment",
            "A simulation Environment",
            "Circulation",
            "Environment"))
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

        /// <summary>
        /// 
        /// </summary>
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("91bd8c49-3958-4dc0-8ab2-a38244e916f8");
            }
        }

        /// <summary>
        /// This makes sure that the parameter does not show up in toolbar
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
        #endregion

        #region preview methods
        /// <summary>
        /// 
        /// </summary>
        public bool IsPreviewCapable
        {
            get
            {
                return true;
            }
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
        }

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            Preview_DrawWires(args);
        }
        #endregion
    }
}
