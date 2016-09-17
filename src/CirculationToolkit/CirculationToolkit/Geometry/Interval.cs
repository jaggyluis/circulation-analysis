using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Geometry
{
    /// <summary>
    /// 2 Dimensional Interval Class
    /// </summary>
    public class Interval
    {
        private double _start;
        private double _end;

        #region constructors
        /// <summary>
        /// Interval Constructor that takes a start number and end number
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Interval(double start, double end)
        {
            _start = start;
            _end = end;
        }
        #endregion

        #region properties
        /// <summary>
        /// Returns the smaller value of the interval range
        /// </summary>
        public double Min
        {
            get
            {
                return Start <= End ? Start : End;
            }
        }

        /// <summary>
        /// Returns the larger value of the interval range
        /// </summary>
        public double Max
        {
            get
            {
                return Start >= End ? Start : End;
            }
        }

        /// <summary>
        /// Returns thesize of the interval range
        /// </summary>
        public double Range
        {
            get
            {
                return Max - Min;
            }
        }

        /// <summary>
        /// Returns the start of the Interval
        /// </summary>
        public double Start
        {
            get
            {
                return _start;
            }

            set
            {
                _start = value;
            }
        }

        /// <summary>
        /// Returns the end if the interval
        /// </summary>
        public double End
        {
            get
            {
                return _end;
            }

            set
            {
                _end = value;
            }
        }
        #endregion

        #region tests
        /// <summary>
        /// Tests number inclusion within the Interval
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool Includes(double num)
        {
            return Min <= num && num <= Max;
        }

        /// <summary>
        /// Tests intersection with another Interval
        /// </summary>
        /// <param name="ival"></param>
        /// <returns></returns>
        public bool Intersects(Interval ival)
        {
            return (Includes(ival.Start) ||
                Includes(ival.End) ||
                ival.Includes(Start) ||
                ival.Includes(End));
        }
        #endregion

        #region static methods
        /// <summary>
        /// Remaps a number from one Interval to another
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="num"></param>
        /// <param name="bounded"></param>
        /// <returns></returns>
        public static double Remap(Interval source, Interval target, double num, bool bounded = false)
        {
            if (source.Range == 0)
            {
                return target.Min;
            }
            else
            {
                double ret = (((num - source.Min) * target.Range) / source.Range) + target.Min;

                if (bounded)
                {
                    if (ret > target.Max)
                    {
                        return target.Max;
                    }
                    else if (ret < target.Min)
                    {
                        return target.Min;
                    }
                }
                return ret;
            }
        }
        #endregion
    }
}
