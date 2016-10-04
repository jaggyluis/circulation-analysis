using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Exceptions
{
    public class NodeNotOnFloorException : Exception
    {
        public NodeNotOnFloorException()
        {
        }

        public NodeNotOnFloorException(string message)
        : base(message)
        {
        }

        public NodeNotOnFloorException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
