using CirculationToolkit.Entities;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Components
{
    class Env_Goo : GH_Goo<SimulationEnvironment>, IGH_PreviewData
    {
        #region constructors
        public Env_Goo()
        {
            Value = new SimulationEnvironment();
        }
        public Env_Goo(SimulationEnvironment environment)
        {
            if (environment == null)
            {
                environment = new SimulationEnvironment();
            }
            Value = environment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
            {
                return "Null Entity";
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
                return ("Defines an Entity for Circulation Simulation");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string TypeName
        {
            get
            {
                return ("Entity");
            }
        }
        #endregion

        #region casting methods
        /// <summary>
        /// 
        /// </summary>
        public bool CastTo<Q>(out Q target)
        {
            if (typeof(Q).IsAssignableFrom(typeof(SimulationEnvironment)))
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
            if (typeof(SimulationEnvironment).IsAssignableFrom(source.GetType()))
            {
                Value = (SimulationEnvironment)source;
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
            return new Env_Goo(Value == null ? new SimulationEnvironment() : Value.Duplicate());
        }
        #endregion
    }
}
