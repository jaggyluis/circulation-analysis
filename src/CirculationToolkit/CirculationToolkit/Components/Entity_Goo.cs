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
    class Entity_Goo : GH_Goo<Entity>, IGH_PreviewData
    {
        #region constructors
        public Entity_Goo()
        {
            Value = new Entity();
        }
        public Entity_Goo(Entity entity)
        {
            if (entity == null)
            {
                entity = new Entity();
            }
            Value = entity;
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
            if (typeof(Q).IsAssignableFrom(typeof(Entity)))
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
            if (typeof(Entity).IsAssignableFrom(source.GetType()))
            {
                Value = (Entity)source;
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
            return new Entity_Goo(Value == null ? new Entity() : Value.Duplicate());
        }
        #endregion
    }
}
