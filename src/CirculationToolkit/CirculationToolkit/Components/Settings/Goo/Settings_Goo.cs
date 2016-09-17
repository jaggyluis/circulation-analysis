using CirculationToolkit.Environment;
using CirculationToolkit.Profiles;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Components
{
    class Settings_Goo : GH_Goo<Profile>, IGH_PreviewData
    {
        #region constructors
        public Settings_Goo()
        {
            Value = new Profile();
        }
        public Settings_Goo(Profile profile)
        {
            if (profile == null)
            {
                profile = new Profile();
            }
            Value = profile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
            {
                return "Null Profile";
            }
            else
            {
                return Value.ToString();
            }
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
                return BoundingBox.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Value != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string TypeDescription
        {
            get
            {
                return ("Defines an Agent Settings for more control of Agent behavior");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string TypeName
        {
            get
            {
                return ("Settings");
            }
        }
        #endregion

        #region casting methods
        /// <summary>
        /// 
        /// </summary>
        public bool CastTo<Q>(out Q target)
        {
            if (typeof(Q).IsAssignableFrom(typeof(Profile)))
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
            if (typeof(Profile).IsAssignableFrom(source.GetType()))
            {
                Value = (Profile)source;
                return true;
            }

            return false;
        }
        #endregion

        #region other
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
        }

        /// <summary>
        /// Duplicate
        /// </summary>
        /// <returns></returns>
        public override IGH_Goo Duplicate()
        {
            return new Settings_Goo(Value == null ? new Profile() : Value.Duplicate());
        }
        #endregion
    }
}
